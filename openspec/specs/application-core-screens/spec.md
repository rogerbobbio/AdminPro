# application-core-screens Specification

## Purpose
Frontend screens for the Application aggregate: the application detail page (with environment CRUD) and the application create/edit form.

## Requirements

### Requirement: Application detail
`ApplicationDetail` (`/proyectos/aplicaciones/:id`) SHALL show the application's name/description/technology fields, an "Ambientes" accordion section listing its `Ambiente` rows with add/edit/delete actions (via modal forms), and empty-state accordion sections for "Reportes", "Notas", "Documentos", "FixDatas", and "Servicios" (no create action for any of the five — those entities are out of scope for this change), per rule PRES-UI-008.

#### Scenario: Adding an environment updates the list without a page reload
- **GIVEN** the user is on an application's detail page
- **WHEN** they submit the "add environment" modal with a valid name
- **THEN** the new environment appears in the "Ambientes" list without navigating away

#### Scenario: Invalid URL shows an inline error
- **GIVEN** the user is on an application's detail page and opens the "add environment" modal
- **WHEN** they enter a `Nombre` and a `Url` of "not-a-url", then submit
- **THEN** an inline error appears on the `Url` field and the modal stays open

#### Scenario: Reportes section shows an empty state
- **WHEN** the user views any application's detail page
- **THEN** the "Reportes" section shows an empty-state message, with no "+ Nuevo Reporte" action

### Requirement: Application create/edit form
`ApplicationForm` (`/proyectos/aplicaciones/nuevo` for create, `/proyectos/aplicaciones/:id/editar` for edit) SHALL be a reactive form for `Nombre` (required), `Descripcion`, `TecnologiaFront`, `TecnologiaBack` (shown by default) plus `RamaDesarrollo`, `ApplicationName`, `TieneProyectoBD`, `RutaLocal`, `RutaGit`, `ComoSeLevanta`, `NotasCompilacion` (grouped under a collapsed "Detalles técnicos" sub-section), surfacing the backend's `400` uniqueness/validation errors inline per-field, and navigating to the application's detail page on success.

#### Scenario: Duplicate name within the same project shows an inline error
- **GIVEN** project "Acme Corp" already has an application "CRM"
- **WHEN** the user submits the create form under "Acme Corp" with `Nombre = "CRM"`
- **THEN** an inline error appears on the `Nombre` field and the user stays on the form

#### Scenario: Successful create navigates to the new application's detail page
- **WHEN** the user submits the create form with a unique, valid name
- **THEN** the app navigates to `/proyectos/aplicaciones/:id` for the newly created application
