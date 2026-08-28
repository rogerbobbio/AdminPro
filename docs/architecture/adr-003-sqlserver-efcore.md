# ADR-003: SQL Server + EF Core 10

## Status
Accepted

## Context
We need robust relational persistence with good .NET support.

## Decision
- **SQL Server 2022**
- **EF Core 10** as main ORM with code-first migrations
- **Repository Pattern** over EF Core
- **Centralized UnitOfWork**
- **Testcontainers** for integration tests

## Consequences

**Positive:**
- End-to-end strong typing
- Versioned migrations
- Realistic tests with Testcontainers

**Rules:**
- DbContext **only in Infrastructure**
- `AsNoTracking()` in read queries
- DTO projections, do not return entities
- Explicit indexes in `IEntityTypeConfiguration<>`
