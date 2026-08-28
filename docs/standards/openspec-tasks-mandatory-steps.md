# OpenSpec Tasks — Mandatory Steps

Every `tasks.md` inside `openspec/changes/<feature>/` **MUST** follow this structure.

## Mandatory Structure

### 0. Setup (MANDATORY FIRST)
- Branch `feature/SCRUM-XXX-<slug>`
- Draft PR pointing to `main`

### 1. Specs & Design
- `proposal.md` reviewed
- `design.md` with technical decisions
- `specs/<capability>/spec.md` with Given/When/Then scenarios

### 2-N. Implementation (TDD per layer)
Follow the TDD workflow defined in [`tasks-driven-tdd`](../../ai-specs/skills/tasks-driven-tdd/SKILL.md) for every layer.

### N. Backend Domain (TDD)
### N+1. Backend Application (TDD)
### N+2. Backend Infrastructure
### N+3. Backend API (TDD)
### N+4. Backend Integration Tests (Testcontainers)

### N+5. Frontend Service (TDD)
### N+6. Frontend Component (TDD)
### N+7. Frontend E2E

### Last. Documentation + Final
- API spec updated
- Minimum coverage met
- Lint + build OK
- Ready for `opsx:archive`

## Rules

- ❌ Do not mark checkboxes without verifying
- ❌ Do not skip tests
- ❌ Do not advance more than 1 task without commit + verification
- ✅ Each completed task = one commit, format per [`commit`](../../ai-specs/skills/commit/SKILL.md) skill
- ✅ If you find ambiguity, escalate with the `product-strategy-analyst`
