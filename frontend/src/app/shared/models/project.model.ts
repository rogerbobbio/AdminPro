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

export interface Ambiente {
  id: number;
  nombre: string;
  url: string | null;
  esWebApi: boolean;
  notas: string | null;
  orden: number;
  activo: boolean;
}

export interface ApplicationDetail {
  id: number;
  proyectoId: number;
  nombre: string;
  descripcion: string | null;
  tecnologiaFront: string | null;
  tecnologiaBack: string | null;
  ramaDesarrollo: string | null;
  applicationName: string | null;
  tieneProyectoBD: string | null;
  rutaLocal: string | null;
  rutaGit: string | null;
  comoSeLevanta: string | null;
  notasCompilacion: string | null;
  orden: number;
  activo: boolean;
  createdAt: string;
  updatedAt: string;
  ambientes: Ambiente[];
  reportes: unknown[];
  notas: unknown[];
  documentos: unknown[];
  fixDatas: unknown[];
  servicios: unknown[];
}

export interface CreateApplicationCommand {
  nombre: string;
  descripcion?: string | null;
  tecnologiaFront?: string | null;
  tecnologiaBack?: string | null;
  ramaDesarrollo?: string | null;
  applicationName?: string | null;
  tieneProyectoBD?: string | null;
  rutaLocal?: string | null;
  rutaGit?: string | null;
  comoSeLevanta?: string | null;
  notasCompilacion?: string | null;
  orden: number;
}

export interface UpdateApplicationCommand {
  id: number;
  nombre: string;
  descripcion?: string | null;
  tecnologiaFront?: string | null;
  tecnologiaBack?: string | null;
  ramaDesarrollo?: string | null;
  applicationName?: string | null;
  tieneProyectoBD?: string | null;
  rutaLocal?: string | null;
  rutaGit?: string | null;
  comoSeLevanta?: string | null;
  notasCompilacion?: string | null;
  orden: number;
}

export interface CreateEnvironmentCommand {
  nombre: string;
  url?: string | null;
  esWebApi: boolean;
  notas?: string | null;
  orden: number;
}

export interface UpdateEnvironmentCommand {
  id: number;
  nombre: string;
  url?: string | null;
  esWebApi: boolean;
  notas?: string | null;
  orden: number;
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
