import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ProjectService } from './project.service';
import { ProjectDetail, ProjectSummary } from '../models/project.model';

describe('ProjectService', () => {
  let service: ProjectService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ProjectService, provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(ProjectService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    TestBed.resetTestingModule();
  });

  it('loadProjects populates the projects signal from GET /api/projects', async () => {
    const mockProjects: ProjectSummary[] = [
      { id: 1, nombre: 'Acme Corp', descripcion: null, activo: true, createdAt: '', updatedAt: '' },
    ];

    const loadPromise = service.loadProjects();
    const req = httpMock.expectOne('/api/projects');
    expect(req.request.method).toBe('GET');
    req.flush(mockProjects);
    await loadPromise;

    expect(service.projects()).toEqual(mockProjects);
  });

  it('getById populates the selectedProject signal from GET /api/projects/:id', async () => {
    const mockDetail: ProjectDetail = {
      id: 1,
      nombre: 'Acme Corp',
      descripcion: null,
      activo: true,
      createdAt: '',
      updatedAt: '',
      basesDeDatos: [],
      applications: [],
    };

    const loadPromise = service.getById(1);
    const req = httpMock.expectOne('/api/projects/1');
    expect(req.request.method).toBe('GET');
    req.flush(mockDetail);
    await loadPromise;

    expect(service.selectedProject()).toEqual(mockDetail);
  });

  it('create posts to /api/projects and reloads the list', async () => {
    const createPromise = service.create({ nombre: 'Globex Corp' });

    const postReq = httpMock.expectOne('/api/projects');
    expect(postReq.request.method).toBe('POST');
    postReq.flush(2);
    await new Promise((resolve) => setTimeout(resolve, 0));

    const listReq = httpMock.expectOne('/api/projects');
    expect(listReq.request.method).toBe('GET');
    listReq.flush([]);

    const id = await createPromise;
    expect(id).toBe(2);
  });

  it('update puts to /api/projects/:id and reloads the list', async () => {
    const updatePromise = service.update(1, { id: 1, nombre: 'Acme Corp Updated' });

    const putReq = httpMock.expectOne('/api/projects/1');
    expect(putReq.request.method).toBe('PUT');
    putReq.flush(null);
    await new Promise((resolve) => setTimeout(resolve, 0));

    const listReq = httpMock.expectOne('/api/projects');
    listReq.flush([]);

    await updatePromise;
  });

  it('delete removes the project from the local signal without a full reload', async () => {
    service.projects.set([
      { id: 1, nombre: 'Acme Corp', descripcion: null, activo: true, createdAt: '', updatedAt: '' },
    ]);

    const deletePromise = service.delete(1);
    const req = httpMock.expectOne('/api/projects/1');
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
    await deletePromise;

    expect(service.projects()).toEqual([]);
  });
});
