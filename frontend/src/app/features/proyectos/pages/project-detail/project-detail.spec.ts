import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { ProjectDetail } from './project-detail';
import { ProjectDetail as ProjectDetailModel } from '../../../../shared/models/project.model';

describe('ProjectDetail', () => {
  let httpMock: HttpTestingController;

  const detail: ProjectDetailModel = {
    id: 1,
    nombre: 'Acme Corp',
    descripcion: 'Sistema',
    activo: true,
    createdAt: '',
    updatedAt: '',
    basesDeDatos: [
      { id: 10, nombre: 'SalesDb', servidor: null, databaseId: null, loginName: null, ambiente: 'desarrollo', notas: null, activo: true },
    ],
    applications: [],
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProjectDetail],
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: ActivatedRoute, useValue: { snapshot: { paramMap: { get: () => '1' } } } },
      ],
    }).compileComponents();

    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    TestBed.resetTestingModule();
  });

  it('renders the databases list and an empty Aplicaciones section', async () => {
    const fixture = TestBed.createComponent(ProjectDetail);
    fixture.detectChanges();
    httpMock.expectOne('/api/projects/1').flush(detail);
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('SalesDb');
    expect(compiled.querySelector('[data-testid="btn-nueva-aplicacion"]')).toBeNull();
    expect(compiled.textContent?.toLowerCase()).toContain('no hay aplicaciones');
  });
});
