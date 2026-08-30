## ADDED Requirements

### Requirement: List active modules
`GET /api/modulos` SHALL return active `Modulo` rows ordered by `Orden` ascending, per rule APP-QRY-001 and `docs/business-rules.md` §3.11 (Dashboard Rules).

#### Scenario: Only active modules returned, in order
- **GIVEN** three `Modulo` rows exist with `Orden` 0, 1, 2, and the row with `Orden` 1 has `Activo = false`
- **WHEN** `GET /api/modulos` is called
- **THEN** the response contains only the rows with `Orden` 0 and 2, in that order

### Requirement: Seed default modules
The database SHALL be seeded via EF Core migration with the two implemented modules — `Gestión de Proyectos` (`RutaBase: proyectos`, `Orden: 0`) and `Catálogo de Servicios` (`RutaBase: servicios`, `Orden: 1`) — both `Activo = true`, matching rule MOD-004. `Presupuesto` is NOT seeded as a `Modulo` row; it remains a static "coming soon" UI element only.

#### Scenario: Fresh database has the two seeded modules
- **WHEN** `dotnet ef database update` runs against a new `AdminPro` database
- **THEN** `GET /api/modulos` returns exactly two modules: "Gestión de Proyectos" and "Catálogo de Servicios"

### Requirement: Dashboard summary
`GET /api/dashboard/summary` SHALL return a single `DashboardSummaryDto` aggregating: total active `Project` count, total active `Application` count, total active `Ambiente` count, total active `Servicio` linked to at least one `Application` (via `AplicacionServicio`), a 7-entry array of `Application` counts created per day for the last 7 calendar days (oldest first), up to 5 most-recently-created active `Application` rows (name, project name, stack, status), and a status breakdown where every active `Application` counts toward `Activo` (100%) until a real application-status rule is introduced (per this change's design decision).

#### Scenario: Empty database returns all zeros
- **GIVEN** no `Project`, `Application`, `Ambiente`, or `Servicio` rows exist
- **WHEN** `GET /api/dashboard/summary` is called
- **THEN** all counts are `0`, the 7-day series is `[0,0,0,0,0,0,0]`, `recentApplications` is an empty array, and the status breakdown reports `activo: 0, enProgreso: 0, pendiente: 0`

#### Scenario: Summary reflects real data
- **GIVEN** 2 active `Project` rows and 3 active `Application` rows exist, one created today
- **WHEN** `GET /api/dashboard/summary` is called
- **THEN** `totalProjects` is `2`, `totalApplications` is `3`, the 7-day series' last entry (today) is at least `1`, and all 3 applications count toward `activo` in the status breakdown

#### Scenario: Inactive rows excluded
- **GIVEN** a `Project` row exists with `Activo = false`
- **WHEN** `GET /api/dashboard/summary` is called
- **THEN** that project is not included in `totalProjects`, consistent with the global soft-delete query filter from `solution-foundation`
