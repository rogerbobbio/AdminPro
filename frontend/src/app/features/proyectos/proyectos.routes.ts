import { Routes } from '@angular/router';
import { ProyectosLayout } from './proyectos-layout';
import { ProjectList } from './pages/project-list/project-list';
import { ProjectDetail } from './pages/project-detail/project-detail';
import { ProjectForm } from './pages/project-form/project-form';

export const proyectosRoutes: Routes = [
  {
    path: 'proyectos',
    component: ProyectosLayout,
    children: [
      { path: '', component: ProjectList },
      { path: 'nuevo', component: ProjectForm },
      { path: ':id', component: ProjectDetail },
      { path: ':id/editar', component: ProjectForm },
    ],
  },
];
