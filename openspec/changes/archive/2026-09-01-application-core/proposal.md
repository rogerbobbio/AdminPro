## Why

Projects can currently hold `BaseDeDatos` children (Phase 3) but `Application` — the core entity of the whole catalog — has no CRUD surface yet: the `Application`/`Ambiente` domain entities and EF configuration exist (Phase 1 foundation) but there is no Application layer (commands/queries), no API controllers, and no frontend screens. Phase 4 of the roadmap (`docs/business-rules.md` "Implementation Phases") closes this gap: Application CRUD, Ambiente (Environment) CRUD, and the Application Detail screen with an Environments accordion.

## What Changes

- Backend: `CreateApplication`/`UpdateApplication`/`DeleteApplication` commands and `GetApplicationsByProject`/`GetApplicationById` queries, per rules APP-CMD-007, APP-QRY-004/005, with FluentValidation per APP-VAL-001.
- Backend: `CreateEnvironment`/`UpdateEnvironment`/`DeleteEnvironment` commands for `Ambiente`, per rule APP-CMD-008 and the Environment Validator in APP-VAL-001.
- Backend: `ApplicationsController` (`/api/applications`, `/api/projects/{projectId}/applications`) and `AmbientesController` (`/api/applications/{appId}/ambientes`, `/api/ambientes/{id}`), reusing the existing `NotFoundException`/`DomainException` HTTP mappings from `projects-api`.
- Backend: enforce APP-001 (Application name unique within Project) and APP-002 (deleting an Application soft-deletes its Ambientes — other child types cascade in later phases since they don't exist yet).
- Backend: extend `ProjectDetailDto`'s `applications` array (currently always-empty per the `projects-screens` note) to reflect real created applications; it stays a flat summary (no `ambientes`) per APP-DTO-001.
- Frontend: `ApplicationDetail` page (`/proyectos/aplicaciones/:id`) showing the application's fields and an "Ambientes" accordion section with add/edit/delete (modal forms), plus empty-state placeholders for the not-yet-built Reportes/Notas/Documentos/FixDatas/Servicios sections (PRES-UI-008), matching the pattern `projects-screens` used for the Aplicaciones empty-state.
- Frontend: `ApplicationForm` page (create/edit) for `Nombre`, `Descripcion`, `TecnologiaFront`, `TecnologiaBack` (the other optional technical fields from APP-CMD-007 — `RamaDesarrollo`, `RutaLocal`, `RutaGit`, etc. — are included as optional fields per the same command shape).
- Frontend: `ProjectDetail`'s "Aplicaciones" section gains a "+ Nueva Aplicación" action and list wiring (replacing the Phase 3 empty-state-only behavior), navigating to the new Application screens.

## Capabilities

### New Capabilities
- `application-core-api`: Backend CRUD API for `Application` and `Ambiente` entities, including name-uniqueness and cascade-delete rules.
- `application-core-screens`: Frontend Application Detail and Application Form screens, plus the Environments accordion.

### Modified Capabilities
- `projects-screens`: The "Aplicaciones" section of `ProjectDetail` changes from a pure empty-state to a real list with a create action, since `Application` CRUD is now in scope.

## Impact

- Backend: new `AdminPro.Application/Applications/` and `AdminPro.Application/Ambientes/` feature folders (Commands, Queries, DTOs, Validators); new `ApplicationsController`, `AmbientesController`; extends `ProjectDetailDto`/`GetProjectByIdQueryHandler` to populate real applications.
- Frontend: new `frontend/src/app/features/proyectos/pages/application-detail/` and `application-form/`; new `ApplicationService`/`EnvironmentService`; updates `proyectos.routes.ts`, `project.model.ts`, and `ProjectDetail`'s Aplicaciones section.
- No schema changes — `Application` and `Ambiente` tables already exist from the Phase 1 `InitialCreate` migration.
