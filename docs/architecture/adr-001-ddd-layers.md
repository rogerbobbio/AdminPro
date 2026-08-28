# ADR-001: DDD 4-Layer Architecture

## Status
Accepted

## Context
We need to separate concerns to support a complex, evolvable domain, avoid framework coupling, and enable testability.

## Decision
We adopt **DDD 4-Layer Architecture** with strict dependencies:
`Domain ← Application ← Infrastructure ← API`.

- **Domain**: entities, value objects, domain events, repository interfaces. No external dependencies.
- **Application**: use cases (Commands/Queries with MediatR), validators, DTOs, service interfaces. Depends only on Domain.
- **Infrastructure**: EF Core, SQL Server, JWT, Email, File storage. Implements Domain/Application interfaces.
- **API**: Controllers, Middleware, Filters, Swagger. HTTP entry point.

## Consequences

**Positive:**
- Pure domain, testable without frameworks
- Infrastructure substitution without touching domain
- Clear pyramid testability

**Negative:**
- More projects (.csproj) to maintain
- Initial boilerplate
- Learning curve for devs unfamiliar with DDD
