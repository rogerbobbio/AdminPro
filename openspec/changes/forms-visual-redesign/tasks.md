## 0. Setup (MANDATORY FIRST)

- [x] 0.1 Create branch `feature/forms-visual-redesign`
- [ ] 0.2 Push branch to `origin` (Draft PR opened manually if `gh` CLI is unavailable, per the note in previous phases' tasks.md)

## 1. Specs & Design

- [x] 1.1 `proposal.md` reviewed
- [x] 1.2 `design.md` with technical decisions (switch/tab dropped, sections trimmed, `Location.back()` for Cancelar, rail content trimmed)
- [x] 1.3 `specs/projects-screens/spec.md`, `specs/application-core-screens/spec.md` (deltas) with Given/When/Then scenarios

## 2. Frontend — ProjectForm (TDD)

- [x] 2.1 TDD: write a failing test asserting `ProjectForm` renders a breadcrumb (`Proyectos / Nuevo Proyecto` in create mode, `Proyectos / {nombre} / Editar` in edit mode); implement the breadcrumb plus the icon page-header (icon, title, subtitle). Deviation: `data-testid="breadcrumb-current"` sits on the trailing active segment ("Nuevo Proyecto" / "Editar"); the edit-mode test instead asserts on the whole `[data-testid="breadcrumb"]` container since the project name lives in a link segment, not the "current" one.
- [x] 2.2 TDD: write a failing test asserting clicking "Cancelar" calls `Location.back()` and makes no HTTP request; implement the button using Angular's `Location` service.
- [x] 2.3 Wrap the existing `Nombre`/`Descripcion` fields in an icon-headed "Información general" section card and add the sticky rail sidebar (single "Nombre" checklist item + footer hint), matching `assets/new-project-mockup.html`'s section/rail visual structure (colors/spacing via existing `--ap-*`/`--bs-*` tokens, not new ones). No new form controls, no behavior change to existing submit/validation tests. Kept Bootstrap's `form-control` class on inputs/textarea (consistent with the rest of the app) instead of hand-rolling input CSS from the mockup.
- [x] 2.4 Run existing `project-form.spec.ts` tests (duplicate-name inline error, successful-create navigation) to confirm no regressions from the layout changes; adjust selectors only if needed (no behavior change expected). Verified green: 5/5 (2 pre-existing + 3 new).

## 3. Frontend — ApplicationForm (TDD)

- [x] 3.1 TDD: write a failing test asserting `ApplicationForm` renders a breadcrumb back through its parent project (fetched via `ProjectService`/`ApplicationService` as needed for the project name in edit mode, or from the `proyectoId` query param context in create mode); implement the breadcrumb plus icon page-header. Added a `ProjectService.getById(projectId)` call to `ngOnInit` (both create and edit mode) purely to resolve the parent project's `nombre` for the breadcrumb — existing duplicate-name/success tests updated to flush that extra `GET /api/projects/1` request.
- [x] 3.2 TDD: write a failing test asserting clicking "Cancelar" calls `Location.back()` and makes no HTTP request; implement.
- [x] 3.3 Convert the existing "Detalles técnicos" collapsed-link into an icon-headed, collapsible "Detalles técnicos" section card (same collapse/expand behavior, new visual treatment); wrap the default-visible fields in an "Información general" section card; add the rail sidebar (single "Nombre" checklist item + footer hint).
- [x] 3.4 Run existing `application-form.spec.ts` tests (duplicate-name inline error, successful-create navigation) to confirm no regressions; adjust selectors only if needed. Verified green: 5/5 (2 pre-existing, adjusted for the new project fetch + 3 new).

## 4. Verification

- [x] 4.1 `ng build` succeeds with zero errors; Angular unit tests all green. Verified: build 0 errors, `ng test` 55/55 passed across 19 spec files.
- [x] 4.2 Manually verify both forms (create + edit mode for each) via the running app — screenshot each, confirm breadcrumb text, Cancelar behavior, and section/rail layout render correctly, matching the visual language of `ProjectDetail`/`ApplicationDetail`. Verified via Playwright against the real running app (`dotnet run` + `ng serve`): `ProjectForm` create + edit (using the real "DOLE" project, read-only — no data changed), `ApplicationForm` create + edit (using a temporary isolated "QA Screenshot Project (temp)" created and then deleted via the API afterward, so no real project's data was touched). All 4 screenshots confirmed correct breadcrumbs, the "Nombre" checklist item turning green when valid, the collapsible "Detalles técnicos" section, and zero console/page errors.
- [x] 4.3 Confirm every task above was committed individually per the `commit` skill format. Verified via `git log --oneline feature/forms-visual-redesign`.
- [x] 4.4 Mark this `tasks.md` complete and ready for `/opsx:archive`.
