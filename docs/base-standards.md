---
description: Hard rules of the project. Apply to ALL AI agents (Claude, Cursor, Codex, Gemini).
alwaysApply: true
---

# Base Standards — ProyectInit

**Welcome!** This is your complete guide to the ProyectInit project. Start here for all rules, agents, skills, and workflow.

---

## 📖 Reading Order

**ALWAYS read in this order before starting any task:**

1. ✅ **This file** — Core principles & spec-driven workflow
2. ✅ [`docs/standards/backend-standards.md`](standards/backend-standards.md) — .NET 10 + DDD rules
3. ✅ [`docs/standards/frontend-standards.md`](standards/frontend-standards.md) — Angular 22 rules
4. ✅ [`docs/standards/openspec-tasks-mandatory-steps.md`](standards/openspec-tasks-mandatory-steps.md) — OpenSpec workflow
5. ✅ Relevant `.claude/sessions/context_session_{feature_name}.md` — Feature-specific context

---

## 1. Core Principles

- **Small tasks, one at a time**: Baby steps. Never advance more than one step.
- **Test-Driven Development**: mandatory, no exceptions — see [`tasks-driven-tdd`](../ai-specs/skills/tasks-driven-tdd/SKILL.md) for the full workflow.
- **Type Safety**: All code 100% typed. No `any`, no `dynamic`.
- **Clear Naming**: Variables, functions, and classes with clear intent.
- **Incremental Changes**: Small, focused changes > massive changes.
- **Question Assumptions**: Question assumptions before acting.
- **Pattern Detection**: Detect and elevate repeated patterns.

## 2. Language

- **100% English**: code, comments, error messages, logs, commits, docs, tickets.

## 3. Spec-Driven Development (RULE #1)

⚠️ **No production code is written without an approved change set in `openspec/changes/`.**

Mandatory workflow:
1. `/opsx:propose "feature"` → creates `openspec/changes/<USDOL130-XXXX>/` with proposal, design, tasks, specs
2. Review and refine artifacts (with the `product-strategy-analyst` agent)
3. `/opsx:apply` → implements task by task with TDD
4. `/opsx:archive` → merges deltas into `openspec/specs/`

## 4. DDD 4-Layer Architecture (Backend)

Full definition (layers, contents, dependency rule) lives in [`docs/architecture/adr-001-ddd-layers.md`](architecture/adr-001-ddd-layers.md). For implementation details (where each thing goes, anti-patterns) see [`ai-specs/skills/ddd-layered-implementation/SKILL.md`](../ai-specs/skills/ddd-layered-implementation/SKILL.md).

## 5. Angular 22 (Frontend)

- Standalone components, OnPush change detection, Signals for local state
- **One feature = one folder in `frontend/src/app/features/<bounded-context>/`**
- Lazy loading per feature
- Bootstrap 5 (classes, no hardcoded CSS)
- See `docs/standards/frontend-standards.md` for details.

## 6. Project Skills

- Skills live in `ai-specs/skills/`.
- When a task matches a skill, **load it automatically** before continuing.

### Available Skills

Load these automatically when your task matches:

| Skill | Purpose |
|-------|---------|
| [`angular-feature-scaffold`](../ai-specs/skills/angular-feature-scaffold/SKILL.md) | Create Angular features with DDD |
| [`artifact-design`](../ai-specs/skills/artifact-design/SKILL.md) | Frontend design: palette, typography, layout — deliberate visual identity |
| [`code-review`](../ai-specs/skills/code-review/SKILL.md) | AI-powered code review |
| [`commit`](../ai-specs/skills/commit/SKILL.md) | Conventional commits |
| [`ddd-layered-implementation`](../ai-specs/skills/ddd-layered-implementation/SKILL.md) | DDD 4-layer scaffolding |
| [`security-audit`](../ai-specs/skills/security-audit/SKILL.md) | Security review |
| [`tasks-driven-tdd`](../ai-specs/skills/tasks-driven-tdd/SKILL.md) | TDD workflow |

## 7. Specialized Agents

Choose the right agent for your task:

### 🤖 Backend Developer
📄 [`ai-specs/agents/backend-developer.md`](../ai-specs/agents/backend-developer.md)

**When to use:** .NET 10, DDD layers, EF Core, SQL Server, MediatR, tests

- C# 14, ASP.NET Core 10
- Domain → Application → Infrastructure → API
- TDD with xUnit + FluentAssertions
- CQRS with MediatR + FluentValidation
- JWT + Refresh tokens

### 🤖 Frontend Developer
📄 [`ai-specs/agents/frontend-developer.md`](../ai-specs/agents/frontend-developer.md)

**When to use:** Angular 22, component design, signals, lazy loading, Cypress

- Standalone components + OnPush change detection
- Signals for local state
- Feature-based structure: `features/{bounded-context}/`
- Bootstrap 5 styling
- Cypress E2E testing

### 🤖 Product Strategy Analyst
📄 [`ai-specs/agents/product-strategy-analyst.md`](../ai-specs/agents/product-strategy-analyst.md)

**When to use:** Feature proposals, spec refinement, task breakdown, design decisions

- Writes `openspec/changes/{USDOL130-XXXX}/proposal.md`
- Designs architecture & dependencies
- Breaks down into tasks with acceptance criteria
- Reviews & refines specs

## 8. Symlink Integrity

- `ai-specs/` is the **canonical source**.
- `.claude/`, `.cursor/`, and other IDEs access via symlinks.
- A change is **incomplete** if it leaves broken symlinks.

## 9. Mandatory OpenSpec Updates Post-Apply

If a new fix request arrives between `opsx:apply` and `opsx:archive`, treat it as a **spec update first**, not as a quick patch.

## 10. Critical Rules

🚫 **NEVER:**
- Write production code without approved OpenSpec changeset
- Mix business logic in API layer
- Use exceptions for normal business flow
- Commit secrets, passwords, JWT keys
- Log PII or sensitive data

✅ **ALWAYS:**
- Work in baby steps, one task at a time
- Follow TDD (see [`tasks-driven-tdd`](../ai-specs/skills/tasks-driven-tdd/SKILL.md))
- Keep Domain layer PURE (no frameworks)
- Use Result pattern instead of exceptions
- Include 3-5 lines of context in file diffs
- Commit format per [`commit`](../ai-specs/skills/commit/SKILL.md) skill
- Save session context in `.claude/sessions/`
- Document work in `.claude/doc/{feature_name}/`

## 11. Typical Workflow

```
1. User runs: /opsx:propose "feature name"
   ↓
2. Agent proposes spec with tasks
   ↓
3. User reviews & refines
   ↓
4. User runs: /opsx:apply
   ↓
5. Agent executes task-by-task with TDD
   ↓
6. User runs: /opsx:archive
   ↓
7. Spec merged into openspec/specs/
```

## 12. Project Structure

```
ProyectInit/
├── 📁 docs/                    ← Rules (READ FIRST)
│   ├── base-standards.md       ← YOU ARE HERE
│   ├── architecture/
│   │   ├── adr-001-ddd-layers.md
│   │   ├── adr-002-cqrs-mediator.md
│   │   └── adr-003-sqlserver-efcore.md
│   ├── standards/
│   │   ├── backend-standards.md
│   │   ├── frontend-standards.md
│   │   ├── documentation-standards.md
│   │   └── openspec-tasks-mandatory-steps.md
│   ├── design/
│       └── DESIGN.md
│
├── 📁 ai-specs/                ← Instructions for AI
│   ├── agents/
│   │   ├── backend-developer.md
│   │   ├── frontend-developer.md
│   │   └── product-strategy-analyst.md
│   └── skills/
│       ├── angular-feature-scaffold/
│       ├── ddd-layered-implementation/
│       ├── tasks-driven-tdd/
│       ├── code-review/
│       ├── commit/
│       ├── artifact-design/
│       └── security-audit/
│
├── 📁 .claude/                 ← Claude IDE config
│   ├── settings.json
│   ├── sessions/               ← Feature context storage
│   └── doc/                    ← Feature documentation
│
├── 📁 backend/                 ← ASP.NET Core 10 + DDD
│   └── src/
│       ├── Domain/
│       ├── Application/
│       ├── Infrastructure/
│       └── API/
│
├── 📁 frontend/                ← Angular 22
│   └── src/
│       └── app/
│           └── features/
│
└── 📁 openspec/                ← Spec-Driven Development
    ├── changes/
    └── specs/
```

## 13. Quick Start Checklist

**You're ready when:**

1. ✅ You've read this entire file (`docs/base-standards.md`)
2. ✅ You understand the 4-layer DDD architecture
3. ✅ You know your role (backend/frontend/product analyst)
4. ✅ You have the feature context from `.claude/sessions/`

**Next:** Wait for the user to run `/opsx:apply` or ask a specific task.

## 14. Tech Stack & Important Links

- **Backend:** .NET 10 + ASP.NET Core 10 + EF Core + SQL Server 2022
- **Frontend:** Angular 22 + Bootstrap 5 + TypeScript 5.6
- **Testing:** xUnit + FluentAssertions (backend), Cypress (frontend)
- **Methodology:** Spec-Driven Development with OpenSpec
- **Architecture:** Domain-Driven Design (4-layer)
- **Language:** 100% English in code, comments, commits
- **Version Control:** Git — commit format per [`commit`](../ai-specs/skills/commit/SKILL.md) skill

## 15. Specific Standards

For detailed rules on specific areas, see:

- [Backend .NET 10](standards/backend-standards.md) — C#, EF Core, MediatR, DDD
- [Frontend Angular 22](standards/frontend-standards.md) — Components, Signals, Cypress
- [Documentation](standards/documentation-standards.md) — Docs format & content
- [OpenSpec Tasks Mandatory Steps](standards/openspec-tasks-mandatory-steps.md) — How to execute tasks

## 16. Security

- ❌ Never commit secrets, passwords, JWT secrets
- ❌ Never log passwords, tokens, PII
- ✅ `.env` in `.gitignore`, `.env.example` committed
- ✅ JWT secret ≥ 256 bits random
- ✅ HTTPS mandatory in production

---

**Status:** ✅ Ready to work  
**Last Updated:** 2026-06-30  
**Version:** 2.0 (Consolidated)
