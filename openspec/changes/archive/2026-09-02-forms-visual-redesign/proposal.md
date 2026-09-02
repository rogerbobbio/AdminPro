## Why

`ProjectForm` and `ApplicationForm` are currently bare, single-column forms with no breadcrumb, no cancel action, and no visual grouping — a noticeable step down from `ProjectDetail`/`ApplicationDetail`'s polished card-based layout. The user supplied a higher-fidelity mockup (`assets/new-project-mockup.html`) for a "Nuevo Proyecto" screen with a two-column layout (icon-headed section cards + a sticky helper sidebar), a page header with icon/subtitle/action bar, and a breadcrumb. This change adopts that visual language for both create/edit forms without changing what data they collect.

## What Changes

- Restyle `ProjectForm` (`/proyectos/nuevo`, `/proyectos/:id/editar`) to match the mockup's layout: breadcrumb, icon page-header with a subtitle and an action bar (Cancelar + Guardar), and the existing `Nombre`/`Descripcion` fields inside a single icon-headed "Información general" section card, with a sticky "rail" sidebar showing a short "Antes de crear" checklist and a footer hint.
- Restyle `ApplicationForm` (`/proyectos/aplicaciones/nuevo`, `/proyectos/aplicaciones/:id/editar`) with the same layout: breadcrumb (back to the parent project), icon page-header, the existing fields (`Nombre`, `Descripcion`, `TecnologiaFront`, `TecnologiaBack`) in an "Información general" section card, the existing "Detalles técnicos" fields in their own section card (replacing the current collapsed-link pattern with a proper icon-headed section, still collapsible), and a matching rail sidebar.
- Add a "Cancelar" action to both forms that navigates back without saving (new behavior — neither form has a cancel button today).
- **Explicitly out of scope** (per user decision): no new fields (`Código corto`, `Cliente`, `Responsable`, project-level `Rutas base`), no "Equipo" (team members) section — AdminPro has no user/auth model — and no "Primera aplicación" inline-create section on `ProjectForm`. The mockup's "DOLE Catalog" branding and its tab "switch" between "Nuevo Proyecto"/"Nueva Aplicación" are also not adopted as literally shown — see design.md for why.
- No backend changes — this is a frontend-only visual change; existing DTOs, commands, and validation are untouched.

## Capabilities

### Modified Capabilities
- `projects-screens`: `ProjectForm`'s requirement gains the breadcrumb/section-card layout and a "Cancelar" action.
- `application-core-screens`: `ApplicationForm`'s requirement gains the same breadcrumb/section-card layout and a "Cancelar" action.

## Impact

- Frontend only: `frontend/src/app/features/proyectos/pages/project-form/` and `.../application-form/` (`.html`/`.scss`/`.ts`), reusing the same CSS custom properties (`--ap-g-900`, `--ap-ink-soft`, `--bs-border-radius-xl`, etc.) already established in `styles.scss` and used by `ProjectDetail`/`ApplicationDetail`.
- No API, DTO, command, or database changes.
