## MODIFIED Requirements

### Requirement: Project create/edit form
`ProjectForm` (`/proyectos/nuevo` for create, `/proyectos/:id/editar` for edit) SHALL be a reactive form for `Nombre` (required) and `Descripcion` (optional), surfacing the backend's `400` uniqueness/validation errors inline per-field, and navigating to the project's detail page on success. It SHALL render a breadcrumb (`Proyectos / Nuevo Proyecto` for create, `Proyectos / {nombre} / Editar` for edit), an icon page-header with a subtitle, the fields grouped inside an icon-headed "Información general" section card, and a "Cancelar" action that navigates back to the previous page without saving.

#### Scenario: Duplicate name shows an inline error
- **GIVEN** a project "Acme Corp" already exists
- **WHEN** the user submits the create form with `Nombre = "Acme Corp"`
- **THEN** an inline error appears on the `Nombre` field and the user stays on the form

#### Scenario: Successful create navigates to the new project's detail page
- **WHEN** the user submits the create form with a unique, valid name
- **THEN** the app navigates to `/proyectos/:id` for the newly created project

#### Scenario: Cancelar discards changes and navigates back
- **GIVEN** the user has typed a `Nombre` into the create or edit form but not submitted it
- **WHEN** they click "Cancelar"
- **THEN** the app navigates back to the previous page and no `POST`/`PUT` request is made
