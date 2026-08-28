# ADR-002: CQRS with MediatR

## Status
Accepted

## Context
System use cases need to be orchestrated consistently.

## Decision
We adopt **CQRS + MediatR** with vertical slices:
- Each operation = separate Command or Query
- Handlers decoupled from Controller via `ISender`
- FluentValidation validators auto-registered
- Result pattern (`ErrorOr<T>`) instead of exceptions

## Consequences

**Positive:**
- Ultra-thin controllers
- Separate, testable validation
- Extensible MediatR pipeline (logging, retry, transactions)

**Negative:**
- More files per feature
- MediatR learning curve
