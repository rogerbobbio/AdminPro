## MODIFIED Requirements

### Requirement: Create application
`POST /api/projects/{projectId}/applications` SHALL create an `Application` under the given project with required `Nombre` (max 100 chars, unique within the project per rule APP-001) and optional `Descripcion`, `TecnologiaFront`, `TecnologiaBack`, `RamaDesarrollo`, `ApplicationName`, `RutaLocal`, `RutaGit`, `ComoSeLevanta`, `NotasCompilacion`, and `Orden` (default 0), returning `201 Created` with the new id. Returns `404` if `projectId` doesn't reference an existing active project.

`TieneProyectoBD` is no longer a field of `Application` — it has been dropped end-to-end (entity, commands, DTO, EF configuration, controller, and a migration dropping the column) as it was unused.

#### Scenario: Duplicate name within same project rejected
- **GIVEN** project "Acme Corp" has an application named "CRM"
- **WHEN** `POST /api/projects/{id}/applications` is called with `{ "nombre": "CRM" }`
- **THEN** the response is `400 Bad Request` with a validation error on `nombre`
