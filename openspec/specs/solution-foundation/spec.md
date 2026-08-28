## Purpose

Defines the backend foundation AdminPro is built on: the DDD solution structure, the domain entity model, persistence (soft-delete, migrations), and the CQRS pipeline (MediatR + FluentValidation behaviors, global exception handling). Every later capability (Dashboard, Projects, Applications, Services, Search, ...) builds on top of this without needing to re-establish these mechanics.

## Requirements

### Requirement: Solution structure
The backend SHALL be organized as a .NET 10 solution `AdminPro.slnx` (the .NET 10 SDK's default solution format) with four DDD layers — `AdminPro.Domain`, `AdminPro.Application`, `AdminPro.Infrastructure`, `AdminPro.Api` — plus `AdminPro.Application.Tests` and `AdminPro.Api.Tests`, matching `docs/design/DESIGN.md` §2.1. Project references: `Domain` has none; `Infrastructure` references `Domain`; `Application` references `Domain` and `Infrastructure` (per the project's no-repository decision — handlers and pipeline behaviors use `AppDbContext` directly, per `docs/design/DESIGN.md` §1.2/§2.4); `Api` references `Application` and `Infrastructure`.

#### Scenario: Solution builds
- **WHEN** `dotnet build AdminPro.slnx` is run from `backend/`
- **THEN** all six projects compile with zero errors

#### Scenario: Domain has no outward dependencies
- **WHEN** the `AdminPro.Domain.csproj` file is inspected
- **THEN** it references no other project in the solution and no ORM/web framework package

### Requirement: Domain entities
`AdminPro.Domain` SHALL contain all entities from `docs/business-rules.md` §2.2 (Modulo, Project, BaseDeDatos, Application, Ambiente, Reporte, Nota, Documento, FixData, Servicio, AplicacionServicio), each implementing `IAuditableEntity` (`Id`, `Activo`, `CreatedAt`, `UpdatedAt`) as defined in `docs/design/DESIGN.md` §2.2. `AplicacionServicio` is the sole exception — it is a pure link entity (composite key `AplicacionId`+`ServicioId`) and does NOT implement `IAuditableEntity`.

#### Scenario: Entity implements audit contract
- **WHEN** a `Project` entity instance is created
- **THEN** it exposes `Id`, `Activo`, `CreatedAt`, and `UpdatedAt` members satisfying `IAuditableEntity`

### Requirement: Persistence with soft-delete filtering
`AppDbContext` SHALL configure every entity via `IEntityTypeConfiguration<T>` classes and SHALL apply a global query filter so inactive rows (`Activo == false`) are excluded from default queries, per rule INF-EF-002. `AplicacionServicio` has no such filter, since it has no `Activo` column.

#### Scenario: Inactive row excluded by default
- **GIVEN** a `Project` row exists with `Activo = false`
- **WHEN** the row is queried through `AppDbContext.Projects` without `IgnoreQueryFilters()`
- **THEN** the row is not returned

#### Scenario: Inactive row visible with explicit override
- **GIVEN** a `Project` row exists with `Activo = false`
- **WHEN** the row is queried through `AppDbContext.Projects.IgnoreQueryFilters()`
- **THEN** the row is returned

### Requirement: Cascade and audit behavior
`Project -> BaseDeDatos`/`Application` foreign keys SHALL use `DeleteBehavior.Restrict` (soft cascade is implemented in the Application layer, per rule INF-EF-003 — this deliberately diverges from the literal "CASCADE DELETE" wording in `docs/business-rules.md`'s table notes). `Application`'s own children (Ambiente, Reporte, Nota, Documento, FixData, AplicacionServicio) SHALL use hard `DeleteBehavior.Cascade`. `Servicio.ProyectoId` SHALL use `DeleteBehavior.SetNull`. `AppDbContext.SaveChangesAsync` SHALL set `UpdatedAt` to `UtcNow` on every `Modified` `IAuditableEntity`, and SHALL NOT touch `CreatedAt`/`UpdatedAt` on newly `Added` entities (handlers set those explicitly on create, per rule APP-CMD-001).

#### Scenario: Modified entity gets a fresh UpdatedAt
- **GIVEN** a tracked `Project` entity is modified
- **WHEN** `SaveChangesAsync` is called
- **THEN** `UpdatedAt` is set to the current UTC time and `CreatedAt` is unchanged

### Requirement: Initial migration creates empty schema
The `InitialCreate` EF Core migration SHALL create all tables and indexes defined in `docs/business-rules.md` §2.2 and §5.1 (rule INF-EF-005) against SQL Server, and SHALL NOT insert any rows. This is an explicit product decision: rule MOD-004 ("ship with a default Module") is deliberately not implemented by this migration.

#### Scenario: Fresh database has no data
- **WHEN** `dotnet ef database update` is run against a new `AdminPro` database
- **THEN** every table exists with zero rows, including `Modulos`

### Requirement: CQRS pipeline behaviors run in order
MediatR SHALL execute `ValidationBehavior`, then `LoggingBehavior`, then `TransactionBehavior` for every request, per `docs/design/DESIGN.md` §2.4. `TransactionBehavior` detects commands via the non-MediatR `ICommandMarker` interface (implemented by both `ICommand` and `ICommand<TResponse>`) rather than via `IRequest`/`IRequest<TResponse>` directly — a command type cannot implement both without making `ISender.Send(...)` ambiguous at every call site. Query handlers SHALL NOT be wrapped in a database transaction, per rule INF-MED-002.

#### Scenario: Validation failure short-circuits the pipeline
- **GIVEN** a request with a registered `IValidator` that fails validation
- **WHEN** the request is sent through `ISender`
- **THEN** a `ValidationException` is thrown before the handler executes, and no transaction is opened

#### Scenario: Command runs inside a transaction
- **GIVEN** a request implementing `ICommand`/`ICommand<TResponse>` that passes validation
- **WHEN** the request is sent through `ISender`
- **THEN** the handler executes inside an EF Core transaction that commits on success and rolls back on an unhandled exception

#### Scenario: Query does not open a transaction
- **GIVEN** a request that does NOT implement `ICommandMarker` (a query)
- **WHEN** the request is sent through `ISender`
- **THEN** `TransactionBehavior` passes through without starting a database transaction

### Requirement: Global exception handling
The API SHALL use a global exception-handling middleware that maps exceptions to HTTP status codes and a structured JSON error body, per rule XCUT-ERR-001.

#### Scenario: Validation exception maps to 400
- **GIVEN** a `ValidationException` is thrown while handling a request
- **WHEN** the middleware catches it
- **THEN** the response is `400 Bad Request` with body `{ "error": "ValidationError", "message": "...", "details": [...] }`

#### Scenario: Unhandled exception maps to 500
- **GIVEN** an unexpected exception is thrown while handling a request
- **WHEN** the middleware catches it
- **THEN** the response is `500 Internal Server Error` with a structured error body and the exception is logged at `Error` level
