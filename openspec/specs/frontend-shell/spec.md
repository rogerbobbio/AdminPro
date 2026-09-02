## Purpose

Defines the Angular 22 application scaffold and the reusable `AppShell` layout (sidebar + topbar) that every screen renders inside. Established by the `frontend-dashboard` change; later screens (Projects, Services, Search, ...) reuse `AppShell` rather than duplicating its markup.

## Requirements

### Requirement: Angular application scaffold
The `frontend/` directory SHALL contain an Angular 22 application using standalone components (no `NgModule`), configured per `docs/design/DESIGN.md` §3.1: a root `App` component with a `<router-outlet>`, `app.config.ts` providing the HTTP client and router (with `withComponentInputBinding()` so route `data` binds to component inputs), and `app.routes.ts` with the app's routes.

#### Scenario: Application builds and serves
- **WHEN** `ng serve` is run from `frontend/`
- **THEN** the app compiles with zero errors and is reachable at `http://localhost:4200`

#### Scenario: Dev server proxies API calls
- **WHEN** the frontend makes a request to `/api/*` while running under `ng serve`
- **THEN** the request is proxied to the backend API per `proxy.conf.json` (target matches whichever local backend port is actually running — `http://localhost:5169` for the `http` launch profile, or `https://localhost:7293` for `https`)

### Requirement: Bootstrap 5 theme mapped to the design mockup
The application SHALL load Bootstrap 5 and Bootstrap Icons, with a global stylesheet (`styles.scss`) that overrides Bootstrap's Sass variables via `@use "bootstrap/scss/bootstrap" with (...)` (colors, border-radius scale, shadows, font stack) to match the approved visual design, rather than introducing a separate/competing CSS framework, per `docs/base-standards.md` §5.

#### Scenario: Theme colors match the design
- **WHEN** any page renders a primary-colored element (e.g. a `.btn-primary`)
- **THEN** its computed background color matches the design's dark-green token (`#0C3B29`), not Bootstrap's default blue

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
