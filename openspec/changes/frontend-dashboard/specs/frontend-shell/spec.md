## ADDED Requirements

### Requirement: Angular application scaffold
The `frontend/` directory SHALL contain an Angular 22 application using standalone components (no `NgModule`), configured per `docs/design/DESIGN.md` §3.1: root `AppComponent` with a `<router-outlet>`, `app.config.ts` providing the HTTP client and router, and `app.routes.ts` with lazy-loaded feature routes.

#### Scenario: Application builds and serves
- **WHEN** `ng serve` is run from `frontend/`
- **THEN** the app compiles with zero errors and is reachable at `http://localhost:4200`

#### Scenario: Dev server proxies API calls
- **WHEN** the frontend makes a request to `/api/*` while running under `ng serve`
- **THEN** the request is proxied to the backend API per `proxy.conf.json` (target `https://localhost:5001`, per `docs/design/DESIGN.md` §12.4)

### Requirement: Bootstrap 5 theme mapped to the design mockup
The application SHALL load Bootstrap 5 and Bootstrap Icons, with a global stylesheet (`styles.scss`) that overrides Bootstrap's Sass variables/CSS custom properties to match the approved visual design (colors, radii, shadows) rather than introducing a separate/competing CSS framework, per `docs/base-standards.md` §5.

#### Scenario: Theme colors match the design
- **WHEN** any page renders a primary-colored element (e.g. a `.btn-primary`)
- **THEN** its computed background color matches the design's dark-green token (`#0C3B29`), not Bootstrap's default blue

### Requirement: Reusable application shell
A standalone `AppShellComponent` SHALL render the sidebar (brand, nav items, "Presupuesto" promo card) and topbar (search, notification/message icons, user info) shared identically across all authenticated screens, accepting the active nav item and page content as inputs/projected content, so feature screens do not duplicate this markup.

#### Scenario: Shell highlights the active section
- **GIVEN** the Dashboard route is active
- **WHEN** `AppShellComponent` renders the sidebar
- **THEN** the "Dashboard" nav item has the active style and no other nav item does

#### Scenario: Shell content area renders page-specific content
- **GIVEN** a route renders inside `AppShellComponent`
- **WHEN** the page navigates between two shell-wrapped routes
- **THEN** the sidebar and topbar remain mounted/unchanged while only the content area updates
