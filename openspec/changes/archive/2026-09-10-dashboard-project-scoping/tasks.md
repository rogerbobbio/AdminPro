## 1. Backend: trim dashboard summary and fix UpdatedAt

- [x] 1.1 Update `DashboardControllerTests.GetSummary_EmptyDatabase_ReturnsAllZeroShape` and `DashboardIntegrationTests.GetDashboardSummary_ReturnsRealAggregateShape` to assert the trimmed `DashboardSummaryDto` shape (no `ApplicationsCreatedLast7Days`/`TotalAmbientes`/`TotalServiciosVinculados`/`StatusBreakdown`; `RecentApplicationDto` has `UpdatedAt` not `Status`) — written first so they fail against the current handler, per this repo's TDD rule. Also updated `GetDashboardSummaryQueryHandlerTests.cs` and `UpdateApplicationTests.cs` (found via grep, not originally listed) to match.
- [x] 1.2 `DashboardSummaryDto.cs`: drop `TotalAmbientes`, `TotalServiciosVinculados`, `ApplicationsCreatedLast7Days`, `StatusBreakdown`, and `ApplicationStatusBreakdownDto`; change `RecentApplicationDto.Status` to `RecentApplicationDto.UpdatedAt` (`DateTime`).
- [x] 1.3 `GetDashboardSummaryQueryHandler.cs`: remove the ambientes/servicios-vinculados counts, the 7-day series build, and the status breakdown; order `recentApplications` by `UpdatedAt` descending instead of `CreatedAt`; map `UpdatedAt` into `RecentApplicationDto`.
- [x] 1.4 `UpdateApplicationCommandHandler.cs`: set `application.UpdatedAt = DateTime.UtcNow` before `SaveChangesAsync` (bug fix — it currently never refreshes `UpdatedAt` on update).
- [x] 1.5 Run the backend test suite (`dotnet test`) and confirm the tests from 1.1 now pass. (`AdminPro.Application.Tests`: 110/110 passed; `AdminPro.Api.Tests` Dashboard/Applications tests: 12/12 passed. Full containerized `DashboardIntegrationTests`/`AdminPro.Api.Tests` suite requiring Testcontainers/Docker was not run in this environment.)

## 2. Frontend: trim shared model and remove dead widgets

- [x] 2.1 `shared/models/dashboard-summary.model.ts`: remove `ApplicationStatusBreakdown`; `RecentApplication.status: string` becomes `updatedAt: string`; `DashboardSummary` drops `totalAmbientes`, `totalServiciosVinculados`, `applicationsCreatedLast7Days`, `statusBreakdown`.
- [x] 2.2 Delete `features/dashboard/components/weekly-chart/`, `features/dashboard/components/status-donut/`, `features/dashboard/components/reminder-card/`, `features/dashboard/components/module-list/` (component, template, styles, and spec files for each).
- [x] 2.3 Confirmed via grep nothing outside `features/dashboard` still imports `ModuloService`/`Modulo` model; both remain in place (out of scope for this change), same for the backend `/api/modulos` endpoint.

## 3. Frontend: update remaining dashboard widgets

- [x] 3.1 `stat-cards.html`/`.ts`: removed the Ambientes and Servicios Vinculados tiles, kept Total Proyectos (dark tile) and Total Aplicaciones.
- [x] 3.2 `stat-cards.scss`: `.stat-grid` is now a wrapping flex row with fixed-width (260px) cards instead of a 4-column grid.
- [x] 3.3 `recent-applications-table.html`/`.ts`: replaced the "Estado" column/status pill with an "Actualizado" column and a `relativeTime()` helper (minutos/horas/ayer/días/semanas/meses); updated `recent-applications-table.spec.ts`. Also removed the now-dead `.status` pill CSS and its `--ap-status-*`/`--ap-accent-blue-*`/`--ap-neutral-dot` tokens from `styles.scss` (their only consumers were this pill and the deleted status-donut/module-list widgets).

## 4. Frontend: new project/application overview components

- [x] 4.1 Created `features/dashboard/components/project-overview/` (`ProjectOverview`, input `project: ProjectDetail`): "Bases de Datos" card (Nombre/Ambiente/Servidor, no password column, "Ver proyecto completo" link to `/proyectos/:id`) shown only when non-empty; "Aplicaciones" card (clickable stack-pill cards to `/proyectos/aplicaciones/:id`) shown only when non-empty.
- [x] 4.2 Created `features/dashboard/components/application-overview/` (`ApplicationOverview`, input `application: ApplicationDetail`): header strip + "Ir al detalle completo" link, plus Ambientes/Reportes/Notas/Documentos/FixDatas cards each gated on that array's length.
- [x] 4.3 Added `.spec.ts` for both: all-sections-present, each section independently omitted (including the "no password in markup" and "omit every section when application has no nested data" cases).

## 5. Frontend: wire the Dashboard shell

- [x] 5.1 `dashboard.ts` rewritten: `ModuloService`/`WeeklyChart`/`ReminderCard`/`ModuleList`/`StatusDonut` removed; injects `ProjectService`/`ApplicationService`; `selectedProjectId`/`selectedApplicationId` signals with `onProjectChange`/`onApplicationChange`/`onClearProject`; `ngOnInit` loads the summary and the project list.
- [x] 5.2 `dashboard.html` rewritten: filter bar (Proyecto select always; Aplicación select + "Ver todos los proyectos" reset once a project is chosen); "Nueva Aplicación" header action disabled (plain `span`) with no project selected, an `routerLink`+`queryParams` link otherwise; three mutually-exclusive states (global / project overview / application drill-down).
- [x] 5.3 `dashboard.scss`: added `.filter-card`/`.filter-left`/`.filter-field`/`.reset-link`; removed the now-dead `.row2`/`.row3`/`.chip-add` rules that belonged to the deleted widgets.

## 6. Verification

- [x] 6.1 `ng build` (frontend, development configuration) succeeds with no errors.
- [x] 6.2 `dotnet build` succeeds with no errors. `dotnet test`: `AdminPro.Application.Tests` 110/110 pass; `AdminPro.Api.Tests` 33/33 pass excluding two UNRELATED pre-existing issues neither touched by this change: `ContainerizedSmokeTests`/`DashboardIntegrationTests` need Docker (not available in this environment), and `AmbientesControllerTests.CreateEnvironment_InvalidUrl_Returns400` fails on `main` independent of this change (a pre-existing URL-validation gap, confirmed via `git status` that no Ambientes-related file was touched here) — flagged to the user, not fixed under this change's scope.
- [x] 6.3 Frontend unit test suite (`ng test`, 18 files / 63 tests, including all specs touched by this change) passes. While running it, found and fixed a pre-existing, unrelated failure: `ThemeService` (from an earlier, separate dark-mode change) calls `window.matchMedia`, which jsdom's test environment doesn't implement — added `src/test-setup.ts` (wired via `angular.json`'s new `setupFiles` option) polyfilling it. Manual walkthrough of the three states in a running `ng serve` was not performed in this environment (no browser available) — recommend the user click through global / project-selected / drill-down states, including a project with zero databases and an application missing some sections, before considering this done.
