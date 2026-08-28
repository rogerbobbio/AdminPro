# Documentation Standards

## Docs Structure

```
docs/
├── base-standards.md        # Hard rules
├── standards/               # Standards per area
│   ├── backend-standards.md
│   ├── frontend-standards.md
│   ├── openspec-tasks-mandatory-steps.md
│   └── documentation-standards.md
├── architecture/            # ADRs
├── api-spec.yml             # OpenAPI
├── data-model.md
└── audits/
```

## Rules

- **Markdown** for everything
- **One ADR per significant architectural decision** (format: Context, Decision, Consequences)
- **Diagrams**: Mermaid inside the .md

## ADR Template

```markdown
# ADR-XXX: <title>

## Status
Proposed | Accepted | Deprecated

## Context
<what problem, what options>

## Decision
<what we decided>

## Consequences
<positive and negative>
```

## Maintenance

- Every code PR must update docs if applicable
- OpenSpec change sets reference ADRs when applicable
