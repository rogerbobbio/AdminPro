import { TestBed } from '@angular/core/testing';
import { ApplicationOverview } from './application-overview';
import { ApplicationDetail } from '../../../../shared/models/project.model';

const baseApplication: ApplicationDetail = {
  id: 1,
  proyectoId: 1,
  nombre: 'EIR Mobile Inspector',
  descripcion: null,
  tipo: null,
  tecnologiaFront: 'React Native',
  tecnologiaBack: '.NET 8',
  ramaDesarrollo: null,
  applicationName: null,
  rutaLocal: null,
  rutaGit: null,
  comoSeLevanta: null,
  notasCompilacion: null,
  orden: 0,
  activo: true,
  createdAt: '2024-01-01T00:00:00Z',
  updatedAt: '2024-01-01T00:00:00Z',
  ambientes: [],
  reportes: [],
  notas: [],
  documentos: [],
  fixDatas: [],
  servicios: [],
};

describe('ApplicationOverview', () => {
  it('renders only the sections that have data', () => {
    const fixture = TestBed.createComponent(ApplicationOverview);
    fixture.componentRef.setInput('application', {
      ...baseApplication,
      ambientes: [{ id: 1, nombre: 'UAT', url: 'https://uat.example.com', esWebApi: false, notas: null, orden: 0, activo: true }],
      reportes: [{ id: 1, reportCode: 'VFL', reportName: 'Volumen de Carga', regionId: null, reportPath: null, spTranship: 'sp_A', spReportViewer: 'sp_B', notas: null, parametrosEjecucion: null, activo: true }],
      notas: [{ id: 1, titulo: 'Pendiente migrar', descripcion: 'Migrar a MSAL', orden: 0, activo: true }],
    });
    fixture.detectChanges();

    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).toContain('Ambientes (1)');
    expect(text).toContain('Reportes (1)');
    expect(text).toContain('Notas (1)');
    expect(text).not.toContain('Documentos');
    expect(text).not.toContain('FixDatas');
  });

  it('omits every section when the application has no nested data', () => {
    const fixture = TestBed.createComponent(ApplicationOverview);
    fixture.componentRef.setInput('application', baseApplication);
    fixture.detectChanges();

    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).not.toContain('Ambientes');
    expect(text).not.toContain('Reportes');
    expect(text).not.toContain('Notas');
    expect(text).not.toContain('Documentos');
    expect(text).not.toContain('FixDatas');
  });
});
