## Context

`frontend/` is currently empty. `backend/` only has the Phase 1 DDD scaffold (`solution-foundation`): entities, `AppDbContext` with soft-delete filters, the MediatR pipeline (Validation → Logging → Transaction), and global exception middleware — no controllers, no CQRS handlers yet.

The user supplied a finished visual design as two static HTML mockups built with Claude Design: `dashboard.html` (this change's target) and `application-detail.html` (a future change). Comparing both confirms the **sidebar and topbar markup/CSS are byte-for-byte identical** across screens — same nav items, same promo card, same search/notification/user cluster. This is the app shell every future screen will reuse.

Per the user's decision: the Dashboard must read from the real API (even while tables are empty), not from mocked data — so today it will show zeros/empty states until the user starts entering projects through later screens.

## Goals / Non-Goals

**Goals:**
- Stand up the Angular 22 app shell (bootstrap, routing, Bootstrap 5 theme, HTTP proxy) per `docs/design/DESIGN.md` §3.1.
- Extract the mockup's sidebar+topbar into a reusable `AppShellComponent` (or `ShellLayoutComponent`) now, so `application-detail` and every later screen slot into it without re-deriving the CSS.
- Reproduce `dashboard.html` visually 1:1 (colors, radii, shadows, spacing, iconography) as Angular components using the CSS custom properties from the mockup, translated into a global Bootstrap 5 theme override (`styles.scss`) rather than re-inventing a design system.
- Back the Dashboard with two new minimal read endpoints so every number/row on screen is real, not hardcoded.

**Non-Goals:**
- No CRUD for Projects/Applications/Ambientes/Servicios (backend or frontend) — that's future, screen-by-screen changes.
- No `proyectos` module shell, no global search, no auth/user menu behavior (the user avatar/email in the topbar is static display only, matching "no authentication" per `docs/design/DESIGN.md` §1.2).
- No real "reminders" feature — the reminder card is a static, hardcoded UI element in this change (no `Recordatorio` entity exists in the domain model); wiring it to real data is future scope once a suitable entity/rule is defined.
- No live weekly-chart granularity beyond what `CreatedAt` timestamps naturally give us (see Decisions).

## Decisions

**1. Extract `AppShellComponent` now, even though only one screen uses it.**
Alternative considered: inline the sidebar/topbar markup into `DashboardComponent` and refactor later. Rejected — `application-detail.html` proves the shell is identical across screens, so duplicating it once already creates the exact copy-paste debt `base-standards.md` principle #7 (pattern detection) warns against. `AppShellComponent` takes the active nav-item id and page title/actions as inputs; `DashboardComponent` and future screens project their content into it via `<router-outlet>` or content projection.

**2. Two new query endpoints instead of one generic "dashboard" query bag.**
- `GET /api/modulos` — reusable by the sidebar badge counts and future module-related screens, not just the dashboard. Follows `docs/business-rules.md` rule APP-QRY-001 (active modules, ordered by `Orden`).
- `GET /api/dashboard/summary` — a single purpose-built read model (`DashboardSummaryDto`) aggregating counts/series/lists in one round trip, since the dashboard is a single-page read and multiple round trips would just add latency for no benefit (no independent loading states needed per widget in the mockup — everything appears at once).
Both are plain MediatR queries per the existing pipeline (no transaction, since `TransactionBehavior` already skips non-`ICommand` requests).

**3. `DashboardSummaryDto` shape** (computed directly in the query handler via `AppDbContext`, no repository, per `solution-foundation`'s no-repository decision):
```csharp
public record DashboardSummaryDto(
    int TotalProjects,
    int TotalApplications,
    int TotalAmbientes,
    int TotalServiciosVinculados,
    IReadOnlyList<int> ApplicationsCreatedLast7Days, // 7 ints, oldest→newest, count of Applications by CreatedAt.Date
    IReadOnlyList<RecentApplicationDto> RecentApplications, // top N by CreatedAt desc
    ApplicationStatusBreakdownDto StatusBreakdown // counts by a computed status (see Decision 4)
);
```
This keeps the contract stable even as later changes add more real data — the shape doesn't change, only the underlying counts grow from zero.

**4. "Estado" (Activo/En progreso/Pendiente) is a computed, not stored, status.**
The mockup's status pill and donut imply a tri-state application status, but `docs/business-rules.md`'s `Application` entity only has `Activo` (bool). Rather than inventing a new persisted enum/column now (out of scope, no business rule defines it), the summary query derives status heuristically: `Activo == false` → treated as excluded (soft-deleted, never shown, consistent with the global query filter); among active rows, "activo" vs "en progreso" vs "pendiente" is left as a **TODO computed 100% "Activo"** for this change — i.e., until a real status rule exists, every active `Application` counts as "Activo" in the breakdown and the donut always reads 100% active / 0% otherwise. This avoids fabricating business logic not in the spec. Documented as an Open Question below for the user to confirm before `apply`.

**5. Bootstrap 5 theme, not a competing CSS system.**
Map the mockup's CSS custom properties (`--g-900`, `--g-100`, `--radius-xl`, etc.) onto Bootstrap 5 Sass variables/CSS vars in `styles.scss` (`$primary: #0C3B29`, custom `--bs-border-radius-xl`, etc.) so Bootstrap's grid/utilities still work, per `docs/base-standards.md` §5 ("Bootstrap 5 classes, no hardcoded CSS") — component-level styles use Bootstrap utility classes first, custom SCSS only for the pieces Bootstrap has no primitive for (the donut chart, the bar chart, the pill nav).

**6. Weekly bar chart and donut are hand-rolled CSS/SVG, no charting library.**
The mockup's bar chart is pure CSS (`height:%` divs) and the donut is a CSS `conic-gradient`. Reproducing them with plain Angular template bindings (`[style.height.%]`, computed `conic-gradient` string) avoids a new dependency (Chart.js, ngx-charts) for two simple static-shape widgets — consistent with `docs/design/DESIGN.md` §8's general bias toward fewer dependencies.

**7. Seed `Modulos` via a new migration, not by editing `InitialCreate`.**
`solution-foundation`'s spec explicitly states `InitialCreate` inserts zero rows (`MOD-004` intentionally deferred). Adding a second migration (`SeedModulos`) that only adds `HasData` rows keeps `foundation-backend`'s already-archived spec accurate and this change's diff isolated to what it actually adds.

## Risks / Trade-offs

- **[Risk]** Computed "100% Activo" status donut is visually misleading once the user has real mixed-status data (there's no way yet to represent "en progreso"/"pendiente"). → **Mitigation**: called out explicitly as an Open Question; revisit once a status/lifecycle rule for `Application` is defined in a later change — the DTO shape already reserves room for it.
- **[Risk]** `ApplicationsCreatedLast7Days` will show a flat empty/zero chart until real usage accumulates a week of history, unlike the mockup's illustrative curve. → **Mitigation**: acceptable per the user's explicit choice (real data, populated later); add a lightweight empty-state (flat bars, no error) rather than hiding the widget.
- **[Risk]** Extracting `AppShellComponent` before a second real screen exists risks over-fitting its API to guesses about `application-detail`'s needs. → **Mitigation**: keep its public inputs minimal (active route id, optional page header slot) and let the next change adjust it — it's a small, cheap-to-change internal component, not a published contract.

## Migration Plan

1. Backend: add `GetModulosQuery`/`GetDashboardSummaryQuery` handlers + DTOs, `ModulosController`/`DashboardController`, `SeedModulos` migration. Run `dotnet ef database update`.
2. Frontend: `ng new` scaffold, Bootstrap 5 + Bootstrap Icons install, proxy config, `AppShellComponent`, `DashboardService` (Signals), `DashboardComponent`, root route `/` → Dashboard.
3. Cypress smoke test for dashboard load + module click.
No rollback complexity — additive only, no destructive migration, no existing screens to break.

## Open Questions (resolved)

- Status donut: confirmed — ship the "100% Activo" placeholder (Decision 4), connected to real data, until a real `Application` status rule exists in a later change.
- Module card click target: confirmed — clicking "Gestión de Proyectos" navigates to a placeholder route (e.g. `/proyectos`) rendering a minimal "Próximamente" page, created in this change and replaced once the real `proyectos` module ships.
