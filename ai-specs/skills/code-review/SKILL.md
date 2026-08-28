---
name: code-review
description: Pre-merge code review checklist focused on DDD, TDD, and Clean Architecture.
---

# Code Review Checklist

## Before starting

- [ ] Is there a PR linked to an OpenSpec change set?
- [ ] Is the task in `tasks.md` marked?

## Backend (.NET 10 / DDD)

- [ ] Is Domain layer free of EF Core, ASP.NET, JSON?
- [ ] Are repository interfaces in `Domain/Interfaces/`?
- [ ] Are use cases MediatR Commands/Queries?
- [ ] Validation with FluentValidation in Application?
- [ ] Business errors with Result/ErrorOr, not exceptions?
- [ ] Tests for Domain, Application, Infrastructure, API?
- [ ] Migration reviewed (non-destructive)?
- [ ] Does Swagger document the new endpoint?

## Frontend (Angular 22)

- [ ] Standalone component with OnPush?
- [ ] Signals or RxJS, no `any`?
- [ ] Lazy loading on the feature route?
- [ ] No hardcoded CSS (only Bootstrap or SCSS tokens)?
- [ ] Basic accessibility (labels, roles, keyboard)?
- [ ] Unit tests for component and service?
- [ ] E2E test for the happy path?

## General

- [ ] Commits in Conventional Commits in English?
- [ ] No secrets in code?
- [ ] Coverage did not drop?
- [ ] CI passes (build + tests + lint)?

## Output

- ✅ Approved with minor suggestions
- ⚠️ Comments that must be resolved
- ❌ Blocker
