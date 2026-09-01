## MODIFIED Requirements

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
