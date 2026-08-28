---
name: tasks-driven-tdd
description: Implement following OpenSpec tasks.md with TDD gating (test → code → refactor).
---

# Tasks-Driven TDD

When an OpenSpec change set is active, implement task by task following `openspec/changes/<feature>/tasks.md`.

## Workflow

1. Read the next pending task in `tasks.md`.
2. Mentally mark the first checkbox you will attack.
3. Write the FAILING test (red). Confirm it fails for the right reason.
4. Implement the MINIMUM code to pass (green). No extras.
5. Refactor while keeping green.
6. Mark the checkbox. Move to the next one.
7. If a task does not apply, document it and move on.

## Rules

- ❌ Do not advance more than one task without verifying.
- ❌ Do not write implementation code before the test.
- ✅ If you find ambiguity, ask before inventing.
- ✅ If you find a bug, open a new change set, do NOT fix it inline.
- ✅ Commit per completed task — format per [`commit`](../commit/SKILL.md) skill.

## Output per task

```
## Task X.Y: <name>
- Test added: <path> (verified red)
- Implementation: <path> (verified green)
- Refactor: <what and why>
- Verification: command + output
```
