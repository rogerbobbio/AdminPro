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
    tipo: 'Web',
    tecnologiaFront: 'Angular',
    tecnologiaBack: '.NET',
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

  async function createAndLoad(overrides: Partial<ApplicationDetailModel> = {}) {
    const fixture = TestBed.createComponent(ApplicationDetail);
    fixture.detectChanges();
    httpMock.expectOne('/api/applications/1').flush({ ...detail, ...overrides });
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();
    return fixture;
  }

  it('renders application fields, the Ambientes list, and empty states with create actions for the other sections', async () => {
    const fixture = await createAndLoad();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('CRM');
    expect(compiled.textContent).toContain('UAT');
    expect(compiled.textContent?.toLowerCase()).toContain('no hay reportes');
    expect(compiled.querySelector('[data-testid="btn-agregar-reporte"]')).not.toBeNull();
    expect(compiled.querySelector('[data-testid="btn-agregar-nota"]')).not.toBeNull();
    expect(compiled.querySelector('[data-testid="btn-agregar-documento"]')).not.toBeNull();
    expect(compiled.querySelector('[data-testid="btn-agregar-fixdata"]')).not.toBeNull();
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

  it('adding a reporte updates the list', async () => {
    const fixture = await createAndLoad();
    const compiled = fixture.nativeElement as HTMLElement;

    (compiled.querySelector('[data-testid="btn-agregar-reporte"]') as HTMLButtonElement).click();
    fixture.detectChanges();

    const codeInput = compiled.querySelector('[data-testid="modal-input-report-code"]') as HTMLInputElement;
    codeInput.value = 'VFL';
    codeInput.dispatchEvent(new Event('input'));
    const nameInput = compiled.querySelector('[data-testid="modal-input-report-name"]') as HTMLInputElement;
    nameInput.value = 'Volumen de Carga';
    nameInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    (compiled.querySelector('[data-testid="modal-btn-guardar-reporte"]') as HTMLButtonElement).click();

    const createReq = httpMock.expectOne('/api/applications/1/reportes');
    createReq.flush(20);
    await new Promise((resolve) => setTimeout(resolve, 0));

    httpMock.expectOne('/api/applications/1').flush({
      ...detail,
      reportes: [
        {
          id: 20,
          reportCode: 'VFL',
          reportName: 'Volumen de Carga',
          regionId: null,
          reportPath: null,
          spTranship: null,
          spReportViewer: null,
          notas: null,
          parametrosEjemplo: null,
          activo: true,
        },
      ],
    });
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();

    expect(compiled.textContent).toContain('Volumen de Carga');
    expect(compiled.querySelector('[data-testid="modal-input-report-code"]')).toBeNull();
  });

  it('note rows are collapsed by default and expand to reveal the description on click', async () => {
    const fixture = await createAndLoad({
      notas: [{ id: 30, titulo: 'nvm use 14.16.0', descripcion: 'Usar Node 14.16.0 para compilar el front.', orden: 0, activo: true }],
    });
    const compiled = fixture.nativeElement as HTMLElement;

    expect(compiled.textContent).toContain('nvm use 14.16.0');
    expect(compiled.querySelector('[data-testid="nota-descripcion"]')).toBeNull();

    (compiled.querySelector('[data-testid="nota-toggle"]') as HTMLElement).click();
    fixture.detectChanges();

    const descripcionEl = compiled.querySelector('[data-testid="nota-descripcion"]');
    expect(descripcionEl?.textContent).toContain('Usar Node 14.16.0 para compilar el front.');
  });

  it('adding a nota updates the count and list', async () => {
    const fixture = await createAndLoad();
    const compiled = fixture.nativeElement as HTMLElement;

    (compiled.querySelector('[data-testid="btn-agregar-nota"]') as HTMLButtonElement).click();
    fixture.detectChanges();

    const tituloInput = compiled.querySelector('[data-testid="modal-input-titulo"]') as HTMLInputElement;
    tituloInput.value = 'Borrar bin/obj';
    tituloInput.dispatchEvent(new Event('input'));
    const descripcionInput = compiled.querySelector('[data-testid="modal-input-descripcion"]') as HTMLTextAreaElement;
    descripcionInput.value = 'Antes de compilar, borrar las carpetas bin y obj.';
    descripcionInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    (compiled.querySelector('[data-testid="modal-btn-guardar-nota"]') as HTMLButtonElement).click();

    const createReq = httpMock.expectOne('/api/applications/1/notas');
    createReq.flush(31);
    await new Promise((resolve) => setTimeout(resolve, 0));

    httpMock.expectOne('/api/applications/1').flush({
      ...detail,
      notas: [{ id: 31, titulo: 'Borrar bin/obj', descripcion: 'Antes de compilar, borrar las carpetas bin y obj.', orden: 0, activo: true }],
    });
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();

    expect(compiled.textContent).toContain('Notas (1)');
    expect(compiled.textContent).toContain('Borrar bin/obj');
  });

  it('opening a documento link points at its urlOneDrive in a new tab', async () => {
    const fixture = await createAndLoad({
      documentos: [{ id: 40, nombreArchivo: 'Manual de Usuario', urlOneDrive: 'https://onedrive.example.com/manual', tipo: 'manual', descripcion: null, orden: 0, activo: true }],
    });
    const compiled = fixture.nativeElement as HTMLElement;

    const link = compiled.querySelector('[data-testid="link-abrir-documento"]') as HTMLAnchorElement;
    expect(link.getAttribute('href')).toBe('https://onedrive.example.com/manual');
    expect(link.getAttribute('target')).toBe('_blank');
  });

  it('adding a documento updates the list', async () => {
    const fixture = await createAndLoad();
    const compiled = fixture.nativeElement as HTMLElement;

    (compiled.querySelector('[data-testid="btn-agregar-documento"]') as HTMLButtonElement).click();
    fixture.detectChanges();

    const nombreInput = compiled.querySelector('[data-testid="modal-input-nombre-archivo"]') as HTMLInputElement;
    nombreInput.value = 'Manual de Usuario';
    nombreInput.dispatchEvent(new Event('input'));
    const urlInput = compiled.querySelector('[data-testid="modal-input-url-onedrive"]') as HTMLInputElement;
    urlInput.value = 'https://onedrive.example.com/manual';
    urlInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    (compiled.querySelector('[data-testid="modal-btn-guardar-documento"]') as HTMLButtonElement).click();

    const createReq = httpMock.expectOne('/api/applications/1/documentos');
    createReq.flush(41);
    await new Promise((resolve) => setTimeout(resolve, 0));

    httpMock.expectOne('/api/applications/1').flush({
      ...detail,
      documentos: [{ id: 41, nombreArchivo: 'Manual de Usuario', urlOneDrive: 'https://onedrive.example.com/manual', tipo: 'manual', descripcion: null, orden: 0, activo: true }],
    });
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();

    expect(compiled.textContent).toContain('Manual de Usuario');
  });

  it('adding a fixData updates the list', async () => {
    const fixture = await createAndLoad();
    const compiled = fixture.nativeElement as HTMLElement;

    (compiled.querySelector('[data-testid="btn-agregar-fixdata"]') as HTMLButtonElement).click();
    fixture.detectChanges();

    const nombreInput = compiled.querySelector('[data-testid="modal-input-fixdata-nombre"]') as HTMLInputElement;
    nombreInput.value = 'Fix duplicate customers';
    nombreInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    (compiled.querySelector('[data-testid="modal-btn-guardar-fixdata"]') as HTMLButtonElement).click();

    const createReq = httpMock.expectOne('/api/applications/1/fixdatas');
    createReq.flush(51);
    await new Promise((resolve) => setTimeout(resolve, 0));

    httpMock.expectOne('/api/applications/1').flush({
      ...detail,
      fixDatas: [{ id: 51, nombre: 'Fix duplicate customers', descripcion: null, script: null, orden: 0, activo: true }],
    });
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();

    expect(compiled.textContent).toContain('Fix duplicate customers');
  });

  it('copying a fixData script writes it to the clipboard', async () => {
    const writeText = vi.fn().mockResolvedValue(undefined);
    Object.assign(navigator, { clipboard: { writeText } });

    const fixture = await createAndLoad({
      fixDatas: [{ id: 60, nombre: 'Fix duplicate customers', descripcion: null, script: 'DELETE FROM Customers;', orden: 0, activo: true }],
    });
    const compiled = fixture.nativeElement as HTMLElement;

    (compiled.querySelector('[data-testid="btn-copiar-script"]') as HTMLButtonElement).click();
    await new Promise((resolve) => setTimeout(resolve, 0));

    expect(writeText).toHaveBeenCalledWith('DELETE FROM Customers;');
  });
});
