## Context

The user supplied `assets/new-project-mockup.html`, a static HTML mockup for a redesigned "Nuevo Proyecto" screen. Its sidebar, topbar (search/notifications/user), and color tokens (`--g-900`/`--g-700`/`--g-500`/`--g-100`/`--ink-soft`) are **already** the real, implemented `AppShell` and `styles.scss` tokens (`--ap-g-900` etc.) — confirmed by comparing the mockup to `app-shell.html` and the Playwright screenshot taken while verifying `application-core`. So the mockup isn't proposing a new visual system; it's proposing a richer *page-content* layout (two-column split, icon-headed section cards, a sticky "rail" helper sidebar) for a screen (`ProjectForm`) that today is just a bare form inside `AppShell`'s existing content area.

The mockup also includes several fields/sections with no backing data model (`Código corto`, `Cliente`, `Responsable`, project-level `Rutas base`, an "Equipo" member-list section) and a "DOLE Catalog"-branded, client-specific header. Per explicit user decision (see conversation), none of that ships here — this change is presentation-only, scoped to the fields `ProjectForm`/`ApplicationForm` already collect.

## Goals / Non-Goals

**Goals:**
- Give `ProjectForm` and `ApplicationForm` the same visual quality as `ProjectDetail`/`ApplicationDetail`: breadcrumb, icon page-header with subtitle, icon-headed section card(s) around the fields, a "Cancelar" action, and a sticky rail sidebar with contextual help.
- Keep both forms' actual fields, validation, and submit/error behavior unchanged — this is styling and layout, not new functionality (beyond the new Cancelar button).

**Non-Goals:**
- No new `Project`/`Application` fields (`Código corto`, `Cliente`, `Responsable`, `Rutas base` at the project level).
- No "Equipo" (team members) section — AdminPro is explicitly single-user/no-auth (`docs/business-rules.md`), so a member list has no real entity to back it.
- No "Primera aplicación" inline-create section on `ProjectForm` — creating an application still requires navigating to `ApplicationForm` (which already needs a real `proyectoId`, so it can't happen before the project itself is saved).
- No "DOLE Catalog" branding — stays "AdminPro" per existing `AppShell`.
- No literal "switch" tab bar between "Nuevo Proyecto" and "Nueva Aplicación" — see Decision 1.

## Decisions

**1. Drop the mockup's "switch" tab bar (Nuevo Proyecto | Nueva Aplicación) instead of reproducing it literally.**
The mockup shows both create screens as interchangeable peers reachable from one tab control. But `Application.ProyectoId` is required (rule APP-VAL-001) — an `Application` cannot be created before its parent `Project` exists, so "switching" from `ProjectForm`'s create mode to `ApplicationForm`'s create mode has no valid target project and would be a dead link. Reproducing the control anyway (disabled, or navigating somewhere arbitrary) would be worse than omitting it. Instead:
- `ProjectForm`: breadcrumb only (`Proyectos / Nuevo Proyecto` or `Proyectos / {nombre} / Editar`), no switch.
- `ApplicationForm`: breadcrumb back through its real parent project (`Proyectos / {proyecto.nombre} / Nueva Aplicación`), which _is_ meaningful since the project already exists by the time you're creating an application under it. No switch control either, since there's still no reciprocal "switch to a new project" action that makes sense mid-flow.

**2. One section card ("Información general") on `ProjectForm`, two on `ApplicationForm` ("Información general" + "Detalles técnicos"), not the mockup's four (Info/Rutas/Equipo/Primera app).**
Directly follows the Non-Goals — only wrap what already exists. `ApplicationForm`'s existing collapsed-link "Detalles técnicos" sub-section (from `application-core`) becomes its own icon-headed section card instead of a plain toggle link, matching the new visual language, but keeps the same collapse/expand behavior (still starts collapsed) rather than always-open, since those 7 fields are secondary for most applications.

**3. The "rail" sidebar keeps its position/shadow/sticky treatment but its content is trimmed to only what's real.**
Mockup's checklist items reference "Nombre y código", "Rutas base", "Equipo" — none of which fully apply. Replaced with a single relevant checklist item ("Nombre" — the one required field) plus the existing footer-style hint text, adapted per form:
- `ProjectForm` rail footer: "Al crear el proyecto se registra en el catálogo y podrás agregar aplicaciones y bases de datos después."
- `ApplicationForm` rail footer: "Al crear la aplicación se registra bajo este proyecto y podrás agregar ambientes después."

**4. "Cancelar" button navigates back via `Location.back()`, not a hardcoded route.**
Both forms are reachable from more than one place conceptually (a project's detail page, or — for `ApplicationForm` — a project's Aplicaciones list), and `ProjectForm` in edit mode is reached from `ProjectDetail`. `Router`-based `Location.back()` (Angular's `Location` service) returns the user to wherever they came from without hardcoding an assumed parent route, avoiding a wrong guess for either form or mode (create vs. edit).

**5. Page-header icon per entity: `bi-folder-plus` for `ProjectForm`, `bi-hdd-stack` for `ApplicationForm` (edit mode reuses the same icon, not a pencil, since the header icon identifies the entity type, not the action).**
Mirrors the mockup's `page-icon` treatment. `bi-hdd-stack` is already used for `ApplicationDetail`'s empty-state icon set (implicitly consistent iconography) — reusing rather than inventing a new glyph.

## Risks / Trade-offs

- **[Risk]** Dropping the switch/tab and three sections is a significant departure from the literal mockup the user provided. → **Mitigation**: this was an explicit, discussed scope decision (see Non-Goals), not a silent deviation — flagged here and in proposal.md so the "why" survives independently of this conversation.
- **[Risk]** `Location.back()` could land the user somewhere confusing if they deep-linked directly into the form (no prior in-app history). → **Mitigation**: acceptable for an internal single-user tool with no bookmarked deep-links in practice; `ProjectForm`/`ApplicationForm` are never the app's entry point (always reached via a link from a list/detail page in normal use).

## Migration Plan

1. Restyle `ProjectForm` (`.html`/`.scss`/`.ts` — add breadcrumb + `Location.back()` for Cancelar).
2. Restyle `ApplicationForm` the same way, converting the "Detalles técnicos" toggle-link into a section-card header (same collapse behavior).
3. Update both components' specs to cover the new breadcrumb text and the Cancelar behavior.
4. `ng build` + `ng test` green; manual/Playwright pass confirming the new layout renders correctly for both create and edit modes.

No backend changes, no rollback complexity — additive, visual-only.
