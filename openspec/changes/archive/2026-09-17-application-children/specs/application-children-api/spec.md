## ADDED Requirements

### Requirement: Create report
`POST /api/applications/{appId}/reportes` SHALL create a `Reporte` under the given application with required `ReportCode` (max 20 chars, unique within the application per rule REP-001) and `ReportName` (max 200 chars), and optional `RegionId` (max 10), `ReportPath` (max 200), `SpTranship`, `SpReportViewer`, `Notas`, `ParametrosEjemplo`, and `Orden` (default 0), per rule APP-CMD-009, returning `201 Created` with the new id. Returns `404` if `appId` doesn't reference an existing active application.

#### Scenario: Duplicate report code within same application rejected
- **GIVEN** application "CRM" has a report with `ReportCode = "VFL"`
- **WHEN** `POST /api/applications/{id}/reportes` is called with `{ "reportCode": "VFL", "reportName": "Volumen de Carga" }` for that application
- **THEN** the response is `400 Bad Request` with a validation error on `reportCode`

#### Scenario: Valid report created
- **GIVEN** application "CRM" exists
- **WHEN** `POST /api/applications/{id}/reportes` is called with `{ "reportCode": "AUT", "reportName": "Autorizaciones" }`
- **THEN** the response is `201 Created`, and the application's detail response subsequently includes it

### Requirement: Update report
`PUT /api/reportes/{id}` SHALL update a `Reporte`'s fields, re-validating the report-code-uniqueness-within-application rule (excluding its own current row), and return `404` if the id doesn't exist.

#### Scenario: Existing report updated
- **GIVEN** a report "VFL" exists under application "CRM"
- **WHEN** `PUT /api/reportes/{id}` is called with an updated `reportName`
- **THEN** the response is `204 No Content` and the application detail reflects the new value

### Requirement: Delete report
`DELETE /api/reportes/{id}` SHALL set `Activo = false` on the row, returning `404` if the id doesn't exist.

#### Scenario: Report soft-deleted
- **GIVEN** a report "VFL" exists under application "CRM"
- **WHEN** `DELETE /api/reportes/{id}` is called
- **THEN** the application's subsequent detail response no longer lists it

### Requirement: Create note
`POST /api/applications/{appId}/notas` SHALL create a `Nota` under the given application with required `Titulo` (non-empty, max 200 chars) and `Descripcion` (non-empty memo text), and optional `Orden` (default 0), per rules APP-CMD-010, NOTE-001, and NOTE-002, returning `201 Created` with the new id. Returns `404` if `appId` doesn't reference an existing active application.

#### Scenario: Missing title rejected
- **GIVEN** application "CRM" exists
- **WHEN** `POST /api/applications/{id}/notas` is called with `{ "titulo": "", "descripcion": "Some memo" }`
- **THEN** the response is `400 Bad Request` with a validation error on `titulo`

#### Scenario: Valid note created
- **GIVEN** application "CRM" exists
- **WHEN** `POST /api/applications/{id}/notas` is called with `{ "titulo": "nvm use 14.16.0", "descripcion": "Usar Node 14.16.0 para compilar el front antes de levantar el proyecto." }`
- **THEN** the response is `201 Created`, and the application's detail response subsequently includes it

### Requirement: Update note
`PUT /api/notas/{id}` SHALL update a `Nota`'s `Titulo`, `Descripcion`, and `Orden`, returning `404` if the id doesn't exist.

#### Scenario: Existing note updated
- **GIVEN** a note "nvm use 14.16.0" exists under application "CRM"
- **WHEN** `PUT /api/notas/{id}` is called with an updated `descripcion`
- **THEN** the response is `204 No Content` and the application detail reflects the new value

### Requirement: Delete note
`DELETE /api/notas/{id}` SHALL set `Activo = false` on the row, returning `404` if the id doesn't exist.

#### Scenario: Note soft-deleted
- **GIVEN** a note exists under application "CRM"
- **WHEN** `DELETE /api/notas/{id}` is called
- **THEN** the application's subsequent detail response no longer lists it

### Requirement: Create document
`POST /api/applications/{appId}/documentos` SHALL create a `Documento` under the given application with required `NombreArchivo` (non-empty, max 200 chars) and `UrlOneDrive` (non-empty, must be a valid absolute URL, per rule DOC-001) and `Tipo` (non-empty, one of "manual", "diagrama", "codigo", "otro", per rule DOC-002), and optional `Descripcion` (max 500) and `Orden` (default 0), per rule APP-CMD-011, returning `201 Created` with the new id. Returns `404` if `appId` doesn't reference an existing active application.

#### Scenario: Invalid URL rejected
- **GIVEN** application "CRM" exists
- **WHEN** `POST /api/applications/{id}/documentos` is called with `{ "nombreArchivo": "Manual", "urlOneDrive": "not-a-url", "tipo": "manual" }`
- **THEN** the response is `400 Bad Request` with a validation error on `urlOneDrive`

#### Scenario: Valid document created
- **GIVEN** application "CRM" exists
- **WHEN** `POST /api/applications/{id}/documentos` is called with `{ "nombreArchivo": "Manual de Usuario", "urlOneDrive": "https://onedrive.example.com/manual", "tipo": "manual" }`
- **THEN** the response is `201 Created`, and the application's detail response subsequently includes it

### Requirement: Update document
`PUT /api/documentos/{id}` SHALL update a `Documento`'s fields, returning `404` if the id doesn't exist.

#### Scenario: Existing document updated
- **GIVEN** a document "Manual de Usuario" exists under application "CRM"
- **WHEN** `PUT /api/documentos/{id}` is called with an updated `urlOneDrive`
- **THEN** the response is `204 No Content` and the application detail reflects the new value

### Requirement: Delete document
`DELETE /api/documentos/{id}` SHALL set `Activo = false` on the row, returning `404` if the id doesn't exist.

#### Scenario: Document soft-deleted
- **GIVEN** a document exists under application "CRM"
- **WHEN** `DELETE /api/documentos/{id}` is called
- **THEN** the application's subsequent detail response no longer lists it

### Requirement: Create fix data
`POST /api/applications/{appId}/fixdatas` SHALL create a `FixData` under the given application with required `Nombre` (non-empty, max 100 chars, per rule FIX-001) and optional `Descripcion` (max 500), `Script` (free SQL text, per rule FIX-002), and `Orden` (default 0), per rule APP-CMD-012, returning `201 Created` with the new id. Returns `404` if `appId` doesn't reference an existing active application.

#### Scenario: Missing name rejected
- **GIVEN** application "CRM" exists
- **WHEN** `POST /api/applications/{id}/fixdatas` is called with `{ "nombre": "" }`
- **THEN** the response is `400 Bad Request` with a validation error on `nombre`

#### Scenario: Valid fix data created
- **GIVEN** application "CRM" exists
- **WHEN** `POST /api/applications/{id}/fixdatas` is called with `{ "nombre": "Fix duplicate customers", "script": "DELETE FROM ..." }`
- **THEN** the response is `201 Created`, and the application's detail response subsequently includes it

### Requirement: Update fix data
`PUT /api/fixdatas/{id}` SHALL update a `FixData`'s fields, returning `404` if the id doesn't exist.

#### Scenario: Existing fix data updated
- **GIVEN** a fix data "Fix duplicate customers" exists under application "CRM"
- **WHEN** `PUT /api/fixdatas/{id}` is called with an updated `script`
- **THEN** the response is `204 No Content` and the application detail reflects the new value

### Requirement: Delete fix data
`DELETE /api/fixdatas/{id}` SHALL set `Activo = false` on the row, returning `404` if the id doesn't exist.

#### Scenario: Fix data soft-deleted
- **GIVEN** a fix data exists under application "CRM"
- **WHEN** `DELETE /api/fixdatas/{id}` is called
- **THEN** the application's subsequent detail response no longer lists it
