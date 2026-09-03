## Why

Phase 4 (`application-core`) shipped `Application` and `Ambiente` CRUD, but `ApplicationDetail` still renders `Reportes`, `Notas`, `Documentos`, and `FixDatas` as permanently-empty placeholder cards (per `ApplicationDetailDto`'s always-empty arrays) — there is no domain model, Application layer, API, or real frontend UI for any of them yet. Phase 5 of the roadmap (`docs/business-rules.md` §8 "Implementation Phases") closes this gap. `Notas` is the most immediately useful of the four (the `ApplicationForm`'s disabled "Notas de arranque" placeholder already hints at the need for startup/how-to notes on an application) and has a concrete UX requirement: a collapsible list (title always visible, description revealed on expand), not a flat always-visible list.

## What Changes

- Backend: `CreateReporte`/`UpdateReporte`/`DeleteReporte`, `CreateNota`/`UpdateNota`/`DeleteNota`, `CreateDocumento`/`UpdateDocumento`/`DeleteDocumento`, and `CreateFixData`/`UpdateFixData`/`DeleteFixData` commands, per rules APP-CMD-009 through APP-CMD-012 and the corresponding validators in APP-VAL-001 (§4.2 and §4.3 of `docs/business-rules.md`).
- Backend: `ReportesController`, `NotasController`, `DocumentosController`, `FixDatasController` (or a shared pattern matching the existing `AmbientesController`), each scoped under `/api/applications/{appId}/...` for create and `/api/{resource}/{id}` for update/delete, mirroring `EnvironmentService`'s existing routes.
- Backend: `GetApplicationByIdQuery` (APP-QRY-005) stops returning always-empty `reportes`/`notas`/`documentos`/`fixDatas` arrays and instead populates them from the database, ordered by `Orden`, matching how `ambientes` already works.
- Backend: enforce REP-001 (`ReportCode` unique within Application), NOTE-001/002 (`Titulo` required, `Descripcion` is unlimited-length memo text), DOC-001/002 (`UrlOneDrive` required + valid URL, `Tipo` from a suggested catalog), FIX-001/002 (`Nombre` required, `Script` free SQL text), and APP-002's cascade (deleting an Application soft-deletes these children too — already true structurally via `OnDelete(Cascade)` per `docs/design/DESIGN.md`, this just adds the commands that create/update/delete rows directly).
- Frontend: `ApplicationDetail` gains four real sections replacing the always-empty placeholders:
  - **Notas**: a collapsible list — each row shows a bold `Titulo` with an expand/collapse chevron; expanding reveals `Descripcion` below it. A count badge next to the "Notas" header and a "+ Agregar Nota" pill button at the bottom, matching the attached reference screenshot. Add/edit via a modal form (same pattern as the existing Ambientes modal).
  - **Reportes**, **Documentos**, **FixDatas**: table/list sections with add/edit/delete via modal forms, following the same empty-state → real-list pattern already established for Ambientes (`No hay reportes/documentos/fix datas todavía.` when empty, "+ Agregar" action, edit/delete buttons per row).
- Frontend: `ApplicationForm`'s disabled "Notas de arranque" **Próximamente** placeholder section is removed (Notas now has a real home on `ApplicationDetail`, matching how Ambientes is managed there rather than embedded in the create/edit form). The "Servicios vinculados" placeholder is untouched — that's Phase 6 (`Services & Search`), out of scope here.

## Capabilities

### New Capabilities
- `application-children-api`: Backend CRUD API for `Reporte`, `Nota`, `Documento`, and `FixData` entities (children of `Application`), including their field-level validation rules.
- `application-children-screens`: Frontend `ApplicationDetail` sections for Reportes, Notas (collapsible list), Documentos, and FixDatas, each with add/edit/delete.

### Modified Capabilities
- `application-core-api`: `GetApplicationByIdQuery` (`GET /api/applications/{id}`) no longer returns always-empty `reportes`/`notas`/`documentos`/`fixDatas` arrays — they're populated from real data.
- `application-core-screens`: `ApplicationForm` no longer renders the disabled "Notas de arranque" placeholder section (Notas moves to `ApplicationDetail`).

## Impact

- Backend: new `AdminPro.Application/Reportes/`, `Notas/`, `Documentos/`, `FixDatas/` feature folders (Commands, Queries where needed, Validators); new `ReportesController`/`NotasController`/`DocumentosController`/`FixDatasController` (or nested routes on `ApplicationsController`, matching the `AmbientesController` precedent); extends `GetApplicationByIdQueryHandler` to populate the four collections.
- Frontend: new `NotaService`/`ReporteService`/`DocumentoService`/`FixDataService` (mirroring `EnvironmentService`); extends `application-detail.html`/`.ts` with four new sections and modal forms; removes the "Notas de arranque" block from `application-form.html`/`.ts`.
- No schema changes — `Reporte`, `Nota`, `Documento`, and `FixData` tables already exist from the Phase 1 `InitialCreate` migration (per `docs/business-rules.md` §2.2.6-2.2.9).
