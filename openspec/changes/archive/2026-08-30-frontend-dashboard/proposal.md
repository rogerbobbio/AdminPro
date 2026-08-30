## Why

Phase 1 (`foundation-backend`) delivered only the DDD scaffold (entities, `AppDbContext`, MediatR pipeline, exception middleware) — there are no feature endpoints and `frontend/` is empty. Per `docs/business-rules.md` §"Implementation Phases", Phase 2 is "Dashboard & Module Launcher". The user has a finished visual design for the dashboard (a static HTML mockup built with Claude Design) that goes beyond the simple module-card wireframe in `docs/design/DESIGN.md` §5.1 — it adds stat cards, a weekly activity chart, a reminder card, a recent-applications table, and an application-status donut. The dashboard must read this data from the real API (even while the database is empty) so that data the user enters later through the app shows up automatically — not from hardcoded/mock values.

## What Changes

- Scaffold the Angular 22 frontend app (`frontend/`) per `docs/design/DESIGN.md` §3.1: standalone components, Signals, OnPush, Bootstrap 5 + Bootstrap Icons, root routing with lazy loading, HTTP client, dev proxy to the API.
- Add the minimum backend surface needed for the Dashboard to show real (not mocked) data — no other CRUD yet:
  - `GetModulosQuery` + `GET /api/modulos` — active modules ordered by `Orden` (rule APP-QRY-001 / PRES-UI-002).
  - `GetDashboardSummaryQuery` + `GET /api/dashboard/summary` — aggregate counts (total projects, applications, environments, linked services), a weekly applications-created series, up to N recent applications with stack/status, and an applications-by-status breakdown, all computed from existing tables (naturally zero/empty until the user starts entering data).
- Build the `DashboardComponent` (route `/`) reproducing the Claude Design mockup layout (`dashboard.html`) 1:1 in Angular + Bootstrap 5: sidebar nav (with a disabled/"coming soon" Presupuesto module), topbar (search, icon buttons, user), stat cards, weekly bar chart, reminder card, module list, recent-applications table, and status donut — all populated from `DashboardService` (Signals-based) hitting the two endpoints above, with empty-state handling (rule set aside for genuinely no data, e.g. 0 projects → "0" counts, empty chart/table).
- Seed the `Modulos` table with the two real modules (`Gestión de Proyectos`, `Catálogo de Servicios`) via migration `HasData`, per `docs/business-rules.md` rule MOD-004 (Phase 1's `InitialCreate` deliberately skipped this — see `openspec/specs/solution-foundation/spec.md` "Initial migration creates empty schema"). Presupuesto stays a static "coming soon" tile, not a DB row, matching the mockup.
- Cypress E2E test: dashboard loads, stat cards render, module cards are visible, clicking the "Gestión de Proyectos" module attempts navigation (target route is a placeholder until that module's change lands).

Out of scope for this change (future changes, screen by screen): Projects/Applications/Ambientes/Servicios CRUD (backend or frontend), the `proyectos` module shell/sidebar, global search, and the reminder card's real data source (it stays a static placeholder card in this change since there's no "reminders" entity in the domain model).

## Capabilities

### New Capabilities
- `frontend-shell`: Angular 22 application scaffold — standalone bootstrap, root routing with lazy loading, Bootstrap 5 theming, HTTP client configuration, dev proxy to the backend API.
- `dashboard`: The module-launcher Dashboard screen — layout, stat cards, weekly chart, module list, recent-applications table, status donut, all backed by real API data via Signals.
- `dashboard-api`: Backend read endpoints (`GET /api/modulos`, `GET /api/dashboard/summary`) that power the Dashboard, plus the `Modulos` seed data.

### Modified Capabilities
(none — `solution-foundation` requirements are unchanged; this change only adds new query handlers/controllers on top of it)

## Impact

- **New**: `frontend/` (entire Angular app), `backend/src/AdminPro.Application/Modulos/Queries/GetModulos/`, `backend/src/AdminPro.Application/Dashboard/Queries/GetDashboardSummary/`, `backend/src/AdminPro.Api/Controllers/ModulosController.cs`, `DashboardController.cs`, a new EF Core migration adding `Modulos` seed rows.
- **Affected code**: none existing modified beyond DI registration in `Program.cs` (register the two new MediatR handlers — already generic via assembly scanning per Phase 1, so likely no change needed) and `AppDbContext`'s `OnModelCreating` (add `HasData` for `Modulo`).
- **Dependencies**: none new; reuses the CQRS pipeline, `AppDbContext`, and exception middleware from `solution-foundation`.
