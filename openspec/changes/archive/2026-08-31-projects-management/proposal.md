## Why

Phase 3 of the roadmap (`docs/business-rules.md` §"Implementation Phases"). The Dashboard's "Gestión de Proyectos" module card currently links to a static "Próximamente" placeholder (`/proyectos`, added in `frontend-dashboard`) — there is no way yet to create, view, edit, or delete a `Project` or its `BaseDeDatos` children. This change replaces that placeholder with the first real screens of the Proyectos module.

## What Changes

- Backend: full CQRS CRUD for `Project` (`CreateProjectCommand`, `UpdateProjectCommand`, `DeleteProjectCommand`, `GetProjectsQuery`, `GetProjectByIdQuery`) per rules APP-CMD-004/005/006 and APP-QRY-002/003, and for `BaseDeDatos` (analogous Create/Update/Delete commands + a list query scoped to a project — no rule numbers exist for this in `docs/business-rules.md`, so this change defines them following the same pattern as `Project`).
- Backend: add the `NotFoundException` → `404` and `DomainException` → `409` middleware mappings that `docs/design/DESIGN.md` §10.1 specifies but `foundation-backend` never implemented (only `ValidationException`/500 exist today) — needed by `GetProjectByIdQuery`/`Update`/`Delete` when the id doesn't exist.
- Backend: `ProjectNameAlreadyExists` (rule PROJECT-001) is enforced via an async FluentValidation rule (`MustAsync`, per `docs/design/DESIGN.md` §2.5's own example), not a thrown `DomainException` — see design.md for why this is a deliberate divergence from the business-rules.md wording.
- Backend: soft-deleting a `Project` cascades to its `BaseDeDatos` rows per rule PROJECT-002 (child `Application` cascade is a no-op today since no applications exist until Phase 4, but the handler is written to cascade to both).
- Frontend: `ProyectosLayout` (sidebar shell wrapper, replacing the flat `ComingSoon` placeholder at `/proyectos`), `ProjectList` (`/proyectos`), `ProjectDetail` (`/proyectos/:id`) showing its `BaseDeDatos` list with add/edit/delete, and `ProjectForm` (create at `/proyectos/nuevo`, edit at `/proyectos/:id/editar`), matching `docs/design/DESIGN.md` §5.2/§5.3 wireframes.
- Frontend: the Project Detail page's "Aplicaciones" section is read-only/empty in this change (no `Application` entity work until Phase 4) — it renders an empty-state, not the mockup's "+ Nueva Aplicación" action.
- Cypress: project CRUD flow test (create → appears in list → edit → view detail → delete), matching `docs/design/DESIGN.md` §11.2's plan.

Out of scope for this change: `Application` CRUD (Phase 4), `Servicio`/global search (later phases), soft-delete of `BaseDeDatos` independent from its parent Project's cascade (a lone `BaseDeDatos` delete is in scope; the cascade-on-project-delete is the only cross-entity behavior here).

## Capabilities

### New Capabilities
- `projects-api`: Backend CQRS CRUD for `Project` and `BaseDeDatos`, plus the `NotFoundException`/`DomainException` → HTTP status middleware mappings.
- `projects-screens`: The Proyectos module's list/detail/form frontend screens and their `ProjectService`/`DatabaseService`.

### Modified Capabilities
(none — `solution-foundation`, `frontend-shell`, `dashboard`, `dashboard-api` are unchanged; this change only adds new capabilities alongside them)

## Impact

- **New (backend)**: `AdminPro.Application/Projects/{Commands,Queries}/`, `AdminPro.Application/BaseDeDatos/{Commands,Queries}/`, `AdminPro.Application/Common/Exceptions/NotFoundException.cs`, `AdminPro.Domain/Exceptions/` usage for a `DomainException`-mapping test, `AdminPro.Api/Controllers/ProjectsController.cs`, `BaseDeDatosController.cs` (nested under `/api/projects/{projectId}/basesdedatos`).
- **Modified (backend)**: `ExceptionHandlerMiddleware` (add the two new mappings).
- **New (frontend)**: `frontend/src/app/features/proyectos/` (layout, pages, services), replacing the `ComingSoon` route registration at `/proyectos` with the real module's routes.
- **Dependencies**: none new; reuses the CQRS pipeline, `AppDbContext`, `AppShell`, and Signal-based service pattern already established.
