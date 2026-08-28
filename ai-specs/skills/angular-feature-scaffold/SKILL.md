---
name: angular-feature-scaffold
description: Create Angular 22 standalone features with Signals, lazy loading, and Bootstrap.
---

# Angular 22 Feature Scaffold

## Structure per feature

```
frontend/src/app/features/<bounded-context>/
├── pages/                    # Routed components (entry points)
│   └── <feature>-list/
│       ├── <feature>-list.component.ts
│       ├── <feature>-list.component.html
│       ├── <feature>-list.component.scss
│       └── <feature>-list.component.spec.ts
├── components/                # Internal reusable components
├── services/                  # HTTP + local state
├── models/                    # Interfaces/DTOs
├── store/                     # NgRx (if applicable)
└── <bounded-context>.routes.ts  # Lazy routes
```

## Conventions

- Standalone components (no NgModules)
- `ChangeDetectionStrategy.OnPush`
- `inject()` instead of constructor DI
- Signals: `signal()`, `computed()`, `effect()`, `linkedSignal()`, `resource()`
- HTTP via `HttpClient` with generic types
- **Bootstrap classes**, no hardcoded CSS

## Typical Snippet

```typescript
// features/products/pages/product-list/product-list.component.ts
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ProductService } from '../../services/product.service';
import { Product } from '../../models/product.model';

@Component({
  selector: 'app-product-list',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss',
})
export class ProductListComponent {
  private readonly productService = inject(ProductService);
  readonly products = signal<Product[]>([]);
  readonly loading = signal(false);

  constructor() { this.load(); }

  async load(): Promise<void> {
    this.loading.set(true);
    try {
      this.products.set(await firstValueFrom(this.productService.getAll()));
    } finally {
      this.loading.set(false);
    }
  }
}
```

```html
<!-- product-list.component.html -->
<div class="container py-4">
  <h1 class="h3 mb-4">Products</h1>
  @if (loading()) {
    <div class="d-flex justify-content-center">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
    </div>
  } @else {
    <div class="row g-3">
      @for (p of products(); track p.id) {
        <div class="col-12 col-md-6 col-lg-4">
          <article class="card h-100 shadow-sm">
            <div class="card-body">
              <h2 class="h5 card-title">{{ p.name }}</h2>
              <p class="card-text text-muted small">{{ p.createdAt | date }}</p>
            </div>
          </article>
        </div>
      } @empty {
        <p class="text-muted">No products available.</p>
      }
    </div>
  }
</div>
```
