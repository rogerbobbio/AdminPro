## Context

`ApplicationDetail` (`frontend/src/app/features/proyectos/pages/application-detail/`) already renders `Reportes`, `Notas`, `Documentos`, and `FixDatas` as static empty-state cards (`No hay X todavía.`), sourced from `ApplicationDetailDto`'s always-empty arrays (`reportes: object[]`, etc. — see `GetApplicationByIdQueryHandler`). The `Ambiente` child entity already went through this exact lifecycle in Phase 4 and is the concrete precedent to mirror: `EnvironmentService` (Angular), `AmbientesController` nested routes (`POST /api/applications/{appId}/ambientes`, `PUT`/`DELETE /api/ambientes/{id}`), a reactive-form modal on `ApplicationDetail`, and `CreateEnvironmentCommand`/`UpdateEnvironmentCommand` + `CreateEnvironmentCommandHandler` in the Application layer.

`docs/design/DESIGN.md` §5.4 already specifies the target UI for all four sections, including the exact "Notas" interaction the user asked for: `▶ nvm use 14.16.0` / description line below when expanded, count badge (`NOTAS (2)`), and a `[+ Agregar Nota]` action — "Notas: Add (modal), Edit, Delete, Expand/Collapse". `docs/design/DESIGN.md` §2 also already names the target controllers (`ReportesController`, `NotasController`, `DocumentosController`, `FixDatasController`), so this design follows that structure rather than nesting everything under `ApplicationsController`/`AmbientesController` (which was itself a pragmatic Phase 4 shortcut — see the archived `application-core` change).

The `Reporte`, `Nota`, `Documento`, `FixData` domain entities and their `AppDbContext` `DbSet`s already exist (Phase 1 `InitialCreate` migration) — this change only adds the Application-layer commands/validators and the API/frontend surface for them.

## Goals / Non-Goals

**Goals:**
- Full CRUD (create, update, delete) for `Reporte`, `Nota`, `Documento`, `FixData`, each scoped to its parent `Application`.
- `GetApplicationByIdQuery` returns real data for all four collections instead of always-empty arrays.
- `ApplicationDetail` renders all four as functional sections; `Notas` specifically as a collapsible list per the user's UX requirement.
- Remove the now-redundant "Notas de arranque" placeholder from `ApplicationForm`.

**Non-Goals:**
- `Servicios`/`AplicacionServicio` (linking services to applications) — Phase 6, untouched here.
- Report *execution* (running `SpTranship`/`SpReportViewer`) — `Reporte` rows are metadata only, per §2.2.6; no execution engine is in scope.
- Document *storage* — `Documento.UrlOneDrive` is a link to an externally-hosted file (OneDrive); no file upload/hosting is added.
- Drag-and-drop reordering (`Orden`) for any of the four — that's explicitly Phase 7 ("Polish & Extensibility", §8) per the roadmap, same as it was deferred for `Ambiente` in Phase 4.
- Global search integration (`GetSearchQuery` surfacing Notas/Reportes) — Phase 6.

## Decisions

**One controller per child entity, nested under `/api/applications/{appId}/...` for create, `/api/{plural}/{id}` for update/delete** — matches the already-documented `ReportesController`/`NotasController`/`DocumentosController`/`FixDatasController` split in `docs/design/DESIGN.md` §2, and mirrors the exact routing shape `AmbientesController` established (`POST /api/applications/{appId}/ambientes`, `PUT`/`DELETE /api/ambientes/{id}`). Alternative considered: nest everything under `ApplicationsController` as more actions (like `CreateEnvironment` was nested there in Phase 4) — rejected because `ApplicationsController` would grow to 12+ actions across 4 unrelated child resources, and the target file tree already names the four controllers explicitly.

**`Notas` is the only child with a bespoke frontend component (expand/collapse list item)** — `Reportes`, `Documentos`, `FixDatas` reuse the same table-row + modal-form pattern already built for `Ambientes` on `ApplicationDetail` (empty-state message, `+ Agregar` pill, edit/delete icon buttons per row). Only `Notas` needs a different list-item template (chevron toggle, title always visible, description conditionally rendered) because that's the specific behavior requested. No shared "accordion list" abstraction is introduced for a single consumer — if a second collapsible-list section appears in a later phase, extract one then.

**Query stays a single `GetApplicationByIdQuery`** — no separate `GetNotasByApplicationQuery` etc. `ApplicationDetailDto` already declares `IReadOnlyList<object>` placeholders for all four; this change replaces those with real typed DTOs (`ReporteDto`, `NotaDto`, `DocumentoDto`, `FixDataDto`) populated in `GetApplicationByIdQueryHandler`, same pattern as `AmbienteDto` today. Alternative (separate queries per child, lazy-loaded) rejected: `ApplicationDetail` already fetches everything in one `getById` call and the data volumes are small (a handful of rows per application), so splitting adds round-trips for no benefit.

**`Notas de arranque` placeholder removal from `ApplicationForm` is a pure deletion, not a redirect** — the section already renders disabled/inert inputs with no `formControlName` binding (see `application-form.html`), so nothing currently submits through it; removing it has zero data-migration concern. Any content a user might have "wanted" to put there belongs in the real `Notas` list on `ApplicationDetail` once the application exists (same reasoning already applied to `Ambientes`: create the app first, then manage its children on the detail page).

**`Documento.Tipo` and `FixData.Script` use plain `<input>`/`<textarea>`, not a closed dropdown** — DOC-002 says `Tipo` "SHALL be one of: manual, diagrama, codigo, otro. The frontend MAY allow free-text entry but SHOULD suggest these values" — implemented as a `<select>` with those 4 options (no "other" free-text escape hatch needed since the rule doesn't require one beyond the catalog).

## Risks / Trade-offs

- **[Four near-identical CRUD stacks inflate the diff size]** → Mitigate by keeping each command/handler/validator minimal and copy-pasting the already-proven `Ambiente` shape rather than inventing new patterns per entity; review scans them as one pattern applied four times, not four novel designs.
- **[`NotaDto`'s `Descripcion` is `nvarchar(max)` — large notes could bloat the `ApplicationDetail` response]** → Acceptable for a single-user, local-only app with realistic note volumes (a handful of short how-to notes per application, per the reference mockup); revisit only if real usage proves otherwise.
- **[Removing "Notas de arranque" from `ApplicationForm` changes an already-shipped screen]** → Low risk: the section is currently disabled/non-functional (per `forms-visual-redesign`), so no working behavior is lost, only a "Próximamente" placeholder.

## Migration Plan

No database migration needed (tables already exist from `InitialCreate`). Deploy as a normal backend + frontend release; no data backfill, no rollback complexity beyond reverting the commit (soft-delete/cascade behavior for these children is unaffected since `Application` deletion already cascades to them structurally via `OnDelete(Cascade)`).

## Open Questions

None — the domain model (`docs/business-rules.md` §2.2.6-2.2.9, §3.6-3.9, §4.3 APP-CMD-009 through 012), the target UI (`docs/design/DESIGN.md` §5.4), and the precedent implementation (`Ambiente`/`AmbientesController`/`EnvironmentService`) are all already specified.
