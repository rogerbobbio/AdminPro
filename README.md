# ProyectInit

Full-stack system based on **DDD (Domain-Driven Design)**, developed with **Spec-Driven Development** using OpenSpec + Claude Code.

## What's in this repo?

Only the **Markdown specifications** that Claude reads to generate all the code (.NET 10 backend, Angular 22 frontend, SQL Server DB).

```
.
├── docs/        ← Rules, standards, ADRs (what Claude MUST follow)
├── ai-specs/    ← Skills + specialized agents (how Claude WORKS)
├── .claude/     ← Claude Code config + symlinks to docs/ and ai-specs/
└── openspec/    ← (Claude will create it with /opsx:init)
```

## Setup

```bash
# 1. Initialize OpenSpec (generates the /opsx:* commands)
npm install -g @fission-ai/openspec@latest
openspec init    # choose "Claude Code"

# 2. (Optional) Create the project folders where Claude will generate code
#    Recommended:
mkdir -p backend frontend
```

## Workflow

1. **Propose** → `> /opsx:propose "feature name"`
2. **Refine** → edit `openspec/changes/<feature>/proposal.md` and `tasks.md`
3. **Apply** → `> /opsx:apply` (Claude implements task by task with TDD)
4. **Archive** → `> /opsx:archive`

## Critical Rules (summary)

- **TDD mandatory**: tests before code
- **DDD 4 layers**: Domain ← Application ← Infrastructure ← API
- **100% English** in code, commits, messages
- **One feature = one change set** in `openspec/changes/`

## More

- Full rules: [`docs/base-standards.md`](docs/base-standards.md)
- Backend standard: [`docs/standards/backend-standards.md`](docs/standards/backend-standards.md)
- Frontend standard: [`docs/standards/frontend-standards.md`](docs/standards/frontend-standards.md)
- OpenSpec: [`docs/standards/openspec-tasks-mandatory-steps.md`](docs/standards/openspec-tasks-mandatory-steps.md)
