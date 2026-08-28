---
name: ddd-layered-implementation
description: Implement features following the 4 DDD layers strictly (Domain → Application → Infrastructure → API).
---

# DDD Layered Implementation

Layer definition and dependency rule: see [ADR-001](../../../docs/architecture/adr-001-ddd-layers.md).

## Where each thing goes

| If you need... | Goes in... |
|-----------------|------------|
| Entity with identity | `Domain/Entities/` |
| Object without identity (Email, Money, Address) | `Domain/ValueObjects/` |
| Domain event | `Domain/Events/` |
| Repository interface | `Domain/Interfaces/` |
| EF Core implementation | `Infrastructure/Persistence/Repositories/` |
| Use case / orchestration | `Application/Commands/` or `Application/Queries/` |
| Input validation | `Application/Validators/` |
| DTO / external contract | `Application/DTOs/` |
| REST controller | `API/Controllers/` |
| Middleware, filters | `API/Middleware/`, `API/Filters/` |

## Anti-patterns (DO NOT)

- ❌ `using Microsoft.EntityFrameworkCore;` in `Domain/`
- ❌ `[HttpGet]` in `Application/`
- ❌ `DbContext` injected in a Controller
- ❌ Business logic in a Controller
- ❌ Concrete repository in `Application/` (use the Domain interface)
- ❌ `DbSet<>` direct in a Service (use repository)

## Typical Snippet — Vertical Slice

```csharp
// Domain/Entities/Product.cs
public class Product : AggregateRoot<Guid>
{
    public string Name { get; private set; } = default!;
    public DateTime CreatedAt { get; private set; }
}

// Application/Products/Commands/CreateProduct/CreateProductCommand.cs
public record CreateProductCommand(string Name) : IRequest<ErrorOr<Guid>>;

// Application/Products/Commands/CreateProduct/CreateProductCommandHandler.cs
public class CreateProductCommandHandler(
    IProductRepository repo,
    IUnitOfWork uow,
    ILogger<CreateProductCommandHandler> log)
    : IRequestHandler<CreateProductCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> Handle(CreateProductCommand req, CancellationToken ct) { ... }
}

// Infrastructure/Persistence/Repositories/ProductRepository.cs
public class ProductRepository(AppDbContext db) : IProductRepository { ... }

// API/Controllers/ProductsController.cs
[ApiController, Route("api/v1/products")]
public class ProductsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductRequest req, CancellationToken ct) { ... }
}
```
