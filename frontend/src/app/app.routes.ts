import { Routes } from '@angular/router';
import { Dashboard } from './features/dashboard/dashboard';
import { proyectosRoutes } from './features/proyectos/proyectos.routes';

export const routes: Routes = [
  { path: '', component: Dashboard },
  ...proyectosRoutes,
];
