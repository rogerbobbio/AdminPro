import { Routes } from '@angular/router';
import { Dashboard } from './features/dashboard/dashboard';
import { proyectosRoutes } from './features/proyectos/proyectos.routes';
import { ComingSoon } from './shared/components/coming-soon/coming-soon';

export const routes: Routes = [
  { path: '', component: Dashboard },
  ...proyectosRoutes,
  {
    path: 'servicios',
    component: ComingSoon,
    data: { activeNav: 'servicios', title: 'Catálogo de Servicios' },
  },
];
