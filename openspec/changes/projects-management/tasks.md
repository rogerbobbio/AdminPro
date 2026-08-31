## 0. Setup (MANDATORY FIRST)

- [x] 0.1 Create branch `feature/projects-management`
- [ ] 0.2 Open Draft PR against `main` (`gh` CLI unavailable in this environment — branch pushed to `origin/feature/projects-management`; open manually at https://github.com/rogerbobbio/AdminPro/pull/new/feature/projects-management)

## 1. Specs & Design

- [x] 1.1 `proposal.md` reviewed
- [x] 1.2 `design.md` with technical decisions
- [x] 1.3 `specs/projects-api/spec.md`, `specs/projects-screens/spec.md` with Given/When/Then scenarios

## 2. Backend Application — Exception mappings (TDD)

- [x] 2.1 Add `NotFoundException` under `AdminPro.Application/Common/Exceptions/`.
- [x] 2.2 TDD: write a failing `WebApplicationFactory` test (extend `TestOnlyController`/`TestingWebApplicationFactory`) proving a thrown `NotFoundException` maps to `404` with `{ error: "NotFoundError" }`; implement the mapping in `ExceptionHandlerMiddleware`. Combined with 2.3 (same test file, same middleware edit). Verified red (500 instead of 404/409) then green (4/4 passed).
- [x] 2.3 TDD: write a failing test proving a thrown `DomainException` (already declared in `AdminPro.Domain.Exceptions`) maps to `409` with `{ error: "DomainError" }`; implement the mapping. See 2.2.

## 3. Backend Application — Project CQRS (TDD)

- [x] 3.1 TDD: write a failing test for `CreateProjectCommandValidator` proving a duplicate `Nombre` fails validation (rule PROJECT-001); implement the validator + `CreateProjectCommand`/Handler (insert, set `CreatedAt`/`UpdatedAt`, return new id). Added `FluentValidation` package reference to `AdminPro.Application.Tests` for `TestValidateAsync`. Verified red (CS0234) then green (3/3 passed).
- [x] 3.2 TDD: write a failing test for `GetProjectsQuery` proving it returns only active projects ordered by `Nombre`, with an `IncludeInactive` flag; implement query + handler + `ProjectSummaryDto`. Verified red then green (2/2 passed).
- [x] 3.3 TDD: write a failing test for `GetProjectByIdQuery` proving it returns a `ProjectDetailDto` with `basesDeDatos` populated and throws `NotFoundException` for a missing/inactive id; implement. `IncludeInactiveChildren` bypasses the global query filter on `BasesDeDatos`/`Applications` only (not on the `Project` row itself) via a per-query `IgnoreQueryFilters()` rather than on the whole `Include()` graph, since ignoring filters on the root query would also un-filter the project. Verified red then green (2/2 passed).
- [ ] 3.4 TDD: write a failing test for `UpdateProjectCommandValidator`/Handler proving renaming to another project's name fails but renaming to its own current name succeeds, and a missing id throws `NotFoundException`; implement.
- [ ] 3.5 TDD: write a failing test for `DeleteProjectCommand` proving it sets `Activo = false` on the project AND cascades to its `BaseDeDatos` (and, via a directly-seeded fake row, `Application`) children, and throws `NotFoundException` for a missing id; implement.

## 4. Backend Application — BaseDeDatos CQRS (TDD)

- [ ] 4.1 TDD: write a failing test for `CreateBaseDeDatosCommand` proving it inserts a row scoped to `ProyectoId` and throws `NotFoundException` if the project doesn't exist; implement command + handler + validator (required `Nombre`, max lengths).
- [ ] 4.2 TDD: write a failing test for `UpdateBaseDeDatosCommand` proving it updates fields and throws `NotFoundException` for a missing id; implement.
- [ ] 4.3 TDD: write a failing test for `DeleteBaseDeDatosCommand` proving it sets `Activo = false` and throws `NotFoundException` for a missing id; implement.

## 5. Backend API (TDD)

- [ ] 5.1 TDD: write failing `WebApplicationFactory` tests for `GET /api/projects` (list, `includeInactive`) and `GET /api/projects/{id}` (detail + 404); implement `ProjectsController`'s GET actions.
- [ ] 5.2 TDD: write failing tests for `POST /api/projects` (201 + duplicate-name 400) and `PUT /api/projects/{id}` (204 + 404 + duplicate-name 400); implement.
- [ ] 5.3 TDD: write a failing test for `DELETE /api/projects/{id}` (204 + cascades reflected in a subsequent detail call + 404 for missing id); implement.
- [ ] 5.4 TDD: write failing tests for `POST /api/projects/{projectId}/basesdedatos` (201 + 404 for missing project), `PUT /api/basesdedatos/{id}` (204 + 404), `DELETE /api/basesdedatos/{id}` (204 + 404); implement `BaseDeDatosController`.
- [ ] 5.5 Backend integration test (Testcontainers): full create → get → update → delete flow for both `Project` and `BaseDeDatos` against a real SQL Server, confirming the cascade-on-delete behavior with real FK constraints.

## 6. Frontend — Proyectos module shell

- [ ] 6.1 TDD: write a failing test asserting `ProyectosLayout` renders `AppShell` with `activeNav="proyectos"` and a `<router-outlet>` for its children; implement.
- [ ] 6.2 Replace the `/proyectos` route registration in `app.routes.ts`: `ProyectosLayout` as a layout route with children `''` (`ProjectList`), `nuevo` (`ProjectForm`), `:id` (`ProjectDetail`), `:id/editar` (`ProjectForm`). Remove the now-unused `ComingSoon` registration for this path (keep the component itself — still usable for future not-yet-built modules like `servicios`).

## 7. Frontend — Services (TDD)

- [ ] 7.1 TDD: write failing tests for `ProjectService` (`loadProjects`, `getById`, `create`, `update`, `delete`, all Signal-based per `docs/design/DESIGN.md` §3.2's pattern); implement, with `Project`/`ProjectDetail` TS models.
- [ ] 7.2 TDD: write failing tests for `DatabaseService` (`create`, `update`, `delete` — no standalone `load`, since the list comes embedded in `ProjectService.selectedProject()`); implement.

## 8. Frontend — Pages (TDD)

- [ ] 8.1 TDD: write a failing test asserting `ProjectList` renders project cards from `ProjectService.projects()` and that typing in the search box filters them client-side; implement.
- [ ] 8.2 TDD: write a failing test asserting `ProjectForm` shows an inline error when the backend returns a `400` validation error on `nombre`, and navigates to the detail page on success; implement the reactive form.
- [ ] 8.3 TDD: write a failing test asserting `ProjectDetail` renders the databases list from `selectedProject().basesDeDatos` and an empty-state "Aplicaciones" section with no create action; implement.
- [ ] 8.4 TDD: write a failing test asserting the "add database" modal calls `DatabaseService.create()` and the databases list updates without a full page reload; implement the modal form (Bootstrap modal) and delete/edit actions.

## 9. Frontend E2E

- [ ] 9.1 Cypress test: full project CRUD flow — create a project, see it in the list, open its detail, add a database, edit the project, delete it, confirm it disappears from the list.

## 10. Documentation + Final

- [ ] 10.1 Update `backend/README.md` if the API surface description needs it (new controllers).
- [ ] 10.2 `dotnet build AdminPro.slnx` succeeds with zero errors; `dotnet test` all green.
- [ ] 10.3 `ng build` succeeds with zero errors; Angular unit tests all green.
- [ ] 10.4 `npx cypress run` — green (environment permitting, per the note in `frontend-dashboard`'s tasks.md).
- [ ] 10.5 Confirm every task above was committed individually per the `commit` skill format.
- [ ] 10.6 Mark this `tasks.md` complete and ready for `/opsx:archive`.
