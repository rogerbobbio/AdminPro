---
name: commit
description: Generate Conventional Commits in English with semantic scope.
---

# Conventional Commits

## Format

```
<type>(<scope>): <description in english, lowercase, imperative>

[optional body]
```

## Types

| Type         | When                                                   |
|--------------|--------------------------------------------------------|
| `feat`       | New feature for the user                                |
| `fix`        | Bug fix                                                 |
| `refactor`   | Internal change without behavior change                 |
| `test`       | Tests only                                              |
| `docs`       | Documentation only                                      |
| `chore`      | Build, deps, config                                     |
| `perf`       | Performance improvement                                 |
| `style`      | Formatting, prettier                                    |
| `ci`         | CI/CD changes                                           |

## Project Scopes

- `be` → backend
- `fe` → frontend
- `auth`, `products`, `orders` → bounded context (adapt to your project's actual contexts)
- `openspec` → openspec/ changes
- `deps` → dependencies
- `docs` → docs/

## Examples

```bash
feat(be): add product creation endpoint with TDD
fix(fe): fix product list empty state on refresh
refactor(be): extract email validation to value object
test(be): add integration test for product creation
docs(openspec): update auth spec with refresh token scenario
chore(deps): bump @angular/core to v22.0.0
```
