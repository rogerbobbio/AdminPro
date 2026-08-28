# Frontend Standards (Angular 22 + Bootstrap 5)

## Stack

- Angular 22 with Signals, standalone components, control flow `@if/@for/@switch`
- TypeScript strict
- Bootstrap 5 (classes, no hardcoded custom CSS)
- RxJS for streams, Signals for local state
- NgRx Store/Effects when global state justifies it
- Karma + Jasmine (unit) + Playwright (E2E)

## Structure

```
frontend/src/app/
├── core/                # Singletons, global providers
│   ├── config/
│   ├── constants/
│   ├── guards/
│   ├── interceptors/    # auth, error, logging
│   ├── services/        # cross-feature
│   └── models/
├── shared/              # Reusable
│   ├── components/
│   ├── directives/
│   ├── pipes/
│   ├── validators/
│   └── layouts/
└── features/            # ⭐ One folder per bounded context
    └── <bounded-context>/
        ├── pages/       # Routed components
        ├── components/
        ├── services/
        ├── models/
        ├── store/       # NgRx (optional)
        └── <bc>.routes.ts
```

## Conventions

- Standalone components (no NgModules)
- `ChangeDetectionStrategy.OnPush` always
- `inject()` preferred over constructor DI
- Signals: `signal()`, `computed()`, `effect()`, `linkedSignal()`, `resource()`
- RxJS for HTTP, async events, streams
- `track` mandatory in `@for`
- **No `any`**, no `unknown` without narrowing
- Selectors `app-<feature>-<element>` (kebab-case)

## Bootstrap 5 — Correct Usage

- ✅ Utilities: `d-flex`, `gap-3`, `col-md-6`, `p-4`
- ✅ Components: `btn`, `card`, `modal`, `navbar`, `alert`, `form-control`
- ❌ Do not write custom CSS for what Bootstrap already provides
- ❌ No inline `style="..."`

## Routing with Lazy Loading

```typescript
// app.routes.ts
export const routes: Routes = [
  {
    path: 'candidates',
    loadChildren: () => import('./features/candidates/candidates.routes').then(m => m.CANDIDATES_ROUTES),
  },
];
```

## HTTP

- One service per resource: `CandidateService`, `AuthService`
- Generic types: `HttpClient.get<Candidate[]>(...)`
- Errors via centralized interceptor
- Tokens via interceptor that adds `Authorization: Bearer ...`

## Testing

- Unit: `ComponentFixture` + `TestBed`, mock with jasmine spies
- E2E: Playwright with `data-testid` selectors
- Minimum coverage: **70%** statements

## Accessibility

- ARIA roles on interactive components
- Labels on all inputs
- Keyboard navigation
- WCAG AA contrast
