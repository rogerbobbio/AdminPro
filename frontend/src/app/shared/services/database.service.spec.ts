import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { DatabaseService } from './database.service';

describe('DatabaseService', () => {
  let service: DatabaseService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [DatabaseService, provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(DatabaseService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    TestBed.resetTestingModule();
  });

  it('create posts to /api/projects/:projectId/basesdedatos', async () => {
    const createPromise = service.create(1, { nombre: 'SalesDb', ambiente: 'desarrollo' });

    const req = httpMock.expectOne('/api/projects/1/basesdedatos');
    expect(req.request.method).toBe('POST');
    req.flush(5);

    const id = await createPromise;
    expect(id).toBe(5);
  });

  it('update puts to /api/basesdedatos/:id', async () => {
    const updatePromise = service.update(5, { id: 5, nombre: 'SalesDb', ambiente: 'uat' });

    const req = httpMock.expectOne('/api/basesdedatos/5');
    expect(req.request.method).toBe('PUT');
    req.flush(null);

    await updatePromise;
  });

  it('delete deletes /api/basesdedatos/:id', async () => {
    const deletePromise = service.delete(5);

    const req = httpMock.expectOne('/api/basesdedatos/5');
    expect(req.request.method).toBe('DELETE');
    req.flush(null);

    await deletePromise;
  });
});
