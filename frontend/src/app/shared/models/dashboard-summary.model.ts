export interface RecentApplication {
  id: number;
  nombre: string;
  projectName: string;
  tecnologiaFront: string | null;
  tecnologiaBack: string | null;
  status: string;
}

export interface ApplicationStatusBreakdown {
  activo: number;
  enProgreso: number;
  pendiente: number;
}

export interface DashboardSummary {
  totalProjects: number;
  totalApplications: number;
  totalAmbientes: number;
  totalServiciosVinculados: number;
  applicationsCreatedLast7Days: number[];
  recentApplications: RecentApplication[];
  statusBreakdown: ApplicationStatusBreakdown;
}
