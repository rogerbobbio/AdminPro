## Context

`Application` and `Ambiente` domain entities, EF configurations, and `AppDbContext` DbSets all already exist from `solution-foundation` (Phase 1) — both tables were part of the initial `InitialCreate` migration, so no schema change is needed. No CQRS handlers or controllers exist for either entity yet. `ProjectDetailDto` (from `projects-management`, Phase 3) already has an `applications: ApplicationSummaryDto[]` field, but it is always empty since nothing can create an `Application` yet. This change follows the exact CQRS/controller/frontend patterns `projects-management` established: thin MediatR handlers using `AppDbContext` directly, manual DTO mapping, `NotFoundException`/`DomainException` middleware mappings already wired up, Signal-based Angular services, reactive forms for the primary entity and modals for its simple child.

## Goals / Non-Goals

**Goals:**
- Full CRUD for `Application` (list by project, detail, create, update, soft-delete) per `docs/business-rules.md` §3.4 (APP-001..004) and API contracts in `docs/design/DESIGN.md` §4.3.
- Full CRUD for `Ambiente` (create, update, soft-delete, listed as part of Application detail) per §3.5 (ENV-001..004) and `docs/design/DESIGN.md` §4.4.
- Replace `ProjectDetail`'s Aplicaciones empty-state (Phase 3 Non-Goal) with a real list + "+ Nueva Aplicación" action.
- `ApplicationDetail` screen (`/proyectos/aplicaciones/:id`) with an Ambientes accordion section, per `docs/design/DESIGN.md` §5.4 and rule PRES-UI-008.

**Non-Goals:**
- `Reporte`, `Nota`, `Documento`, `FixData` CRUD — Phase 5. `ApplicationDetail`'s corresponding accordion sections render as empty-states, matching how `projects-screens` handled the not-yet-built Aplicaciones section in Phase 3.
- `Servicio` catalog and `AplicacionServicio` linking — Phase 6. The Servicios accordion section also renders empty.
- Drag-and-drop reordering of Ambientes (rule PRES-UI-007) — Phase 7 polish. `Orden` is settable on create/edit but there's no reorder-by-dragging UI yet.
- Global search — Phase 6.

## Decisions

**1. `ApplicationNameAlreadyExistsInProject` enforced via FluentValidation `MustAsync`, scoped to `ProyectoId`, not a thrown `DomainException`.**
Same reasoning as `projects-management`'s Decision 1 for `ProjectNameAlreadyExists` (rule APP-001 reads as a domain error but `docs/design/DESIGN.md`'s validation pattern and `XCUT-ERR-001`'s duplicate-name example both point to a 400 validation error). The uniqueness check is scoped `WHERE ProyectoId = @id AND Nombre = @nombre`, unlike `Project`'s global uniqueness check.

**2. `Application` CQRS shape, per `docs/business-rules.md` APP-CMD-007 and the Application Validator in APP-VAL-001:**
```csharp
public record CreateApplicationCommand(int ProyectoId, string Nombre, string? Descripcion, string? TecnologiaFront, string? TecnologiaBack, string? RamaDesarrollo, string? ApplicationName, string? TieneProyectoBD, string? RutaLocal, string? RutaGit, string? ComoSeLevanta, string? NotasCompilacion, int Orden) : ICommand<int>;
public record UpdateApplicationCommand(int Id, string Nombre, string? Descripcion, string? TecnologiaFront, string? TecnologiaBack, string? RamaDesarrollo, string? ApplicationName, string? TieneProyectoBD, string? RutaLocal, string? RutaGit, string? ComoSeLevanta, string? NotasCompilacion, int Orden) : ICommand;
public record DeleteApplicationCommand(int Id) : ICommand;
```
`ProyectoId` must reference an existing active `Project` (404 via `NotFoundException` if not, mirroring `CreateBaseDeDatosCommandHandler`'s project-existence check).

**3. `Ambiente` CQRS shape, per APP-CMD-008 and the Environment Validator:**
```csharp
public record CreateEnvironmentCommand(int AplicacionId, string Nombre, string? Url, bool EsWebApi, string? Notas, int Orden) : ICommand<int>;
public record UpdateEnvironmentCommand(int Id, string Nombre, string? Url, bool EsWebApi, string? Notas, int Orden) : ICommand;
public record DeleteEnvironmentCommand(int Id) : ICommand;
```
`Url`, when non-empty, must be a valid absolute `http://`/`https://` URL (rule ENV-002), validated with the same regex as `PRES-VAL-001`. No separate "list environments" endpoint — `GetApplicationByIdQuery`'s `ApplicationDetailDto` already embeds `ambientes`, matching how `projects-management` skipped a standalone "list databases" endpoint.

**4. Routes: `POST` nested, `PUT`/`DELETE` flat — same asymmetry as `projects-api`'s `BaseDeDatos` endpoints.**
- `GET /api/projects/{projectId}/applications` (list), `GET /api/applications/{id}` (detail), `POST /api/projects/{projectId}/applications` (create — needs `projectId` for the command), `PUT /api/applications/{id}`, `DELETE /api/applications/{id}`.
- `POST /api/applications/{appId}/ambientes` (create — needs `appId`), `PUT /api/ambientes/{id}`, `DELETE /api/ambientes/{id}`.

**5. Soft-delete cascade: `DeleteApplicationCommandHandler` deactivates its `Ambiente` children (rule APP-002), written generically for all listed child types.**
Per rule APP-002, deleting an Application cascades to Environments, Reports, Notes, Documents, FixDatas, and Application-Service links. Only `Ambiente` exists today, so the handler iterates `IgnoreQueryFilters()`-loaded `Ambiente` rows and sets `Activo = false`; the same handler also (no-op today, but written as empty `IgnoreQueryFilters()` iterations, matching `projects-management` Risk mitigation #2's "write it now, cheap" approach) leaves clearly-marked extension points as comments for Reportes/Notas/Documentos/FixDatas so Phase 5 wires them in without re-deriving the pattern.

**6. `ProjectDetailDto.Applications` now returns real rows; `ApplicationSummaryDto` shape unchanged (id, nombre, tecnologiaFront, tecnologiaBack, orden, activo) per `APP-DTO-001` (flat DTOs for lists) — no `GetProjectByIdQueryHandler` DTO changes needed, only the underlying query stops returning an empty list.**

**7. Angular: `ApplicationDetail` and `ApplicationForm` follow `ProjectDetail`/`ProjectForm`'s page-per-entity pattern; Ambientes use a modal, matching how `BaseDeDatos` used a modal under `ProjectDetail`.**
- `ProjectDetail`'s Aplicaciones section gains a "+ Nueva Aplicación" button (→ `/proyectos/aplicaciones/nuevo?proyectoId=:id`, mirroring how `BaseDeDatos`'s modal captures `ProyectoId` from the parent route) and a clickable list (→ `/proyectos/aplicaciones/:id`).
- `ApplicationDetail` (`/proyectos/aplicaciones/:id`): header with Application fields, then accordion sections per PRES-UI-008 (Ambientes, Reportes, Notas, Documentos, FixDatas, Servicios) — only Ambientes is populated; the other five render `projects-screens`-style empty-states with no create action.
- `ApplicationForm` (`/proyectos/aplicaciones/nuevo`, `/proyectos/aplicaciones/:id/editar`): reactive form for `Nombre` (required), `Descripcion`, `TecnologiaFront`, `TecnologiaBack` (the wireframe's visible fields per `docs/design/DESIGN.md` §5.5); the remaining optional command fields (`RamaDesarrollo`, `ApplicationName`, `TieneProyectoBD`, `RutaLocal`, `RutaGit`, `ComoSeLevanta`, `NotasCompilacion`) are included as additional optional inputs on the same form rather than deferred, since the command already requires them and splitting the form would need a second round-trip.

## Risks / Trade-offs

- **[Risk]** `DeleteApplicationCommandHandler`'s cascade only actually deactivates `Ambiente` today; the Reportes/Notas/Documentos/FixDatas extension points are unused code until Phase 5. → **Mitigation**: keep them as short, obviously-dead comments (not speculative code) referencing APP-002 and the Phase 5 change name, so a future reader knows the cascade contract without half-built branches that could hide bugs.
- **[Risk]** `ApplicationForm` exposing 11 fields at once (vs. `ProjectForm`'s 2) risks a cluttered create form for a "just get started" flow. → **Mitigation**: only `Nombre` is required; the wireframe-visible fields (`Descripcion`, `TecnologiaFront`, `TecnologiaBack`) are shown by default, the rest are grouped under a collapsed "Detalles técnicos" sub-section — form validation and submission are unaffected, this is a presentation grouping only.
- **[Risk]** `ApplicationDetail`'s five empty accordion sections could look unfinished/broken to a user. → **Mitigation**: identical empty-state pattern users already saw in `ProjectDetail`'s Aplicaciones section during Phase 3, so it reads as "coming in a later phase," not as a bug.

## Migration Plan

1. Backend: `Application` CQRS (Create/Update/Delete/GetByProjectId/GetById) with validators; `Ambiente` CQRS (Create/Update/Delete) with validators; `ApplicationsController`, `AmbientesController`. `dotnet ef database update` not needed — tables already exist.
2. Frontend: `ApplicationService`/`EnvironmentService`, `ApplicationDetail`, `ApplicationForm`, Ambiente modal; update `ProjectDetail`'s Aplicaciones section and `proyectos.routes.ts`.
3. Cypress: Create Application under a Project → Add Environment → verify detail page (per rule PRES-E2E-001's second flow).

No rollback complexity — additive only, no destructive migration.
