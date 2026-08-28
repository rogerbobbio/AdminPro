## 0. Setup (MANDATORY FIRST)

- [x] 0.1 Create branch `feature/foundation-backend`
- [x] 0.2 Open Draft PR against `main` (https://github.com/rogerbobbio/AdminPro/pull/1)

## 1. Specs & Design

- [x] 1.1 `proposal.md` reviewed
- [x] 1.2 `design.md` with technical decisions
- [x] 1.3 `specs/solution-foundation/spec.md` with Given/When/Then scenarios

## 2. Backend Domain (TDD)

- [x] 2.1 Create `AdminPro.slnx` (the .NET 10 SDK's new default solution format, replacing `.sln`) and the 6 projects (`AdminPro.Domain`, `AdminPro.Application`, `AdminPro.Infrastructure`, `AdminPro.Api`, `AdminPro.Application.Tests`, `AdminPro.Api.Tests`) under `backend/src/` and `backend/tests/`, wired per the corrected reference graph in `specs/solution-foundation/spec.md` (Domain: none; Infrastructure → Domain; Application → Domain, Infrastructure; Api → Application, Infrastructure). Verified: `dotnet build AdminPro.slnx` succeeds with 0 errors; `AdminPro.Domain.csproj` has no `ProjectReference`/package beyond the SDK.
- [x] 2.2 TDD: write a failing test in `AdminPro.Application.Tests` asserting `IAuditableEntity` exposes `Id`, `Activo`, `CreatedAt`, `UpdatedAt`; implement `IAuditableEntity` in `AdminPro.Domain/Interfaces/`. Verified red (CS0234/CS0246 — type didn't exist) then green (1/1 passed).
- [x] 2.3 TDD: write a failing test instantiating `Modulo` and asserting it satisfies `IAuditableEntity` plus its own properties (`Nombre`, `Icono`, `RutaBase`, `Color`, `Orden`) from `docs/business-rules.md` §2.2.1; implement `Modulo`. Verified red (CS0234) then green (2/2 passed).
- [x] 2.4 TDD: repeat 2.3 for `Project` (`docs/business-rules.md` §2.2.2). Verified red (CS0234) then green (batch run, 12/12 passed).
- [x] 2.5 TDD: repeat 2.3 for `BaseDeDatos` (§2.2.3). Verified red then green (batch run, 12/12 passed).
- [x] 2.6 TDD: repeat 2.3 for `Application`, including navigation collections (`Ambientes`, `Reportes`, `Notas`, `Documentos`, `FixDatas`, `AplicacionServicios`) per `docs/design/DESIGN.md` §2.2 (§2.2.4). Verified red then green (batch run, 12/12 passed).
- [x] 2.7 TDD: repeat 2.3 for `Ambiente` (§2.2.5). Verified red then green (batch run, 12/12 passed).
- [x] 2.8 TDD: repeat 2.3 for `Reporte` (§2.2.6). Verified red then green (batch run, 12/12 passed).
- [x] 2.9 TDD: repeat 2.3 for `Nota` (§2.2.7). Verified red then green (batch run, 12/12 passed).
- [x] 2.10 TDD: repeat 2.3 for `Documento` (§2.2.8). Verified red then green (batch run, 12/12 passed).
- [x] 2.11 TDD: repeat 2.3 for `FixData` (§2.2.9). Verified red then green (batch run, 12/12 passed).
- [x] 2.12 TDD: repeat 2.3 for `Servicio` (§2.2.10). Verified red then green (batch run, 12/12 passed).
- [x] 2.13 TDD: write a failing test for `AplicacionServicio` composite-key shape (`AplicacionId` + `ServicioId`, `NotasEspecificas`); implement it (§2.2.11). Verified red then green (batch run, 12/12 passed). Deliberately does NOT implement `IAuditableEntity` — it's a pure link entity per business-rules.md §2.2.11 (no `Id`/`Activo` column).
- [x] 2.14 Add `DomainException` base class in `AdminPro.Domain/Exceptions/`.

## 3. Backend Application (TDD)

- [x] 3.1 Add MediatR and FluentValidation NuGet packages to `AdminPro.Application` (MediatR pinned to 12.5.0 — the last version before its v13 commercial license change, mirroring the same decision already made for FluentAssertions), Microsoft.EntityFrameworkCore to `AdminPro.Infrastructure` (needed for the `AppDbContext` shell below), and NSubstitute + Microsoft.EntityFrameworkCore.Sqlite to `AdminPro.Application.Tests` (Sqlite supports real transactions for testing `TransactionBehavior`, unlike the EF Core InMemory provider). Verified: `dotnet build AdminPro.slnx` succeeds with 0 errors.
- [x] 3.2 Define the `ICommand`/`ICommand<T>` marker interfaces used by `TransactionBehavior` to tell commands from queries (rule INF-MED-002). Verified red (CS0234/CS0246) then green (14/14 passed).
- [x] 3.3 TDD: write a failing test proving that when a registered validator fails, the pipeline throws `ValidationException` and the inner handler is never invoked; implement `ValidationBehavior` (`docs/design/DESIGN.md` §2.4). Verified red (CS0234) then green (19/19 passed, including the no-validators-registered and validation-succeeds cases).
- [ ] 3.4 TDD: write a failing test proving `LoggingBehavior` logs at `Information` level on entry and exit with the request type name; implement `LoggingBehavior` (rule INF-LOG-001)
- [ ] 3.5 TDD: write a failing test proving `TransactionBehavior` opens/commits an EF Core transaction for `ICommand` requests and rolls back on exception, and passes through untouched for non-command (query) requests; implement `TransactionBehavior`
- [ ] 3.6 Register the three behaviors in order (Validation → Logging → Transaction) via an `AddApplicationServices` DI extension method
- [x] 3.7 Add the Application-layer `ValidationException` (`Common/Exceptions/ValidationException.cs`) carrying FluentValidation failures. Done ahead of 3.3-3.5 since `ValidationBehavior` needs it to compile. Verified red (CS0234) then green (16/16 passed).

## 4. Backend Infrastructure

- [ ] 4.1 Add EF Core + SQL Server provider NuGet packages to `AdminPro.Infrastructure`
- [ ] 4.2 Implement `AppDbContext` with a `DbSet<T>` for all 11 entities and `ApplyConfigurationsFromAssembly` in `OnModelCreating` (`docs/design/DESIGN.md` §2.3)
- [ ] 4.3 TDD: write a failing test (EF Core InMemory provider) proving a row with `Activo = false` is excluded from a default query but returned with `IgnoreQueryFilters()`; implement the global soft-delete query filter for every entity (rule INF-EF-002)
- [ ] 4.4 Add the `IEntityTypeConfiguration<T>` classes for all 11 entities under `Persistence/Configurations/`, including the indexes from rule INF-EF-005 and the cascade-delete rules from rule INF-EF-003
- [ ] 4.5 TDD: write a failing test proving `SaveChangesAsync` sets `UpdatedAt` to `UtcNow` on modified entities; implement the `SaveChangesAsync` override (`docs/design/DESIGN.md` §2.3)
- [ ] 4.6 Generate the initial migration: `dotnet ef migrations add InitialCreate --project backend/src/AdminPro.Infrastructure --startup-project backend/src/AdminPro.Api` — verify the migration contains **no** `HasData`/seed calls
- [ ] 4.7 Apply it locally: `dotnet ef database update --project backend/src/AdminPro.Infrastructure --startup-project backend/src/AdminPro.Api` against `(localdb)\MSSQLLocalDB`; verify with `sqlcmd` that database `AdminPro` exists, all tables exist, and every table has zero rows

## 5. Backend API (TDD)

- [ ] 5.1 Add MediatR, Serilog (+ Console/File sinks), and Swagger NuGet packages to `AdminPro.Api`
- [ ] 5.2 Implement the abstract `ApiController` base class (`docs/design/DESIGN.md` §2.6)
- [ ] 5.3 TDD: write a failing `WebApplicationFactory` test proving a `ValidationException` thrown by a test endpoint returns `400` with body `{ "error": "ValidationError", ... }`; implement `ExceptionHandlerMiddleware` for this case (rule XCUT-ERR-001)
- [ ] 5.4 TDD: write a failing test proving an unhandled exception returns `500` with a structured error body and is logged at `Error` level; extend `ExceptionHandlerMiddleware`
- [ ] 5.5 Configure Serilog in `Program.cs` per rules INF-LOG-001/INF-LOG-002 (structured fields: Command/Query name, DurationMs)
- [ ] 5.6 Wire `Program.cs`: `AppDbContext` (SQL Server, connection string from configuration), MediatR, FluentValidation validators, the three pipeline behaviors, the exception middleware
- [ ] 5.7 Add `appsettings.json` with a placeholder `DefaultConnection`, and `appsettings.Development.json` (confirm it's covered by `.gitignore`) with the real LocalDB connection string `Server=(localdb)\MSSQLLocalDB;Database=AdminPro;Trusted_Connection=True;MultipleActiveResultSets=true`

## 6. Backend Integration Tests (Testcontainers)

- [ ] 6.1 Add Testcontainers.MsSql to `AdminPro.Api.Tests`; write a smoke test that boots the API against a containerized SQL Server, applies migrations, and confirms the app starts with an empty `AdminPro` schema
- [ ] 6.2 Integration test proving the full pipeline order end-to-end: send a test-only command through `ISender` against the containerized DB and assert Validation → Logging → Transaction executed in order (via captured logs) and the transaction committed

## 7. Documentation + Final

- [ ] 7.1 Add `backend/README.md` with local run instructions (`dotnet restore`, `dotnet ef database update`, `dotnet run`) per `docs/design/DESIGN.md` §12
- [ ] 7.2 `dotnet build AdminPro.sln` succeeds with zero warnings treated as errors where configured
- [ ] 7.3 `dotnet test` — all unit and integration tests green
- [ ] 7.4 Confirm every task above was committed individually per the `commit` skill format
- [ ] 7.5 Mark this `tasks.md` complete and ready for `/opsx:archive`
