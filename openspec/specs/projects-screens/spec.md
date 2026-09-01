# projects-screens Specification

## Purpose
Frontend screens for the `/proyectos` module: project list, project detail (with database CRUD), and the project create/edit form.

## Requirements

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
`ProjectDetail` (`/proyectos/:id`) SHALL show the project's name/description, a "Bases de Datos" section listing its `BaseDeDatos` rows with add/edit/delete actions (via modal forms), and an "Aplicaciones" section listing its `Application` rows (name, technology pills) loaded from the project detail response, with a "+ Nueva Aplicación" action navigating to `/proyectos/aplicaciones/nuevo?proyectoId=:id` and each row navigating to `/proyectos/aplicaciones/:id`. The add/edit database modal SHALL include `Nombre`, `Servidor`, `DatabaseId`, `Usuario` (bound to the `LoginName` field), `Password`, `Ambiente`, and `Notas`.

#### Scenario: Adding a database updates the list without a page reload
- **GIVEN** the user is on a project's detail page
- **WHEN** they submit the "add database" modal with a valid name
- **THEN** the new database appears in the "Bases de Datos" list without navigating away

#### Scenario: Adding a database with connection credentials
- **GIVEN** the user is on a project's detail page and opens the "add database" modal
- **WHEN** they fill `Nombre`, `DatabaseId`, `Usuario`, and `Password`, then submit
- **THEN** the new database is created with those values and appears in the "Bases de Datos" list

#### Scenario: Aplicaciones section lists existing applications
- **GIVEN** project "Acme Corp" has an application "CRM"
- **WHEN** the user views the project's detail page
- **THEN** the "Aplicaciones" section shows "CRM", and clicking it navigates to `/proyectos/aplicaciones/:id`

#### Scenario: Aplicaciones section shows an empty state when there are none
- **GIVEN** a project has no applications
- **WHEN** the user views its detail page
- **THEN** the "Aplicaciones" section shows an empty-state message alongside the "+ Nueva Aplicación" action

### Requirement: Project create/edit form
`ProjectForm` (`/proyectos/nuevo` for create, `/proyectos/:id/editar` for edit) SHALL be a reactive form for `Nombre` (required) and `Descripcion` (optional), surfacing the backend's `400` uniqueness/validation errors inline per-field, and navigating to the project's detail page on success.

#### Scenario: Duplicate name shows an inline error
- **GIVEN** a project "Acme Corp" already exists
- **WHEN** the user submits the create form with `Nombre = "Acme Corp"`
- **THEN** an inline error appears on the `Nombre` field and the user stays on the form

#### Scenario: Successful create navigates to the new project's detail page
- **WHEN** the user submits the create form with a unique, valid name
- **THEN** the app navigates to `/proyectos/:id` for the newly created project
