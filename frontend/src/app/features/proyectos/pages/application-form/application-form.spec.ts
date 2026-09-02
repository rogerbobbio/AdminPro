import { TestBed } from '@angular/core/testing';
import { Location } from '@angular/common';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ActivatedRoute, Router, provideRouter } from '@angular/router';
import { ApplicationForm } from './application-form';

describe('ApplicationForm', () => {
  let httpMock: HttpTestingController;
  let router: Router;

  async function setup(routeParams: Record<string, string> = {}, queryParams: Record<string, string> = {}) {
    await TestBed.configureTestingModule({
      imports: [ApplicationForm],
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: { get: (key: string) => routeParams[key] ?? null },
              queryParamMap: { get: (key: string) => queryParams[key] ?? null },
            },
          },
        },
      ],
    }).compileComponents();

    httpMock = TestBed.inject(HttpTestingController);
    router = TestBed.inject(Router);
  }

  afterEach(() => {
    httpMock.verify();
    TestBed.resetTestingModule();
  });

  const mockProject = {
    id: 1,
    nombre: 'Acme Corp',
    descripcion: null,
    activo: true,
    createdAt: '',
    updatedAt: '',
    basesDeDatos: [],
    applications: [],
  };

  it('shows an inline error when the backend rejects a duplicate name', async () => {
    await setup({}, { proyectoId: '1' });
    const fixture = TestBed.createComponent(ApplicationForm);
    fixture.detectChanges();
    httpMock.expectOne('/api/projects/1').flush(mockProject);
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();

    const nombreInput = fixture.nativeElement.querySelector('[data-testid="input-nombre"]') as HTMLInputElement;
    nombreInput.value = 'CRM';
    nombreInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    (fixture.nativeElement.querySelector('[data-testid="btn-guardar"]') as HTMLButtonElement).click();

    const req = httpMock.expectOne('/api/projects/1/applications');
    req.flush(
      {
        error: 'ValidationError',
        message: 'Errores de validación',
        details: [{ field: 'Nombre', error: 'Ya existe una aplicación con ese nombre en este proyecto.' }],
      },
      { status: 400, statusText: 'Bad Request' },
    );
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();

    const errorEl = fixture.nativeElement.querySelector('[data-testid="error-nombre"]');
    expect(errorEl?.textContent).toContain('Ya existe una aplicación con ese nombre en este proyecto.');
  });

  it('navigates to the new application detail page on success', async () => {
    await setup({}, { proyectoId: '1' });
    const fixture = TestBed.createComponent(ApplicationForm);
    fixture.detectChanges();
    httpMock.expectOne('/api/projects/1').flush(mockProject);
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();
    const navigateSpy = vi.spyOn(router, 'navigate').mockResolvedValue(true);

    const nombreInput = fixture.nativeElement.querySelector('[data-testid="input-nombre"]') as HTMLInputElement;
    nombreInput.value = 'CRM';
    nombreInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    (fixture.nativeElement.querySelector('[data-testid="btn-guardar"]') as HTMLButtonElement).click();

    const req = httpMock.expectOne('/api/projects/1/applications');
    req.flush(3);
    await new Promise((resolve) => setTimeout(resolve, 0));

    expect(navigateSpy).toHaveBeenCalledWith(['/proyectos/aplicaciones', 3]);
  });

  it('renders a breadcrumb with the parent project name in create mode', async () => {
    await setup({}, { proyectoId: '1' });
    const fixture = TestBed.createComponent(ApplicationForm);
    fixture.detectChanges();
    httpMock.expectOne('/api/projects/1').flush(mockProject);
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();

    const breadcrumb = fixture.nativeElement.querySelector('[data-testid="breadcrumb"]');
    expect(breadcrumb?.textContent).toContain('Acme Corp');
    expect(breadcrumb?.textContent).toContain('Nueva Aplicación');
  });

  it('renders a breadcrumb with the application name in edit mode', async () => {
    await setup({ id: '5' });
    const fixture = TestBed.createComponent(ApplicationForm);
    fixture.detectChanges();
    httpMock.expectOne('/api/applications/5').flush({
      id: 5,
      proyectoId: 1,
      nombre: 'CRM',
      descripcion: null,
      tecnologiaFront: null,
      tecnologiaBack: null,
      ramaDesarrollo: null,
      applicationName: null,
      rutaLocal: null,
      rutaGit: null,
      comoSeLevanta: null,
      notasCompilacion: null,
      orden: 0,
      activo: true,
      createdAt: '',
      updatedAt: '',
      ambientes: [],
      reportes: [],
      notas: [],
      documentos: [],
      fixDatas: [],
      servicios: [],
    });
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();
    httpMock.expectOne('/api/projects/1').flush(mockProject);
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();

    const breadcrumb = fixture.nativeElement.querySelector('[data-testid="breadcrumb"]');
    expect(breadcrumb?.textContent).toContain('Acme Corp');
    expect(breadcrumb?.textContent).toContain('CRM');
  });

  it('clicking Cancelar navigates back without making an HTTP request', async () => {
    await setup({}, { proyectoId: '1' });
    const fixture = TestBed.createComponent(ApplicationForm);
    fixture.detectChanges();
    httpMock.expectOne('/api/projects/1').flush(mockProject);
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();
    const location = TestBed.inject(Location);
    const backSpy = vi.spyOn(location, 'back').mockImplementation(() => {});

    (fixture.nativeElement.querySelector('[data-testid="btn-cancelar"]') as HTMLButtonElement).click();

    expect(backSpy).toHaveBeenCalled();
    httpMock.expectNone('/api/projects/1/applications');
  });
});
