## MODIFIED Requirements

### Requirement: Application create/edit form
`ApplicationForm` (`/proyectos/aplicaciones/nuevo` for create, `/proyectos/aplicaciones/:id/editar` for edit) SHALL be a reactive form for `Nombre` (required), `Descripcion`, `TecnologiaFront`, `TecnologiaBack` (shown by default) plus `RamaDesarrollo`, `ApplicationName`, `TieneProyectoBD`, `RutaLocal`, `RutaGit`, `ComoSeLevanta`, `NotasCompilacion` (grouped under a collapsible "Detalles técnicos" section card), surfacing the backend's `400` uniqueness/validation errors inline per-field, and navigating to the application's detail page on success. It SHALL render a breadcrumb back through its parent project (`Proyectos / {proyecto} / Nueva Aplicación` for create, `Proyectos / {proyecto} / {aplicación} / Editar` for edit), an icon page-header with a subtitle, the default-visible fields grouped inside an icon-headed "Información general" section card, and a "Cancelar" action that navigates back to the previous page without saving.

#### Scenario: Duplicate name within the same project shows an inline error
- **GIVEN** project "Acme Corp" already has an application "CRM"
- **WHEN** the user submits the create form under "Acme Corp" with `Nombre = "CRM"`
- **THEN** an inline error appears on the `Nombre` field and the user stays on the form

#### Scenario: Successful create navigates to the new application's detail page
- **WHEN** the user submits the create form with a unique, valid name
- **THEN** the app navigates to `/proyectos/aplicaciones/:id` for the newly created application

#### Scenario: Cancelar discards changes and navigates back
- **GIVEN** the user has typed a `Nombre` into the create or edit form but not submitted it
- **WHEN** they click "Cancelar"
- **THEN** the app navigates back to the previous page and no `POST`/`PUT` request is made
