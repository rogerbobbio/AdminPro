## MODIFIED Requirements

### Requirement: Application create/edit form
`ApplicationForm` (`/proyectos/aplicaciones/nuevo` for create, `/proyectos/aplicaciones/:id/editar` for edit) SHALL be a reactive form grouped into icon-headed section cards: "Información general" (`Nombre` required, `Descripcion`, and a visual-only `Tipo` chip selector — Web/API/Mobile — that is not persisted), "Stack" (`TecnologiaFront`, `TecnologiaBack`, `RamaDesarrollo`), "Rutas" (`RutaLocal`, `RutaGit`, `ComoSeLevanta`), and "Ambientes" (an inline, addable/removable list of ambiente rows — `Nombre`, `Url`, Web/API type — created via `POST /api/applications/{id}/ambientes` right after the application itself is created). It SHALL also render a disabled "Próximamente" placeholder section for "Servicios vinculados" (no backend support yet) — the earlier "Notas de arranque" placeholder is removed now that Notas has a real, functional home on `ApplicationDetail`. `ApplicationName` and `NotasCompilacion` are no longer separate inputs: on submit they SHALL be populated automatically from `Nombre` and `ComoSeLevanta` respectively. The form SHALL surface the backend's `400` uniqueness/validation errors inline per-field and navigate to the application's detail page on success. It SHALL render a breadcrumb back through its parent project (`Proyectos / {proyecto} / Nueva Aplicación` for create, `Proyectos / {proyecto} / {aplicación} / Editar` for edit), an icon page-header with a subtitle, and a "Cancelar" action that navigates back to the previous page without saving.

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

#### Scenario: Notas de arranque placeholder no longer renders
- **WHEN** the user views the create or edit form
- **THEN** no "Notas de arranque" section appears anywhere on the page
