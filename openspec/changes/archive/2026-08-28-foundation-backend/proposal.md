## Why

AdminPro currently has no backend code — `backend/` is empty. Every future feature (Phase 2 onward: Dashboard, Projects, Applications, Services, Search) depends on having the solution scaffold, the persistence model, and the CQRS pipeline in place first. This change delivers that foundation so subsequent phases can add commands/queries incrementally on top of a working, tested base.

## What Changes

- Create the .NET 10 solution `AdminPro.sln` with 4 DDD layers: `AdminPro.Domain`, `AdminPro.Application`, `AdminPro.Infrastructure`, `AdminPro.Api`, plus `AdminPro.Application.Tests` and `AdminPro.Api.Tests`.
- Add all Domain entities from `docs/business-rules.md` §2.2 (Modulo, Project, BaseDeDatos, Application, Ambiente, Reporte, Nota, Documento, FixData, Servicio, AplicacionServicio), each implementing `IAuditableEntity`.
- Add `AppDbContext` with fluent configuration per entity, the soft-delete global query filter (rule INF-EF-002), and the indexes from rule INF-EF-005.
- Add the initial EF Core migration (`InitialCreate`) targeting SQL Server LocalDB (`(localdb)\MSSQLLocalDB`, Windows Authentication, database `AdminPro`). **No seed data** — the schema is created empty (explicit product decision; this intentionally leaves rule MOD-004 "ship with a default Module" unimplemented for now).
- Add the MediatR + FluentValidation pipeline: `ValidationBehavior`, `LoggingBehavior`, `TransactionBehavior` (design.md §2.4). No business commands/queries yet — that starts in Phase 2.
- Add the base `ApiController`, `ExceptionHandlerMiddleware` (rule XCUT-ERR-001), `Program.cs` DI wiring, and Serilog configuration (rules INF-LOG-001/002).
- Frontend is explicitly **out of scope** for this change — Angular scaffold will be a separate change set once the UI designs are ready.

## Capabilities

### New Capabilities
- `solution-foundation`: DDD solution scaffold (4 layers + test projects), all domain entities, `AppDbContext` with soft-delete filters and indexes, the initial empty migration against LocalDB, the MediatR/FluentValidation pipeline behaviors, and the base API/middleware/logging setup. No business use cases (commands/queries) and no seed data.

### Modified Capabilities
(none — first backend capability in the project)

## Impact

- **New code**: `backend/AdminPro.sln` and all 6 projects under `backend/src/` and `backend/tests/`.
- **Database**: creates a new local database `AdminPro` on `(localdb)\MSSQLLocalDB` via `dotnet ef database update` — no impact on any existing database.
- **No API endpoints yet** beyond the middleware/DI plumbing (no controllers with real actions — those arrive with Phase 2's Dashboard/Module capability).
- **Dependencies added**: MediatR, FluentValidation, EF Core (SqlServer provider), Serilog (+ sinks), xUnit/FluentAssertions/NSubstitute for tests.
