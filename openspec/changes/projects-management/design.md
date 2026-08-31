## Context

`Project` and `BaseDeDatos` entities, `AppDbContext` configuration (soft-delete filter, `DeleteBehavior.Restrict` on `Project → BaseDeDatos`), and the CQRS pipeline all already exist from `solution-foundation`. No CQRS handlers or controllers exist yet for either entity — this change adds the first ones on top of the Foundation scaffold, following the exact patterns `docs/design/DESIGN.md` already lays out (§2.4-§2.6) and that `frontend-dashboard` proved out for `Modulo`/dashboard-summary (thin MediatR handlers using `AppDbContext` directly, manual DTO mapping, Signal-based Angular services).

## Goals / Non-Goals

**Goals:**
- Full CRUD for `Project` (list, detail, create, update, soft-delete) and `BaseDeDatos` (create, update, soft-delete, listed as part of Project detail) per `docs/business-rules.md` §3.2/§3.3 and API contracts in `docs/design/DESIGN.md` §4.2.
- Wire up the two middleware exception mappings (`NotFoundException`→404, `DomainException`→409) that were specified in Phase 1's design but never implemented, since this change is the first to actually need them.
- Replace the `/proyectos` `ComingSoon` placeholder with the real module: a layout (sidebar sub-nav, matching `docs/design/DESIGN.md` §3.1's `ProyectosLayoutComponent`), list, detail, and form pages.

**Non-Goals:**
- `Application` CRUD or its child entities (Ambiente, Reporte, Nota, Documento, FixData, AplicacionServicio) — Phase 4. The Project Detail page's "Aplicaciones" section renders empty/read-only.
- `Servicio` catalog, global search — later phases.
- Drag-and-drop reordering, bulk import — not in any current wireframe for this phase.

## Decisions

**1. `ProjectNameAlreadyExists` enforced via FluentValidation `MustAsync`, not a thrown `DomainException`.**
`docs/business-rules.md` rule PROJECT-001 says the operation "SHALL be rejected with a domain error: `ProjectNameAlreadyExists`", which reads as a `DomainException` (→409 per the design's own error table). But `docs/design/DESIGN.md` §2.5 gives a concrete, working example of exactly this rule implemented as an async FluentValidation rule on `CreateProjectCommandValidator` (→400, per rule XCUT-ERR-001's "duplicate name" example). Following the concrete, already-designed implementation over the more ambiguous prose: uniqueness is a *validation* concern (it's about the shape/acceptability of the input given current state), consistent with how `ValidationBehavior` already runs before any handler logic, and it keeps the pattern identical for `Update` (same validator, `MustAsync` excludes the current row's own id).

**2. `NotFoundException` and `DomainException` middleware mappings, added now.**
`docs/design/DESIGN.md` §10.1 documents both, but `foundation-backend`'s tasks 5.3/5.4 only implemented `ValidationException` and the unhandled-exception fallback (verified by reading `ExceptionHandlerMiddleware.cs` — no `NotFoundException`/`DomainException` branches exist). This change needs `NotFoundException` for `GetProjectByIdQuery`/`UpdateProjectCommand`/`DeleteProjectCommand`/(same for `BaseDeDatos`) when the id doesn't exist (rule XCUT-API-003, 404). `DomainException` is added for completeness per the original design table (mapped to 409) even though this change has no scenario that throws it yet — it's the base class already declared in `AdminPro.Domain.Exceptions` from Foundation, just never wired to the middleware.

**3. `BaseDeDatos` CQRS shape (no rule numbers exist for it in business-rules.md — designed here, mirroring `Project`'s APP-CMD-004/005/006 shape and the entity's columns from §2.2.3):**
```csharp
public record CreateBaseDeDatosCommand(int ProyectoId, string Nombre, string? Servidor, int? DatabaseId, string? LoginName, string? Ambiente, string? Notas) : ICommand<int>;
public record UpdateBaseDeDatosCommand(int Id, string Nombre, string? Servidor, int? DatabaseId, string? LoginName, string? Ambiente, string? Notas) : ICommand;
public record DeleteBaseDeDatosCommand(int Id) : ICommand;
```
No separate "list databases by project" query is needed as its own endpoint — `GetProjectByIdQuery`'s `ProjectDetailDto` already embeds the `basesDeDatos` array per `docs/design/DESIGN.md` §4.2's documented response shape, so the Project Detail page's initial load already has everything; only the 3 mutations need their own endpoints (nested under `/api/projects/{projectId}/basesdedatos`).

**Amendment (post-implementation, user request):** added a `Password` field — a new `nvarchar` column on `BaseDeDatos`, stored as plain text. This is an explicit user decision for this single-user, no-auth internal tool, consistent with `Servidor`/`LoginName`/`Notas` already being plain text with no encryption — added via a new `AddBaseDeDatosPassword` migration (the entity/table already exists from Foundation, so this is the first schema-changing migration since `SeedModulos`). The UI's "Agregar/Editar Base de Datos" modal was also missing two already-designed fields — `LoginName` (labeled "Usuario" in the UI; no new backend field) and `DatabaseId` — which are now exposed in the form alongside `Password`. Updated command shape:
```csharp
public record CreateBaseDeDatosCommand(int ProyectoId, string Nombre, string? Servidor, int? DatabaseId, string? LoginName, string? Password, string? Ambiente, string? Notas) : ICommand<int>;
public record UpdateBaseDeDatosCommand(int Id, string Nombre, string? Servidor, int? DatabaseId, string? LoginName, string? Password, string? Ambiente, string? Notas) : ICommand;
```

**4. Soft-delete cascade implemented in the `DeleteProjectCommand` handler, not via a DB trigger.**
Per `solution-foundation`'s own decision (`Project → BaseDeDatos`/`Application` use `DeleteBehavior.Restrict`, "soft cascade implemented in the Application layer"). `DeleteProjectCommandHandler` sets `Activo = false` on the `Project`, then iterates `IgnoreQueryFilters()`-loaded `BaseDeDatos` and `Application` children (currently always empty for `Application`, but written generically) and sets `Activo = false` on each, in one transaction (via `TransactionBehavior`, already automatic for any `ICommand`).

**5. Angular: `ProyectosLayout` wraps `AppShell`, replacing `ComingSoon` at `/proyectos`.**
Matches `docs/design/DESIGN.md` §3.1's nesting (`ProyectosLayoutComponent` renders the module's own sub-nav — Proyectos/Servicios/Buscar — inside the existing shell) and §6's component hierarchy. Implemented as a layout route: `/proyectos` and its children render inside `ProyectosLayout`, which itself renders inside `AppShell` (passing `activeNav="proyectos"` once, not per-child-page — the one gap `frontend-dashboard`'s `AppShell` design left open, resolved now that a second real screen exists per that change's design.md Risk #3).
- `ProjectList` (`/proyectos`): cards, search (client-side filter, per `docs/design/DESIGN.md` §5.2), "Nuevo Proyecto" → `/proyectos/nuevo`.
- `ProjectDetail` (`/proyectos/:id`): databases table (add/edit/delete via modal forms) + an empty-state "Aplicaciones" card (no create action, per Non-Goals).
- `ProjectForm` (`/proyectos/nuevo`, `/proyectos/:id/editar`): reactive form, mirrors backend validation (required `Nombre`, max length, uniqueness surfaced from the 400 response).

**6. Modals for `BaseDeDatos` CRUD, full pages for `Project` CRUD.**
`BaseDeDatos` is a small, single-purpose child form (6 fields, no nested children of its own) — a Bootstrap modal avoids a route/page per mockup's implied "add database" interaction. `Project` gets full pages since it's the primary navigable entity with its own detail view and deep-linkable URL.

## Risks / Trade-offs

- **[Risk]** Diverging from business-rules.md's literal "domain error" wording for PROJECT-001 (Decision 1) could surprise a reader expecting a `DomainException`/409. → **Mitigation**: documented explicitly here and in the `projects-api` spec; behavior (rejection + specific error code) is preserved, only the HTTP status/exception type differs from a literal reading.
- **[Risk]** `DeleteProjectCommandHandler`'s generic cascade-to-`Application` code path is untestable with real data until Phase 4 ships. → **Mitigation**: write it now (cheap, ~3 lines) and cover it with a unit test seeding a fake `Application` row directly via `AppDbContext`, rather than deferring and risking forgetting the cascade when Phase 4 lands.
- **[Risk]** Nested route `/api/projects/{projectId}/basesdedatos` for `BaseDeDatos` mutations vs. a flat `/api/basesdedatos/{id}` — the nested form requires `projectId` in the URL for create but not for update/delete (which only need the row's own id). → **Mitigation**: use `/api/projects/{projectId}/basesdedatos` for `POST` only (need `projectId` to construct the command); `PUT /api/basesdedatos/{id}` and `DELETE /api/basesdedatos/{id}` are flat, since those commands don't need `projectId`. Documented per-endpoint in the `projects-api` spec, not assumed to be uniform.

## Migration Plan

1. Backend: `NotFoundException` + middleware mapping (needed by everything else). `Project` CQRS (Create/Update/Delete/GetList/GetById) with validators. `BaseDeDatos` CQRS (Create/Update/Delete). Controllers. `dotnet ef database update` not needed — no schema change, tables already exist from Foundation.
2. Frontend: `ProyectosLayout`, `ProjectService`/`DatabaseService`, `ProjectList`, `ProjectDetail`, `ProjectForm`, database modal. Replace the `/proyectos` route registration (was `ComingSoon`) with the new layout + children.
3. Cypress: full CRUD flow test.
No rollback complexity — additive only, no destructive migration.
