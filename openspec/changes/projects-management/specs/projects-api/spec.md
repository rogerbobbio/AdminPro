## ADDED Requirements

### Requirement: Not-found and domain-error HTTP mappings
`ExceptionHandlerMiddleware` SHALL map a thrown `NotFoundException` to `404 Not Found` and a thrown `DomainException` to `409 Conflict`, both with the same structured JSON error body shape as the existing `ValidationException`/500 mappings, per `docs/design/DESIGN.md` §10.1 and rule XCUT-API-003.

#### Scenario: Missing entity maps to 404
- **GIVEN** a `NotFoundException` is thrown while handling a request
- **WHEN** the middleware catches it
- **THEN** the response is `404 Not Found` with body `{ "error": "NotFoundError", "message": "...", "details": [] }`

#### Scenario: Domain rule violation maps to 409
- **GIVEN** a `DomainException` is thrown while handling a request
- **WHEN** the middleware catches it
- **THEN** the response is `409 Conflict` with body `{ "error": "DomainError", "message": "...", "details": [] }`

### Requirement: List projects
`GET /api/projects` SHALL return `ProjectSummaryDto` rows ordered by `Nombre` ascending, per rule APP-QRY-002. An `includeInactive` query flag (default `false`) SHALL include soft-deleted projects when `true`.

#### Scenario: Default excludes inactive projects
- **GIVEN** an active project "Acme Corp" and an inactive project "Old Co" exist
- **WHEN** `GET /api/projects` is called with no query string
- **THEN** the response contains only "Acme Corp"

### Requirement: Get project detail
`GET /api/projects/{id}` SHALL return a `ProjectDetailDto` including its `basesDeDatos` and `applications` arrays (the latter always empty until Phase 4), per rule APP-QRY-003. It SHALL return 404 if the project doesn't exist or is inactive (unless `includeInactiveChildren` semantics from `docs/design/DESIGN.md` §4.2 apply to children, not to the project itself).

#### Scenario: Existing project returns full detail
- **GIVEN** project "Acme Corp" exists with 2 active `BaseDeDatos` rows
- **WHEN** `GET /api/projects/{id}` is called with that project's id
- **THEN** the response includes `nombre: "Acme Corp"` and `basesDeDatos` with 2 entries

#### Scenario: Missing project returns 404
- **WHEN** `GET /api/projects/{id}` is called with an id that doesn't exist
- **THEN** the response is `404 Not Found`

### Requirement: Create project
`POST /api/projects` SHALL create a `Project` with `Nombre` (required, unique across all projects per rule PROJECT-001, max 100 chars) and optional `Descripcion` (max 500 chars), returning `201 Created` with the new id.

#### Scenario: Duplicate name rejected
- **GIVEN** a project named "Acme Corp" already exists
- **WHEN** `POST /api/projects` is called with `{ "nombre": "Acme Corp" }`
- **THEN** the response is `400 Bad Request` with a validation error on `nombre`

#### Scenario: Valid project created
- **WHEN** `POST /api/projects` is called with `{ "nombre": "Globex Corp", "descripcion": "Nuevo cliente" }`
- **THEN** the response is `201 Created` with the new project's id, and `GET /api/projects` subsequently includes it

### Requirement: Update project
`PUT /api/projects/{id}` SHALL update `Nombre`/`Descripcion`, re-validating the name-uniqueness rule against every other project (excluding the project's own current row), and return `404` if the id doesn't exist.

#### Scenario: Renaming to another project's name is rejected
- **GIVEN** projects "Acme Corp" (id 1) and "Globex Corp" (id 2) exist
- **WHEN** `PUT /api/projects/1` is called with `{ "id": 1, "nombre": "Globex Corp" }`
- **THEN** the response is `400 Bad Request`

#### Scenario: Renaming to its own current name succeeds
- **GIVEN** project "Acme Corp" (id 1) exists
- **WHEN** `PUT /api/projects/1` is called with `{ "id": 1, "nombre": "Acme Corp", "descripcion": "Updated" }`
- **THEN** the response is `204 No Content`

### Requirement: Soft-delete project cascades to children
`DELETE /api/projects/{id}` SHALL set `Activo = false` on the project and cascade `Activo = false` to all its `BaseDeDatos` and `Application` rows (rule PROJECT-002), returning `404` if the project doesn't exist.

#### Scenario: Deleting a project deactivates its databases
- **GIVEN** project "Acme Corp" has 2 active `BaseDeDatos` rows
- **WHEN** `DELETE /api/projects/{id}` is called
- **THEN** the project and both `BaseDeDatos` rows have `Activo = false`, and `GET /api/projects/{id}` subsequently returns `404`

### Requirement: Create database
`POST /api/projects/{projectId}/basesdedatos` SHALL create a `BaseDeDatos` under the given project with required `Nombre` (max 100 chars) and optional `Servidor`, `DatabaseId`, `LoginName`, `Password`, `Ambiente`, `Notas`, returning `201 Created` with the new id. Returns `404` if `projectId` doesn't reference an existing active project. `Password` is stored as plain text (no encryption), per explicit user decision for this single-user, no-auth internal tool.

#### Scenario: Database created under project
- **GIVEN** project "Acme Corp" exists
- **WHEN** `POST /api/projects/{id}/basesdedatos` is called with `{ "nombre": "SalesDb", "ambiente": "desarrollo" }`
- **THEN** the response is `201 Created`, and the project's detail response subsequently includes it

#### Scenario: Database created with connection credentials
- **GIVEN** project "Acme Corp" exists
- **WHEN** `POST /api/projects/{id}/basesdedatos` is called with `{ "nombre": "SalesDb", "databaseId": 42, "loginName": "app_user", "password": "s3cr3t" }`
- **THEN** the response is `201 Created`, and the project's detail response subsequently includes a `basesDeDatos` entry with `databaseId: 42` and `loginName: "app_user"`

### Requirement: Update database
`PUT /api/basesdedatos/{id}` SHALL update a `BaseDeDatos`'s fields (including `DatabaseId`, `LoginName`, `Password`), returning `404` if the id doesn't exist.

#### Scenario: Existing database updated
- **GIVEN** a `BaseDeDatos` "SalesDb" exists
- **WHEN** `PUT /api/basesdedatos/{id}` is called with an updated `ambiente`
- **THEN** the response is `204 No Content` and the project detail reflects the new value

### Requirement: Delete database
`DELETE /api/basesdedatos/{id}` SHALL set `Activo = false` on the row, returning `404` if the id doesn't exist.

#### Scenario: Database soft-deleted
- **GIVEN** a `BaseDeDatos` "SalesDb" exists under project "Acme Corp"
- **WHEN** `DELETE /api/basesdedatos/{id}` is called
- **THEN** the project's subsequent detail response no longer lists it
