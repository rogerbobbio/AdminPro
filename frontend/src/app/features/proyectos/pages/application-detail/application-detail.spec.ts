import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { ApplicationDetail } from './application-detail';
import { ApplicationDetail as ApplicationDetailModel } from '../../../../shared/models/project.model';

describe('ApplicationDetail', () => {
  let httpMock: HttpTestingController;

  const detail: ApplicationDetailModel = {
    id: 1,
    proyectoId: 1,
    nombre: 'CRM',
    descripcion: 'Customer Relationship Manager',
    tecnologiaFront: 'Angular',
    tecnologiaBack: '.NET',
    ramaDesarrollo: null,
    applicationName: null,
    tieneProyectoBD: null,
    rutaLocal: null,
    rutaGit: null,
    comoSeLevanta: null,
    notasCompilacion: null,
    orden: 0,
    activo: true,
    createdAt: '',
    updatedAt: '',
    ambientes: [{ id: 10, nombre: 'UAT', url: 'https://uat.example.com', esWebApi: false, notas: null, orden: 0, activo: true }],
    reportes: [],
    notas: [],
    documentos: [],
    fixDatas: [],
    servicios: [],
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ApplicationDetail],
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

  async function createAndLoad() {
    const fixture = TestBed.createComponent(ApplicationDetail);
    fixture.detectChanges();
    httpMock.expectOne('/api/applications/1').flush(detail);
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();
    return fixture;
  }

  it('renders application fields, the Ambientes list, and empty-state sections with no create action', async () => {
    const fixture = await createAndLoad();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('CRM');
    expect(compiled.textContent).toContain('UAT');
    expect(compiled.textContent?.toLowerCase()).toContain('no hay reportes');
    expect(compiled.querySelector('[data-testid="btn-nuevo-reporte"]')).toBeNull();
  });

  it('adding an environment updates the list without navigating away', async () => {
    const fixture = await createAndLoad();
    const compiled = fixture.nativeElement as HTMLElement;

    (compiled.querySelector('[data-testid="btn-agregar-ambiente"]') as HTMLButtonElement).click();
    fixture.detectChanges();

    const nombreInput = compiled.querySelector('[data-testid="modal-input-nombre"]') as HTMLInputElement;
    nombreInput.value = 'PROD';
    nombreInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    (compiled.querySelector('[data-testid="modal-btn-guardar"]') as HTMLButtonElement).click();

    const createReq = httpMock.expectOne('/api/applications/1/ambientes');
    createReq.flush(11);
    await new Promise((resolve) => setTimeout(resolve, 0));

    httpMock.expectOne('/api/applications/1').flush({
      ...detail,
      ambientes: [...detail.ambientes, { id: 11, nombre: 'PROD', url: null, esWebApi: false, notas: null, orden: 0, activo: true }],
    });
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();

    expect(compiled.textContent).toContain('PROD');
    expect(compiled.querySelector('[data-testid="modal-input-nombre"]')).toBeNull();
  });

  it('shows an inline error when the backend rejects an invalid URL', async () => {
    const fixture = await createAndLoad();
    const compiled = fixture.nativeElement as HTMLElement;

    (compiled.querySelector('[data-testid="btn-agregar-ambiente"]') as HTMLButtonElement).click();
    fixture.detectChanges();

    const nombreInput = compiled.querySelector('[data-testid="modal-input-nombre"]') as HTMLInputElement;
    nombreInput.value = 'PROD';
    nombreInput.dispatchEvent(new Event('input'));
    const urlInput = compiled.querySelector('[data-testid="modal-input-url"]') as HTMLInputElement;
    urlInput.value = 'not-a-url';
    urlInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    (compiled.querySelector('[data-testid="modal-btn-guardar"]') as HTMLButtonElement).click();

    const createReq = httpMock.expectOne('/api/applications/1/ambientes');
    createReq.flush(
      { error: 'ValidationError', message: 'Errores de validación', details: [{ field: 'Url', error: 'La URL debe ser una dirección absoluta http:// o https://.' }] },
      { status: 400, statusText: 'Bad Request' },
    );
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();

    const errorEl = compiled.querySelector('[data-testid="modal-error-url"]');
    expect(errorEl?.textContent).toContain('http://');
    expect(compiled.querySelector('[data-testid="modal-input-nombre"]')).not.toBeNull();
  });
});
