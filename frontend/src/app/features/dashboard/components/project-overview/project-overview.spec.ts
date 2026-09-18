import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { ProjectOverview } from './project-overview';
import { ProjectDetail } from '../../../../shared/models/project.model';

const baseProject: ProjectDetail = {
  id: 1,
  nombre: 'EIR',
  descripcion: 'Equipment Inspection Report',
  activo: true,
  createdAt: '2024-01-01T00:00:00Z',
  updatedAt: '2024-01-01T00:00:00Z',
  basesDeDatos: [],
  applications: [],
};

describe('ProjectOverview', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideRouter([])],
    });
  });

  it('renders both sections when the project has databases and applications', () => {
    const fixture = TestBed.createComponent(ProjectOverview);
    fixture.componentRef.setInput('project', {
      ...baseProject,
      basesDeDatos: [
        { id: 1, nombre: 'EIR_PROD', servidor: 'srv-01', databaseId: null, loginName: null, password: null, ambiente: 'Producción', notas: null, activo: true },
      ],
      applications: [
        { id: 1, nombre: 'EIR Web', tecnologiaFront: 'Angular', tecnologiaBack: '.NET', orden: 0, activo: true },
      ],
    });
    fixture.detectChanges();

    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).toContain('Bases de Datos (1)');
    expect(text).toContain('EIR_PROD');
    expect(text).toContain('Aplicaciones (1)');
    expect(text).toContain('EIR Web');
  });

  it('omits the Bases de Datos section entirely when the project has none', () => {
    const fixture = TestBed.createComponent(ProjectOverview);
    fixture.componentRef.setInput('project', {
      ...baseProject,
      applications: [
        { id: 1, nombre: 'EIR Web', tecnologiaFront: 'Angular', tecnologiaBack: '.NET', orden: 0, activo: true },
      ],
    });
    fixture.detectChanges();

    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).not.toContain('Bases de Datos');
    expect(text).toContain('Aplicaciones (1)');
  });

  it('omits the Aplicaciones section entirely when the project has none', () => {
    const fixture = TestBed.createComponent(ProjectOverview);
    fixture.componentRef.setInput('project', {
      ...baseProject,
      basesDeDatos: [
        { id: 1, nombre: 'EIR_PROD', servidor: 'srv-01', databaseId: null, loginName: null, password: null, ambiente: 'Producción', notas: null, activo: true },
      ],
    });
    fixture.detectChanges();

    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).toContain('Bases de Datos (1)');
    expect(text).not.toContain('Aplicaciones');
  });

  it('does not render a password column for databases', () => {
    const fixture = TestBed.createComponent(ProjectOverview);
    fixture.componentRef.setInput('project', {
      ...baseProject,
      basesDeDatos: [
        { id: 1, nombre: 'EIR_PROD', servidor: 'srv-01', databaseId: null, loginName: 'sa', password: 'super-secret', ambiente: 'Producción', notas: null, activo: true },
      ],
    });
    fixture.detectChanges();

    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).not.toContain('super-secret');
  });
});
