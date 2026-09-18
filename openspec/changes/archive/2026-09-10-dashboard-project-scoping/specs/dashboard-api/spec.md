## MODIFIED Requirements

### Requirement: Dashboard summary
`GET /api/dashboard/summary` SHALL return a single `DashboardSummaryDto` aggregating: total active `Project` count, total active `Application` count, and up to 5 active `Application` rows most recently modified (name, project name, stack, `UpdatedAt`). The prior 7-day creation series, ambientes/servicios-vinculados totals, and status breakdown are removed (nothing computes or consumes a real "application status" concept).

#### Scenario: Empty database returns all zeros
- **GIVEN** no `Project` or `Application` rows exist
- **WHEN** `GET /api/dashboard/summary` is called
- **THEN** `totalProjects` and `totalApplications` are both `0` and `recentApplications` is an empty array

#### Scenario: Summary reflects real data
- **GIVEN** 2 active `Project` rows and 3 active `Application` rows exist
- **WHEN** `GET /api/dashboard/summary` is called
- **THEN** `totalProjects` is `2` and `totalApplications` is `3`

#### Scenario: Inactive rows excluded
- **GIVEN** a `Project` row exists with `Activo = false`
- **WHEN** `GET /api/dashboard/summary` is called
- **THEN** that project is not included in `totalProjects`, consistent with the global soft-delete query filter from `solution-foundation`

#### Scenario: Recent applications are ordered by last modification, not creation
- **GIVEN** application A was created before application B but B was updated most recently, while A was never updated after creation
- **WHEN** `GET /api/dashboard/summary` is called
- **THEN** `recentApplications` lists B before A, and each entry's `updatedAt` reflects that application's `Application.UpdatedAt` value

#### Scenario: Updating an application refreshes its UpdatedAt
- **GIVEN** an existing `Application` row
- **WHEN** it is updated via `PUT /api/applications/{id}`
- **THEN** its `UpdatedAt` is set to the time of that update, not left at its original creation time
