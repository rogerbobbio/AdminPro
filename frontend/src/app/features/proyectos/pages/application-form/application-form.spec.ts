import { TestBed } from '@angular/core/testing';
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

  it('shows an inline error when the backend rejects a duplicate name', async () => {
    await setup({}, { proyectoId: '1' });
    const fixture = TestBed.createComponent(ApplicationForm);
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
});
