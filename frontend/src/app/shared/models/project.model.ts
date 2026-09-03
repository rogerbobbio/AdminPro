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
  tipo: string | null;
  tecnologiaFront: string | null;
  tecnologiaBack: string | null;
  ramaDesarrollo: string | null;
  applicationName: string | null;
  rutaLocal: string | null;
  rutaGit: string | null;
  comoSeLevanta: string | null;
  notasCompilacion: string | null;
  orden: number;
  activo: boolean;
  createdAt: string;
  updatedAt: string;
  ambientes: Ambiente[];
  reportes: Reporte[];
  notas: Nota[];
  documentos: Documento[];
  fixDatas: FixData[];
  servicios: unknown[];
}

export interface Reporte {
  id: number;
  reportCode: string;
  reportName: string;
  regionId: string | null;
  reportPath: string | null;
  spTranship: string | null;
  spReportViewer: string | null;
  notas: string | null;
  parametrosEjemplo: string | null;
  activo: boolean;
}

export interface Nota {
  id: number;
  titulo: string;
  descripcion: string;
  orden: number;
  activo: boolean;
}

export interface Documento {
  id: number;
  nombreArchivo: string;
  urlOneDrive: string;
  tipo: string;
  descripcion: string | null;
  orden: number;
  activo: boolean;
}

export interface FixData {
  id: number;
  nombre: string;
  descripcion: string | null;
  script: string | null;
  orden: number;
  activo: boolean;
}

export interface CreateApplicationCommand {
  nombre: string;
  descripcion?: string | null;
  tipo?: string | null;
  tecnologiaFront?: string | null;
  tecnologiaBack?: string | null;
  ramaDesarrollo?: string | null;
  applicationName?: string | null;
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
  tipo?: string | null;
  tecnologiaFront?: string | null;
  tecnologiaBack?: string | null;
  ramaDesarrollo?: string | null;
  applicationName?: string | null;
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

export interface CreateReporteCommand {
  reportCode: string;
  reportName: string;
  regionId?: string | null;
  reportPath?: string | null;
  spTranship?: string | null;
  spReportViewer?: string | null;
  notas?: string | null;
  parametrosEjemplo?: string | null;
}

export interface UpdateReporteCommand {
  id: number;
  reportCode: string;
  reportName: string;
  regionId?: string | null;
  reportPath?: string | null;
  spTranship?: string | null;
  spReportViewer?: string | null;
  notas?: string | null;
  parametrosEjemplo?: string | null;
}

export interface CreateNotaCommand {
  titulo: string;
  descripcion: string;
  orden: number;
}

export interface UpdateNotaCommand {
  id: number;
  titulo: string;
  descripcion: string;
  orden: number;
}

export interface CreateDocumentoCommand {
  nombreArchivo: string;
  urlOneDrive: string;
  tipo: string;
  descripcion?: string | null;
  orden: number;
}

export interface UpdateDocumentoCommand {
  id: number;
  nombreArchivo: string;
  urlOneDrive: string;
  tipo: string;
  descripcion?: string | null;
  orden: number;
}

export interface CreateFixDataCommand {
  nombre: string;
  descripcion?: string | null;
  script?: string | null;
  orden: number;
}

export interface UpdateFixDataCommand {
  id: number;
  nombre: string;
  descripcion?: string | null;
  script?: string | null;
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
