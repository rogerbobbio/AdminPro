import { TestBed } from '@angular/core/testing';
import { Location } from '@angular/common';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ActivatedRoute, Router, provideRouter } from '@angular/router';
import { ProjectForm } from './project-form';

describe('ProjectForm', () => {
  let httpMock: HttpTestingController;
  let router: Router;

  async function setup(routeParams: Record<string, string> = {}) {
    await TestBed.configureTestingModule({
      imports: [ProjectForm],
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
        {
          provide: ActivatedRoute,
          useValue: { snapshot: { paramMap: { get: (key: string) => routeParams[key] ?? null } } },
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

  it('shows an inline error when the backend rejects a duplicate name', async () => {
    await setup();
    const fixture = TestBed.createComponent(ProjectForm);
    fixture.detectChanges();

    const nombreInput = fixture.nativeElement.querySelector('[data-testid="input-nombre"]') as HTMLInputElement;
    nombreInput.value = 'Acme Corp';
    nombreInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    (fixture.nativeElement.querySelector('[data-testid="btn-guardar"]') as HTMLButtonElement).click();

    const req = httpMock.expectOne('/api/projects');
    req.flush(
      { error: 'ValidationError', message: 'Errores de validación', details: [{ field: 'Nombre', error: 'Ya existe un proyecto con ese nombre.' }] },
      { status: 400, statusText: 'Bad Request' },
    );
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();

    const errorEl = fixture.nativeElement.querySelector('[data-testid="error-nombre"]');
    expect(errorEl?.textContent).toContain('Ya existe un proyecto con ese nombre.');
  });

  it('navigates to the new project detail page on success', async () => {
    await setup();
    const fixture = TestBed.createComponent(ProjectForm);
    fixture.detectChanges();
    const navigateSpy = vi.spyOn(router, 'navigate').mockResolvedValue(true);

    const nombreInput = fixture.nativeElement.querySelector('[data-testid="input-nombre"]') as HTMLInputElement;
    nombreInput.value = 'Globex Corp';
    nombreInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    (fixture.nativeElement.querySelector('[data-testid="btn-guardar"]') as HTMLButtonElement).click();

    const req = httpMock.expectOne('/api/projects');
    req.flush(3);
    await new Promise((resolve) => setTimeout(resolve, 0));
    httpMock.expectOne('/api/projects').flush([]);
    await new Promise((resolve) => setTimeout(resolve, 0));

    expect(navigateSpy).toHaveBeenCalledWith(['/proyectos', 3]);
  });

  it('renders a breadcrumb ending in "Nuevo Proyecto" in create mode', async () => {
    await setup();
    const fixture = TestBed.createComponent(ProjectForm);
    fixture.detectChanges();

    const breadcrumb = fixture.nativeElement.querySelector('[data-testid="breadcrumb-current"]');
    expect(breadcrumb?.textContent).toContain('Nuevo Proyecto');
  });

  it('renders a breadcrumb with the project name in edit mode', async () => {
    await setup({ id: '7' });
    const fixture = TestBed.createComponent(ProjectForm);
    fixture.detectChanges();

    httpMock.expectOne('/api/projects/7').flush({
      id: 7,
      nombre: 'Acme Corp',
      descripcion: null,
      activo: true,
      createdAt: '',
      updatedAt: '',
      basesDeDatos: [],
      applications: [],
    });
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();

    const breadcrumb = fixture.nativeElement.querySelector('[data-testid="breadcrumb"]');
    expect(breadcrumb?.textContent).toContain('Acme Corp');
  });

  it('clicking Cancelar navigates back without making an HTTP request', async () => {
    await setup();
    const fixture = TestBed.createComponent(ProjectForm);
    fixture.detectChanges();
    const location = TestBed.inject(Location);
    const backSpy = vi.spyOn(location, 'back').mockImplementation(() => {});

    const nombreInput = fixture.nativeElement.querySelector('[data-testid="input-nombre"]') as HTMLInputElement;
    nombreInput.value = 'Draft Name';
    nombreInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    (fixture.nativeElement.querySelector('[data-testid="btn-cancelar"]') as HTMLButtonElement).click();

    expect(backSpy).toHaveBeenCalled();
    httpMock.expectNone('/api/projects');
  });
});
