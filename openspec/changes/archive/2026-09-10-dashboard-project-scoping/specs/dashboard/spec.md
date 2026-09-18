## MODIFIED Requirements

### Requirement: Dashboard route and layout
The root route (`/`) SHALL render the `Dashboard` component inside `AppShell` with the "Dashboard" nav item active: a page header (title/subtitle, a "Nuevo Proyecto" action, and a "Nueva Aplicación" action that is disabled until a project is selected in the filter bar), a filter bar (see "Project/application filter bar"), and, below it, either the global overview or a project/application-scoped overview depending on the current selection.

#### Scenario: Dashboard is the default route
- **WHEN** the user navigates to `/`
- **THEN** the `Dashboard` component renders inside the app shell with the "Dashboard" nav item highlighted, no project selected, and the global overview shown

#### Scenario: "Nueva Aplicación" is disabled with no project selected
- **GIVEN** no project is selected in the filter bar
- **WHEN** the Dashboard renders
- **THEN** the "Nueva Aplicación" header action is disabled and does not navigate when interacted with

#### Scenario: "Nueva Aplicación" opens the creation form pre-scoped to the selected project
- **GIVEN** a project is selected in the filter bar
- **WHEN** the user activates "Nueva Aplicación"
- **THEN** the app navigates to the existing application creation form (`/proyectos/aplicaciones/nuevo`) with `proyectoId` set to the selected project's id as a query parameter

### Requirement: Stat cards show real aggregate counts
The Dashboard's global overview (no project selected) SHALL display two stat cards — Total Proyectos and Total Aplicaciones — populated from `GET /api/dashboard/summary`, with no hardcoded numbers. The Ambientes and Servicios Vinculados stat cards are removed (see dashboard-api's modified summary shape).

#### Scenario: Stat cards reflect zero state
- **GIVEN** the API returns both counts as `0`
- **WHEN** the Dashboard loads with no project selected
- **THEN** each of the two stat cards displays `0`, not a placeholder or mock value

#### Scenario: Stat cards are hidden once a project is selected
- **GIVEN** the user has selected a project in the filter bar
- **WHEN** the Dashboard renders
- **THEN** the global stat cards are not shown; the project-scoped overview is shown instead

### Requirement: Recent applications table
The Dashboard's global overview SHALL display up to 5 applications, most-recently-modified first (name, project name, stack pills, relative "updated" time), from `recentApplications` in the summary response. The previous status pill (always "Activo") is removed.

#### Scenario: Empty table state
- **GIVEN** `recentApplications` is an empty array
- **WHEN** the Dashboard renders the table
- **THEN** the table shows an empty-state row/message instead of a blank or broken table

#### Scenario: Relative time reflects last modification
- **GIVEN** an application's `updatedAt` is 2 hours before the current time
- **WHEN** the Dashboard renders that row
- **THEN** the "Actualizado" column shows a relative label consistent with "hace 2 horas" (not its creation time, and not a raw timestamp)

## REMOVED Requirements

### Requirement: Weekly applications chart
**Reason**: The chart consumed `applicationsCreatedLast7Days`, which is removed from the summary response — it added a chart of creation activity with no scoping to a project and no decision it supported.
**Migration**: No replacement. None of this data is computed by the backend anymore.

### Requirement: Module list and navigation
**Reason**: The "Módulos" card duplicated the sidebar's own "Proyectos"/"Servicios" navigation items with no additional information.
**Migration**: Use the sidebar navigation directly. `GET /api/modulos` and `ModuloService` are unchanged and still available for any future consumer — only the Dashboard's rendering of them is removed.

### Requirement: Application status donut
**Reason**: `statusBreakdown` always reported every active application as 100% "Activo" — no real application-status concept exists in the domain, so the donut visualized fabricated data.
**Migration**: No replacement. If a real status concept is introduced later, it should get its own proposal rather than resurrecting this donut over the old fake breakdown.

### Requirement: Static reminder card
**Reason**: The reminder card was hardcoded presentational content with no backing entity or data source, as the original requirement itself noted ("no `Recordatorio` entity exists in the domain model").
**Migration**: None — this was never real functionality.

## ADDED Requirements

### Requirement: Project/application filter bar
The Dashboard SHALL render a filter bar directly below the page header containing a "Proyecto" select populated from all projects, and, only once a project is selected, an "Aplicación" select populated from that project's applications (with a default "Todas" option) plus a control to clear the project selection and return to the global overview.

#### Scenario: Aplicación select appears only after a project is chosen
- **GIVEN** no project is selected
- **WHEN** the Dashboard renders
- **THEN** only the "Proyecto" select is visible; no "Aplicación" select is rendered

#### Scenario: Selecting a project populates the Aplicación select
- **WHEN** the user selects a project with 3 applications
- **THEN** the "Aplicación" select appears, defaulted to "Todas", offering exactly those 3 applications as additional options

#### Scenario: Clearing the project selection returns to the global overview
- **GIVEN** a project is selected
- **WHEN** the user activates the "back to all projects" control
- **THEN** both selects reset and the Dashboard shows the global overview again

### Requirement: Project-scoped overview
When a project is selected and no specific application is selected, the Dashboard SHALL show that project's "Bases de Datos" (Nombre/Ambiente/Servidor, no password column) and "Aplicaciones" (name + tech-stack pills, each entry navigating to that application's detail page) as two full-width, independently-rendered sections — each section entirely omitted (not shown with an empty-state message) when that project has zero rows for it.

#### Scenario: Both sections render when the project has data
- **GIVEN** a selected project has 3 databases and 4 applications
- **WHEN** the Dashboard renders
- **THEN** a "Bases de Datos (3)" section and an "Aplicaciones (4)" section both render, in that order

#### Scenario: A section with no data is omitted entirely
- **GIVEN** a selected project has 0 databases and 2 applications
- **WHEN** the Dashboard renders
- **THEN** no "Bases de Datos" section appears anywhere on the page, while the "Aplicaciones (2)" section renders normally

### Requirement: Application drill-down overview
When a project and one of its applications are both selected, the Dashboard SHALL show that application's name and tech stack, a link to its full detail page, and its Ambientes/Reportes/Notas/Documentos/FixDatas — each of those five sections shown only when that application has at least one row in it.

#### Scenario: Only non-empty sections render
- **GIVEN** the selected application has 2 ambientes, 2 reportes, and 1 nota, but 0 documentos and 0 fixDatas
- **WHEN** the Dashboard renders the drill-down
- **THEN** Ambientes, Reportes, and Notas sections render with their data, and no Documentos or FixDatas section appears anywhere on the page

#### Scenario: Link to full detail page is always present
- **GIVEN** any application is selected in the drill-down
- **WHEN** the Dashboard renders
- **THEN** a link to that application's full detail page (`/proyectos/aplicaciones/:id`) is shown, regardless of which sections are present
