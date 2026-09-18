## Context

The Dashboard (`frontend/src/app/features/dashboard`) is a single global view backed by `GET /api/dashboard/summary`. Three of its five widgets carry fabricated or dead signal: the weekly chart and status donut are driven by a `statusBreakdown` that hardcodes every active application as 100% "Activo" (no real status concept exists in the domain), and the reminder card is static markup with no backing entity. The módulos list duplicates sidebar navigation. This was flagged directly by the project owner after reviewing a Claude Design mockup canvas (5 artboards: global state, 3 layout directions for the project-scoped view, and the application drill-down state) and choosing the "stacked" layout direction.

`ProjectService.getById(id)` already returns a `ProjectDetail` with both `basesDeDatos` and `applications` in one call, and `ApplicationService.getById(id)` already returns the full `ApplicationDetail` (ambientes/reportes/notas/documentos/fixDatas). Both are used today by `project-detail` and `application-detail`. No new backend read model is required to support project- or application-scoped rendering.

## Goals / Non-Goals

**Goals:**
- Let the user pick a project (and, within it, an application) from the Dashboard and see that entity's real data without navigating away.
- Remove dashboard widgets that show fabricated or non-actionable data.
- Keep the recent-applications table meaningful by sorting/labeling it by actual last-modified time.
- Reuse existing endpoints and services; no new API surface.

**Non-Goals:**
- No new CRUD UI on the Dashboard itself (databases and applications are still created/edited via their existing full-page forms and the project-detail drawer). The Dashboard's scoped views are read-only, with links out to the pages that already own editing.
- No password display anywhere in the new Bases de Datos card — the Dashboard's table matches `project-detail`'s existing read-only table exactly (Nombre/Ambiente/Servidor only), it does not introduce a new masked-password affordance that doesn't exist elsewhere in the app.
- No real "application status" concept is introduced. Removing `statusBreakdown` is a deletion, not a replacement.
- `ModuloService` / `GET /api/modulos` are not touched — only their one UI consumer (the módulos list widget) is removed.

## Decisions

1. **Selection state lives in the `Dashboard` component as two signals** (`selectedProjectId`, `selectedApplicationId`), not in a shared service or the URL. Rationale: this is view-local, transient state (nothing else in the app needs to react to "which project is the dashboard currently showing"); route/query-param-driven state was considered but rejected as unnecessary complexity for a same-page filter with no deep-linking requirement in this change.
2. **The "Aplicación" dropdown's options come from `ProjectDetail.applications`** (already fetched by `ProjectService.getById`), not a separate `ApplicationService.loadByProject` call. Rationale: avoids a redundant HTTP round-trip — the data is already on hand.
3. **Selecting a specific application calls `ApplicationService.getById(appId)`** to get the full `ApplicationDetail` needed for the drill-down sections (ambientes/reportes/notas/documentos/fixDatas), since `ApplicationSummary` (what's embedded in `ProjectDetail`) doesn't carry those nested collections.
4. **Two new presentational components** — `ProjectOverview` (input: `ProjectDetail`) and `ApplicationOverview` (input: `ApplicationDetail`) — each internally `@if`-gating every section on that section's array length, matching `stat-cards`/`recent-applications-table`'s existing "dumb input-driven component" pattern rather than inlining all the conditional markup into `dashboard.html`.
5. **`DashboardSummaryDto` is trimmed, not versioned or deprecated-in-place.** Since nothing else consumes `totalAmbientes`/`totalServiciosVinculados`/`applicationsCreatedLast7Days`/`statusBreakdown` after the widget deletions (confirmed by grep), keeping them would be dead backend computation with no reader. This is a breaking response-shape change but the endpoint has exactly one consumer (this Dashboard) in this single-user, no-external-integrations app.
6. **`RecentApplicationDto.Status` becomes `RecentApplicationDto.UpdatedAt`**, and the query's `OrderByDescending` switches from `CreatedAt` to `UpdatedAt`. This surfaced a latent bug: `UpdateApplicationCommandHandler` never set `UpdatedAt`, so every application's `UpdatedAt` equaled its `CreatedAt` forever. Fixed as part of this change (one line, same handler already being touched for the DTO consumers) rather than filed separately, since shipping `UpdatedAt`-based sorting on top of a handler that never updates it would ship a feature that quietly doesn't work.
7. **Relative-time formatting ("hace 2 horas", "ayer") is computed client-side** in `RecentApplicationsTable` from the ISO `updatedAt` string, not returned pre-formatted by the API. Rationale: keeps the API a plain data contract; avoids locale/timezone formatting logic on the backend.
8. **Bases de Datos and Aplicaciones cards are each independently omitted when empty** (no "no hay bases de datos todavía" placeholder), per the approved mockup's "only sections with value" rule — a deliberate departure from `project-detail`'s own empty-state messages, which stay as-is (this change only touches the Dashboard, not `project-detail`).

## Risks / Trade-offs

- [Trimming `DashboardSummaryDto` is a breaking API change] → Mitigated: single in-repo consumer, both backend tests are updated in the same change, no external clients exist (no auth/multi-tenant, local single-user app per `docs/design/DESIGN.md`).
- [`UpdatedAt` bug fix changes existing data's future behavior] → Only affects applications updated *after* this ships; no backfill needed since "recently modified" is inherently forward-looking.
- [Two new components add a small amount of structure for what is currently simple conditional markup] → Accepted: matches the existing Dashboard convention of one component per card (`stat-cards`, `recent-applications-table`), keeps `dashboard.html` legible, and each is unit-testable in isolation like its siblings.

## Migration Plan

No data migration. Deploy order: backend (DTO/handler/tests) then frontend in the same change — the frontend is rewritten to match the new DTO shape in the same commit set, so there is no intermediate state where frontend expects fields the backend no longer sends. Rollback is a plain revert (no migrations to reverse).

## Open Questions

None outstanding — all product decisions were resolved with the user via the mockup review before this proposal was written.
