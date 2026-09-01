# application-core-api Specification

## Purpose
Backend HTTP API for managing `Application` and `Ambiente` entities: CRUD endpoints, name-uniqueness within a project, and soft-delete cascade from an application to its environments.

## Requirements

### Requirement: List applications by project
`GET /api/projects/{projectId}/applications` SHALL return `ApplicationSummaryDto` rows for the given project ordered by `Orden` ascending then `Nombre`, per rule APP-QRY-004. An `includeInactive` query flag (default `false`) SHALL include soft-deleted applications when `true`.

#### Scenario: Default excludes inactive applications
- **GIVEN** project "Acme Corp" has an active application "CRM" and an inactive application "Old App"
- **WHEN** `GET /api/projects/{id}/applications` is called with no query string
- **THEN** the response contains only "CRM"

### Requirement: Get application detail
`GET /api/applications/{id}` SHALL return an `ApplicationDetailDto` including its `ambientes` array plus always-empty `reportes`, `notas`, `documentos`, `fixDatas`, and `servicios` arrays (populated by later phases), per rule APP-QRY-005. It SHALL return 404 if the application doesn't exist or is inactive.

#### Scenario: Existing application returns full detail
- **GIVEN** application "CRM" exists with 1 active `Ambiente` row
- **WHEN** `GET /api/applications/{id}` is called with that application's id
- **THEN** the response includes `nombre: "CRM"` and `ambientes` with 1 entry

#### Scenario: Missing application returns 404
- **WHEN** `GET /api/applications/{id}` is called with an id that doesn't exist
- **THEN** the response is `404 Not Found`

### Requirement: Create application
`POST /api/projects/{projectId}/applications` SHALL create an `Application` under the given project with required `Nombre` (max 100 chars, unique within the project per rule APP-001) and optional `Descripcion`, `TecnologiaFront`, `TecnologiaBack`, `RamaDesarrollo`, `ApplicationName`, `TieneProyectoBD`, `RutaLocal`, `RutaGit`, `ComoSeLevanta`, `NotasCompilacion`, and `Orden` (default 0), returning `201 Created` with the new id. Returns `404` if `projectId` doesn't reference an existing active project.

#### Scenario: Duplicate name within same project rejected
- **GIVEN** project "Acme Corp" has an application named "CRM"
- **WHEN** `POST /api/projects/{id}/applications` is called with `{ "nombre": "CRM" }`
- **THEN** the response is `400 Bad Request` with a validation error on `nombre`

#### Scenario: Same name allowed in a different project
- **GIVEN** project "Acme Corp" has an application named "CRM" and project "Globex Corp" has none named "CRM"
- **WHEN** `POST /api/projects/{globexId}/applications` is called with `{ "nombre": "CRM" }`
- **THEN** the response is `201 Created`

#### Scenario: Valid application created
- **WHEN** `POST /api/projects/{id}/applications` is called with `{ "nombre": "Report Viewer", "tecnologiaFront": "Angular 18" }`
- **THEN** the response is `201 Created` with the new application's id, and the project's detail response subsequently includes it

### Requirement: Update application
`PUT /api/applications/{id}` SHALL update an `Application`'s fields, re-validating the name-uniqueness-within-project rule (excluding the application's own current row), and return `404` if the id doesn't exist.

#### Scenario: Renaming to another application's name in the same project is rejected
- **GIVEN** project "Acme Corp" has applications "CRM" (id 1) and "Billing" (id 2)
- **WHEN** `PUT /api/applications/1` is called with `{ "id": 1, "nombre": "Billing" }`
- **THEN** the response is `400 Bad Request`

#### Scenario: Renaming to its own current name succeeds
- **GIVEN** application "CRM" (id 1) exists
- **WHEN** `PUT /api/applications/1` is called with `{ "id": 1, "nombre": "CRM", "descripcion": "Updated" }`
- **THEN** the response is `204 No Content`

### Requirement: Soft-delete application cascades to environments
`DELETE /api/applications/{id}` SHALL set `Activo = false` on the application and cascade `Activo = false` to all its `Ambiente` rows (rule APP-002), returning `404` if the application doesn't exist.

#### Scenario: Deleting an application deactivates its environments
- **GIVEN** application "CRM" has 2 active `Ambiente` rows
- **WHEN** `DELETE /api/applications/{id}` is called
- **THEN** the application and both `Ambiente` rows have `Activo = false`, and `GET /api/applications/{id}` subsequently returns `404`

### Requirement: Create environment
`POST /api/applications/{appId}/ambientes` SHALL create an `Ambiente` under the given application with required `Nombre` (max 50 chars) and optional `Url` (must be a valid absolute `http://`/`https://` URL if provided, per rule ENV-002), `EsWebApi` (default `false`), `Notas`, and `Orden` (default 0), returning `201 Created` with the new id. Returns `404` if `appId` doesn't reference an existing active application.

#### Scenario: Environment created under application
- **GIVEN** application "CRM" exists
- **WHEN** `POST /api/applications/{id}/ambientes` is called with `{ "nombre": "UAT", "url": "https://uat.example.com" }`
- **THEN** the response is `201 Created`, and the application's detail response subsequently includes it

#### Scenario: Invalid URL rejected
- **GIVEN** application "CRM" exists
- **WHEN** `POST /api/applications/{id}/ambientes` is called with `{ "nombre": "UAT", "url": "not-a-url" }`
- **THEN** the response is `400 Bad Request` with a validation error on `url`

### Requirement: Update environment
`PUT /api/ambientes/{id}` SHALL update an `Ambiente`'s fields, returning `404` if the id doesn't exist.

#### Scenario: Existing environment updated
- **GIVEN** an `Ambiente` "UAT" exists
- **WHEN** `PUT /api/ambientes/{id}` is called with an updated `url`
- **THEN** the response is `204 No Content` and the application detail reflects the new value

### Requirement: Delete environment
`DELETE /api/ambientes/{id}` SHALL set `Activo = false` on the row, returning `404` if the id doesn't exist.

#### Scenario: Environment soft-deleted
- **GIVEN** an `Ambiente` "UAT" exists under application "CRM"
- **WHEN** `DELETE /api/ambientes/{id}` is called
- **THEN** the application's subsequent detail response no longer lists it
