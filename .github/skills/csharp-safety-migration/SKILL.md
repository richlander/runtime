---
name: csharp-safety-migration
description: Migrate C# code to the proposed safety scheme with minimal inner unsafe blocks, explicit outer safety decisions, and `// SAFETY:` documentation.
---

# C# Safety Migration

Use this skill to migrate existing C# code to the proposed safety model used in the runtime safety experiment.

## When to Use This Skill

Use this skill when:
- A method directly uses `Unsafe.*` or another unsafe-equivalent helper
- You need to convert broad `unsafe` regions into narrow inner `unsafe {}` blocks
- You need to decide whether a method should stay outer `unsafe` or become an outward-safe boundary
- You want to add or review `// SAFETY:` comments that describe the remaining caller obligation
- You are preparing code for the explicit-`safe` or implicit-safe experiment variants

## Inputs

Before editing:
1. Identify the requested style:
   - `safe_style = explicit`
   - `safe_style = implicit`
2. Read the full file, not just the local hunk.
3. Inspect immediate callers and nearby helpers that participate in the safety proof.
4. Check whether the existing guards are real runtime checks or only `Debug.Assert` statements.

## Safety Model

### Rule 1: Unsafe-equivalent operations need a minimal inner `unsafe {}` block

Treat the following as unsafe-equivalent operations unless a narrower project rule says otherwise:

- `Unsafe.*`
- `MemoryMarshal.*`
- `SequenceMarshal.*`
- `Buffer.Memmove`
- `stackalloc`
- `fixed`
- low-level ref reinterpretation or unchecked ref arithmetic

Use [dotnet/runtime#41418](https://github.com/dotnet/runtime/issues/41418) as guidance for APIs that should be reviewed as unsafe-equivalent from a type-safety or memory-safety perspective.

Keep the inner `unsafe {}` block as small as possible. It should cover only the exact sequence that depends on unsafety.

### Rule 2: Every inner `unsafe {}` forces an outer safety decision

Once a method contains an inner `unsafe {}` block, decide whether the method:

- remains **outer `unsafe`** because the caller still has a real obligation, or
- is **outward-safe** because the method fully discharges the obligation internally

### Rule 3: Explicit vs. implicit `safe` is only a presentation switch

The migration logic is the same in both variants:

- **`safe_style = explicit`**: mark outward-safe methods with `safe`
- **`safe_style = implicit`**: leave outward-safe methods unmarked

Do not invent different proof rules for the two modes.

### Rule 4: Outer `unsafe` methods need a `// SAFETY:` comment

Every outer `unsafe` method should include a `// SAFETY:` comment that states the remaining obligation precisely.

Good `// SAFETY:` comments:
- explain what invariant must hold
- say who is responsible for maintaining it
- mention the guard or validation that makes the unsafe sequence valid

Bad `// SAFETY:` comments:
- restate the code without explaining the obligation
- hand-wave about performance or internal trust
- avoid saying what could go wrong

If you cannot write a convincing `// SAFETY:` comment, the method is probably not a good safe-boundary candidate.

### Rule 5: `Debug.Assert` is not a safety proof in release builds

If the safety argument depends on a check, prefer a real runtime guard over `Debug.Assert`.

## Migration Workflow

### Step 1: Inventory likely migration sites

Start with grep, not the LSP. Useful searches include:

```bash
rg -n '\bunsafe\b|Unsafe\.|MemoryMarshal\.|SequenceMarshal\.|Buffer\.Memmove|stackalloc|fixed' src/
rg -n 'Debug\.Assert|ThrowHelper|ArgumentOutOfRange|ArgumentException' src/
```

For the explicit-`safe` variant, `safe` roots should also be easy to enumerate:

```bash
rg -n '\bsafe\b' src/
```

### Step 2: Isolate the minimal unsafe sequence

Move direct unsafe-equivalent operations into a narrow inner `unsafe {}` block. Do not leave the whole method broad `unsafe` unless the entire method truly carries caller obligations.

### Step 3: Decide the outer boundary honestly

Ask:
1. Does this method fully validate inputs and preserve the necessary invariants?
2. Is any remaining obligation still pushed onto the caller?
3. Would a reviewer understand the safety proof from the code and comment?

Decision rule:
- If the obligation is discharged locally, the method is outward-safe.
- If the caller must still uphold a nontrivial invariant, the method stays outer `unsafe`.

### Step 4: Add or strengthen guards

When an unsafe sequence relies on length, type, alignment, null, pinning, or aliasing assumptions:
- enforce those assumptions with real runtime checks when needed
- avoid relying only on debug-time assertions
- keep the proof close to the unsafe sequence

### Step 5: Propagate only when necessary

Keep the diff narrow:
- migrate the target method first
- touch immediate callers or wrappers only when the contract requires it
- avoid subsystem-wide rewrites unless the experiment specifically needs broader coverage

## Tooling Guidance

### Current shipping C#

- Use grep first to enumerate candidate roots.
- Use the compiler and C# LSP second for symbol navigation, type information, and call flow confirmation.

### Design-stage vnext variants

- Do **not** rely on the shipping compiler or official C# LSP to understand the new syntax model.
- Use grep, direct source inspection, and narrow scripts if needed.
- For the explicit-`safe` variant, ordinary grep should be sufficient for root discovery; do not assume a special script is required.

## Deliverable

When you finish, provide:
1. The patch
2. A short explanation of each outer `unsafe` or outward-safe decision
3. The `// SAFETY:` comments you added
4. Any methods that could not honestly be made outward-safe
