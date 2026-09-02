## MODIFIED Requirements

### Requirement: Reusable application shell
A standalone `AppShell` component SHALL render the sidebar (brand, nav items, "Presupuesto" promo card) and topbar (search, notification/message icons, user info) shared identically across all screens, accepting the active nav item (`activeNav` input) and page content (via content projection, `<ng-content>`), so feature screens do not duplicate this markup. The "Proyectos" nav item SHALL show a badge with the current total project count, loaded via `ProjectService` when the shell initializes.

#### Scenario: Shell highlights the active section
- **GIVEN** the Dashboard route is active
- **WHEN** `AppShell` renders the sidebar
- **THEN** the "Dashboard" nav item has the active style and no other nav item does

#### Scenario: Shell content area renders page-specific content
- **GIVEN** a page wraps its content in `<app-shell>`
- **WHEN** the page renders
- **THEN** the sidebar and topbar render around the projected content unchanged, regardless of what that page's content is

#### Scenario: Proyectos nav item shows the project count
- **GIVEN** the catalog has 2 active projects
- **WHEN** `AppShell` renders the sidebar
- **THEN** the "Proyectos" nav item shows a badge reading "2"
