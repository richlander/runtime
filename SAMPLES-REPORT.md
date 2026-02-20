# Collections XML Docs & Samples Report

> **Note:** This report documents the initial collections pilot and is not intended
> as a long-term repo asset. It captures decisions, tradeoffs, and context that are
> useful during review of this branch but can be deleted once the work is merged.

## Overview

This branch (`dev/richlander/collections-xmldocs`) migrates XML documentation and
adds runnable code samples for the core `System.Collections.Generic` types. The
work spans two library assemblies and covers 11 types with 352 mapped API members.

The migration was performed with the [`slash` CLI toolkit](https://github.com/richlander/slash-slash-slash-ftw),
which audits, quality-checks, and merges XML documentation from `dotnet-api-docs`
into C# `///` comments and links runnable samples via `samples.json`.

### Design goals

Three properties of the tool are central to the workflow:

- **Any scope** — the tool can be run against a single file, a directory, a library,
  or the whole repo. This makes it equally useful for one-type interactive work and
  fleet-scale batch processing.

- **Repeatable with no unique edits in C# source** — the `///` comments written to
  `.cs` files are entirely derived from other sources: `dotnet-api-docs` XML,
  `samples.json`, and the sample `.cs` files. The tool can be re-run any number of
  times without losing data or human effort. This means we can re-run on a fresh
  branch to avoid merge conflicts, regenerate after upstream XML doc improvements,
  or replay after samples are added — the `.cs` doc comments are always a computed
  output, never a hand-maintained artifact.

- **Built for agent fleets** — the tool and its companion
  [agent skill](https://github.com/richlander/slash-slash-slash-ftw/blob/main/SKILL.md)
  are purpose-built for multi-agent automation. Each type is an independent work unit:
  a doc agent audits, writes samples, registers them in `samples.json`, and runs the
  tool to regenerate — no coordination with other agents required. The skill document
  describes triage-lead, doc-agent, and reviewer roles that can run in parallel.

  **Rough fleet estimate:** the `dotnet-api-docs` repo contains ~18,000 type-level XML
  files. Excluding frameworks not in `dotnet/runtime` (WinForms, WPF, WCF, ASP.NET
  WebForms, etc.) leaves an estimated 8,000–10,000 runtime-relevant types. The
  collections pilot processed 11 types (352 members, 11 sample files) — each type
  taking roughly 15–30 minutes of agent time end-to-end (audit, sample authoring,
  merge, verify). At that rate, a fleet of 20 parallel agents could process the full
  runtime corpus in approximately 4–8 days of wall-clock time. Doc-only migration
  (without writing new samples) is significantly faster — under a minute per type —
  and could cover the full set in hours.

  **Open constraint: sample prioritization.** No analysis has been done yet on how to
  select which types should get samples first. Not every type benefits equally — high-traffic
  APIs (collections, IO, networking) are far more valuable to sample than rarely-used
  internal types. An initial rollout should define criteria for high-value samples (e.g.,
  page-view data from learn.microsoft.com, NuGet download counts, or API usage telemetry)
  and use that to rank the work. Without this, a fleet would produce thousands of samples
  of uniform effort but uneven impact. This prioritization analysis should be completed
  before scaling beyond the pilot.

## What was added

### Samples infrastructure

Each library with samples has a `samples/` directory containing:

- **`Directory.Build.props`** — isolates samples from the repo build system
  (`TargetFramework=net11.0`, `IsSourceProject=false`, `TreatWarningsAsErrors=false`).
- **`Directory.Build.targets`** — empty, blocks repo targets inheritance.
- **`samples.json`** — maps DocFX UIDs (e.g. `M:System.Collections.Generic.List`1.Add`)
  to a specific `.cs` file and method name, enabling tooling to link API docs to
  runnable examples.
- **`*.Examples.cs`** — standalone file-based apps (top-level statements) that
  exercise multiple API members per file. Each file calls local methods, prints
  output with `// Output:` comments for expected values, and returns 0 on success.

### Coverage

| Type | Assembly | DocFX UIDs | Sample File | Lines |
|------|----------|------------|-------------|-------|
| Dictionary | System.Private.CoreLib | 18 | Dictionary.Examples.cs | 151 |
| HashSet | System.Private.CoreLib | 17 | HashSet.Examples.cs | 122 |
| List | System.Private.CoreLib | 55 | List.Examples.cs | 234 |
| Queue | System.Private.CoreLib | 16 | Queue.Examples.cs | 102 |
| LinkedList | System.Collections | 24 | LinkedList.Examples.cs | 94 |
| LinkedListNode | System.Collections | 7 | — | — |
| OrderedDictionary | System.Collections | 70 | OrderedDictionary.Examples.cs | 149 |
| PriorityQueue | System.Collections | 24 | PriorityQueue.Examples.cs | 108 |
| SortedDictionary | System.Collections | 40 | SortedDictionary.Examples.cs | 117 |
| SortedList | System.Collections | 27 | SortedList.Examples.cs | 164 |
| SortedSet | System.Collections | 29 | SortedSet.Examples.cs | 135 |
| Stack | System.Collections | 24 | Stack.Examples.cs | 128 |
| **Total (11 types)** | | **351** | **11** | **~1,504** |

### Managing duplication: samples.json vs .cs files

A key design question is where duplication lives. A single sample function often
demonstrates multiple API members — for example, `AddNodes` in
`LinkedList.Examples.cs` exercises `AddFirst`, `AddLast`, `AddAfter`, and `AddBefore`.
That's one function but six DocFX UIDs that should all link to it. The duplication has
to exist somewhere: either every member's `///` comment contains the full code block,
or only one does and the rest point to it.

We push duplication into `samples.json` and keep the `.cs` files clean:

- **`samples.json` is intentionally verbose** — every DocFX UID that a sample covers
  gets its own entry, all pointing to the same file and method. LinkedList has 24
  entries for 5 local functions. This is repetitive but explicit: you can look up any
  member and immediately see which sample covers it, without needing to read the `.cs`
  file or infer grouping.

- **`.cs` files have no duplication** — each local function appears once. There are no
  duplicate code blocks, no copied snippets, no parallel maintenance. The sample file
  reads like normal code.

- **`///` comments use primary/ref to avoid bloat** — when multiple members share a
  sample, `samples.json` marks one entry as `"primary": true`. The tool emits the
  full `<code source="..." method="..." id="...">` block on the primary member and a
  lightweight `<code ref="..." />` cross-reference on the others. This keeps the
  generated `///` comments small: 20 members can share a sample without 20 copies of
  the code block in the source file.

- **Type-level fallback reduces entries** — constructors and other members that are
  naturally demonstrated by a type-level overview sample don't need explicit
  `samples.json` entries. The tool automatically considers `T:` entries when looking
  up any member of that type.

The result is that `samples.json` is the only place that knows the many-to-one mapping
from members to functions. It is verbose by design — easy to audit, diff, and
generate — while the `.cs` files stay focused on being readable, runnable code.

### Sample runner

`eng/run-samples.sh` discovers and runs every sample, reporting pass/fail:

```
$ ./eng/run-samples.sh
Found 11 sample file(s).

  Running System.Collections/.../LinkedList.Examples.cs ... passed
  Running System.Collections/.../Stack.Examples.cs ... passed
  Running System.Private.CoreLib/.../List.Examples.cs ... passed
  ...

==========================================
  Samples: 11  Passed: 11  Failed: 0
==========================================
```

## Source and fidelity to dotnet-api-docs

The XML documentation was migrated from
[dotnet/dotnet-api-docs](https://github.com/dotnet/dotnet-api-docs). The content
in that repo is the canonical source for .NET API reference documentation shown on
learn.microsoft.com.

### What was migrated faithfully

- **`<summary>` and `<param>` descriptions** — text was carried over verbatim from
  dotnet-api-docs XML, preserving wording, `<see cref="..."/>` references, and
  `<paramref name="..."/>` usage.
- **`<returns>` documentation** — kept as-is from the upstream source.
- **`<exception>` tags** — exception documentation was migrated and verified against
  the actual source implementation to ensure accuracy.
- **`<remarks>` content** — longer-form remarks were migrated where they exist in
  dotnet-api-docs, converted from raw Markdown to XML doc format.

### Where samples diverge from dotnet-api-docs

The code samples in dotnet-api-docs are typically embedded within `<example>` blocks
as non-runnable snippets. The samples in this branch are **new, purpose-built
runnable programs** that differ in several ways:

1. **Runnable and self-contained** — each `.Examples.cs` file is a complete file-based
   app that compiles and runs with `dotnet run`. The dotnet-api-docs snippets are
   often fragments that require surrounding context.

2. **Grouped by usage pattern, not by member** — rather than one tiny snippet per API
   member, each sample file groups related members into logical methods
   (e.g. `AddAndInsert`, `SearchItems`, `RemoveItems` for List). The `samples.json`
   mapping file then associates each DocFX UID with the appropriate method.

3. **Modern C# idioms** — samples use collection expressions (`[1, 2, 3]`), top-level
   statements, `new()` target-typed construction, and other current language features.
   The dotnet-api-docs examples often use older C# syntax.

4. **Output comments** — each `Console.WriteLine` is followed by a `// Output:` comment
   showing the expected result, making the samples useful as inline documentation
   without running them.

### Why local functions instead of `#region`

The established pattern for sample boundaries in .NET — used by
[Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) and supported by the
Sandcastle/DocFX `<code source="..." region="...">` attribute — wraps sample code in
`#region`/`#endregion` markers. The doc build system extracts the content between the
markers and injects it into the rendered documentation.

We deliberately avoided this pattern. Regions are a poor fit for samples, especially
samples that new users will encounter:

- **Regions are not real code structure** — they are preprocessor directives that the
  compiler ignores. A `#region` can start mid-statement, span unrelated code, or depend
  on variables declared outside the region. A new user who copies the region content
  gets a fragment that may not compile. This is the opposite of what a sample should be.

- **Regions require invisible scaffolding** — the code inside a `#region` still needs a
  containing method, class, and `Main` to compile and run. That scaffolding is outside
  the region and invisible in the rendered docs, so the user sees a snippet but not what
  it takes to actually run it. Local functions in a top-level program are complete — a
  reader can copy the function and run it with `dotnet run`.

- **Regions aren't standard or pretty** — `#region`/`#endregion` markers are visual
  noise that most style guides discourage in production code. Putting them in samples
  signals that this is an acceptable pattern, which it isn't. New users learning from
  samples should not be picking up `#region` as an idiom.

- **Regions are fragile at scale** — during fleet processing, agents occasionally
  insert `// <Tag>` comment markers, `#region` directives, or other boundary conventions
  when instructed to "mark sample boundaries." Local functions are unambiguous — there
  is no creative interpretation of what a boundary looks like.

We chose **local functions** with a `method="..."` attribute instead. A local function
has a clear scope (parameters, local variables, return), is self-contained and
copy-pasteable, and is already runnable — which is what makes `eng/run-samples.sh`
possible. Adding `#region` markers on top of functions would be redundant nesting with
no benefit.

The tradeoff is that `method="..."` is not a standard DocFX attribute today. If the
runtime's doc build pipeline is extended to render these samples, it would need to
support extracting local function bodies — a straightforward addition (parse the file,
find the function, emit its body) but not yet implemented. The
[`dotnet-inspect` sample-references doc](https://github.com/richlander/dotnet-inspect/blob/main/docs/sample-references.md)
surveys the `region`-based and other patterns across the ecosystem for additional
context.

## Branch history

| Commit | Description |
|--------|-------------|
| `06e32c5` | Scaffold `samples/` directories with `Directory.Build.props`, `Directory.Build.targets`, and empty `samples.json` for both libraries |
| `3c02b72` | Add samples for List, Dictionary, HashSet, Queue (System.Private.CoreLib) |
| `e347a6c` | Add samples for Stack, LinkedList, SortedSet, PriorityQueue (System.Collections) |
| `8c1bd5a` | Add samples for SortedDictionary, SortedList, OrderedDictionary (System.Collections) |
| `ca912aa` | Add `eng/run-samples.sh` sample runner script |

## How eng/run-samples.sh works

The script is a standalone Bash utility with no MSBuild integration. It:

1. **Discovers samples** — uses `find` to locate all `*.Examples.cs` files under
   `src/libraries/*/samples/`.

2. **Resolves the build context** — for each file, walks up the directory tree to find
   the enclosing `samples/` directory (where `Directory.Build.props` lives). This is
   the working directory for `dotnet run`.

3. **Runs each sample** — executes `dotnet run <relative-path>` using the repo's
   `.dotnet` SDK. The file-based app feature in .NET 10+ compiles and runs the `.cs`
   file directly.

4. **Captures results** — stdout/stderr are captured. On failure (non-zero exit code),
   the full output is printed indented under the file name. On success, output is
   suppressed unless `--verbose` is passed.

5. **Reports a summary** — prints total/passed/failed counts and lists all failed files.
   Exits with code 1 if any sample failed, 0 otherwise.

### Usage

```bash
./eng/run-samples.sh              # Run all samples, show failures only
./eng/run-samples.sh --verbose    # Run all samples, show all output
./eng/run-samples.sh --help       # Print help
```

### Failure detection

A sample is considered failed if `dotnet run` returns a non-zero exit code. This
catches:

- **Explicit failure** — samples that `return 1` (or any non-zero value).
- **Unhandled exceptions** — runtime errors produce a stack trace and a non-zero exit.
- **Compilation errors** — `dotnet run` itself fails with a non-zero exit code if the
  file doesn't compile.

### Ideal: run samples as CI tests

`eng/run-samples.sh` is a standalone script that works today but is not wired into
the repo's CI pipeline. The ideal end state is for sample failures to break CI, just
like unit tests. Here is what that path looks like:

1. **Add a `libs.samples` build subset** — register a new subset in `eng/Subsets.props`
   alongside `libs.tests`. This gives `./build.sh -s libs.samples` a first-class
   entry point without coupling samples to the existing test infrastructure.

2. **Create an MSBuild project that discovers and runs samples** — a thin `.proj` or
   `.targets` file under `eng/testing/` that uses `<Exec>` to run `dotnet run` on each
   `*.Examples.cs` file. This replaces the Bash script with something the existing
   build orchestration can schedule, log, and report on natively.

3. **Integrate into Helix** — the runtime repo runs tests on Helix for cross-platform
   coverage. Each sample is a tiny, self-contained workload — ideal for Helix work
   items. The MSBuild project would produce Helix payloads the same way library tests
   do, giving samples the same OS/arch matrix as the rest of the repo.

4. **Wire into the libraries pipeline** — add the `libs.samples` subset to the
   appropriate legs in `eng/pipelines/`. Samples are fast (compile + run in seconds)
   so they can run alongside `libs.tests` without meaningful CI cost.

5. **Report results in test infrastructure** — emit results in a format the repo's
   test reporting understands (TRX or Helix-compatible). This surfaces sample failures
   in PR checks and the build dashboard alongside unit test results.

The current `eng/run-samples.sh` script is the right starting point — it already
handles discovery, execution, and failure reporting. The work is to lift it into the
build system so that the results are visible in the same place as all other test
results and failures block merge.

## Directory.Build.props isolation tradeoff

Each `samples/` directory contains a `Directory.Build.props` that replaces the repo's
build-property chain and an empty `Directory.Build.targets` that blocks repo-level
targets inheritance. Together they create a clean isolation boundary so that samples
compile as standalone file-based apps via `dotnet run`.

### Why isolation is needed

The runtime repo's `Directory.Build.props` chain imports Arcade SDK orchestration,
strong-name keys, AOT analyzers, trimming props, and project-classification logic.
Those settings assume every project is a library, test, or reference assembly in the
repo's build graph. A plain `.cs` file-based app breaks under those assumptions:

- **`IsSourceProject` / `IsReferenceAssemblyProject`** — triggers signing, packaging,
  and platform-specific build logic that doesn't apply to a sample.
- **`TreatWarningsAsErrors`** — the repo enforces this globally, but samples may use
  simplified code that produces nullable or other warnings irrelevant to their purpose.
- **`PublishAot` / `IsAotCompatible`** — the repo enables these for shipping libraries;
  they add analyzer overhead and errors for samples that don't need AOT.
- **`TargetFramework` inference** — the repo's TFW pipeline is complex; samples just
  need `net11.0`.

### The tradeoff

Full isolation means samples don't get *any* repo-level settings — no shared analyzers,
no repo version properties, no common packaging configuration. They are effectively
standalone console apps that happen to live inside the repo tree. This is safe and
simple, but it can silently drift from repo conventions over time.

### What would change to accept repo settings

To let the repo's build infrastructure flow through instead:

1. **Import the parent chain and override selectively** — replace the current
   `Directory.Build.props` with one that imports the repo-level file and then overrides
   only the properties that break samples (`IsSourceProject`, `PublishAot`, etc.).
   Do the same for `.targets`.

2. **Fix sample warnings instead of suppressing them** — remove the blanket
   `TreatWarningsAsErrors=false` and either fix the warnings or suppress specific
   codes with `<NoWarn>`.

3. **Remove the hardcoded `TargetFramework`** — let the repo's TFW inference set it,
   or use a shared property, so samples stay in sync when the repo rolls forward.

4. **Validate against the full repo build** — run `build.cmd` / `build.sh` to confirm
   that samples aren't pulled into packaging, signing, or CI pipelines. The
   `IsSourceProject=false` override should prevent this, but it needs verification
   against Arcade's project-discovery logic.

5. **Update `eng/run-samples.sh`** — if repo settings flow in, samples may need to be
   built with the repo SDK (`.dotnet/dotnet`) rather than a global SDK, and the script
   may need to pass additional properties or adjust working directories.

The core tension is that full isolation is fragile to drift (samples silently diverge
from repo conventions) while full inheritance is fragile to breakage (repo build changes
break unrelated samples). The current approach chose isolation as the safer default for
a first pass.

## Batching work into PRs

At scale, the migration will produce changes across dozens of libraries. Merging
everything in one PR would be unreviewable, but one PR per type creates too much
overhead. Several batching strategies are viable:

### Option A: One PR per library (recommended)

Each PR covers all types within a single library assembly (e.g., `System.Collections`,
`System.Private.CoreLib`, `System.Net.Http`). This is the natural unit because:

- A library's `samples/` directory, `samples.json`, and `Directory.Build.props` are
  self-contained — no cross-library dependencies.
- Reviewers familiar with a library can review all its types together.
- The sample runner can validate just that library's samples before merge.
- PR size stays manageable: the collections pilot covered 11 types across 2 libraries
  and the total diff is modest.

### Option B: One PR per namespace

For large libraries with many namespaces (e.g., `System.Private.CoreLib` spans
`System`, `System.Collections.Generic`, `System.Threading`, etc.), split by namespace.
This keeps PRs smaller but requires more coordination on shared files like
`samples.json`.

### Option C: Infrastructure first, then content

Separate the scaffolding (`Directory.Build.props`, `Directory.Build.targets`,
`samples.json` skeleton, `eng/run-samples.sh`, CI integration) into a single
infrastructure PR. Subsequent PRs add only sample files and `samples.json` entries.
This is useful if the infrastructure needs its own review cycle — especially the
`Directory.Build.props` isolation tradeoff and CI wiring — before the fleet starts
producing content.

### General guidance

Regardless of batching strategy:

- **Doc migration (`///` comments) and samples should ship in the same PR** — they are
  produced by the same tool run and validated together. Splitting them risks partial
  states where doc comments reference samples that don't exist yet.
- **Re-run the tool on a fresh branch before opening the PR** — since `///` comments
  are a computed output (see design goals), regenerating on a fresh branch avoids
  merge conflicts with any `main` changes that touched the same files.
- **Include the `slash quality` before/after output in the PR description** — this
  gives reviewers a quick summary of what improved without reading every diff line.
- **Re-run after merge to update incrementally** — because `///` comments are derived
  and the tool works at any scope, a merged library can be re-run at any time. For
  example, after the initial `System.Collections` merge, a follow-up PR could add new
  samples, remove outdated ones, or pick up upstream XML doc edits from `dotnet-api-docs`
  — just re-run `slash merge` against the same files and only the delta shows up in the
  diff. This is especially relevant if `dotnet-api-docs` continues to be edited in
  parallel for some transition period.
