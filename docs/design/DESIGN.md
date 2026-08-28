Design Document — AdminPro
> **Stack:** .NET 10 · C# 14 · ASP.NET Core 10 · EF Core 10 · SQL Server 2022 · MediatR (CQRS) · FluentValidation · Serilog  
> **Frontend:** Angular 22 · TypeScript 5.6 · Standalone Components · Signals · OnPush · Bootstrap 5 · Cypress  
> **Auth:** None (single-user, local-only)  
> **Spec Framework:** OpenSpec (Spec-Driven Development)
---
1. Architecture Overview
1.1 Architectural Style
Clean Architecture with CQRS (Command Query Responsibility Segregation) via MediatR.
The backend is organized into concentric layers:
```
┌─────────────────────────────────────────┐
│         Presentation Layer              │  ← ASP.NET Core Controllers
│         (API Controllers)               │
├─────────────────────────────────────────┤
│         Application Layer               │  ← Commands, Queries, Validators, DTOs
│         (CQRS + MediatR)                │
├─────────────────────────────────────────┤
│         Domain Layer                    │  ← Entities, Domain Rules, Interfaces
│         (Pure C#)                       │
├─────────────────────────────────────────┤
│         Infrastructure Layer            │  ← EF Core, Repositories, Logging
│         (Persistence + External)          │
└─────────────────────────────────────────┘
```
1.2 Why Clean Architecture + CQRS
Decision	Rationale
Clean Architecture	Business rules are isolated and testable. Framework changes don't affect domain logic.
CQRS with MediatR	Separates reads from writes. Commands have side effects; queries are read-only. Easy to add cross-cutting concerns (validation, logging, transactions) via pipeline behaviors.
No Repository Pattern	EF Core `DbContext` already implements Unit of Work + Repository. Adding a generic repository is an unnecessary abstraction. Use `DbContext` directly in handlers.
No AutoMapper	For a single-user CRUD app, manual mapping in handlers is explicit and avoids hidden complexity. If mapping grows, introduce it later.
No Authentication	Single-user local app. No JWT, no Identity, no cookies.
1.3 Frontend Architecture
Component-Based Architecture with Angular Standalone Components and Signals.
```
App Shell
├── Dashboard (standalone route)
├── Module Shell (lazy-loaded per module)
│   ├── Sidebar (persistent)
│   └── Router Outlet (module pages)
└── Shared Components (reusable)
```
Decision	Rationale
Standalone Components	Simpler than NgModules. No module boilerplate. Tree-shakeable.
Signals	Fine-grained reactivity without RxJS overhead for simple state. `computed()` for derived state. `effect()` for side effects.
OnPush	Forces immutable state updates. Better performance with Signals.
No NgRx / Akita	Overkill for a single-user CRUD app. Service-level Signals are sufficient.
Bootstrap 5	Familiar, responsive grid, ready-made components (modal, accordion, tabs).
---
2. Backend Design
2.1 Solution Structure
```
AdminPro.sln
├── src/
│   ├── AdminPro.Domain/              ← Entities, interfaces, domain exceptions
│   │   ├── Entities/
│   │   │   ├── Modulo.cs
│   │   │   ├── Project.cs
│   │   │   ├── BaseDeDatos.cs
│   │   │   ├── Application.cs
│   │   │   ├── Ambiente.cs
│   │   │   ├── Reporte.cs
│   │   │   ├── Nota.cs
│   │   │   ├── Documento.cs
│   │   │   ├── FixData.cs
│   │   │   ├── Servicio.cs
│   │   │   └── AplicacionServicio.cs
│   │   ├── Interfaces/
│   │   │   └── IAuditableEntity.cs
│   │   └── Exceptions/
│   │       └── DomainException.cs
│   │
│   ├── AdminPro.Application/         ← CQRS handlers, validators, DTOs
│   │   ├── Common/
│   │   │   ├── Behaviors/
│   │   │   │   ├── ValidationBehavior.cs
│   │   │   │   ├── LoggingBehavior.cs
│   │   │   │   └── TransactionBehavior.cs
│   │   │   ├── Exceptions/
│   │   │   │   └── ValidationException.cs
│   │   │   └── Mappings/
│   │   │       └── (manual mapping extensions)
│   │   ├── Modulos/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateModulo/
│   │   │   │   ├── UpdateModulo/
│   │   │   │   └── DeleteModulo/
│   │   │   └── Queries/
│   │   │       └── GetModulos/
│   │   ├── Projects/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateProject/
│   │   │   │   ├── UpdateProject/
│   │   │   │   └── DeleteProject/
│   │   │   └── Queries/
│   │   │       ├── GetProjects/
│   │   │       └── GetProjectById/
│   │   ├── Applications/
│   │   │   ├── Commands/
│   │   │   │   ├── CreateApplication/
│   │   │   │   ├── UpdateApplication/
│   │   │   │   └── DeleteApplication/
│   │   │   └── Queries/
│   │   │       ├── GetApplicationsByProject/
│   │   │       └── GetApplicationById/
│   │   ├── BaseDeDatos/
│   │   ├── Ambientes/
│   │   ├── Reportes/
│   │   ├── Notas/
│   │   ├── Documentos/
│   │   ├── FixDatas/
│   │   ├── Servicios/
│   │   └── Search/
│   │       └── Queries/
│   │           └── Search/
│   │
│   ├── AdminPro.Infrastructure/        ← EF Core, DbContext, migrations
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Configurations/
│   │   │   │   ├── ModuloConfiguration.cs
│   │   │   │   ├── ProjectConfiguration.cs
│   │   │   │   └── ... (one per entity)
│   │   │   └── Migrations/
│   │   └── Logging/
│   │       └── SerilogConfig.cs
│   │
│   └── AdminPro.Api/                 ← Controllers, middleware, DI registration
│       ├── Controllers/
│       │   ├── ModulosController.cs
│       │   ├── ProjectsController.cs
│       │   ├── ApplicationsController.cs
│       │   ├── BaseDeDatosController.cs
│       │   ├── AmbientesController.cs
│       │   ├── ReportesController.cs
│       │   ├── NotasController.cs
│       │   ├── DocumentosController.cs
│       │   ├── FixDatasController.cs
│       │   ├── ServiciosController.cs
│       │   └── SearchController.cs
│       ├── Middleware/
│       │   └── ExceptionHandlerMiddleware.cs
│       └── Program.cs
│
├── tests/
│   ├── AdminPro.Application.Tests/     ← Unit tests for handlers & validators
│   └── AdminPro.Api.Tests/             ← Integration tests for controllers
│
└── AdminPro.sln
```
2.2 Entity Design (Domain Layer)
Each entity implements `IAuditableEntity`:
```csharp
public interface IAuditableEntity
{
    int Id { get; set; }
    bool Activo { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}
```
Project Entity
```csharp
public class Project : IAuditableEntity
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public ICollection<BaseDeDatos> BasesDeDatos { get; set; } = [];
    public ICollection<Application> Applications { get; set; } = [];
}
```
Application Entity
```csharp
public class Application : IAuditableEntity
{
    public int Id { get; set; }
    public int ProyectoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? TecnologiaFront { get; set; }
    public string? TecnologiaBack { get; set; }
    public string? RamaDesarrollo { get; set; }
    public string? ApplicationName { get; set; }
    public string? TieneProyectoBD { get; set; }
    public string? RutaLocal { get; set; }
    public string? RutaGit { get; set; }
    public string? ComoSeLevanta { get; set; }
    public string? NotasCompilacion { get; set; }
    public int Orden { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public Project Project { get; set; } = null!;
    public ICollection<Ambiente> Ambientes { get; set; } = [];
    public ICollection<Reporte> Reportes { get; set; } = [];
    public ICollection<Nota> Notas { get; set; } = [];
    public ICollection<Documento> Documentos { get; set; } = [];
    public ICollection<FixData> FixDatas { get; set; } = [];
    public ICollection<AplicacionServicio> AplicacionServicios { get; set; } = [];
}
```
2.3 DbContext & Configuration
```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Modulo> Modulos => Set<Modulo>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<BaseDeDatos> BasesDeDatos => Set<BaseDeDatos>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<Ambiente> Ambientes => Set<Ambiente>();
    public DbSet<Reporte> Reportes => Set<Reporte>();
    public DbSet<Nota> Notas => Set<Nota>();
    public DbSet<Documento> Documentos => Set<Documento>();
    public DbSet<FixData> FixDatas => Set<FixData>();
    public DbSet<Servicio> Servicios => Set<Servicio>();
    public DbSet<AplicacionServicio> AplicacionServicios => Set<AplicacionServicio>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var entries = ChangeTracker.Entries<IAuditableEntity>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return base.SaveChangesAsync(ct);
    }
}
```
2.4 MediatR Pipeline Behaviors
```csharp
// 1. ValidationBehavior — runs first
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, ct)));
        var failures = results.SelectMany(r => r.Errors).Where(f => f != null).ToList();

        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}

// 2. LoggingBehavior
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var name = typeof(TRequest).Name;
        logger.LogInformation("Handling {Command}", name);
        var response = await next();
        logger.LogInformation("Handled {Command}", name);
        return response;
    }
}

// 3. TransactionBehavior
public class TransactionBehavior<TRequest, TResponse>(AppDbContext dbContext)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (request is not ICommand) return await next();

        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            var response = await next();
            await dbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return response;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
```
2.5 FluentValidation Example
```csharp
public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator(AppDbContext dbContext)
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100)
            .MustAsync(async (nombre, ct) => 
                !await dbContext.Projects.AnyAsync(p => p.Nombre == nombre && p.Activo, ct))
            .WithMessage("Ya existe un proyecto con ese nombre.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500);
    }
}
```
2.6 Controller Pattern
All controllers inherit from a base `ApiController`:
```csharp
[ApiController]
[Route("api/[controller]")]
public abstract class ApiController(ISender sender) : ControllerBase
{
    protected ISender Sender => sender;
}

public class ProjectsController(ISender sender) : ApiController(sender)
{
    [HttpGet]
    public async Task<ActionResult<List<ProjectSummaryDto>>> GetAll(CancellationToken ct)
        => await Sender.Send(new GetProjectsQuery(), ct);

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDetailDto>> GetById(int id, CancellationToken ct)
        => await Sender.Send(new GetProjectByIdQuery(id), ct);

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateProjectCommand command, CancellationToken ct)
    {
        var id = await Sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateProjectCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest();
        await Sender.Send(command, ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await Sender.Send(new DeleteProjectCommand(id), ct);
        return NoContent();
    }
}
```
---
3. Frontend Design
3.1 Project Structure
```
adminpro-ui/
├── src/
│   ├── app/
│   │   ├── app.component.ts              ← Root shell (router-outlet)
│   │   ├── app.config.ts                 ← Providers, HTTP client, routing
│   │   ├── app.routes.ts                 ← Root routes with lazy loading
│   │   │
│   │   ├── dashboard/                    ← Module launcher
│   │   │   ├── dashboard.component.ts
│   │   │   ├── dashboard.component.html
│   │   │   ├── dashboard.component.scss
│   │   │   └── dashboard.routes.ts
│   │   │
│   │   ├── proyectos/                    ← "Gestión de Proyectos" module
│   │   │   ├── proyectos.routes.ts
│   │   │   ├── proyectos-layout.component.ts   ← Sidebar + router-outlet
│   │   │   ├── services/
│   │   │   │   ├── project.service.ts
│   │   │   │   ├── application.service.ts
│   │   │   │   ├── database.service.ts
│   │   │   │   ├── environment.service.ts
│   │   │   │   ├── report.service.ts
│   │   │   │   ├── note.service.ts
│   │   │   │   ├── document.service.ts
│   │   │   │   ├── fixdata.service.ts
│   │   │   │   └── service-catalog.service.ts
│   │   │   ├── pages/
│   │   │   │   ├── project-list/
│   │   │   │   ├── project-detail/
│   │   │   │   ├── project-form/
│   │   │   │   ├── application-detail/
│   │   │   │   ├── application-form/
│   │   │   │   ├── service-list/
│   │   │   │   └── search-results/
│   │   │   └── components/
│   │   │       ├── project-card/
│   │   │       ├── application-card/
│   │   │       ├── environment-list/
│   │   │       ├── report-list/
│   │   │       ├── note-list/
│   │   │       ├── document-list/
│   │   │       ├── fixdata-list/
│   │   │       ├── service-linker/
│   │   │       └── breadcrumbs/
│   │   │
│   │   ├── presupuesto/                  ← Future module placeholder
│   │   │   ├── presupuesto.routes.ts
│   │   │   └── ...
│   │   │
│   │   ├── shared/                       ← Reusable across all modules
│   │   │   ├── components/
│   │   │   │   ├── confirm-modal/
│   │   │   │   ├── empty-state/
│   │   │   │   ├── copy-button/
│   │   │   │   ├── sortable-list/
│   │   │   │   ├── search-bar/
│   │   │   │   └── toast/
│   │   │   ├── models/
│   │   │   │   ├── modulo.model.ts
│   │   │   │   ├── project.model.ts
│   │   │   │   ├── application.model.ts
│   │   │   │   └── ... (all DTO interfaces)
│   │   │   └── services/
│   │   │       ├── modulo.service.ts
│   │   │       └── search.service.ts
│   │   │
│   │   └── core/                         ← App-level concerns
│   │       ├── interceptors/
│   │       │   └── error.interceptor.ts
│   │       └── guards/
│   │           └── modulo-exists.guard.ts
│   │
│   ├── assets/
│   │   └── ...
│   ├── index.html
│   ├── main.ts
│   └── styles.scss
│
├── cypress/
│   └── e2e/
│       ├── dashboard.cy.ts
│       ├── projects.cy.ts
│       └── applications.cy.ts
│
├── angular.json
├── package.json
└── tsconfig.json
```
3.2 Service Design with Signals
Each service exposes a `Signal`-based store:
```typescript
// project.service.ts
@Injectable({ providedIn: 'root' })
export class ProjectService {
  private http = inject(HttpClient);
  private apiUrl = '/api/projects';

  // State
  readonly projects = signal<ProjectSummary[]>([]);
  readonly selectedProject = signal<ProjectDetail | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  // Computed
  readonly activeProjects = computed(() => this.projects().filter(p => p.activo));

  async loadProjects(): Promise<void> {
    this.loading.set(true);
    try {
      const data = await firstValueFrom(this.http.get<ProjectSummary[]>(this.apiUrl));
      this.projects.set(data);
    } catch (err) {
      this.error.set('Error cargando proyectos');
    } finally {
      this.loading.set(false);
    }
  }

  async getById(id: number): Promise<void> {
    const data = await firstValueFrom(this.http.get<ProjectDetail>(`${this.apiUrl}/${id}`));
    this.selectedProject.set(data);
  }

  async create(command: CreateProjectCommand): Promise<number> {
    const id = await firstValueFrom(this.http.post<number>(this.apiUrl, command));
    await this.loadProjects();
    return id;
  }

  async update(id: number, command: UpdateProjectCommand): Promise<void> {
    await firstValueFrom(this.http.put(`${this.apiUrl}/${id}`, command));
    await this.loadProjects();
  }

  async delete(id: number): Promise<void> {
    await firstValueFrom(this.http.delete(`${this.apiUrl}/${id}`));
    this.projects.update(list => list.filter(p => p.id !== id));
  }
}
```
3.3 Component Design Pattern
Every page component follows this pattern:
```typescript
@Component({
  selector: 'app-project-list',
  standalone: true,
  imports: [CommonModule, RouterLink, CopyButtonComponent],
  templateUrl: './project-list.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProjectListComponent implements OnInit {
  private projectService = inject(ProjectService);
  private router = inject(Router);

  // Expose signals directly to template
  readonly projects = this.projectService.projects;
  readonly loading = this.projectService.loading;

  async ngOnInit() {
    await this.projectService.loadProjects();
  }

  async onDelete(id: number) {
    if (confirm('¿Eliminar este proyecto?')) {
      await this.projectService.delete(id);
    }
  }
}
```
---
4. API Contracts
4.1 Modulos
GET /api/modulos
Response: `200 OK`
```json
[
  {
    "id": 1,
    "nombre": "Gestión de Proyectos",
    "icono": "bi-kanban",
    "rutaBase": "proyectos",
    "color": "primary",
    "orden": 0,
    "activo": true,
    "createdAt": "2026-08-27T10:00:00Z",
    "updatedAt": "2026-08-27T10:00:00Z"
  }
]
```
POST /api/modulos
Request:
```json
{
  "nombre": "Presupuesto",
  "icono": "bi-cash-stack",
  "rutaBase": "presupuesto",
  "color": "success",
  "orden": 1
}
```
Response: `201 Created` → `Location: /api/modulos/2`
```json
2
```
---
4.2 Projects
GET /api/projects
Query: `?includeInactive=false`  
Response: `200 OK`
```json
[
  {
    "id": 1,
    "nombre": "Acme Corp",
    "descripcion": "Sistema de gestión empresarial",
    "activo": true,
    "createdAt": "2026-08-27T10:00:00Z",
    "updatedAt": "2026-08-27T10:00:00Z"
  }
]
```
GET /api/projects/{id}
Query: `?includeInactiveChildren=false`  
Response: `200 OK`
```json
{
  "id": 1,
  "nombre": "Acme Corp",
  "descripcion": "Sistema de gestión empresarial",
  "activo": true,
  "createdAt": "2026-08-27T10:00:00Z",
  "updatedAt": "2026-08-27T10:00:00Z",
  "basesDeDatos": [
    {
      "id": 1,
      "nombre": "SalesDb",
      "servidor": "SQLSRV01.corp.acme.local",
      "ambiente": "desarrollo",
      "activo": true
    }
  ],
  "applications": [
    {
      "id": 1,
      "nombre": "CRM",
      "tecnologiaFront": "Angular 6",
      "tecnologiaBack": ".NET 6",
      "orden": 0,
      "activo": true
    }
  ]
}
```
POST /api/projects
Request:
```json
{
  "nombre": "Globex Corp",
  "descripcion": "Nuevo cliente"
}
```
Response: `201 Created`
```json
2
```
PUT /api/projects/{id}
Request:
```json
{
  "id": 2,
  "nombre": "Globex Corp Updated",
  "descripcion": "Descripción actualizada"
}
```
Response: `204 No Content`
DELETE /api/projects/{id}
Response: `204 No Content`
---
4.3 Applications
GET /api/applications/{id}
Response: `200 OK`
```json
{
  "id": 1,
  "proyectoId": 1,
  "nombre": "CRM",
  "descripcion": "Customer Relationship Manager",
  "tecnologiaFront": "Angular 6",
  "tecnologiaBack": ".NET 6",
  "ramaDesarrollo": "origin/main",
  "applicationName": "Company.CRM",
  "tieneProyectoBD": "SI",
  "rutaLocal": "C:\Roger\Acme GIT\CRM",
  "rutaGit": "https://github.com/acme/crm",
  "comoSeLevanta": "Abrir con VS y VSCode",
  "notasCompilacion": "nvm use 14.16.0",
  "orden": 0,
  "activo": true,
  "createdAt": "2026-08-27T10:00:00Z",
  "updatedAt": "2026-08-27T10:00:00Z",
  "ambientes": [
    {
      "id": 1,
      "nombre": "UAT",
      "url": "https://mango-dune-00285950f.2.azurestaticapps.net",
      "esWebApi": false,
      "orden": 0
    }
  ],
  "reportes": [],
  "notas": [],
  "documentos": [],
  "fixDatas": [],
  "servicios": []
}
```
POST /api/applications
Request:
```json
{
  "proyectoId": 1,
  "nombre": "Report Viewer",
  "descripcion": "Visor de reportes",
  "tecnologiaFront": "Angular 18",
  "tecnologiaBack": null,
  "orden": 1
}
```
Response: `201 Created`
```json
3
```
---
4.4 Ambientes
GET /api/applications/{appId}/ambientes
Response: `200 OK`
```json
[
  {
    "id": 1,
    "aplicacionId": 1,
    "nombre": "UAT",
    "url": "https://example.com",
    "esWebApi": false,
    "notas": null,
    "orden": 0,
    "activo": true
  }
]
```
POST /api/applications/{appId}/ambientes
Request:
```json
{
  "nombre": "PROD",
  "url": "https://prod.example.com",
  "esWebApi": false,
  "notas": "Requiere VPN",
  "orden": 1
}
```
Response: `201 Created`
```json
2
```
---
4.5 Search
GET /api/search?term=eir
Response: `200 OK`
```json
{
  "projects": [
    { "id": 1, "nombre": "Acme Corp", "matchField": "applications.nombre" }
  ],
  "applications": [
    { "id": 1, "nombre": "CRM", "projectName": "Acme Corp" }
  ],
  "basesDeDatos": [],
  "reportes": [],
  "notas": [],
  "servicios": []
}
```
---
5. Screen Wireframes
5.1 Dashboard (Module Launcher)
```
┌─────────────────────────────────────────────────────────────┐
│  🏠 AdminPro                                    [⚙️]  │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│              ┌─────────────────────────┐                   │
│              │   📋 Gestión de         │                   │
│              │      Proyectos          │                   │
│              │                         │                   │
│              │   [Ir al módulo →]     │                   │
│              └─────────────────────────┘                   │
│                                                             │
│              ┌─────────────────────────┐                   │
│              │   💰 Presupuesto        │                   │
│              │      (Próximamente)     │                   │
│              │                         │                   │
│              │   [Ir al módulo →]     │                   │
│              └─────────────────────────┘                   │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```
Components: `DashboardComponent` → renders `ModuloCardComponent[]`  
Data: `moduloService.modulos()` (Signal)  
Action: Click card → `router.navigate(['/', modulo.rutaBase])`
---
5.2 Project List (within "proyectos" module)
```
┌─────────────────────────────────────────────────────────────┐
│  🏠 AdminPro    >    📋 Gestión de Proyectos          │
├──────────┬────────────────────────────────────────────────┤
│          │  [🔍 Buscar...]              [+ Nuevo Proyecto]│
│  📋 GdP  │                                                │
│  ─────── │  ┌──────────────────────────────────────────┐  │
│  Proyectos│  │ 🏢 Acme Corp                                   │  │
│  Servicios│  │ Sistema de gestión empresarial              │  │
│  Buscar  │  │ [Ver →]  [✏️]  [🗑️]                      │  │
│          │  └──────────────────────────────────────────┘  │
│          │  ┌──────────────────────────────────────────┐  │
│          │  │ 🏢 Globex Corp                                  │  │
│          │  │ Nuevo cliente                             │  │
│          │  │ [Ver →]  [✏️]  [🗑️]                      │  │
│          │  └──────────────────────────────────────────┘  │
│          │                                                │
│          │  [← Volver al Dashboard]                     │
│          │                                                │
└──────────┴────────────────────────────────────────────────┘
```
Route: `/proyectos/proyectos`  
Components: `ProyectosLayoutComponent` (sidebar) → `ProjectListComponent`  
Data: `projectService.projects()`  
Actions:
Search: filters local list (client-side, small dataset)
New: opens `ProjectFormComponent` in modal or navigates to `/proyectos/proyectos/nuevo`
View: `/proyectos/proyectos/:id`
Edit: `/proyectos/proyectos/:id/editar`
Delete: confirmation modal → `projectService.delete(id)`
---
5.3 Project Detail
```
┌─────────────────────────────────────────────────────────────┐
│  🏠 > 📋 GdP > 🏢 Acme Corp                                      │
├──────────┬────────────────────────────────────────────────┤
│          │  🏢 Acme Corp                                          │
│  📋 GdP  │  Sistema de gestión empresarial                     │
│  ─────── │                                                   │
│  Proyectos│  [✏️ Editar]  [🗑️ Eliminar]                      │
│  Servicios│                                                   │
│  Buscar  │  ┌─────────────────────────────────────────────┐  │
│          │  │ 📊 BASES DE DATOS (3)                       │  │
│          │  │ ───────────────────────────────────────────── │  │
│          │  │ • SalesDb     | desarrollo | WLV-DFF...   │  │
│          │  │ • SalesDb_Test  | uat       | WLV-DFF...   │  │
│          │  │ • AuthDb    | -          | -            │  │
│          │  └─────────────────────────────────────────────┘  │
│          │                                                   │
│          │  ┌─────────────────────────────────────────────┐  │
│          │  │ 🖥️ APLICACIONES (5)                         │  │
│          │  │ ───────────────────────────────────────────── │  │
│          │  │ #  Nombre         Front        Back         │  │
│          │  │ 1  CRM            Angular 6    .NET 6       │  │
│          │  │ 2  Report Viewer  Angular 18   -          │  │
│          │  │ 3  InventoryWeb    Angular 4.4    -          │  │
│          │  │ ...                                         │  │
│          │  │ [+ Nueva Aplicación]                        │  │
│          │  └─────────────────────────────────────────────┘  │
│          │                                                   │
└──────────┴───────────────────────────────────────────────────┘
```
Route: `/proyectos/proyectos/:id`  
Components: `ProjectDetailComponent`  
Data: `projectService.selectedProject()`  
Child Actions:
Click Application row → `/proyectos/aplicaciones/:appId`
New Application → opens `ApplicationFormComponent`
---
5.4 Application Detail (Master View)
```
┌─────────────────────────────────────────────────────────────┐
│  🏠 > 📋 GdP > 🏢 Acme Corp > 🖥️ CRM                             │
├──────────┬────────────────────────────────────────────────┤
│          │  🖥️ CRM                                          │
│  📋 GdP  │  Customer Relationship Manager                    │
│  ─────── │  Angular 6 | .NET 6                               │
│  Proyectos│                                                   │
│  Servicios│  [✏️ Editar]  [🗑️ Eliminar]  [📋 Copiar Info]   │
│  Buscar  │                                                   │
│          │  ┌─────────────────────────────────────────────┐  │
│          │  │ 📁 RUTAS                                      │  │
│          │  │ Local:  C:\Roger\Acme GIT\CRM  [📋]          │  │
│          │  │ Git:    https://github.com/acme/crm  [📋]     │  │
│          │  │ Levantar: Abrir con VS y VSCode               │  │
│          │  └─────────────────────────────────────────────┘  │
│          │                                                   │
│          │  ┌─────────────────────────────────────────────┐  │
│          │  │ 🌐 AMBIENTES                                  │  │
│          │  │ ───────────────────────────────────────────── │  │
│          │  │ #  Nombre   URL                    Tipo     │  │
│          │  │ 1  UAT      https://...            Web    │  │
│          │  │ 2  PROD     https://...            Web    │  │
│          │  │ [+ Agregar]  [↕️ Reordenar]                 │  │
│          │  └─────────────────────────────────────────────┘  │
│          │                                                   │
│          │  ┌─────────────────────────────────────────────┐  │
│          │  │ 📊 REPORTES (0)                               │  │
│          │  │ [+ Agregar Reporte]                           │  │
│          │  └─────────────────────────────────────────────┘  │
│          │                                                   │
│          │  ┌─────────────────────────────────────────────┐  │
│          │  │ 📝 NOTAS (2)                                  │  │
│          │  │ ───────────────────────────────────────────── │  │
│          │  │ ▶ nvm use 14.16.0                             │  │
│          │  │   Usar Node 14.16.0 para compilar...          │  │
│          │  │ ▶ Borrar bin/obj                              │  │
│          │  │   Antes de compilar, borrar...                │  │
│          │  │ [+ Agregar Nota]                              │  │
│          │  └─────────────────────────────────────────────┘  │
│          │                                                   │
│          │  ┌─────────────────────────────────────────────┐  │
│          │  │ 📎 DOCUMENTOS (1)                             │  │
│          │  │ • Manual de Usuario  [manual]  [🔗 Abrir]     │  │
│          │  │ [+ Agregar Documento]                         │  │
│          │  └─────────────────────────────────────────────┘  │
│          │                                                   │
│          │  ┌─────────────────────────────────────────────┐  │
│          │  │ 🔧 FIX DATAS (0)                              │  │
│          │  │ [+ Agregar FixData]                           │  │
│          │  └─────────────────────────────────────────────┘  │
│          │                                                   │
│          │  ┌─────────────────────────────────────────────┐  │
│          │  │ 🔗 SERVICIOS VINCULADOS (1)                   │  │
│          │  │ • Security API UAT  [Seguridad]  [🔗]  [✕]    │  │
│          │  │ [+ Vincular Servicio]                         │  │
│          │  └─────────────────────────────────────────────┘  │
│          │                                                   │
└──────────┴───────────────────────────────────────────────────┘
```
Route: `/proyectos/aplicaciones/:id`  
Components: `ApplicationDetailComponent`  
Data: `applicationService.selectedApplication()`  
Sections: Each section is a collapsible accordion panel (Bootstrap).
Per-section actions:
Ambientes: Add (modal form), Edit inline, Delete, Reorder (drag-drop)
Reportes: Add (modal), Edit, Delete
Notas: Add (modal), Edit, Delete, Expand/Collapse
Documentos: Add (modal), Open link (new tab), Delete
FixDatas: Add (modal with textarea for script), Edit, Delete, Copy script
Servicios: Link (modal with service picker), Unlink, Open link
---
5.5 Application Form (Create/Edit)
```
┌─────────────────────────────────────────────────────────────┐
│  🏠 > 📋 GdP > 🏢 Acme Corp > 🖥️ [Nueva Aplicación / Editar CRM]│
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  * Nombre:        [CRM                              ]      │
│  Descripción:     [Customer Relationship Manager      ]      │
│  Tecnología Front:[Angular 6                          ]      │
│  Tecnología Back: [.NET 6                             ]      │
│  Rama Desarrollo: [origin/main                        ]      │
│  App Name:        [Company.CRM                   ]      │
│  Tiene Proy. BD:  [SI                                 ]      │
│  Ruta Local:      [C:\Roger\Acme GIT\CRM            ] 📋   │
│  Ruta Git:        [https://github.com/acme/crm       ] 📋   │
│  Cómo se levanta: [Abrir con VS y VSCode            ]      │
│  Notas Compilación:[nvm use 14.16.0                  ]      │
│                    [                                  ]      │
│  Orden:           [0                                  ]      │
│                                                             │
│  [💾 Guardar]  [❌ Cancelar]                                │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```
Route: `/proyectos/aplicaciones/nuevo?proyectoId=1` or `/proyectos/aplicaciones/:id/editar`  
Component: `ApplicationFormComponent` (reactive form)  
Validation: Mirrors FluentValidation rules (required, maxLength, URL format where applicable).
---
5.6 Service Catalog
```
┌─────────────────────────────────────────────────────────────┐
│  🏠 > 📋 GdP > 🔗 Servicios                                 │
├──────────┬────────────────────────────────────────────────┤
│          │  [🔍 Buscar servicio...]    [+ Nuevo Servicio]  │
│  📋 GdP  │                                                   │
│  ─────── │  ┌──────────────────────────────────────────┐    │
│  Proyectos│  │ 🔗 Security API UAT                       │    │
│  Servicios│  │ Tipo: Seguridad | Ambiente: UAT           │    │
│  Buscar  │  │ URL: https://...swagger/index.html  [🔗]  │    │
│          │  │ [✏️]  [🗑️]                                │    │
│          │  └──────────────────────────────────────────┘    │
│          │  ┌──────────────────────────────────────────┐    │
│          │  │ 🔗 Security API PROD                      │    │
│          │  │ Tipo: Seguridad | Ambiente: PROD          │    │
│          │  │ URL: https://...swagger/index.html  [🔗]  │    │
│          │  │ [✏️]  [🗑️]                                │    │
│          │  └──────────────────────────────────────────┘    │
│          │                                                   │
└──────────┴───────────────────────────────────────────────────┘
```
Route: `/proyectos/servicios`  
Component: `ServiceListComponent`  
Data: `serviceCatalogService.servicios()`
---
5.7 Global Search Results
```
┌─────────────────────────────────────────────────────────────┐
│  🏠 > 🔍 Resultados de búsqueda: "eir"                     │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│  🏢 PROYECTOS (1)                                           │
│  • Acme Corp  (coincide en aplicaciones)  [Ver →]               │
│                                                             │
│  🖥️ APLICACIONES (2)                                        │
│  • CRM (Acme Corp)                        [Ver →]               │
│  • CRM Mobile (Acme Corp)                 [Ver →]               │
│                                                             │
│  📊 REPORTES (0)                                            │
│                                                             │
│  📝 NOTAS (1)                                               │
│  • "Configurar CRM en PROD" (CRM)    [Ver →]               │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```
Route: `/search?q=eir`  
Component: `SearchResultsComponent`  
Data: `searchService.results()`
---
6. Component Hierarchy (Angular)
```
AppComponent
└── RouterOutlet
    ├── / (DashboardComponent)
    │   └── ModuloCardComponent[]
    │
    ├── /proyectos (ProyectosLayoutComponent)
    │   ├── SidebarComponent
    │   └── RouterOutlet
    │       ├── /proyectos (ProjectListComponent)
    │       │   ├── SearchBarComponent
    │       │   └── ProjectCardComponent[]
    │       ├── /proyectos/:id (ProjectDetailComponent)
    │       │   ├── DatabaseListComponent
    │       │   └── ApplicationTableComponent
    │       ├── /proyectos/:id/editar (ProjectFormComponent)
    │       ├── /aplicaciones/:id (ApplicationDetailComponent)
    │       │   ├── BreadcrumbsComponent
    │       │   ├── EnvironmentListComponent
    │       │   │   └── SortableListDirective
    │       │   ├── ReportListComponent
    │       │   ├── NoteListComponent
    │       │   ├── DocumentListComponent
    │       │   ├── FixDataListComponent
    │       │   └── ServiceLinkerComponent
    │       ├── /aplicaciones/nuevo (ApplicationFormComponent)
    │       ├── /servicios (ServiceListComponent)
    │       │   └── ServiceCardComponent[]
    │       └── /buscar (SearchResultsComponent)
    │
    └── /presupuesto (PresupuestoLayoutComponent)
        └── ... (future)

Shared Components (used everywhere):
├── CopyButtonComponent
├── ConfirmModalComponent
├── EmptyStateComponent
├── ToastComponent
├── BreadcrumbsComponent
└── SearchBarComponent
```
---
7. State Management Flow
7.1 Backend → Frontend Data Flow
```
┌─────────────┐     HTTP GET      ┌─────────────┐
│   API       │ ────────────────→ │   Service   │
│  Controller │                   │  (Angular)  │
└─────────────┘                   └──────┬────┘
                                           │ signal.set()
                                           ▼
                                    ┌─────────────┐
                                    │   Signal    │
                                    │   Store     │
                                    └──────┬────┘
                                           │ signal()
                                           ▼
                                    ┌─────────────┐
                                    │  Component  │
                                    │  Template   │
                                    │  {{ data }} │
                                    └─────────────┘
```
7.2 Write Operation Flow
```
User clicks "Save"
    │
    ▼
Component calls service.create(command)
    │
    ▼
Service sends HTTP POST
    │
    ▼
API receives → MediatR sends command
    │
    ▼
ValidationBehavior runs FluentValidation
    │
    ▼
TransactionBehavior opens EF transaction
    │
    ▼
Handler executes → SaveChanges → Commit
    │
    ▼
Service receives 201 → reloads list signal
    │
    ▼
UI updates automatically (OnPush + Signal)
```
---
8. Key Technical Decisions
Decision	Chosen	Rejected	Rationale
Architecture	Clean + CQRS	N-Layer, DDD	Separation of concerns, testable, pipeline behaviors for cross-cutting
ORM	EF Core 10	Dapper	Rapid CRUD development, migrations, change tracking
Repository	None (DbContext directly)	Generic Repository	EF Core is already a UoW+Repository. Extra layer adds no value for this scope.
Mapping	Manual	AutoMapper	Explicit, no hidden complexity, easy to debug
Frontend State	Signals	NgRx, RxJS Subjects	Fine-grained reactivity, less boilerplate, built into Angular 22
HTTP Client	`HttpClient` + `firstValueFrom`	Axios, Fetch	Native Angular, interceptors, type-safe
Styling	Bootstrap 5	Tailwind, Material	Familiar, fast to prototype, good component library
Forms	Reactive Forms	Template-driven	Complex validation, dynamic fields, type-safe
Drag & Drop	`@angular/cdk/drag-drop`	Custom implementation	Official, accessible, well-tested
Testing	Cypress E2E	Jest unit tests	For a CRUD app, E2E gives more confidence than unit tests
---
9. Database Migration Strategy
9.1 Initial Migration
```bash
dotnet ef migrations add InitialCreate --project src/AdminPro.Infrastructure --startup-project src/AdminPro.Api
dotnet ef database update --project src/AdminPro.Infrastructure --startup-project src/AdminPro.Api
```
9.2 Seed Data
In `AppDbContext.OnModelCreating` or a separate `DbSeeder`:
```csharp
modelBuilder.Entity<Modulo>().HasData(new Modulo
{
    Id = 1,
    Nombre = "Gestión de Proyectos",
    Icono = "bi-kanban",
    RutaBase = "proyectos",
    Color = "primary",
    Orden = 0,
    Activo = true,
    CreatedAt = new DateTime(2026, 8, 27),
    UpdatedAt = new DateTime(2026, 8, 27)
});
```
9.3 Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\mssqllocaldb;Database=AdminPro;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```
---
10. Error Handling Strategy
10.1 Backend
Global Exception Handler Middleware catches all unhandled exceptions and returns:
```json
{
  "error": "ValidationError",
  "message": "Uno o más errores de validación ocurrieron.",
  "details": [
    { "field": "Nombre", "error": "El nombre es obligatorio." },
    { "field": "Nombre", "error": "Ya existe un proyecto con ese nombre." }
  ]
}
```
Exception types mapped to HTTP status:
Exception Type	HTTP Status	Example
`ValidationException`	400 Bad Request	Duplicate name, invalid URL
`NotFoundException`	404 Not Found	Entity doesn't exist or is inactive
`DomainException`	409 Conflict	Business rule violation
Unhandled	500 Internal Server Error	Unexpected error
10.2 Frontend
HTTP Interceptor catches errors and displays a Bootstrap toast:
```typescript
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError(err => {
        if (err.status === 400) {
          // Show validation errors in form
          toastService.show('Errores de validación', 'warning');
        } else if (err.status === 404) {
          toastService.show('Recurso no encontrado', 'danger');
          router.navigate(['/']);
        } else {
          toastService.show('Error inesperado', 'danger');
        }
        return throwError(() => err);
      })
    );
  }
}
```
---
11. Cypress E2E Test Plan
11.1 Test: Dashboard Navigation
```javascript
describe('Dashboard', () => {
  it('should display modules and navigate to proyectos', () => {
    cy.visit('/');
    cy.contains('Gestión de Proyectos').should('be.visible');
    cy.get('[data-testid="modulo-card"]').first().click();
    cy.url().should('include', '/proyectos');
    cy.contains('Proyectos').should('be.visible');
  });
});
```
11.2 Test: CRUD Project
```javascript
describe('Projects', () => {
  it('should create, read, update, and delete a project', () => {
    cy.visit('/proyectos/proyectos');
    cy.get('[data-testid="btn-nuevo"]').click();
    cy.get('[data-testid="input-nombre"]').type('Test Project');
    cy.get('[data-testid="btn-guardar"]').click();
    cy.contains('Test Project').should('be.visible');

    cy.get('[data-testid="btn-editar"]').first().click();
    cy.get('[data-testid="input-nombre"]').clear().type('Test Updated');
    cy.get('[data-testid="btn-guardar"]').click();
    cy.contains('Test Updated').should('be.visible');

    cy.get('[data-testid="btn-eliminar"]').first().click();
    cy.get('[data-testid="btn-confirmar"]').click();
    cy.contains('Test Updated').should('not.exist');
  });
});
```
11.3 Test: Application Detail with Children
```javascript
describe('Application Detail', () => {
  it('should display application with environments and allow adding notes', () => {
    cy.visit('/proyectos/aplicaciones/1');
    cy.contains('CRM').should('be.visible');
    cy.get('[data-testid="panel-ambientes"]').click();
    cy.get('[data-testid="ambiente-row"]').should('have.length.at.least', 1);

    cy.get('[data-testid="panel-notas"]').click();
    cy.get('[data-testid="btn-agregar-nota"]').click();
    cy.get('[data-testid="input-titulo"]').type('Nota de prueba');
    cy.get('[data-testid="input-descripcion"]').type('Contenido de la nota');
    cy.get('[data-testid="btn-guardar-nota"]').click();
    cy.contains('Nota de prueba').should('be.visible');
  });
});
```
---
12. Development Environment Setup
12.1 Prerequisites
.NET 10 SDK
Node.js 22+ (LTS)
SQL Server 2022 LocalDB or Developer Edition
VS Code or Visual Studio 2022
12.2 Backend Startup
```bash
cd src/AdminPro.Api
dotnet restore
dotnet ef database update
dotnet run
# API available at https://localhost:5001
```
12.3 Frontend Startup
```bash
cd adminpro-ui
npm install
ng serve
# App available at http://localhost:4200
```
12.4 Proxy Configuration
`proxy.conf.json` for Angular dev server:
```json
{
  "/api": {
    "target": "https://localhost:5001",
    "secure": false,
    "changeOrigin": true
  }
}
```
---
Document version: 1.0  
OpenSpec-compatible format — companion to business-rules.md v3.0