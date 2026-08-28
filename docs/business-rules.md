Business Rules — AdminPro
> **Stack:** .NET 10 · C# 14 · ASP.NET Core 10 · EF Core 10 · SQL Server 2022 · MediatR (CQRS) · FluentValidation · Serilog  
> **Frontend:** Angular 22 · TypeScript 5.6 · Standalone Components · Signals · OnPush · Bootstrap 5 · Cypress  
> **Auth:** None (single-user, local-only)  
> **Spec Framework:** OpenSpec (Spec-Driven Development)
---
1. Context & Scope
1.1 Purpose
Build a modular web application to replace ad-hoc spreadsheets used to catalog projects, applications, environments, reports, databases, services, and operational notes. AdminPro is project-agnostic: it can track any number of unrelated projects/clients/initiatives, not tied to a single organization. The system is designed to grow into additional modules (e.g., Budget/Presupuesto) from day one.
1.2 Scope
Modular navigation: Each module is an independent functional area with its own routes and menu. The `Modulo` entity drives only the dashboard launcher and sidebar navigation.
Module "Gestión de Proyectos": Full CRUD for Projects, Applications, Databases, Environments, Reports, Notes, Documents, FixDatas, and Services.
Extensibility: Future modules (e.g., Presupuesto) plug in by adding their own backend endpoints and Angular routes. The `Modulo` table is only a navigation registry.
Read-heavy with occasional writes (single user).
No authentication or authorization — the app runs locally and is used by a single developer.
OneDrive URLs are stored as references; no file upload or sync.
1.3 Domain Language
Term	Definition
Modulo	A navigation entry for a functional area (e.g., "Gestión de Proyectos", "Presupuesto"). Drives the dashboard launcher and sidebar. Has no data relationships with other entities.
Project	Any project, client, or initiative being tracked (e.g., "Acme Corp", "Globex Corp") — fully generic, not tied to any specific organization. Owns databases and applications.
Database	A SQL Server database instance belonging to a Project.
Application	A software system belonging to a Project. Has environments, reports, notes, documents, and fix-datas.
Environment	A deployment target of an Application (e.g., DEV, UAT, PRE-PROD, PROD). Each app defines its own set.
Report	A reporting artifact linked to an Application, with stored procedures and parameters.
Note	A titled memo attached to an Application.
Document	A OneDrive-linked file reference (manual, diagram, code, etc.) attached to an Application.
FixData	A named SQL script with description, attached to an Application.
Service	An external endpoint (Security, Mobile, etc.) that can be global, project-level, or application-level.
---
2. Data Model
2.1 Entity Relationship Diagram
```
┌─────────────┐
│   Modulo    │     ← Navigation-only. No FK relationships.
│─────────────│
│ Id (PK)     │
│ Nombre (UQ) │
│ Icono       │
│ RutaBase    │
│ Color       │
│ Orden       │
│ Activo      │
└─────────────┘

┌─────────────┐       ┌─────────────────┐       ┌─────────────┐
│  Project    │1─────N│  BaseDeDatos    │       │  Servicio   │
│─────────────│       │─────────────────│       │─────────────│
│ Id (PK)     │       │ Id (PK)         │       │ Id (PK)     │
│ Nombre (UQ) │       │ ProyectoId (FK) │       │ Nombre      │
│ Descripcion │       │ Nombre          │       │ Tipo        │
│ Activo      │       │ Servidor        │       │ Ambiente    │
│ ...         │       │ DatabaseId      │       │ Url         │
└─────────────┘       │ LoginName       │       │ Notas       │
       │1             │ Ambiente        │       │ EsGlobal    │
       │             │ Notas           │       │ ProyectoId  │
       N│             │ Activo          │       │ Activo      │
       │             └─────────────────┘       └──────┬──────┘
       │                                               │
       │1                                             N│
       └─────────────┐                    ┌───────────┘
                     N│                    │N
              ┌───────▼───────┐    ┌───────▼───────────┐
              │  Application  │    │ AplicacionServicio│
              │───────────────│    │───────────────────│
              │ Id (PK)       │    │ AplicacionId (FK) │
              │ ProyectoId(FK)│    │ ServicioId (FK)   │
              │ Nombre        │    │ NotasEspecificas  │
              │ Descripcion   │    └───────────────────┘
              │ TecnologiaFront│
              │ TecnologiaBack │
              │ RamaDesarrollo │
              │ ApplicationName│
              │ TieneProyectoBD│
              │ RutaLocal      │
              │ RutaGit        │
              │ ComoSeLevanta  │
              │ NotasCompilacion│
              │ Orden          │
              │ Activo         │
              └───────┬───────┘
                      │1
          ┌───────────┼───────────┬───────────┬───────────┐
         N│          N│          N│          N│          N│
    ┌─────▼─────┐ ┌───▼────┐ ┌────▼────┐ ┌────▼────┐ ┌────▼────┐
    │Ambiente   │ │Reporte │ │  Nota   │ │Documento│ │ FixData │
    │───────────│ │────────│ │─────────│ │─────────│ │─────────│
    │Id (PK)    │ │Id (PK) │ │Id (PK)  │ │Id (PK)  │ │Id (PK)  │
    │AppId (FK) │ │AppId(FK)│ │AppId(FK)│ │AppId(FK)│ │AppId(FK)│
    │Nombre     │ │ReportCode│ │Titulo   │ │Nombre   │ │Nombre   │
    │Url        │ │ReportName│ │Descripcion│ │Url      │ │Descripcion│
    │EsWebApi   │ │RegionId │ │Orden    │ │Tipo     │ │Script   │
    │Notas      │ │ReportPath│ │Activo   │ │Descripcion│ │Orden    │
    │Orden      │ │SpTranship│ └─────────┘ │Orden    │ │Activo   │
    │Activo     │ │SpRV      │             │Activo   │ └─────────┘
    └───────────┘ │Notas    │             └─────────┘
                  │Parametros│
                  │Activo   │
                  └─────────┘
```
2.2 Table Specifications
2.2.1 Modulos — Navigation Registry Only
Column	Type	Constraints	Notes
Id	`int`	PK, Identity	
Nombre	`nvarchar(100)`	NOT NULL, UNIQUE	e.g., "Gestión de Proyectos", "Presupuesto"
Icono	`nvarchar(50)`	NULL	Bootstrap icon class or SVG name
RutaBase	`nvarchar(50)`	NOT NULL, UNIQUE	Angular route prefix: "proyectos", "presupuesto"
Color	`nvarchar(20)`	NULL	Bootstrap color class: "primary", "success", "danger"
Orden	`int`	NOT NULL, DEFAULT 0	Dashboard display order
Activo	`bit`	NOT NULL, DEFAULT 1	Soft delete
CreatedAt	`datetime2`	NOT NULL, DEFAULT GETUTCDATE()	
UpdatedAt	`datetime2`	NOT NULL, DEFAULT GETUTCDATE()	
⚠️ IMPORTANT: `Modulo` has NO foreign key relationships to any other table. It is a pure navigation/menu entity.
2.2.2 Projects
Column	Type	Constraints	Notes
Id	`int`	PK, Identity	
Nombre	`nvarchar(100)`	NOT NULL, UNIQUE	e.g., "Acme Corp", "Globex Corp"
Descripcion	`nvarchar(500)`	NULL	
Activo	`bit`	NOT NULL, DEFAULT 1	Soft delete
CreatedAt	`datetime2`	NOT NULL, DEFAULT GETUTCDATE()	
UpdatedAt	`datetime2`	NOT NULL, DEFAULT GETUTCDATE()	
2.2.3 BaseDeDatos (Databases)
Column	Type	Constraints	Notes
Id	`int`	PK, Identity	
ProyectoId	`int`	FK → Projects.Id, CASCADE DELETE	
Nombre	`nvarchar(100)`	NOT NULL	e.g., "SalesDb"
Servidor	`nvarchar(200)`	NULL	e.g., "SQLSRV01.corp.acme.local"
DatabaseId	`int`	NULL	Numeric ID from server
LoginName	`nvarchar(100)`	NULL	e.g., "app_user"
Ambiente	`nvarchar(50)`	NULL	Free text: "desarrollo", "uat", "pre-prod", "production"
Notas	`nvarchar(max)`	NULL	
Activo	`bit`	NOT NULL, DEFAULT 1	
CreatedAt	`datetime2`	NOT NULL	
UpdatedAt	`datetime2`	NOT NULL	
2.2.4 Applications
Column	Type	Constraints	Notes
Id	`int`	PK, Identity	
ProyectoId	`int`	FK → Projects.Id, CASCADE DELETE	
Nombre	`nvarchar(100)`	NOT NULL	e.g., "CRM", "Report Viewer"
Descripcion	`nvarchar(500)`	NULL	
TecnologiaFront	`nvarchar(100)`	NULL	Free text, informational
TecnologiaBack	`nvarchar(100)`	NULL	Free text, informational
RamaDesarrollo	`nvarchar(100)`	NULL	e.g., "origin/base"
ApplicationName	`nvarchar(100)`	NULL	Internal app name
TieneProyectoBD	`nvarchar(50)`	NULL	"SI", "NO", or specific name
RutaLocal	`nvarchar(500)`	NULL	Local dev path
RutaGit	`nvarchar(500)`	NULL	Git repository path
ComoSeLevanta	`nvarchar(500)`	NULL	How to run it
NotasCompilacion	`nvarchar(max)`	NULL	Build notes, nvm versions, etc.
Orden	`int`	NOT NULL, DEFAULT 0	Display order
Activo	`bit`	NOT NULL, DEFAULT 1	
CreatedAt	`datetime2`	NOT NULL	
UpdatedAt	`datetime2`	NOT NULL	
Unique Constraint: `IX_Applications_ProyectoId_Nombre` — Nombre must be unique within a Project.
2.2.5 Ambientes (Environments)
Column	Type	Constraints	Notes
Id	`int`	PK, Identity	
AplicacionId	`int`	FK → Applications.Id, CASCADE DELETE	
Nombre	`nvarchar(50)`	NOT NULL	e.g., "DEV", "UAT", "PRE-PROD", "PROD", "QA"
Url	`nvarchar(500)`	NULL	Must be valid URL format
EsWebApi	`bit`	NOT NULL, DEFAULT 0	
Notas	`nvarchar(max)`	NULL	
Orden	`int`	NOT NULL, DEFAULT 0	
Activo	`bit`	NOT NULL, DEFAULT 1	
CreatedAt	`datetime2`	NOT NULL	
UpdatedAt	`datetime2`	NOT NULL	
2.2.6 Reportes (Reports)
Column	Type	Constraints	Notes
Id	`int`	PK, Identity	
AplicacionId	`int`	FK → Applications.Id, CASCADE DELETE	
ReportCode	`nvarchar(20)`	NOT NULL	e.g., "VFL", "AUT"
ReportName	`nvarchar(200)`	NOT NULL	e.g., "Volumen de Carga"
RegionId	`nvarchar(10)`	NULL	e.g., "DLA", "DNA"
ReportPath	`nvarchar(200)`	NULL	e.g., "/volume-for-load"
SpTranship	`nvarchar(200)`	NULL	Stored procedure name
SpReportViewer	`nvarchar(200)`	NULL	Stored procedure name
Notas	`nvarchar(max)`	NULL	
ParametrosEjemplo	`nvarchar(max)`	NULL	Example execution params
Activo	`bit`	NOT NULL, DEFAULT 1	
CreatedAt	`datetime2`	NOT NULL	
UpdatedAt	`datetime2`	NOT NULL	
Unique Constraint: `IX_Reportes_AplicacionId_ReportCode`
2.2.7 Notas (Notes)
Column	Type	Constraints	Notes
Id	`int`	PK, Identity	
AplicacionId	`int`	FK → Applications.Id, CASCADE DELETE	
Titulo	`nvarchar(200)`	NOT NULL	
Descripcion	`nvarchar(max)`	NOT NULL	Memo / large text
Orden	`int`	NOT NULL, DEFAULT 0	
Activo	`bit`	NOT NULL, DEFAULT 1	
CreatedAt	`datetime2`	NOT NULL	
UpdatedAt	`datetime2`	NOT NULL	
2.2.8 Documentos (Documents)
Column	Type	Constraints	Notes
Id	`int`	PK, Identity	
AplicacionId	`int`	FK → Applications.Id, CASCADE DELETE	
NombreArchivo	`nvarchar(200)`	NOT NULL	Display name
UrlOneDrive	`nvarchar(500)`	NOT NULL	Must be valid URL
Tipo	`nvarchar(50)`	NOT NULL	"manual", "diagrama", "codigo", "otro"
Descripcion	`nvarchar(500)`	NULL	
Orden	`int`	NOT NULL, DEFAULT 0	
Activo	`bit`	NOT NULL, DEFAULT 1	
CreatedAt	`datetime2`	NOT NULL	
UpdatedAt	`datetime2`	NOT NULL	
2.2.9 FixDatas
Column	Type	Constraints	Notes
Id	`int`	PK, Identity	
AplicacionId	`int`	FK → Applications.Id, CASCADE DELETE	
Nombre	`nvarchar(100)`	NOT NULL	
Descripcion	`nvarchar(500)`	NULL	
Script	`nvarchar(max)`	NULL	SQL script content
Orden	`int`	NOT NULL, DEFAULT 0	
Activo	`bit`	NOT NULL, DEFAULT 1	
CreatedAt	`datetime2`	NOT NULL	
UpdatedAt	`datetime2`	NOT NULL	
2.2.10 Servicios (Services)
Column	Type	Constraints	Notes
Id	`int`	PK, Identity	
ProyectoId	`int`	FK → Projects.Id, SET NULL	Nullable — project scope
Nombre	`nvarchar(100)`	NOT NULL	e.g., "Security API UAT"
Tipo	`nvarchar(50)`	NOT NULL	e.g., "Seguridad", "Mobile", "ReportViewer"
Ambiente	`nvarchar(50)`	NULL	e.g., "UAT", "PROD"
Url	`nvarchar(500)`	NOT NULL	Must be valid URL
Notas	`nvarchar(max)`	NULL	
EsGlobal	`bit`	NOT NULL, DEFAULT 0	If true, available to all Applications
Activo	`bit`	NOT NULL, DEFAULT 1	
CreatedAt	`datetime2`	NOT NULL	
UpdatedAt	`datetime2`	NOT NULL	
2.2.11 AplicacionServicio (Application-Service Link)
Column	Type	Constraints	Notes
AplicacionId	`int`	FK → Applications.Id, CASCADE DELETE	Composite PK
ServicioId	`int`	FK → Servicios.Id, CASCADE DELETE	Composite PK
NotasEspecificas	`nvarchar(500)`	NULL	Usage notes for this app
CreatedAt	`datetime2`	NOT NULL	
---
3. Domain Layer Rules
> Pure business invariants. No framework, no persistence, no UI concerns.
3.1 Modulo Aggregate — Navigation Only
Rule: MOD-001 — Module Name Uniqueness
A Module name SHALL be unique across the entire system.
Scenario: Creating a module with duplicate name
GIVEN a Module named "Gestión de Proyectos" already exists
WHEN the system attempts to create another Module named "Gestión de Proyectos"
THEN the operation SHALL be rejected with a domain error: `ModuleNameAlreadyExists`
Rule: MOD-002 — Route Base Uniqueness
A Module's `RutaBase` SHALL be unique and URL-safe (no spaces, no special chars except hyphen and underscore).
Rule: MOD-003 — Module Deletion Is Soft Only
Deleting a Module SHALL only set `Activo = 0` on the Module itself. It SHALL NOT cascade to any other entity because Modules have no data relationships.
Rule: MOD-004 — Default Module
The system SHALL ship with at least one default Module: "Gestión de Proyectos" (`RutaBase = "proyectos"`).
Rule: MOD-005 — Module Is Pure Navigation
Modules SHALL NOT enforce data isolation, ownership, or filtering on Projects, Applications, Services, or any other entity. They exist solely for menu/dashboard rendering.
3.2 Project Aggregate
Rule: PROJECT-001 — Project Name Uniqueness
A Project name SHALL be unique across the entire system.
Scenario: Creating a project with duplicate name
GIVEN a Project named "Acme Corp" already exists
WHEN the system attempts to create another Project named "Acme Corp"
THEN the operation SHALL be rejected with a domain error: `ProjectNameAlreadyExists`
Rule: PROJECT-002 — Project Deletion Cascades
Deleting a Project SHALL logically delete (soft-delete) all its Databases and Applications.
Scenario: Soft-deleting a project
GIVEN Project "Acme Corp" has 3 Databases and 5 Applications
WHEN the Project is deactivated (Activo = 0)
THEN all 3 Databases and 5 Applications SHALL also have Activo = 0
AND their child entities (Environments, Reports, Notes, Documents, FixDatas) SHALL also have Activo = 0
3.3 Database Aggregate (Child of Project)
Rule: DB-001 — Database Belongs to Exactly One Project
A Database MUST have a valid Project reference. It cannot exist independently.
Rule: DB-002 — Environment Free Text
The `Ambiente` field is free text but SHOULD be normalized by convention: "desarrollo", "uat", "pre-prod", "production".
3.4 Application Aggregate (Child of Project)
Rule: APP-001 — Application Name Uniqueness Within Project
An Application name SHALL be unique within its parent Project.
Scenario: Duplicate app name in same project
GIVEN Project "Acme Corp" has an Application named "CRM"
WHEN creating another Application named "CRM" under "Acme Corp"
THEN the operation SHALL be rejected with `ApplicationNameAlreadyExistsInProject`
Rule: APP-002 — Application Deletion Cascades
Deleting an Application SHALL soft-delete all child entities: Environments, Reports, Notes, Documents, FixDatas, and Application-Service links.
Rule: APP-003 — Technology Fields Are Informational
`TecnologiaFront` and `TecnologiaBack` are optional, free-text, and carry no validation logic beyond max length.
Rule: APP-004 — Display Order
Applications within a Project SHALL be sortable via the `Orden` field. Lower values appear first.
3.5 Environment Aggregate (Child of Application)
Rule: ENV-001 — Environment Name Required
Every Environment MUST have a non-empty `Nombre`.
Rule: ENV-002 — URL Format
If `Url` is provided, it SHALL be a valid absolute URL (http:// or https://).
Rule: ENV-003 — Custom Environments Allowed
An Application MAY define any environment name (e.g., "QA", "STAGING", "DEMO"). There is no closed catalog.
Rule: ENV-004 — Web API Flag
`EsWebApi` indicates whether this environment URL points to a Web API (Swagger) rather than a UI.
3.6 Report Aggregate (Child of Application)
Rule: REP-001 — Report Code Uniqueness Within Application
A ReportCode SHALL be unique within its parent Application.
Rule: REP-002 — Stored Procedures Are Optional
`SpTranship` and `SpReportViewer` MAY be null if the report does not use that data source.
3.7 Note Aggregate (Child of Application)
Rule: NOTE-001 — Title Required
A Note MUST have a non-empty `Titulo`.
Rule: NOTE-002 — Description Is Memo
`Descripcion` accepts unlimited length (nvarchar(max)) and MAY contain newlines, markdown, or HTML.
3.8 Document Aggregate (Child of Application)
Rule: DOC-001 — OneDrive URL Required
A Document MUST have a non-empty `UrlOneDrive` that is a valid URL.
Rule: DOC-002 — Document Type Catalog
`Tipo` SHALL be one of: "manual", "diagrama", "codigo", "otro". The frontend MAY allow free-text entry but SHOULD suggest these values.
3.9 FixData Aggregate (Child of Application)
Rule: FIX-001 — Name Required
A FixData MUST have a non-empty `Nombre`.
Rule: FIX-002 — Script Content
`Script` MAY contain any SQL text and is stored as nvarchar(max).
3.10 Service Entity
Rule: SVC-001 — Service Scope Hierarchy
A Service exists at one of three scopes:
Global — `EsGlobal = 1`, `ProyectoId IS NULL`. Available to all Applications.
Project-level — `EsGlobal = 0`, `ProyectoId IS NOT NULL`. Available to all Applications in that Project.
Application-level — Linked via `AplicacionServicio`. Specific to one Application.
Rule: SVC-002 — URL Required and Valid
Every Service MUST have a non-empty, valid absolute URL.
Rule: SVC-003 — Service Type Is Free Text
`Tipo` is free text (e.g., "Seguridad", "Mobile", "ReportViewer") but the UI SHOULD suggest known values.
Rule: SVC-004 — Application-Service Link Is Optional
An Application MAY reference zero or more Services. A Service MAY be referenced by zero or more Applications.
3.11 Dashboard Rules
Rule: DASH-001 — Show Only Active Modules
The dashboard SHALL display only Modules where `Activo = 1`, ordered by `Orden` ascending.
Rule: DASH-002 — Module Navigation
Clicking a Module tile SHALL navigate to `/{RutaBase}`.
Rule: DASH-003 — Module Context Is Presentation-Only
The active module context in the UI (sidebar highlight, breadcrumb) is purely a frontend concern. The backend SHALL NOT filter or scope any data based on the active module.
---
4. Application Layer Rules
> CQRS commands, queries, validation, and orchestration logic.
4.1 CQRS Pattern
Rule: APP-CQRS-001 — Command/Query Separation
Every write operation SHALL be implemented as a Command (MediatR `IRequest<T>`).  
Every read operation SHALL be implemented as a Query (MediatR `IRequest<T>`).
Rule: APP-CQRS-002 — No Domain Logic in Handlers
Command/Query handlers SHALL orchestrate only. All business invariants SHALL live in the Domain layer (entities, domain services) or be enforced by FluentValidation.
4.2 Validation (FluentValidation)
Rule: APP-VAL-001 — Validation Rules Per Entity
Module Validator:
`Nombre`: NotEmpty, MaxLength(100), Must be unique (async DB check).
`RutaBase`: NotEmpty, MaxLength(50), Matches `^[a-z0-9_-]+$`, Must be unique.
`Icono`: MaxLength(50).
`Color`: MaxLength(20).
Project Validator:
`Nombre`: NotEmpty, MaxLength(100), Must be unique globally (async DB check).
`Descripcion`: MaxLength(500).
Database Validator:
`Nombre`: NotEmpty, MaxLength(100).
`Servidor`: MaxLength(200).
`Ambiente`: MaxLength(50).
`ProyectoId`: NotNull, must reference existing active Project.
Application Validator:
`Nombre`: NotEmpty, MaxLength(100).
`ProyectoId`: NotNull, must reference existing active Project.
`TecnologiaFront`: MaxLength(100).
`TecnologiaBack`: MaxLength(100).
`RamaDesarrollo`: MaxLength(100).
`RutaLocal`: MaxLength(500).
`RutaGit`: MaxLength(500).
`Orden`: GreaterThanOrEqualTo(0).
Environment Validator:
`Nombre`: NotEmpty, MaxLength(50).
`Url`: Must be valid absolute URL if not null/empty.
`AplicacionId`: NotNull, must reference existing active Application.
`Orden`: GreaterThanOrEqualTo(0).
Report Validator:
`ReportCode`: NotEmpty, MaxLength(20).
`ReportName`: NotEmpty, MaxLength(200).
`RegionId`: MaxLength(10).
`ReportPath`: MaxLength(200).
`AplicacionId`: NotNull.
Note Validator:
`Titulo`: NotEmpty, MaxLength(200).
`Descripcion`: NotEmpty.
`AplicacionId`: NotNull.
Document Validator:
`NombreArchivo`: NotEmpty, MaxLength(200).
`UrlOneDrive`: NotEmpty, Must be valid absolute URL.
`Tipo`: NotEmpty, MaxLength(50).
`AplicacionId`: NotNull.
FixData Validator:
`Nombre`: NotEmpty, MaxLength(100).
`AplicacionId`: NotNull.
Service Validator:
`Nombre`: NotEmpty, MaxLength(100).
`Tipo`: NotEmpty, MaxLength(50).
`Url`: NotEmpty, Must be valid absolute URL.
`Ambiente`: MaxLength(50).
4.3 Commands
Rule: APP-CMD-001 — Create Module
```
CreateModuloCommand
├── Nombre: string
├── Icono: string?
├── RutaBase: string
├── Color: string?
├── Orden: int
└── Returns: int (Module Id)
```
Validate uniqueness of Nombre and RutaBase.
Set Activo = 1, CreatedAt = UpdatedAt = UtcNow.
Rule: APP-CMD-002 — Update Module
```
UpdateModuloCommand
├── Id: int
├── Nombre: string
├── Icono: string?
├── RutaBase: string
├── Color: string?
├── Orden: int
└── Returns: Unit
```
Rule: APP-CMD-003 — Soft Delete Module
```
DeleteModuloCommand
├── Id: int
└── Returns: Unit
```
Set Activo = 0 on the Module ONLY. No cascade.
Rule: APP-CMD-004 — Create Project
```
CreateProjectCommand
├── Nombre: string
├── Descripcion: string?
└── Returns: int (Project Id)
```
Rule: APP-CMD-005 — Update Project
```
UpdateProjectCommand
├── Id: int
├── Nombre: string
├── Descripcion: string?
└── Returns: Unit
```
Rule: APP-CMD-006 — Soft Delete Project
```
DeleteProjectCommand
├── Id: int
└── Returns: Unit
```
Set Activo = 0 for Project and all cascading children.
Rule: APP-CMD-007 — Create Application
```
CreateApplicationCommand
├── ProyectoId: int
├── Nombre: string
├── Descripcion: string?
├── TecnologiaFront: string?
├── TecnologiaBack: string?
├── RamaDesarrollo: string?
├── ApplicationName: string?
├── TieneProyectoBD: string?
├── RutaLocal: string?
├── RutaGit: string?
├── ComoSeLevanta: string?
├── NotasCompilacion: string?
├── Orden: int
└── Returns: int (Application Id)
```
Rule: APP-CMD-008 — Create Environment
```
CreateEnvironmentCommand
├── AplicacionId: int
├── Nombre: string
├── Url: string?
├── EsWebApi: bool
├── Notas: string?
├── Orden: int
└── Returns: int
```
Rule: APP-CMD-009 — Create Report
```
CreateReportCommand
├── AplicacionId: int
├── ReportCode: string
├── ReportName: string
├── RegionId: string?
├── ReportPath: string?
├── SpTranship: string?
├── SpReportViewer: string?
├── Notas: string?
├── ParametrosEjemplo: string?
└── Returns: int
```
Rule: APP-CMD-010 — Create Note
```
CreateNoteCommand
├── AplicacionId: int
├── Titulo: string
├── Descripcion: string
├── Orden: int
└── Returns: int
```
Rule: APP-CMD-011 — Create Document
```
CreateDocumentCommand
├── AplicacionId: int
├── NombreArchivo: string
├── UrlOneDrive: string
├── Tipo: string
├── Descripcion: string?
├── Orden: int
└── Returns: int
```
Rule: APP-CMD-012 — Create FixData
```
CreateFixDataCommand
├── AplicacionId: int
├── Nombre: string
├── Descripcion: string?
├── Script: string?
├── Orden: int
└── Returns: int
```
Rule: APP-CMD-013 — Create Service
```
CreateServiceCommand
├── ProyectoId: int?
├── Nombre: string
├── Tipo: string
├── Ambiente: string?
├── Url: string
├── Notas: string?
├── EsGlobal: bool
└── Returns: int
```
Rule: APP-CMD-014 — Link Service to Application
```
LinkServiceToApplicationCommand
├── AplicacionId: int
├── ServicioId: int
├── NotasEspecificas: string?
└── Returns: Unit
```
Validate that the Service is either global, project-level (same project as app), or already application-level.
Prevent duplicate links.
4.4 Queries
Rule: APP-QRY-001 — List Modules (Dashboard)
```
GetModulosQuery
├── IncludeInactive: bool (default false)
└── Returns: List<ModuloDto>
```
Ordered by Orden ascending, then Nombre.
If IncludeInactive = false, filter Activo = 1.
Rule: APP-QRY-002 — List Projects
```
GetProjectsQuery
├── IncludeInactive: bool
└── Returns: List<ProjectSummaryDto>
```
Ordered by Nombre ascending.
Rule: APP-QRY-003 — Get Project Detail
```
GetProjectByIdQuery
├── Id: int
├── IncludeInactiveChildren: bool (default false)
└── Returns: ProjectDetailDto
```
Rule: APP-QRY-004 — List Applications by Project
```
GetApplicationsByProjectQuery
├── ProyectoId: int
├── IncludeInactive: bool
└── Returns: List<ApplicationSummaryDto>
```
Ordered by Orden, then Nombre.
Rule: APP-QRY-005 — Get Application Detail
```
GetApplicationByIdQuery
├── Id: int
└── Returns: ApplicationDetailDto
```
Includes Environments, Reports, Notes, Documents, FixDatas, and linked Services.
Child collections ordered by Orden.
Rule: APP-QRY-006 — Global Search
```
SearchQuery
├── Term: string
└── Returns: SearchResultDto
```
Searches across: Project names, Application names, Database names, Report codes/names, Note titles, Service names.
Case-insensitive, partial match.
Returns grouped results by entity type.
Rule: APP-QRY-007 — List Services
```
GetServicesQuery
├── ProyectoId: int? (filter)
├── Tipo: string? (filter)
├── Ambiente: string? (filter)
└── Returns: List<ServiceDto>
```
4.5 DTOs
Rule: APP-DTO-001 — Flat DTOs for Lists
List endpoints SHALL return flat DTOs without nested collections.
Rule: APP-DTO-002 — Detailed DTOs for Single Entity
Single-entity endpoints (GetById) SHALL return detailed DTOs with all child collections.
Rule: APP-DTO-003 — Audit Fields
All DTOs SHALL include `Id`, `Activo`, `CreatedAt`, `UpdatedAt`.
---
5. Infrastructure Layer Rules
> Persistence, logging, external concerns.
5.1 Entity Framework Core
Rule: INF-EF-001 — DbContext Configuration
Use `DbContext` with explicit fluent configuration in `OnModelCreating`.
Rule: INF-EF-002 — Soft Delete Global Filter
Apply a global query filter for soft delete:
```csharp
modelBuilder.Entity<Modulo>().HasQueryFilter(m => m.Activo);
modelBuilder.Entity<Project>().HasQueryFilter(p => p.Activo);
modelBuilder.Entity<Application>().HasQueryFilter(a => a.Activo);
// ... etc for all entities
```
Rule: INF-EF-003 — Cascade Delete Strategy
Hard cascade for child entities of Application (Environments, Reports, Notes, Documents, FixDatas, AplicacionServicio) via `OnDelete(Cascade)`.
Soft cascade for Project → Applications/Databases: implemented in Application layer command handler.
No cascade for Modulo → anything. Modulo deletion is isolated.
Rule: INF-EF-004 — Audit Shadow Properties OR Explicit Fields
Use explicit `CreatedAt` and `UpdatedAt` fields on every entity. Override `SaveChanges` to auto-set `UpdatedAt`.
Rule: INF-EF-005 — Indexing
Create composite indexes:
`IX_Modulos_Nombre` (unique, filtered where Activo = 1)
`IX_Modulos_RutaBase` (unique, filtered where Activo = 1)
`IX_Projects_Nombre` (unique, filtered where Activo = 1)
`IX_Applications_ProyectoId_Nombre` (unique, filtered)
`IX_Reportes_AplicacionId_ReportCode` (unique, filtered)
`IX_Ambientes_AplicacionId_Orden`
`IX_Servicios_ProyectoId_Tipo`
`IX_Servicios_EsGlobal`
5.2 SQL Server 2022
Rule: INF-SQL-001 — Collation
Use `SQL_Latin1_General_CP1_CI_AS` or `Latin1_General_100_CI_AS_SC_UTF8` for case-insensitive search.
Rule: INF-SQL-002 — nvarchar for Unicode
All string columns SHALL use `nvarchar` to support Spanish accents and special characters.
Rule: INF-SQL-003 — datetime2
All timestamps SHALL use `datetime2(7)`.
5.3 Serilog Logging
Rule: INF-LOG-001 — Log Levels
`Information`: Command/Query execution start and completion.
`Warning`: Validation failures, not-found scenarios.
`Error`: Unhandled exceptions.
Rule: INF-LOG-002 — Structured Logging
Log with context:
```json
{
  "Command": "CreateApplicationCommand",
  "EntityId": 42,
  "DurationMs": 145,
  "User": "local"
}
```
5.4 MediatR Behaviors
Rule: INF-MED-001 — Pipeline Behaviors
Implement three behaviors:
ValidationBehavior — runs FluentValidation before handler.
LoggingBehavior — logs command/query name and duration.
TransactionBehavior — wraps command in EF Core transaction (SaveChanges).
Rule: INF-MED-002 — No Transactions for Queries
Query handlers SHALL NOT open explicit transactions.
---
6. Presentation Layer Rules
> Angular 22 frontend, UI/UX, and client-side validation.
6.1 Architecture
Rule: PRES-ARCH-001 — Standalone Components
All components SHALL be standalone (`standalone: true`). No NgModules except `AppModule` for bootstrapping if required by legacy patterns.
Rule: PRES-ARCH-002 — Signals for State
Use Angular Signals for component-level and service-level state. Avoid RxJS `BehaviorSubject` for simple state.
Rule: PRES-ARCH-003 — OnPush Change Detection
All components SHALL use `ChangeDetectionStrategy.OnPush`.
Rule: PRES-ARCH-004 — Service Layer
Create one API service per aggregate:
`ModuloService`
`ProjectService`
`ApplicationService`
`DatabaseService`
`EnvironmentService`
`ReportService`
`NoteService`
`DocumentService`
`FixDataService`
`ServiceCatalogService`
Each service uses `HttpClient` and exposes Signal-based state where appropriate.
Rule: PRES-ARCH-005 — Module-Based Folder Structure
```
src/app/
├── dashboard/
│   ├── dashboard.component.ts
│   └── dashboard.routes.ts
├── proyectos/                    ← Module "Gestión de Proyectos"
│   ├── proyectos.routes.ts
│   ├── services/
│   ├── components/
│   └── pages/
├── presupuesto/                  ← Future module placeholder
│   ├── presupuesto.routes.ts
│   └── ...
├── shared/
│   ├── components/
│   ├── models/
│   └── services/
└── core/
    ├── interceptors/
    └── guards/
```
6.2 Routing
Rule: PRES-ROUT-001 — Route Structure
```
/                     → Dashboard (module launcher)
/proyectos            → Module Home (Gestión de Proyectos)
/proyectos/proyectos  → Project List
/proyectos/proyectos/:id → Project Detail
/proyectos/aplicaciones/:id → Application Detail
/proyectos/servicios  → Service Catalog
/search?q=term        → Global Search
```
Rule: PRES-ROUT-002 — Lazy Loading
Feature modules/routes SHALL be lazy-loaded:
`dashboard` route → lazy load Dashboard component
`proyectos` route → lazy load proyectos module routes
`presupuesto` route → lazy load presupuesto module routes
Rule: PRES-ROUT-003 — Module Guard
A simple guard SHALL verify that the route module exists in the Modulos table before loading. If not found, redirect to `/`.
6.3 UI/UX Rules
Rule: PRES-UI-001 — Bootstrap 5 Styling
Use Bootstrap 5 classes. Custom CSS only for layout tweaks.
Rule: PRES-UI-002 — Dashboard Design
The dashboard SHALL display active Modules as cards/tiles:
Card shows: Icono, Nombre, Color accent.
Click navigates to `/{RutaBase}`.
Cards ordered by `Orden`.
Rule: PRES-UI-003 — Module Sidebar
Inside a Module, a sidebar SHALL show:
Module name and icon (header)
Navigation links: Projects, Services, Search
"Back to Dashboard" button at bottom
Rule: PRES-UI-004 — Forms
All forms SHALL use Reactive Forms (`FormBuilder`).
Client-side validation MUST mirror server-side FluentValidation rules.
Display validation errors inline below each field.
Rule: PRES-UI-005 — URLs Are Clickable
All `Url` and `UrlOneDrive` fields SHALL render as `<a>` tags with `target="_blank"`.
Rule: PRES-UI-006 — Copy to Clipboard
RutaLocal, RutaGit, Script, and ParametrosEjemplo fields SHALL have a "Copy" button.
Rule: PRES-UI-007 — Sortable Lists
Environments, Notes, Documents, and FixDatas SHALL be sortable via drag-and-drop (update `Orden`).
Rule: PRES-UI-008 — Expandable Sections
Application detail page SHALL use accordion or tabs:
Ambientes
Reportes
Notas
Documentos
FixDatas
Servicios
Rule: PRES-UI-009 — Search Highlight
Search results SHALL highlight the matching term.
Rule: PRES-UI-010 — Empty States
Every list SHALL show an empty-state message when no items exist.
Rule: PRES-UI-011 — Breadcrumbs
Breadcrumbs SHALL show: `Dashboard > [Module] > [Project] > [Application]`.
6.4 Client-Side Validation
Rule: PRES-VAL-001 — URL Validation
Client-side URL regex: `^https?://.+` (same as server).
Rule: PRES-VAL-002 — Required Fields
Mark required fields with red asterisk (`*`).
Rule: PRES-VAL-003 — Max Length
Enforce `maxlength` attributes matching database limits.
6.5 Cypress E2E
Rule: PRES-E2E-001 — Critical Paths
Cover these flows:
Dashboard → Click Module → See Project List.
Create Project → Create Application → Add Environment → Verify detail page.
Search for Application by name → Navigate to detail.
Add Note to Application → Verify it appears in list.
Add Document with OneDrive URL → Verify link opens.
Reorder Environments via drag-and-drop → Verify persistence after refresh.
---
7. Cross-Cutting Concerns
7.1 No Authentication
Rule: XCUT-AUTH-001 — Anonymous Access
The application SHALL NOT implement authentication, authorization, JWT, cookies, or sessions.
Rule: XCUT-AUTH-002 — Local-Only Deployment
The app is designed to run on `localhost` with SQL Server LocalDB or Developer Edition. No security hardening for public exposure is required.
7.2 Error Handling
Rule: XCUT-ERR-001 — Global Exception Handler
ASP.NET Core SHALL use a global exception handler middleware returning:
```json
{
  "error": "ValidationError",
  "message": "...",
  "details": [ { "field": "Nombre", "error": "Required" } ]
}
```
Rule: XCUT-ERR-002 — Frontend Error Interceptor
Angular HTTP interceptor SHALL catch 400/500 errors and display a Bootstrap toast notification.
7.3 API Conventions
Rule: XCUT-API-001 — RESTful Endpoints
```
GET    /api/modulos
GET    /api/modulos/{id}
POST   /api/modulos
PUT    /api/modulos/{id}
DELETE /api/modulos/{id}

GET    /api/projects
GET    /api/projects/{id}
POST   /api/projects
PUT    /api/projects/{id}
DELETE /api/projects/{id}

GET    /api/projects/{projectId}/applications
GET    /api/projects/{projectId}/databases

GET    /api/applications/{id}
POST   /api/applications
PUT    /api/applications/{id}
DELETE /api/applications/{id}

GET    /api/applications/{appId}/environments
POST   /api/applications/{appId}/environments
// ... etc for reports, notes, documents, fixdatas

GET    /api/services
POST   /api/services
POST   /api/applications/{appId}/services/{serviceId}
DELETE /api/applications/{appId}/services/{serviceId}

GET    /api/search?term={term}
```
Note: No `{moduloId}` in project/service endpoints. Modules are navigation-only.
Rule: XCUT-API-002 — HTTP 204 for Delete
Successful soft-delete returns `204 No Content`.
Rule: XCUT-API-003 — HTTP 404 for Not Found
Requesting a non-existent or inactive entity returns `404 Not Found`.
7.4 Naming Conventions
Rule: XCUT-NAM-001 — Spanish Domain Language
Database tables, entities, and DTO properties use Spanish names matching the Excel (e.g., `Modulo`, `Aplicacion`, `Ambiente`, `BaseDeDatos`).
Rule: XCUT-NAM-002 — English for Code Artifacts
C# classes, methods, and Angular services use English (e.g., `CreateApplicationCommand`, `ApplicationService`).
Rule: XCUT-NAM-003 — Module Route Prefix
Angular route segments match `Modulo.RutaBase` exactly (lowercase, hyphenated if needed).
---
8. Implementation Phases (OpenSpec Changes)
> Suggested breakdown for `/opsx:propose` commands.
Phase 1: Foundation
Initialize solution, projects, EF Core, MediatR, FluentValidation, Serilog.
Create `AppDbContext` with all entities and fluent configuration.
Run initial migration with seed data (default "Gestión de Proyectos" module).
Create base `ApiController`, `ExceptionHandlerMiddleware`.
Angular: scaffold app, Bootstrap 5, routing, dashboard shell.
Phase 2: Dashboard & Module Launcher
Backend: `GetModulosQuery`, `ModuloController`.
Frontend: Dashboard component with module cards, dynamic lazy-loaded routes.
Cypress: dashboard navigation test.
Phase 3: Project & Database
Backend: Project CRUD, BaseDeDatos CRUD.
Frontend: Project list, detail, form pages.
Phase 4: Application Core
Backend: Application CRUD, Ambiente CRUD.
Frontend: Application detail page with Environment accordion.
Phase 5: Application Children
Backend: Reporte, Nota, Documento, FixData CRUD.
Frontend: Tabs/accordions for each child collection.
Phase 6: Services & Search
Backend: Servicio CRUD, AplicacionServicio link/unlink, Search endpoint.
Frontend: Service catalog, application-service linking, global search.
Phase 7: Polish & Extensibility
Cypress E2E tests for all critical paths.
Copy-to-clipboard buttons.
Drag-and-drop ordering.
URL clickability.
Presupuesto module placeholder (routes, empty shell).
Final UI polish.
---
Document version: 3.0  
OpenSpec-compatible format — ready for `/opsx:propose` workflow