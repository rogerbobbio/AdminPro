## 0. Setup (MANDATORY FIRST)

- [x] 0.1 Create branch `feature/application-children`
- [x] 0.2 Push branch to `origin` (Draft PR opened manually if `gh` CLI is unavailable)

## 1. Backend — Reporte CRUD (TDD)

- [x] 1.1 TDD: write failing validator/handler tests for `CreateReporteCommand` (rules APP-CMD-009, REP-001 report-code-uniqueness-within-application) in `backend/tests/AdminPro.Application.Tests/Reportes/CreateReporteTests.cs`; implement `CreateReporteCommand`, `CreateReporteCommandValidator`, `CreateReporteCommandHandler` in `backend/src/AdminPro.Application/Reportes/Commands/CreateReporte/`, mirroring `Applications/Commands/CreateApplication/`.
- [x] 1.2 TDD: write failing tests for `UpdateReporteCommand`/`DeleteReporteCommand` handlers (re-validate REP-001 excluding own row on update; soft-delete via `Activo = false` on delete); implement in `backend/src/AdminPro.Application/Reportes/Commands/UpdateReporte/` and `DeleteReporte/`. Deviation: `POST` lives on `ApplicationsController` (not `ReportesController`), matching the precedent `CreateEnvironment` set on `AmbientesController`/`ApplicationsController` in Phase 4.
- [x] 1.3 Add `ReportesController` (`backend/src/AdminPro.Api/Controllers/ReportesController.cs`): `PUT /api/reportes/{id}`, `DELETE /api/reportes/{id}` (create is `POST /api/applications/{appId}/reportes` on `ApplicationsController`, see 1.2 deviation note).

## 2. Backend — Nota CRUD (TDD)

- [x] 2.1 TDD: write failing validator/handler tests for `CreateNotaCommand` (rules APP-CMD-010, NOTE-001 `Titulo` required, NOTE-002 `Descripcion` required memo text) in `backend/tests/AdminPro.Application.Tests/Notas/CreateNotaTests.cs`; implement `CreateNotaCommand`, `CreateNotaCommandValidator`, `CreateNotaCommandHandler` in `backend/src/AdminPro.Application/Notas/Commands/CreateNota/`.
- [x] 2.2 TDD: write failing tests for `UpdateNotaCommand`/`DeleteNotaCommand` handlers; implement in `backend/src/AdminPro.Application/Notas/Commands/UpdateNota/` and `DeleteNota/`.
- [x] 2.3 Add `NotasController` (`backend/src/AdminPro.Api/Controllers/NotasController.cs`): `PUT /api/notas/{id}`, `DELETE /api/notas/{id}` (create is `POST /api/applications/{appId}/notas` on `ApplicationsController`, same deviation as Reportes).

## 3. Backend — Documento CRUD (TDD)

- [x] 3.1 TDD: write failing validator/handler tests for `CreateDocumentoCommand` (rules APP-CMD-011, DOC-001 `UrlOneDrive` required + valid absolute URL, DOC-002 `Tipo` one of manual/diagrama/codigo/otro) in `backend/tests/AdminPro.Application.Tests/Documentos/CreateDocumentoTests.cs`; implement `CreateDocumentoCommand`, `CreateDocumentoCommandValidator`, `CreateDocumentoCommandHandler` in `backend/src/AdminPro.Application/Documentos/Commands/CreateDocumento/`. Deviation: validator checks `Tipo` is non-empty (max 50 chars) rather than restricting to the 4-value catalog, per DOC-002's "MAY allow free-text entry"; the frontend select still suggests the 4 values.
- [x] 3.2 TDD: write failing tests for `UpdateDocumentoCommand`/`DeleteDocumentoCommand` handlers; implement in `backend/src/AdminPro.Application/Documentos/Commands/UpdateDocumento/` and `DeleteDocumento/`.
- [x] 3.3 Add `DocumentosController` (`backend/src/AdminPro.Api/Controllers/DocumentosController.cs`): `PUT /api/documentos/{id}`, `DELETE /api/documentos/{id}` (create is `POST /api/applications/{appId}/documentos` on `ApplicationsController`, same deviation as Reportes/Notas).

## 4. Backend — FixData CRUD (TDD)

- [x] 4.1 TDD: write failing validator/handler tests for `CreateFixDataCommand` (rules APP-CMD-012, FIX-001 `Nombre` required, FIX-002 `Script` free text) in `backend/tests/AdminPro.Application.Tests/FixDatas/CreateFixDataTests.cs`; implement `CreateFixDataCommand`, `CreateFixDataCommandValidator`, `CreateFixDataCommandHandler` in `backend/src/AdminPro.Application/FixDatas/Commands/CreateFixData/`.
- [x] 4.2 TDD: write failing tests for `UpdateFixDataCommand`/`DeleteFixDataCommand` handlers; implement in `backend/src/AdminPro.Application/FixDatas/Commands/UpdateFixData/` and `DeleteFixData/`.
- [x] 4.3 Add `FixDatasController` (`backend/src/AdminPro.Api/Controllers/FixDatasController.cs`): `PUT /api/fixdatas/{id}`, `DELETE /api/fixdatas/{id}` (create is `POST /api/applications/{appId}/fixdatas` on `ApplicationsController`, same deviation as Reportes/Notas/Documentos).

## 5. Backend — Populate real children in Application detail (TDD)

- [x] 5.1 TDD: write a failing test asserting `GetApplicationByIdQueryHandler` returns real `ReporteDto`/`NotaDto`/`DocumentoDto`/`FixDataDto` arrays (each ordered by `Orden`) instead of the always-empty placeholders; add the four DTOs to `ApplicationDetailDto.cs` (mirroring `AmbienteDto`) and populate them in `GetApplicationByIdQueryHandler`, per rule APP-QRY-005. Verify green. Deviation: `ReporteDto` has no `Orden` (the `Reporte` entity has none per business-rules.md §2.2.6) — ordered by `ReportCode` instead.
- [x] 5.2 Run the full backend test suite (`dotnet test`) to confirm no regressions. Verified: 111 Application.Tests + 46 Api.Tests, all green.

## 6. Frontend — Shared model & services

- [x] 6.1 Extend `frontend/src/app/shared/models/project.model.ts`: add `Reporte`, `Nota`, `Documento`, `FixData` interfaces and their `Create*Command`/`Update*Command` types; update `ApplicationDetail`'s `reportes`/`notas`/`documentos`/`fixDatas` fields from `unknown[]` to the new typed arrays.
- [x] 6.2 Add `ReporteService`, `NotaService`, `DocumentoService`, `FixDataService` under `frontend/src/app/shared/services/`, each mirroring `EnvironmentService` (`create(applicationId, command)`, `update(id, command)`, `delete(id)` against the new controllers).

## 7. Frontend — ApplicationDetail: Reportes/Documentos/FixDatas sections (TDD)

- [x] 7.1 TDD: write failing tests asserting the "Reportes" section renders an empty state, then a list after creation, with edit/delete wired to `ReporteService`; implement in `application-detail.html`/`.ts` reusing the existing Ambientes modal-form pattern (own modal + form group for `ReportCode`/`ReportName`/etc.).
- [x] 7.2 TDD: same pattern for "Documentos" (`NombreArchivo`/`UrlOneDrive`/`Tipo` select/`Descripcion`), including the "open in new tab" link on `UrlOneDrive`.
- [x] 7.3 TDD: same pattern for "FixDatas" (`Nombre`/`Descripcion`/`Script` textarea), including a "copy script" action that copies `Script` to the clipboard.

## 8. Frontend — ApplicationDetail: Notas collapsible list (TDD)

- [x] 8.1 TDD: write a failing test asserting a note row renders only its `Titulo` when collapsed and reveals `Descripcion` when clicked/expanded; implement the "Notas" section in `application-detail.html`/`.ts` as a collapsible list (chevron icon, click toggles a per-row expanded state), with the count badge and "+ Agregar Nota" action, matching `docs/design/DESIGN.md` §5.4 and the reference mockup (`▶ nvm use 14.16.0` / description on expand).
- [x] 8.2 TDD: write failing tests for add/edit/delete via the Nota modal form (`Titulo`, `Descripcion`), wired to `NotaService`; implement and verify green.

## 9. Frontend — Remove Notas de arranque placeholder from ApplicationForm

- [x] 9.1 Remove the disabled "Notas de arranque" section from `application-form.html` (Servicios vinculados placeholder stays); remove any now-unused CSS rules scoped only to that section from `application-form.scss`.
- [x] 9.2 Run `application-form.spec.ts` to confirm no regressions (no test currently asserts on that placeholder, but re-run to be sure). Verified: 62/62 green across the full suite.

## 10. Verification

- [x] 10.1 `dotnet build`/`dotnet test` succeed with zero errors across the backend solution. Verified: 111 Application.Tests + 46 Api.Tests, all green.
- [x] 10.2 `ng build` succeeds with zero errors; Angular unit tests all green (`ng test`). Verified: build succeeded (pre-existing budget warnings only, no errors), 62/62 tests green across 19 spec files.
- [x] 10.3 Manually verify all four sections plus the Notas expand/collapse behavior via the running app (temporary project/application created via API, verified, then deleted — no real project's data touched); screenshot each section's empty state, populated state, and (for Notas) collapsed vs. expanded row. Verified via Playwright against the real running app: empty states for all sections, then Reporte/Nota x2/Documento/FixData created through the UI, confirmed persisted via `GET /api/applications/{id}`, first Nota expanded showing its description while the second stayed collapsed — matching the reference mockup exactly. Zero console/page errors. Temp project deleted afterward.
- [x] 10.4 Confirm every task above was committed individually per the repo's commit conventions. Verified via `git log --oneline feature/application-children`.
- [x] 10.5 Mark this `tasks.md` complete and ready for `/opsx:archive`.
