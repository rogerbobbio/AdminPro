## Purpose

Defines the module-launcher Dashboard screen: layout, stat cards, weekly chart, module list, recent-applications table, and status donut, all backed by real API data. Established by the `frontend-dashboard` change.

## Requirements

### Requirement: Dashboard route and layout
The root route (`/`) SHALL render the `Dashboard` component inside `AppShell` with the "Dashboard" nav item active, reproducing the approved Claude Design mockup layout: page header with title/subtitle and a "Nuevo Proyecto" action, a 4-column stat-card grid, a 3-column row (weekly chart, reminder card, module list), and a 2-column row (recent-applications table, status donut).

#### Scenario: Dashboard is the default route
- **WHEN** the user navigates to `/`
- **THEN** the `Dashboard` component renders inside the app shell with the "Dashboard" nav item highlighted

### Requirement: Stat cards show real aggregate counts
The Dashboard SHALL display four stat cards (Total Proyectos, Aplicaciones, Ambientes, Servicios Vinculados) populated from `GET /api/dashboard/summary`, with no hardcoded numbers.

#### Scenario: Stat cards reflect zero state
- **GIVEN** the API returns all counts as `0`
- **WHEN** the Dashboard loads
- **THEN** each stat card displays `0`, not a placeholder or mock value

### Requirement: Weekly applications chart
The Dashboard SHALL render a 7-bar chart representing the `applicationsCreatedLast7Days` series from the summary endpoint, with bar height proportional to that day's count relative to the series maximum (or a minimum visible height when the maximum is `0`).

#### Scenario: Flat chart on empty data
- **GIVEN** `applicationsCreatedLast7Days` is `[0,0,0,0,0,0,0]`
- **WHEN** the Dashboard renders the chart
- **THEN** all 7 bars render at their minimum height with no error, not a hidden/broken widget

### Requirement: Module list and navigation
The Dashboard SHALL display the modules returned by `GET /api/modulos` in the "Módulos" card (name + status/count subtitle), plus the static "Presupuesto" (coming soon) entry, matching `docs/business-rules.md` rule PRES-UI-002.

#### Scenario: Clicking an active module navigates
- **GIVEN** the "Gestión de Proyectos" module is listed
- **WHEN** the user clicks its card/row
- **THEN** the app navigates to `/proyectos`, which renders the project list (see `projects-screens`)

### Requirement: Recent applications table
The Dashboard SHALL display up to 5 recently created applications (name, project name, stack pills, status pill) from `recentApplications` in the summary response.

#### Scenario: Empty table state
- **GIVEN** `recentApplications` is an empty array
- **WHEN** the Dashboard renders the table
- **THEN** the table shows an empty-state row/message instead of a blank or broken table

### Requirement: Application status donut
The Dashboard SHALL render a donut chart from `statusBreakdown`, computed via CSS (`conic-gradient`) with no external charting library, showing the percentage of applications in each status.

#### Scenario: All-active placeholder rendering
- **GIVEN** `statusBreakdown` reports `activo: 3, enProgreso: 0, pendiente: 0`
- **WHEN** the Dashboard renders the donut
- **THEN** it shows 100% in the "Activo" segment and its center label reads "100%"

### Requirement: Static reminder card
The Dashboard SHALL display a static reminder card (title, description, "Ir a Aplicación" button) as presentational-only content, since no `Recordatorio` entity exists in the domain model.

#### Scenario: Reminder card renders without a data dependency
- **WHEN** the Dashboard loads, even if `GET /api/dashboard/summary` has not yet resolved
- **THEN** the reminder card is visible immediately (it does not wait on or depend on the summary API call)
