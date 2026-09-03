import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ApplicationService } from './application.service';
import { ApplicationDetail, ApplicationSummary } from '../models/project.model';

describe('ApplicationService', () => {
  let service: ApplicationService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ApplicationService, provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(ApplicationService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    TestBed.resetTestingModule();
  });

  it('loadByProject populates the applications signal from GET /api/projects/:id/applications', async () => {
    const mock: ApplicationSummary[] = [
      { id: 1, nombre: 'CRM', tecnologiaFront: 'Angular', tecnologiaBack: null, orden: 0, activo: true },
    ];

    const loadPromise = service.loadByProject(1);
    const req = httpMock.expectOne('/api/projects/1/applications');
    expect(req.request.method).toBe('GET');
    req.flush(mock);
    await loadPromise;

    expect(service.applications()).toEqual(mock);
  });

  it('getById populates the selectedApplication signal from GET /api/applications/:id', async () => {
    const mock: ApplicationDetail = {
      id: 1,
      proyectoId: 1,
      nombre: 'CRM',
      descripcion: null,
      tipo: null,
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
    };

    const loadPromise = service.getById(1);
    const req = httpMock.expectOne('/api/applications/1');
    expect(req.request.method).toBe('GET');
    req.flush(mock);
    await loadPromise;

    expect(service.selectedApplication()).toEqual(mock);
  });

  it('create posts to /api/projects/:projectId/applications', async () => {
    const createPromise = service.create(1, { nombre: 'CRM', orden: 0 });

    const req = httpMock.expectOne('/api/projects/1/applications');
    expect(req.request.method).toBe('POST');
    req.flush(5);

    const id = await createPromise;
    expect(id).toBe(5);
  });

  it('update puts to /api/applications/:id', async () => {
    const updatePromise = service.update(5, { id: 5, nombre: 'CRM Updated', orden: 0 });

    const req = httpMock.expectOne('/api/applications/5');
    expect(req.request.method).toBe('PUT');
    req.flush(null);

    await updatePromise;
  });

  it('delete deletes /api/applications/:id', async () => {
    const deletePromise = service.delete(5);

    const req = httpMock.expectOne('/api/applications/5');
    expect(req.request.method).toBe('DELETE');
    req.flush(null);

    await deletePromise;
  });
});
