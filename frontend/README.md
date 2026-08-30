# AdminPro Frontend

Angular 22 frontend for AdminPro (standalone components, Signals, OnPush, Bootstrap 5). See `openspec/changes/frontend-dashboard/` for the proposal/design/specs behind this scaffold and the Dashboard screen, and `docs/design/DESIGN.md` at the repo root for the overall frontend architecture.

## Prerequisites

- Node.js ≥ 22.22.3 (Angular CLI 22 requirement) — if you're on nvm-windows, `nvm install 22.22.3 && nvm use 22.22.3` (the latter needs an elevated/Administrator shell)
- The backend running locally (see `backend/README.md`) — the dev server proxies `/api/*` to it

## Development server

```bash
npm install
npm start
```

This runs `ng serve`, which uses `proxy.conf.json` to forward `/api/*` requests to the backend at `https://localhost:7293` (the port from `backend/src/AdminPro.Api/Properties/launchSettings.json`'s `https` profile — update the proxy target if you run the backend on a different port). Open `http://localhost:4200`.

## Building

```bash
npm run build
```

## Unit tests

```bash
npm test
```

Runs the Vitest-based unit tests (`@angular/build:unit-test`).

## End-to-end tests (Cypress)

```bash
npm run e2e
```

Requires both the backend and `ng serve` (`npm start`) running first — Cypress drives the real app at `http://localhost:4200`. See `cypress/e2e/dashboard.cy.ts`.

## Structure

```
src/app/
├── app.routes.ts                  # Root routes (Dashboard at "/", placeholders for not-yet-built modules)
├── features/
│   └── dashboard/                 # Dashboard screen + its widgets (stat cards, weekly chart, ...)
└── shared/
    ├── components/
    │   ├── app-shell/             # Sidebar + topbar, reused by every screen
    │   └── coming-soon/           # Generic "under construction" placeholder page
    ├── models/                    # TS interfaces mirroring backend DTOs
    └── services/                  # Signal-based services (ModuloService, DashboardService)
```
