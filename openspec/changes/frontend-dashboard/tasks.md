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
- [ ] 4.3 Backend integration test (Testcontainers, matching the pattern from `foundation-backend` task 6.1): boot against a containerized SQL Server, run migrations (confirming the `SeedModulos` migration applies), call both endpoints, assert real seeded/aggregate data.

## 5. Frontend Scaffold

- [ ] 5.1 `ng new adminpro-ui` (or equivalent Angular 22 CLI scaffold) inside `frontend/`, standalone + routing, per `docs/design/DESIGN.md` §3.1. Verified: `ng serve` boots with the default template with zero errors.
- [ ] 5.2 Install and configure Bootstrap 5 + Bootstrap Icons; add `proxy.conf.json` pointing `/api` at the backend's HTTPS URL and wire it into `angular.json`'s serve target. Verified: a manual `fetch('/api/modulos')` from the browser console during `ng serve` reaches the backend (200 or expected error, not a CORS/404 from the dev server itself).
- [ ] 5.3 Build `styles.scss` mapping the mockup's CSS custom properties (colors, radii, shadows from `dashboard.html`) onto Bootstrap 5 Sass variables/overrides, per `specs/frontend-shell/spec.md` "Bootstrap 5 theme mapped to the design mockup". Verified: a `.btn-primary` renders with the dark-green (`#0C3B29`) background.

## 6. Frontend Shell Component (TDD)

- [ ] 6.1 TDD (Angular/Karma): write a failing test asserting `AppShellComponent` marks the nav item matching its `activeNav` input as active and no others; implement `AppShellComponent` (sidebar: brand, nav items, Presupuesto promo card; topbar: search, icon buttons, user block), reproducing `dashboard.html`'s sidebar/topbar markup 1:1.
- [ ] 6.2 Wire root routing: `app.routes.ts` renders `AppShellComponent` as a layout route wrapping child routes (Dashboard at `/`, placeholder `/proyectos` route rendering a minimal "Próximamente" page per design.md's resolved open question).

## 7. Frontend Dashboard Service (TDD)

- [ ] 7.1 TDD: write a failing test asserting `ModuloService.loadModulos()` populates a `modulos` signal from `GET /api/modulos`; implement the service (`HttpClient` + `firstValueFrom`, per `docs/design/DESIGN.md` §3.2 pattern).
- [ ] 7.2 TDD: write a failing test asserting `DashboardService.loadSummary()` populates a `summary` signal (typed to the `DashboardSummaryDto` shape) from `GET /api/dashboard/summary`, including a `loading`/`error` signal pair; implement the service.

## 8. Frontend Dashboard Component (TDD)

- [ ] 8.1 TDD: write a failing test asserting the 4 stat cards render the values from `DashboardService.summary()` (including the all-zero case); implement the stat-card grid markup/component.
- [ ] 8.2 TDD: write a failing test asserting the weekly bar chart renders 7 bars with height derived from `applicationsCreatedLast7Days`, including the flat/minimum-height case when all values are `0`; implement the chart (plain template bindings, no charting library, per design.md Decision 6).
- [ ] 8.3 Implement the static reminder card (no test needed — purely presentational, per `specs/dashboard/spec.md` "Static reminder card").
- [ ] 8.4 TDD: write a failing test asserting the module list renders modules from `ModuloService.modulos()` plus the static Presupuesto tile, and that clicking an active module navigates via `Router`; implement.
- [ ] 8.5 TDD: write a failing test asserting the recent-applications table renders rows from `summary().recentApplications` and an empty-state message when the array is empty; implement the table.
- [ ] 8.6 TDD: write a failing test asserting the status donut's computed `conic-gradient` and center label reflect `statusBreakdown` (100%/0%/0% case); implement the donut (CSS-only, per design.md Decision 6).
- [ ] 8.7 Assemble `DashboardComponent` wiring all of the above together behind `AppShellComponent`, calling `loadModulos()`/`loadSummary()` on init.

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
