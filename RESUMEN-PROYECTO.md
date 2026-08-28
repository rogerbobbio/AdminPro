# Resumen del Proyecto — ProyectInit

## ¿Qué es este proyecto?

**ProyectInit** es una plantilla base (template) de **especificaciones y reglas** (no de código) para arrancar proyectos full-stack. La idea es que Claude Code (u otro agente de IA) lea estas reglas y, siguiendo una metodología de **Spec-Driven Development (OpenSpec)**, genere el código real del backend y frontend paso a paso, con TDD — para cualquier dominio de negocio que se defina en cada proyecto concreto que nazca a partir de esta base.

- **Estado actual:** solo existe el "andamiaje" documental (reglas, arquitectura, agentes, skills). Las carpetas `backend/` y `frontend/` están vacías, no hay código, no hay tests, no hay CI/CD todavía.
- **Metodología:** OpenSpec — se propone un cambio (`/opsx:propose`), se revisa, se implementa tarea por tarea con TDD (`/opsx:apply`) y se archiva mergeando la spec final (`/opsx:archive`).
- **Arquitectura:** Domain-Driven Design (DDD) de 4 capas: `Domain ← Application ← Infrastructure ← API`. Este patrón es independiente del stack tecnológico elegido.

## Stack tecnológico

El stack por defecto documentado en `docs/standards/` y en las ADRs de `docs/architecture/` es:

| Área | Tecnologías (default actual) |
|---|---|
| **Backend** | .NET 10 · C# 14 · ASP.NET Core 10 · EF Core 10 · SQL Server 2022 · MediatR (CQRS) · FluentValidation · JWT + Refresh Tokens · Serilog |
| **Frontend** | Angular 22 · TypeScript 5.6 · Standalone Components + OnPush + Signals · Bootstrap 5 · Cypress (E2E) |
| **Testing** | xUnit + FluentAssertions + NSubstitute + Testcontainers (backend) · Karma/Jasmine + Cypress (frontend) |
| **Metodología** | OpenSpec (Spec-Driven Development) |

> ⚠️ **Este stack es reemplazable.** Los principios de arquitectura (DDD 4 capas, CQRS, Result pattern, TDD, spec-driven development) son agnósticos de tecnología. Si un proyecto futuro necesita otro lenguaje/framework (p. ej. Node/NestJS, Java/Spring, Python/FastAPI, React/Vue en vez de Angular), lo que hay que actualizar es un conjunto acotado de archivos — ver "Cómo adaptar el stack" más abajo — no la metodología ni los agentes.

### Cómo adaptar el stack en un proyecto nuevo

1. **ADRs** (`docs/architecture/adr-002-cqrs-mediator.md`, `adr-003-sqlserver-efcore.md`): reemplazar por las decisiones de persistencia/mensajería del nuevo stack (adr-001 de capas DDD es agnóstico, no requiere cambios).
2. **Estándares de código** (`docs/standards/backend-standards.md`, `frontend-standards.md`): reescribir con las convenciones concretas del nuevo lenguaje/framework.
3. **Skills técnicas** (`ai-specs/skills/ddd-layered-implementation/`, `ai-specs/skills/angular-feature-scaffold/`): actualizar los snippets de ejemplo al nuevo stack.
4. **Agentes** (`ai-specs/agents/backend-developer.md`, `frontend-developer.md`): actualizar la sección de expertise técnico; el rol y las reglas de trabajo (leer sesión previa, documentar en `.claude/doc/`, no ejecutar builds) se mantienen igual.
5. **`docs/base-standards.md`**: actualizar las referencias cruzadas a la nueva stack en la tabla resumen.

## Cómo adaptar el dominio de negocio en un proyecto nuevo

Los ejemplos de código en las skills (`ddd-layered-implementation`, `angular-feature-scaffold`, `commit`) usan una entidad neutra (`Product`) solo como ilustración didáctica. Para un proyecto concreto:

1. Completar `docs/business-rules.md` con las reglas de negocio reales del dominio.
2. Completar `docs/design/DESIGN.md` con las decisiones de diseño visual/UX reales.
3. Reemplazar el ejemplo `Product` en las skills por el bounded context real del proyecto (opcional, solo mejora la claridad de los ejemplos).
4. Actualizar los scopes de `ai-specs/skills/commit/SKILL.md` con los bounded contexts reales.

## Estructura de carpetas — ¿para qué sirve cada una?

```
ProyectInit/
├── CLAUDE.md                   ← Punto de entrada para Claude Code
├── README.md                   ← Presentación general del proyecto
├── RESUMEN-PROYECTO.md         ← Este archivo
├── .gitignore
│
├── docs/                       ← Las REGLAS del proyecto (leer primero)
├── ai-specs/                   ← Instrucciones para la IA (agentes y skills)
├── .claude/                    ← Configuración de Claude Code
├── backend/                    ← Código del backend — vacío por ahora
├── frontend/                   ← Código del frontend — vacío por ahora
└── openspec/                   ← Cambios propuestos/aplicados (aún no creada)
```

### 📁 `docs/` — Las reglas del proyecto

Es la fuente de verdad de **cómo** se debe construir el sistema.

| Archivo/carpeta | Para qué sirve |
|---|---|
| `base-standards.md` | **Guía maestra**. Punto de partida obligatorio: orden de lectura, principios core, arquitectura, agentes, skills, checklist de inicio. |
| `business-rules.md` | Reglas de negocio del dominio del proyecto concreto. *Vacío en la plantilla base, se completa por proyecto.* |
| `design/DESIGN.md` | Decisiones de diseño visual/UX del proyecto concreto. *Vacío en la plantilla base, se completa por proyecto.* |
| `architecture/adr-001-ddd-layers.md` | ADR (Architecture Decision Record): define oficialmente las 4 capas DDD y la regla de dependencias. Agnóstico de stack. |
| `architecture/adr-002-cqrs-mediator.md` | ADR: por qué se usa CQRS + MediatR para los casos de uso (stack .NET por defecto). |
| `architecture/adr-003-sqlserver-efcore.md` | ADR: por qué SQL Server + EF Core como persistencia (stack .NET por defecto). |
| `standards/backend-standards.md` | Convenciones concretas de código backend (stack .NET por defecto). |
| `standards/frontend-standards.md` | Convenciones concretas de código frontend (stack Angular por defecto). |
| `standards/documentation-standards.md` | Cómo se debe escribir y formatear la documentación del proyecto. |
| `standards/openspec-tasks-mandatory-steps.md` | Estructura obligatoria que debe tener cada `tasks.md` de OpenSpec. |

### 📁 `ai-specs/` — Instrucciones para la IA

Es la carpeta **canónica** (fuente original) que le dice a Claude *quién* debe actuar y *cómo* en cada situación.

| Subcarpeta | Para qué sirve |
|---|---|
| `agents/backend-developer.md` | Perfil del agente especializado en backend (stack .NET + DDD por defecto). |
| `agents/frontend-developer.md` | Perfil del agente especializado en frontend (stack Angular por defecto). |
| `agents/product-strategy-analyst.md` | Perfil del agente que convierte ideas vagas en specs de OpenSpec (proposal, tasks, design). Agnóstico de stack. |
| `skills/ddd-layered-implementation/` | Cómo implementar código respetando las 4 capas DDD (dónde va cada cosa, anti-patrones). |
| `skills/tasks-driven-tdd/` | Cómo ejecutar las tareas de OpenSpec siguiendo TDD estricto (test → código → refactor). Agnóstico de stack. |
| `skills/commit/` | Formato oficial de Conventional Commits (tipos, scopes, ejemplos). |
| `skills/code-review/` | Checklist de revisión de código antes de mergear (backend, frontend, general). |
| `skills/security-audit/` | Checklist de seguridad OWASP adaptado al stack por defecto. |
| `skills/angular-feature-scaffold/` | Cómo scaffoldear un feature nuevo en Angular siguiendo la estructura del proyecto. |
| `skills/artifact-design/` | Guía de diseño visual para páginas/dashboards que se generen como artifacts. |
| `scripts/code_review.sh` | Script de apoyo para el proceso de code review. |

### 📁 `.claude/` — Configuración de Claude Code

- `settings.json`: permisos de herramientas (permite `dotnet`, `npm`, `npx`, `git`, `openspec`; bloquea comandos peligrosos como `rm -rf`, `curl | bash`, `eval`).
- Carpetas `sessions/` y `doc/` (mencionadas en las reglas para guardar contexto de features y documentación generada por los agentes) **todavía no existen** — se crean cuando se empieza a trabajar en una feature real.

### 📁 `backend/` — Código del backend

Vacía por ahora. Cuando se implemente con el stack por defecto, seguirá la estructura:
```
backend/src/
├── Domain/          ← Entidades, Value Objects, eventos de dominio (sin frameworks)
├── Application/     ← Casos de uso (Commands/Queries con MediatR), validadores, DTOs
├── Infrastructure/  ← EF Core, JWT, repositorios concretos
└── API/             ← Controllers, Middleware, Program.cs
```

### 📁 `frontend/` — Código del frontend

Vacía por ahora. Cuando se implemente con el stack por defecto, seguirá la estructura:
```
frontend/src/app/features/<bounded-context>/   ← Un feature = una carpeta, con lazy loading
```

### 📁 `openspec/` (aún no creada)

Se crea al correr `openspec init`. Contendrá:
- `changes/` → propuestas de cambio en curso (proposal, design, tasks, specs).
- `specs/` → especificaciones ya aprobadas y archivadas (mergeadas).

## Archivos raíz

| Archivo | Para qué sirve |
|---|---|
| `CLAUDE.md` | Punto de entrada corto que redirige a `docs/base-standards.md`. Es lo primero que lee Claude Code al abrir el proyecto. |
| `README.md` | Presentación general del proyecto para cualquier persona (humana) que lo abra. |
| `.gitignore` | Ignora `bin/`, `obj/`, `node_modules/`, `dist/`, secretos (`.env*`), logs — pero **sí** versiona `openspec/`. |

## Próximos pasos al arrancar un proyecto nuevo desde esta base

1. (Opcional) Adaptar el stack tecnológico si el proyecto no usa .NET/Angular — ver "Cómo adaptar el stack".
2. Correr `openspec init` para crear la carpeta `openspec/`.
3. Completar `docs/business-rules.md` y `docs/design/DESIGN.md` con el dominio real del proyecto.
4. Proponer el primer change set con `/opsx:propose "<feature>"` para empezar a generar código real.

---
*Actualizado el 2026-08-10 — versión genérica reutilizable como base de proyectos.*
