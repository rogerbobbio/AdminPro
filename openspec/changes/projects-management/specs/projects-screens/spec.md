## ADDED Requirements

### Requirement: Proyectos module layout
The `/proyectos` route and its children SHALL render inside a `ProyectosLayout` component (module sub-nav: Proyectos/Servicios/Buscar) which itself renders inside `AppShell` with `activeNav="proyectos"`, replacing the `ComingSoon` placeholder registered by `frontend-dashboard`, per `docs/design/DESIGN.md` §3.1/§6.

#### Scenario: Navigating from the Dashboard reaches the real module
- **GIVEN** the user is on the Dashboard
- **WHEN** they click the "Gestión de Proyectos" module card
- **THEN** they land on `/proyectos` showing the project list, not the "Próximamente" placeholder

### Requirement: Project list
`ProjectList` (`/proyectos`) SHALL display active projects as cards (name, description) loaded from `GET /api/projects`, with a client-side search box and a "Nuevo Proyecto" action navigating to `/proyectos/nuevo`.

#### Scenario: Search filters the visible list
- **GIVEN** projects "Acme Corp" and "Globex Corp" are both listed
- **WHEN** the user types "acme" into the search box
- **THEN** only "Acme Corp" remains visible

### Requirement: Project detail
`ProjectDetail` (`/proyectos/:id`) SHALL show the project's name/description, a "Bases de Datos" section listing its `BaseDeDatos` rows with add/edit/delete actions (via modal forms), and an "Aplicaciones" section rendering an empty-state (no create action — `Application` CRUD is out of scope for this change).

#### Scenario: Adding a database updates the list without a page reload
- **GIVEN** the user is on a project's detail page
- **WHEN** they submit the "add database" modal with a valid name
- **THEN** the new database appears in the "Bases de Datos" list without navigating away

#### Scenario: Aplicaciones section shows an empty state
- **WHEN** the user views any project's detail page
- **THEN** the "Aplicaciones" section shows an empty-state message, with no "+ Nueva Aplicación" action

### Requirement: Project create/edit form
`ProjectForm` (`/proyectos/nuevo` for create, `/proyectos/:id/editar` for edit) SHALL be a reactive form for `Nombre` (required) and `Descripcion` (optional), surfacing the backend's `400` uniqueness/validation errors inline per-field, and navigating to the project's detail page on success.

#### Scenario: Duplicate name shows an inline error
- **GIVEN** a project "Acme Corp" already exists
- **WHEN** the user submits the create form with `Nombre = "Acme Corp"`
- **THEN** an inline error appears on the `Nombre` field and the user stays on the form

#### Scenario: Successful create navigates to the new project's detail page
- **WHEN** the user submits the create form with a unique, valid name
- **THEN** the app navigates to `/proyectos/:id` for the newly created project
