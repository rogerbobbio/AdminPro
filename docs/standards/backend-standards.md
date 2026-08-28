# Backend Standards (.NET 10 + DDD + SQL Server)

## Stack

- .NET 10 LTS · C# 14 · ASP.NET Core 10
- EF Core 10 + SQL Server 2022
- MediatR (CQRS) + FluentValidation
- Serilog (structured logging)
- JWT (Microsoft.AspNetCore.Authentication.JwtBearer) + BCrypt
- xUnit + FluentAssertions + NSubstitute + Testcontainers

## Layers

See [ADR-001](../architecture/adr-001-ddd-layers.md) for the layer definition and dependency rule, and [`ddd-layered-implementation`](../../ai-specs/skills/ddd-layered-implementation/SKILL.md) for anti-patterns and where each thing goes.

## Code Conventions

- NAMESPACE = folder path: `ProyectInit.Application.Candidates.Commands.CreateCandidate`
- Immutable records for Commands/Queries: `public record CreateCandidateCommand(...) : IRequest<ErrorOr<Guid>>;`
- Validators with FluentValidation, in `Application/Validators/` or co-located
- Business errors with `ErrorOr<T>`, **never** exceptions for normal flow
- DTOs in `Application/DTOs/`, do not expose Domain entities
- Constructor injection, `sealed` by default
- `async/await` for IO, avoid `.Result`/`.Wait()`

## EF Core

- Migrations in `Infrastructure/Persistence/Migrations/`
- Configurations per entity in `Infrastructure/Persistence/Configurations/`
- `DbContext` only in Infrastructure
- `IUnitOfWork` with centralized `SaveChangesAsync`
- Unique indexes on `Email`, `Username`, etc.

## API

- Version in URL: `/api/v1/...`
- Controllers with `[ApiController]`, `[Route("api/v1/[controller]")]`
- Correct status codes: 200/201/204/400/401/403/404/409/422/500

## Auth

- JWT access (15-60 min) + refresh token (7-30 days)
- Hash with BCrypt (work factor ≥ 11)
- Claims: `sub`, `email`, `role`, `tenant_id` (if multi-tenant)

## Testing

- **Unit (Domain)**: pure logic, no mocks
- **Unit (Application)**: handlers with NSubstitute for repos
- **Integration (Infrastructure)**: Testcontainers real SQL Server
- **API**: `WebApplicationFactory<Program>` + FluentAssertions

Minimum coverage: **80%** lines, **70%** branches.

## Performance

- Mandatory pagination in lists
- `AsNoTracking()` in read-only queries
- DTO projections in Application

## Logging

- Structured logging with Serilog
- `LogInformation` for business events, `LogWarning` for recoverable failures
- Include `CorrelationId` (middleware) in all logs
- **DO NOT log**: passwords, JWT, PII
