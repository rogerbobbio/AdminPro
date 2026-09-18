## Why

The Dashboard is currently a single global view with widgets that don't help a user working on one project at a time: a weekly-creation chart and status donut computed from fabricated data (status is hardcoded to 100% "Activo"), a static reminder card with no backing entity, and a módulos list that only duplicates sidebar navigation. Meanwhile there is no way to pick a project from the Dashboard and immediately see its databases and applications, or drill into one application's environments/reports/notes without leaving the page. This change replaces the global-only dashboard with a project- and application-scoped view, and removes the widgets that were carrying no real signal.

## What Changes

- Add a filter bar below the Dashboard header: a "Proyecto" select (all projects) and, once a project is chosen, an "Aplicación" select (that project's applications, default "Todas") plus a link back to the global view.
- **BREAKING**: Global (no project selected) Dashboard view is trimmed to 2 stat tiles (Total Proyectos, Total Aplicaciones) and a "recently modified applications" table with a real relative-updated-time column, replacing the always-"Activo" status pill.
- **BREAKING**: Remove the weekly applications chart, the status donut, the static reminder card, and the módulos list from the Dashboard entirely (all four widgets and their components are deleted).
- Add a project-scoped Dashboard view: when a project is selected (no specific application), show a Bases de Datos card and an Aplicaciones card, each only when the project actually has that data.
- Add an application drill-down view: when a project and one of its applications are both selected, show that application's Ambientes/Reportes/Notas/Documentos/FixDatas, each only when non-empty, plus a link to the full application detail page.
- Add "+ Nuevo Proyecto" / "+ Nueva Aplicación" actions to the Dashboard header (the latter disabled until a project is selected), reusing the existing project/application creation forms — no new forms are built.
- **BREAKING**: `GET /api/dashboard/summary` drops `totalAmbientes`, `totalServiciosVinculados`, `applicationsCreatedLast7Days`, and `statusBreakdown`, and its `recentApplications` entries drop the fake `status` field in favor of a real `updatedAt` timestamp, ordered by most-recently-updated.
- Fix a bug found while wiring `updatedAt`: `UpdateApplicationCommandHandler` never refreshed `Application.UpdatedAt`, so every application looked permanently "unmodified" since creation.

## Capabilities

### New Capabilities
(none — this reshapes existing dashboard behavior, it doesn't introduce a new bounded capability)

### Modified Capabilities
- `dashboard`: replaces the single global-widget layout with project/application-scoped views; removes the weekly chart, status donut, reminder card, and módulos list requirements; changes the recent-applications requirement to sort/display by last-modified time instead of a fabricated status.
- `dashboard-api`: `GET /api/dashboard/summary` response shape shrinks (drops the four fields above) and `recentApplications` changes from status-tagged to timestamp-tagged, ordered by `UpdatedAt` descending instead of `CreatedAt` descending.

## Impact

- Frontend: `features/dashboard/dashboard.ts`/`.html`/`.scss`, `shared/models/dashboard-summary.model.ts`, `features/dashboard/components/stat-cards`, `features/dashboard/components/recent-applications-table`; deletes `features/dashboard/components/{weekly-chart,status-donut,reminder-card,module-list}`; adds two new presentational components (project overview, application overview) under `features/dashboard/components/`. Reuses existing `ProjectService`/`ApplicationService` — no new frontend services.
- Backend: `Application.Dashboard.Queries.GetDashboardSummary` (`DashboardSummaryDto`, `GetDashboardSummaryQueryHandler`), `Application.Applications.Commands.UpdateApplication.UpdateApplicationCommandHandler` (bug fix), and the two dashboard test files (`DashboardControllerTests.cs`, `DashboardIntegrationTests.cs`). No new endpoints, no migrations — `GET /api/projects/{id}` and `GET /api/applications/{id}` already return everything the scoped views need.
- `ModuloService`/`GET /api/modulos` are NOT removed — they lose their only UI consumer (the módulos list widget) but remain intact as a capability, per `dashboard-api`'s "List active modules" requirement, which is untouched.
