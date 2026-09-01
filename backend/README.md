# AdminPro Backend

.NET 10 / C# 14 backend for AdminPro, built with Clean Architecture (4 DDD layers) + CQRS via MediatR. See `openspec/changes/foundation-backend/` for the proposal/design/specs behind this scaffold, and `docs/business-rules.md` / `docs/design/DESIGN.md` at the repo root for the full domain model.

## Solution layout

```
backend/
├── AdminPro.slnx
├── src/
│   ├── AdminPro.Domain/           # Entities, interfaces, domain exceptions — no dependencies
│   ├── AdminPro.Application/      # CQRS commands/queries, validators, pipeline behaviors
│   ├── AdminPro.Infrastructure/   # EF Core AppDbContext, entity configs, migrations
│   └── AdminPro.Api/              # Controllers, middleware, Program.cs composition root
└── tests/
    ├── AdminPro.Application.Tests/  # Unit tests (Domain + Application layers)
    └── AdminPro.Api.Tests/          # WebApplicationFactory + Testcontainers integration tests
```

Project references: `Domain` has none; `Infrastructure` → `Domain`; `Application` → `Domain`, `Infrastructure` (no Repository pattern — handlers and pipeline behaviors use `AppDbContext` directly); `Api` → `Application`, `Infrastructure`.

## Prerequisites

- .NET 10 SDK
- SQL Server LocalDB (instance `(localdb)\MSSQLLocalDB`) — or adjust the connection string for another SQL Server instance
- Docker Desktop running (only required to run the Testcontainers-based integration tests in `AdminPro.Api.Tests`)
- `dotnet-ef` global tool: `dotnet tool install --global dotnet-ef`

## First-time setup

1. Copy `src/AdminPro.Api/appsettings.json`'s `ConnectionStrings:DefaultConnection` into a local `src/AdminPro.Api/appsettings.Development.json` (gitignored) with your real connection string, e.g.:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=AdminPro;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```
2. Apply the migrations:
   ```bash
   dotnet ef database update --project src/AdminPro.Infrastructure --startup-project src/AdminPro.Api
   ```
   This creates the `AdminPro` database with all 11 domain tables. The `InitialCreate` migration itself inserts no rows (intentional — see `openspec/changes/archive/2026-08-28-foundation-backend/design.md`); a later `SeedModulos` migration adds the 2 real navigation modules (see `openspec/changes/frontend-dashboard/`).

## Running

```bash
cd src/AdminPro.Api
dotnet run
```

The API boots with Serilog logging to console and `logs/adminpro-*.log`. Live endpoints so far:
- `GET /api/modulos`, `GET /api/dashboard/summary` (Phase 2 — Dashboard)
- `GET/POST/PUT/DELETE /api/projects`, `POST /api/projects/{id}/basesdedatos`, `PUT/DELETE /api/basesdedatos/{id}` (Phase 3 — Projects & Databases)
- `GET /api/projects/{id}/applications`, `POST /api/projects/{id}/applications`, `GET/PUT/DELETE /api/applications/{id}`, `POST /api/applications/{id}/ambientes`, `PUT/DELETE /api/ambientes/{id}` (Phase 4 — Applications & Ambientes)

Report/Note/Document/FixData/Service endpoints arrive in later phases, screen by screen.

With `ASPNETCORE_ENVIRONMENT=Development` (the default via `launchSettings.json`), you get:
- **Scalar UI** (API explorer): `https://localhost:7293/scalar/v1`
- Raw OpenAPI document: `https://localhost:7293/openapi/v1.json`

## Testing

```bash
# Unit tests (Domain + Application) — fast, no external dependencies
dotnet test tests/AdminPro.Application.Tests

# Integration tests — requires Docker running (spins up a real SQL Server container)
dotnet test tests/AdminPro.Api.Tests

# Everything
dotnet test AdminPro.slnx
```

## Adding a new migration

```bash
dotnet ef migrations add <Name> --project src/AdminPro.Infrastructure --startup-project src/AdminPro.Api --output-dir Persistence/Migrations
dotnet ef database update --project src/AdminPro.Infrastructure --startup-project src/AdminPro.Api
```
