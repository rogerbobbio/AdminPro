export interface ProjectSummary {
  id: number;
  nombre: string;
  descripcion: string | null;
  activo: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface BaseDeDatos {
  id: number;
  nombre: string;
  servidor: string | null;
  databaseId: number | null;
  loginName: string | null;
  password: string | null;
  ambiente: string | null;
  notas: string | null;
  activo: boolean;
}

export interface ApplicationSummary {
  id: number;
  nombre: string;
  tecnologiaFront: string | null;
  tecnologiaBack: string | null;
  orden: number;
  activo: boolean;
}

export interface ProjectDetail extends ProjectSummary {
  basesDeDatos: BaseDeDatos[];
  applications: ApplicationSummary[];
}

export interface CreateProjectCommand {
  nombre: string;
  descripcion?: string | null;
}

export interface UpdateProjectCommand {
  id: number;
  nombre: string;
  descripcion?: string | null;
}

export interface CreateBaseDeDatosCommand {
  nombre: string;
  servidor?: string | null;
  databaseId?: number | null;
  loginName?: string | null;
  password?: string | null;
  ambiente?: string | null;
  notas?: string | null;
}

export interface UpdateBaseDeDatosCommand {
  id: number;
  nombre: string;
  servidor?: string | null;
  databaseId?: number | null;
  loginName?: string | null;
  password?: string | null;
  ambiente?: string | null;
  notas?: string | null;
}
