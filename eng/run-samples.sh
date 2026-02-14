#!/usr/bin/env bash

set -uo pipefail

source="${BASH_SOURCE[0]}"
while [[ -h "$source" ]]; do
  scriptroot="$( cd -P "$( dirname "$source" )" && pwd )"
  source="$(readlink "$source")"
  [[ $source != /* ]] && source="$scriptroot/$source"
done
scriptroot="$( cd -P "$( dirname "$source" )" && pwd )"
reporoot="$( cd -P "$scriptroot/.." && pwd )"

usage()
{
  echo "Usage: run-samples.sh [options]"
  echo ""
  echo "Discovers and runs all *.Examples.cs sample files under src/libraries/*/samples/."
  echo "Each sample is a file-based app executed with 'dotnet run'. A non-zero exit code"
  echo "from any sample is treated as a failure."
  echo ""
  echo "Options:"
  echo "  --verbose (-v)    Show output from every sample, not just failures."
  echo "  --help (-h)       Print this help message."
  echo ""
  echo "Examples:"
  echo "  ./eng/run-samples.sh              # Run all samples, report failures"
  echo "  ./eng/run-samples.sh --verbose    # Run all samples, show all output"
}

verbose=false

while [[ $# -gt 0 ]]; do
  opt="$(echo "${1/#--/-}" | tr "[:upper:]" "[:lower:]")"
  case "$opt" in
    -help|-h|-\?)
      usage
      exit 0
      ;;
    -verbose|-v)
      verbose=true
      shift 1
      ;;
    *)
      echo "Unknown option: $1" 1>&2
      usage
      exit 1
      ;;
  esac
done

export PATH="$reporoot/.dotnet:$PATH"

# Verify the SDK is available.
if ! command -v dotnet &> /dev/null; then
  echo "error: 'dotnet' not found. Build the repo first (./build.sh) to provision the SDK."
  exit 1
fi

# Discover all sample files.
mapfile -t sample_files < <(find "$reporoot/src/libraries" -path '*/samples/*/*.Examples.cs' -type f | sort)

if [[ ${#sample_files[@]} -eq 0 ]]; then
  echo "No sample files found."
  exit 0
fi

echo "Found ${#sample_files[@]} sample file(s)."
echo ""

passed=0
failed=0
declare -a failed_files=()

for file in "${sample_files[@]}"; do
  # Determine the samples/ directory (where Directory.Build.props lives).
  samples_dir="${file}"
  while [[ "$(basename "$samples_dir")" != "samples" ]]; do
    samples_dir="$(dirname "$samples_dir")"
  done

  # Build a relative path from the samples dir to the .cs file.
  relative_path="${file#"$samples_dir/"}"

  # Friendly display name: library + relative path.
  lib_dir="$(dirname "$samples_dir")"
  lib_name="$(basename "$lib_dir")"
  display_name="$lib_name/samples/$relative_path"

  echo -n "  Running $display_name ... "

  # Run the sample from the samples/ directory.
  output=$(cd "$samples_dir" && dotnet run "$relative_path" 2>&1)
  exit_code=$?

  if [[ $exit_code -eq 0 ]]; then
    echo "passed"
    ((passed++))
    if [[ "$verbose" == true ]]; then
      echo "$output" | sed 's/^/    | /'
    fi
  else
    echo "FAILED (exit code $exit_code)"
    ((failed++))
    failed_files+=("$display_name")
    echo "$output" | sed 's/^/    | /'
  fi
done

echo ""
echo "=========================================="
echo "  Samples: $((passed + failed))  Passed: $passed  Failed: $failed"
echo "=========================================="

if [[ $failed -gt 0 ]]; then
  echo ""
  echo "Failed samples:"
  for f in "${failed_files[@]}"; do
    echo "  - $f"
  done
  exit 1
fi

exit 0
