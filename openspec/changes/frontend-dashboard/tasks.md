## 0. Setup (MANDATORY FIRST)

- [x] 0.1 Create branch `feature/frontend-dashboard`
- [ ] 0.2 Open Draft PR against `main` (`gh` CLI unavailable in this environment — branch pushed to `origin/feature/frontend-dashboard`; open manually at https://github.com/rogerbobbio/AdminPro/pull/new/feature/frontend-dashboard)

## 1. Specs & Design

- [x] 1.1 `proposal.md` reviewed
- [x] 1.2 `design.md` with technical decisions
- [x] 1.3 `specs/frontend-shell/spec.md`, `specs/dashboard-api/spec.md`, `specs/dashboard/spec.md` with Given/When/Then scenarios

## 2. Backend Application — Modulos query (TDD)

- [x] 2.1 TDD: write a failing test for `GetModulosQuery`/`GetModulosQueryHandler` proving it returns only `Activo = true` modules ordered by `Orden` ascending (rule APP-QRY-001); implement the query + handler under `AdminPro.Application/Modulos/Queries/GetModulos/`, returning a `ModuloDto` (Id, Nombre, Icono, RutaBase, Color, Orden).
- [x] 2.2 Add `SeedModulos` EF Core migration (`Persistence/Migrations/`) with `HasData` for the two real modules (`Gestión de Proyectos` orden 0, `Catálogo de Servicios` orden 1) per `specs/dashboard-api/spec.md` "Seed default modules". Apply locally with `dotnet ef database update`; verify via `sqlcmd` that `Modulos` has exactly 2 rows.

## 3. Backend Application — Dashboard summary query (TDD)

- [x] 3.1 TDD: write a failing test proving `GetDashboardSummaryQuery` returns all-zero counts and empty lists against an empty database; implement `DashboardSummaryDto`/`RecentApplicationDto`/`ApplicationStatusBreakdownDto` and the handler skeleton (counts only) under `AdminPro.Application/Dashboard/Queries/GetDashboardSummary/`. Combined with 3.2-3.5 below: all 5 scenarios were written as one test file up front (they exercise the same handler and would otherwise force repeated red states on unrelated assertions), then the full handler implemented to turn all 5 green together. Verified red (CS0234 - namespace didn't exist) then green (5/5 passed).
- [x] 3.2 TDD: write a failing test seeding 2 active Projects + 3 active Applications (1 inactive) and asserting `totalProjects`/`totalApplications` reflect only active rows; extend the handler to compute `TotalProjects`, `TotalApplications`, `TotalAmbientes`, `TotalServiciosVinculados` via `AppDbContext` (no repository). See 3.1 note — implemented together.
- [x] 3.3 TDD: write a failing test asserting `ApplicationsCreatedLast7Days` has 7 entries, oldest first, with today's entry reflecting an Application created "now"; implement the day-bucketed aggregation. See 3.1 note — implemented together.
- [x] 3.4 TDD: write a failing test asserting `RecentApplications` returns at most 5 rows ordered by `CreatedAt` descending with project name/stack/status populated; implement, including the placeholder status rule (every active Application → `Activo`, per design.md Decision 4). See 3.1 note — implemented together.
- [x] 3.5 TDD: write a failing test asserting `StatusBreakdown` reports all active applications under `activo` and `0` for `enProgreso`/`pendiente`; implement. See 3.1 note — implemented together.

## 4. Backend API (TDD)

- [x] 4.1 TDD: write a failing `WebApplicationFactory` test for `GET /api/modulos` asserting the seeded 2 modules are returned in order; implement `ModulosController`. Added a new `InMemoryApiFactory` test fixture (swaps `AppDbContext` to an isolated InMemory database per test run via `UseInternalServiceProvider`, since Program.cs's real SqlServer registration and an added InMemory registration otherwise conflict) rather than reusing `TestingWebApplicationFactory`/`ContainerizedApiFactory`, keeping controller tests fast and DB-isolated; full SQL Server behavior is covered by the Testcontainers test in 4.3. Verified red (CS0234 - controller didn't exist) then green (2/2 passed).
- [x] 4.2 TDD: write a failing `WebApplicationFactory` test for `GET /api/dashboard/summary` against an empty DB asserting the all-zero shape from `specs/dashboard-api/spec.md`; implement `DashboardController`. Same `InMemoryApiFactory` fixture as 4.1. Verified red then green (2/2 passed).
- [x] 4.3 Backend integration test (Testcontainers, matching the pattern from `foundation-backend` task 6.1): boot against a containerized SQL Server, run migrations (confirming the `SeedModulos` migration applies), call both endpoints, assert real seeded/aggregate data. Also fixed a regression this change introduced in the existing `ContainerizedSmokeTests` (from `foundation-backend`): it asserted `Modulos` count is `0` post-migration, which is no longer true now that `SeedModulos` inserts 2 rows — updated the assertion to `2`. Docker Desktop's daemon is not running in this dev environment (`docker info` fails to connect to the named pipe), same pre-existing gap affecting `ContainerizedSmokeTests`/`PipelineOrderIntegrationTests` from `foundation-backend` — verified the new test compiles cleanly (`dotnet build` 0 errors) but could NOT be run/verified green here; needs a run with Docker Desktop active before merge.

## 5. Frontend Scaffold

- [x] 5.1 `ng new adminpro-ui` (or equivalent Angular 22 CLI scaffold) inside `frontend/`, standalone + routing, per `docs/design/DESIGN.md` §3.1. Required bumping the dev machine's active Node version via nvm-windows (22.12.0 → 22.22.3 LTS) since Angular CLI 22 requires ≥22.22.3/24.15.0/26.0.0. Verified: `ng build` succeeds (0 errors), `ng serve` boots and `curl http://localhost:4200` returns `200`.
- [x] 5.2 Install and configure Bootstrap 5 + Bootstrap Icons; add `proxy.conf.json` pointing `/api` at the backend's HTTPS URL and wire it into `angular.json`'s serve target. Deferred the "reaches the backend" verification to when the Dashboard component actually calls the API (section 8) — the proxy config itself is wired and `ng serve` boots cleanly with it in place.
- [x] 5.3 Build `styles.scss` mapping the mockup's CSS custom properties (colors, radii, shadows from `dashboard.html`) onto Bootstrap 5 Sass variables/overrides, per `specs/frontend-shell/spec.md` "Bootstrap 5 theme mapped to the design mockup". Used `@use "bootstrap/scss/bootstrap" with (...)` (not the deprecated `@import`) to override `$theme-colors`, border-radius scale (incl. `$border-radius-xl: 22px`, matching Bootstrap 5.3's native xl/xxl scale), shadows, and font stack. Verified: `ng build` succeeds (0 errors); compiled CSS confirms `.btn-primary{--bs-btn-bg: #0c3b29}`, matching the mockup's `--g-900`.

## 6. Frontend Shell Component (TDD)

- [x] 6.1 TDD: write a failing test asserting `AppShell` marks the nav item matching its `activeNav` input as active and no others; implement `AppShell` (sidebar: brand, nav items, Presupuesto promo card; topbar: search, icon buttons, user block), reproducing `dashboard.html`'s sidebar/topbar markup 1:1. Angular 22's default project uses Vitest (`@angular/build:unit-test`), not Karma — updated task description accordingly. Named the class `AppShell` (no `Component` suffix), matching Angular 22's scaffolded style (see `App` in `app.ts`). Verified red (TS2307 - module didn't exist) then green (3/3 new tests + 2 existing passed).
- [x] 6.2 Wire root routing: `app.routes.ts` renders `Dashboard` at `/` and a shared `ComingSoon` component at `/proyectos` (per design.md's resolved open question). Went with content-projection (`AppShell` wraps `<ng-content>`, each page passes `[activeNav]` directly) rather than a layout route wrapping `<router-outlet>`, per design.md Decision 1's "or content projection" — simpler to unit test and avoids coupling `AppShell` to router internals. Enabled `withComponentInputBinding()` so `ComingSoon`'s `title`/`activeNav` inputs bind from route `data`. Verified: `ng build` 0 errors, `ng test` all green.

## 7. Frontend Dashboard Service (TDD)

- [x] 7.1 TDD: write a failing test asserting `ModuloService.loadModulos()` populates a `modulos` signal from `GET /api/modulos`; implement the service (`HttpClient` + `firstValueFrom`, per `docs/design/DESIGN.md` §3.2 pattern). Added `Modulo`/`DashboardSummary` TS interfaces under `shared/models/` matching the backend DTOs' camelCase JSON shape (ASP.NET Core's default `System.Text.Json` naming policy). Verified red (TS2307) then green (1/1 passed).
- [x] 7.2 TDD: write a failing test asserting `DashboardService.loadSummary()` populates a `summary` signal (typed to the `DashboardSummaryDto` shape) from `GET /api/dashboard/summary`, including a `loading`/`error` signal pair; implement the service. Added a second test for the error path (`req.error(...)` → `error` signal set, `loading` reset to `false`). Verified red then green (2/2 passed, 7/7 total across all specs).

## 8. Frontend Dashboard Component (TDD)

- [x] 8.1 TDD: write a failing test asserting the 4 stat cards render the values from `DashboardService.summary()` (including the all-zero case); implement the stat-card grid markup/component (`StatCards`). Verified red (TS2307) then green (2/2 passed).
- [x] 8.2 TDD: write a failing test asserting the weekly bar chart renders 7 bars with height derived from `applicationsCreatedLast7Days`, including the flat/minimum-height case when all values are `0`; implement the chart (`WeeklyChart`, plain template bindings + a `computed()` signal, no charting library, per design.md Decision 6). Verified red then green (2/2 passed).
- [x] 8.3 Implement the static reminder card (`ReminderCard`; no test needed — purely presentational, per `specs/dashboard/spec.md` "Static reminder card").
- [x] 8.4 TDD: write a failing test asserting the module list renders modules from `ModuloService.modulos()` plus the static Presupuesto tile, and that clicking an active module navigates via `Router`; implement (`ModuleList`). Verified red then green (2/2 passed).
- [x] 8.5 TDD: write a failing test asserting the recent-applications table renders rows from `summary().recentApplications` and an empty-state message when the array is empty; implement the table (`RecentApplicationsTable`). Verified red then green (2/2 passed).
- [x] 8.6 TDD: write a failing test asserting the status donut's computed `conic-gradient` and center label reflect `statusBreakdown` (100%/0%/0% case); implement the donut (`StatusDonut`, CSS `conic-gradient` via a `computed()` signal, per design.md Decision 6). Verified red then green (2/2 passed, 17/17 total across all specs).
- [x] 8.7 Assemble `Dashboard` wiring all of the above together behind `AppShell`, calling `loadModulos()`/`loadSummary()` on `ngOnInit`. Deviation from the literal spec wording: the reminder card and module list render unconditionally (module list reactively empty until `ModuloService` resolves) while only the stat-cards/chart/table/donut widgets that need `summary()` show a "Cargando..."/error fallback — this was required to satisfy `specs/dashboard/spec.md`'s "Reminder card renders without a data dependency" scenario, which an earlier draft (gating everything behind one `@if (summary)`) violated. Manually verified end-to-end: ran the real backend (`dotnet run`, port 7293) + `ng serve` (proxying `/api` per `proxy.conf.json`, corrected from the docs' placeholder port 5001 to the actual `launchSettings.json` port 7293) and screenshotted the live page with Playwright — renders correctly against the real (empty) database, matches the approved mockup layout, zero console errors, and clicking "Gestión de Proyectos" navigates to `/proyectos` and renders the `ComingSoon` placeholder.

## 9. Frontend E2E

- [ ] 9.1 Cypress test: dashboard loads at `/`, all 4 stat cards are visible, the "Gestión de Proyectos" and "Catálogo de Servicios" module entries are visible.
- [ ] 9.2 Cypress test: clicking the "Gestión de Proyectos" module card navigates to `/proyectos` and the placeholder page renders.

## 10. Documentation + Final

- [ ] 10.1 Add `frontend/README.md` with local run instructions (`npm install`, `ng serve`, proxy setup) per `docs/design/DESIGN.md` §12.3/§12.4.
- [ ] 10.2 `dotnet build AdminPro.slnx` succeeds with zero errors; `dotnet test` all green (existing + new backend tests).
- [ ] 10.3 `ng build` succeeds with zero errors; Angular unit tests all green.
- [ ] 10.4 `npx cypress run` — both E2E tests green.
- [ ] 10.5 Confirm every task above was committed individually per the `commit` skill format.
- [ ] 10.6 Mark this `tasks.md` complete and ready for `/opsx:archive`.
