## MODIFIED Requirements

### Requirement: Get application detail
`GET /api/applications/{id}` SHALL return an `ApplicationDetailDto` including its `ambientes`, `reportes`, `notas`, `documentos`, and `fixDatas` arrays populated from real data (ordered by `Orden`), plus an always-empty `servicios` array (populated by a later phase), per rule APP-QRY-005. It SHALL return 404 if the application doesn't exist or is inactive.

#### Scenario: Existing application returns full detail
- **GIVEN** application "CRM" exists with 1 active `Ambiente` row and 2 active `Nota` rows
- **WHEN** `GET /api/applications/{id}` is called with that application's id
- **THEN** the response includes `nombre: "CRM"`, `ambientes` with 1 entry, and `notas` with 2 entries

#### Scenario: Missing application returns 404
- **WHEN** `GET /api/applications/{id}` is called with an id that doesn't exist
- **THEN** the response is `404 Not Found`
