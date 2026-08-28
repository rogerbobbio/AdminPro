## Context

`backend/` is currently empty. `docs/design/DESIGN.md` already specifies the target solution structure, entity shapes, DbContext, and pipeline behaviors for the full AdminPro backend (all phases). This design document scopes down to exactly what Phase 1 (Foundation) needs, and records the decisions specific to this environment (LocalDB, no seed data) that aren't already settled by `docs/design/DESIGN.md`.

## Goals / Non-Goals

**Goals:**
- Stand up a buildable, DDD-layered .NET 10 solution matching `docs/design/DESIGN.md` §2.1.
- Model the full domain (all 11 entities from `docs/business-rules.md` §2.2) so later phases only add behavior, not schema surgery.
- Get one working migration against local SQL Server (LocalDB) with an empty schema.
- Wire the cross-cutting plumbing (MediatR pipeline, validation, logging, exception handling) so Phase 2+ commands/queries plug in without re-touching `Program.cs`.

**Non-Goals:**
- No business commands/queries (no Create/Update/Delete/Get handlers) — Phase 2 onward.
- No controllers with real actions — only the abstract `ApiController` base.
- No seed data of any kind, including the default "Gestión de Proyectos" module from rule MOD-004. That rule is explicitly deferred, not implemented incorrectly.
- No Angular/frontend work.

## Decisions

**Connection target: LocalDB via Windows Authentication, not the `Server=(localdb)\mssqllocaldb` example in `docs/design/DESIGN.md` §9.3.**
`docs/design/DESIGN.md`'s example connection string is close but the actual instance name confirmed by the user is `(localdb)\MSSQLLocalDB` (uppercase, default named instance). Connection string: `Server=(localdb)\MSSQLLocalDB;Database=AdminPro;Trusted_Connection=True;MultipleActiveResultSets=true`, stored in `appsettings.Development.json` (gitignored) — `appsettings.json` keeps a placeholder only, since this repo is meant to be portable to other machines/environments later.

**No seed data.**
`docs/business-rules.md` rule MOD-004 ("ship with at least one default Module") is intentionally not implemented in this change — explicit user decision. The migration creates schema only. Revisit when the Dashboard/Module capability (Phase 2) is proposed; if MOD-004 is still desired then, seed it there instead of here.

**No Repository pattern, no AutoMapper** (already decided in `docs/design/DESIGN.md` §1.2/§8) — carried forward as-is: handlers use `AppDbContext` directly, mapping is manual. No new decision needed, just confirming this Foundation change doesn't introduce either.

**Pipeline behavior order**: `ValidationBehavior` → `LoggingBehavior` → `TransactionBehavior`, per `docs/design/DESIGN.md` §2.4 registration order. Validation fails fast before any logging/transaction cost; logging wraps the transaction so duration includes commit/rollback.

**Migrations project**: EF Core migrations live in `AdminPro.Infrastructure` (per `docs/design/DESIGN.md` §2.1 `Persistence/Migrations/`), with `AdminPro.Api` as the startup project for `dotnet ef` commands — matches the `dotnet ef migrations add ... --project src/AdminPro.Infrastructure --startup-project src/AdminPro.Api` commands already documented in `docs/design/DESIGN.md` §9.1.

## Risks / Trade-offs

- **No seed data means the app is unusable end-to-end until Phase 2 adds a way to create a Modulo/Project.** → Acceptable for this change; Foundation's success criterion is "compiles + migrates cleanly + pipeline behaviors are unit-tested," not "usable UI flow."
- **LocalDB is developer-machine-specific.** → Connection string lives in `appsettings.Development.json` (not committed), so moving to a shared/real SQL Server later is a config change, not a code change.
- **Modeling all 11 entities now, before any of their commands/queries exist, risks the schema being wrong once real CRUD rules are implemented in later phases** → Mitigated by keeping entity classes exactly as specified in `docs/design/DESIGN.md` §2.2 and `docs/business-rules.md` §2.2 (already fully designed), and by not seeding data, so schema drift so far only costs a migration regeneration, not data migration.

## Migration Plan

```bash
dotnet ef migrations add InitialCreate --project backend/src/AdminPro.Infrastructure --startup-project backend/src/AdminPro.Api
dotnet ef database update --project backend/src/AdminPro.Infrastructure --startup-project backend/src/AdminPro.Api
```
Rollback: `dotnet ef database update 0` (drops all applied migrations) or `dotnet ef database drop` since this is a fresh local database with no data to preserve.

## Open Questions

None blocking — MOD-004 (default module seed) is deferred by decision, not an open question.
