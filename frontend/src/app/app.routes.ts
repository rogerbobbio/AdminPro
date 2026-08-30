import { Routes } from '@angular/router';
import { Dashboard } from './features/dashboard/dashboard';
import { ComingSoon } from './shared/components/coming-soon/coming-soon';

export const routes: Routes = [
  { path: '', component: Dashboard },
  {
    path: 'proyectos',
    component: ComingSoon,
    data: { activeNav: 'proyectos', title: 'Gestión de Proyectos' },
  },
];
