## 0. Setup (MANDATORY FIRST)

- [x] 0.1 Create branch `feature/application-core`
- [x] 0.2 Push branch to `origin` (Draft PR opened manually if `gh` CLI is unavailable, per the note in `projects-management`'s tasks.md)

## 1. Specs & Design

- [x] 1.1 `proposal.md` reviewed
- [x] 1.2 `design.md` with technical decisions
- [x] 1.3 `specs/application-core-api/spec.md`, `specs/application-core-screens/spec.md`, `specs/projects-screens/spec.md` (delta) with Given/When/Then scenarios

## 2. Backend Application — Application CQRS (TDD)

- [x] 2.1 TDD: write a failing test for `CreateApplicationCommandValidator` proving a duplicate `Nombre` within the same `ProyectoId` fails validation (rule APP-001) but the same name under a different project succeeds; implement the validator + `CreateApplicationCommand`/Handler (insert, throw `NotFoundException` if the project doesn't exist or is inactive, set `CreatedAt`/`UpdatedAt`, return new id). Written together with 2.2-2.5 (one pass across all Application CQRS, verified together). Hit and fixed an EF Core InMemory-provider gotcha: `UseInMemoryDatabase(name)` shares the same store across DbContext instances whenever the name string matches, even across unrelated test classes/collections — several new test methods coincidentally reused method names already used in `Projects`/`Databases` tests (e.g. `Handler_MissingProject_ThrowsNotFoundException`, `Validator_AllowsRenamingToOwnCurrentName`), causing cross-test data bleed and one flaky failure in `UpdateProjectTests`. Fixed by prefixing every new test class's in-memory db name with `nameof(<TestClass>)`.
- [x] 2.2 TDD: write a failing test for `GetApplicationsByProjectQuery` proving it returns only active applications for the given project ordered by `Orden` then `Nombre`, with an `IncludeInactive` flag; implement query + handler + `ApplicationSummaryDto`. See 2.1.
- [x] 2.3 TDD: write a failing test for `GetApplicationByIdQuery` proving it returns an `ApplicationDetailDto` with `ambientes` populated and always-empty `reportes`/`notas`/`documentos`/`fixDatas`/`servicios`, and throws `NotFoundException` for a missing/inactive id; implement. See 2.1.
- [x] 2.4 TDD: write a failing test for `UpdateApplicationCommandValidator`/Handler proving renaming to another application's name in the same project fails but renaming to its own current name succeeds, and a missing id throws `NotFoundException`; implement. See 2.1.
- [x] 2.5 TDD: write a failing test for `DeleteApplicationCommand` proving it sets `Activo = false` on the application AND cascades to its `Ambiente` children, and throws `NotFoundException` for a missing id; implement, including the marked extension-point comments for the not-yet-existing Reportes/Notas/Documentos/FixDatas cascades (rule APP-002, design.md Decision 5). See 2.1.

## 3. Backend Application — Ambiente CQRS (TDD)

- [x] 3.1 TDD: write a failing test for `CreateEnvironmentCommand` proving it inserts a row scoped to `AplicacionId`, throws `NotFoundException` if the application doesn't exist, and rejects an invalid `Url` (rule ENV-002); implement command + handler + validator. Written together with 3.2/3.3, verified together (see 2.1's note on the shared-db-name fix, applied here too).
- [x] 3.2 TDD: write a failing test for `UpdateEnvironmentCommand` proving it updates fields and throws `NotFoundException` for a missing id; implement. See 3.1.
- [x] 3.3 TDD: write a failing test for `DeleteEnvironmentCommand` proving it sets `Activo = false` and throws `NotFoundException` for a missing id; implement. See 3.1.

## 4. Backend Application — Project detail integration

- [x] 4.1 TDD: update `GetProjectByIdQueryHandlerTests` to prove `ProjectDetailDto.Applications` now returns real created applications (not always-empty); update `GetProjectByIdQueryHandler` accordingly (no DTO shape change needed per design.md Decision 6). Deviation: `GetProjectByIdQueryHandler` already queried `dbContext.Applications` for the summary list since `projects-management` (it was just always empty because nothing could create an `Application` yet) — no handler code change was needed, only the new test proving it now returns real rows. Verified green: 79/79 in `AdminPro.Application.Tests` (was 56/56 before this change).

## 5. Backend API (TDD)

- [x] 5.1 TDD: write failing `WebApplicationFactory` tests for `GET /api/projects/{projectId}/applications` (list, `includeInactive`) and `GET /api/applications/{id}` (detail + 404); implement `ApplicationsController`'s GET actions. Written together with 5.2-5.4 (one pass, `ApplicationsControllerTests`/`AmbientesControllerTests`, verified together — same rationale `projects-management` used for its 5.1-5.3). Hit and fixed a real ambiguous-reference compile error: `ApplicationSummaryDto` exists in both the new `Applications.Queries.GetApplicationsByProject` namespace and the pre-existing `Projects.Queries.GetProjectById` namespace; fully qualified the type in `ProjectsController.GetApplications`'s return type instead of adding another `using`.
- [x] 5.2 TDD: write failing tests for `POST /api/projects/{projectId}/applications` (201 + duplicate-name-in-project 400 + 404 for missing project) and `PUT /api/applications/{id}` (204 + 404 + duplicate-name 400); implement. Nested `POST` added to `ProjectsController` (mirrors `CreateDatabaseRequest`'s pattern), flat `GET`/`PUT`/`DELETE` on the new `ApplicationsController`. See 5.1.
- [x] 5.3 TDD: write a failing test for `DELETE /api/applications/{id}` (204 + cascade to ambientes reflected in a subsequent detail call + 404 for missing id); implement. See 5.1.
- [x] 5.4 TDD: write failing tests for `POST /api/applications/{appId}/ambientes` (201 + 404 for missing application + 400 for invalid url), `PUT /api/ambientes/{id}` (204 + 404), `DELETE /api/ambientes/{id}` (204 + 404); implement `AmbientesController`. See 5.1. Verified red (routes didn't exist) then green (45/45 in `AdminPro.Api.Tests`, up from 28/28).
- [x] 5.5 Backend integration test (Testcontainers): full create → get → update → delete flow for both `Application` and `Ambiente` against a real SQL Server, confirming the cascade-on-delete behavior with real FK constraints. Added `ApplicationsIntegrationTests.cs` mirroring `ProjectsIntegrationTests.cs`. Verified green against a real containerized SQL Server (Docker Desktop running). Full backend suite: 79/79 (`AdminPro.Application.Tests`) + 46/46 (`AdminPro.Api.Tests`) = 125/125.

## 6. Frontend — Services (TDD)

- [ ] 6.1 TDD: write failing tests for `ApplicationService` (`loadByProject`, `getById`, `create`, `update`, `delete`, Signal-based, mirroring `ProjectService`'s pattern); implement, with `Application`/`ApplicationDetail`/`ApplicationSummary` TS models added to `project.model.ts` (or a new `application.model.ts` if that keeps the file focused).
- [ ] 6.2 TDD: write failing tests for `EnvironmentService` (`create`, `update`, `delete` — no standalone `load`, mirroring `DatabaseService`'s pattern since the list comes embedded in `ApplicationService.selectedApplication()`); implement.

## 7. Frontend — Pages (TDD)

- [ ] 7.1 TDD: write a failing test asserting `ApplicationForm` shows an inline error when the backend returns a `400` validation error on `nombre`, and navigates to the detail page on success; implement the reactive form (handles both create via `?proyectoId=` query param and edit via `:id` route param), with the "Detalles técnicos" fields grouped under a collapsed sub-section per design.md Decision 7.
- [ ] 7.2 TDD: write a failing test asserting `ApplicationDetail` renders the application's fields, the `Ambientes` list from `selectedApplication().ambientes`, and empty-state sections for Reportes/Notas/Documentos/FixDatas/Servicios with no create action; implement the accordion layout per PRES-UI-008.
- [ ] 7.3 TDD: write a failing test asserting the "add environment" modal calls `EnvironmentService.create()`, the ambientes list updates without a full page reload, and an invalid URL shows an inline error; implement the modal form and edit/delete actions.
- [ ] 7.4 TDD: update `ProjectDetail`'s existing "Aplicaciones section shows an empty state" test to also cover the populated case (application list rendered from `selectedProject().applications`, "+ Nueva Aplicación" navigating to `/proyectos/aplicaciones/nuevo?proyectoId=:id`, and each row navigating to `/proyectos/aplicaciones/:id`); implement.

## 8. Frontend — Routing

- [ ] 8.1 Add `aplicaciones/nuevo`, `aplicaciones/:id`, `aplicaciones/:id/editar` routes to `proyectos.routes.ts`, wired to `ApplicationForm`/`ApplicationDetail`.

## 9. Frontend E2E

- [ ] 9.1 Cypress test: create an application under a project, add an environment, verify it appears on the detail page (per rule PRES-E2E-001's "Create Project → Create Application → Add Environment → Verify detail page" flow). Note in this environment: Cypress's Electron binary cannot launch here (`bad option: --smoke-test`, documented in `projects-management`'s tasks.md) — verify the equivalent flow manually via Playwright if the same blocker persists.

## 10. Documentation + Final

- [ ] 10.1 Update `backend/README.md` with the new `Applications`/`Ambientes` endpoints.
- [ ] 10.2 `dotnet build AdminPro.slnx` succeeds with zero errors; `dotnet test` all green.
- [ ] 10.3 `ng build` succeeds with zero errors; Angular unit tests all green.
- [ ] 10.4 `npx cypress run` — green (environment permitting); otherwise manually verify via Playwright and note it.
- [ ] 10.5 Confirm every task above was committed individually per the `commit` skill format.
- [ ] 10.6 Mark this `tasks.md` complete and ready for `/opsx:archive`.
