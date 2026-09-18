export interface RecentApplication {
  id: number;
  nombre: string;
  projectName: string;
  tecnologiaFront: string | null;
  tecnologiaBack: string | null;
  updatedAt: string;
}

export interface DashboardSummary {
  totalProjects: number;
  totalApplications: number;
  recentApplications: RecentApplication[];
}
