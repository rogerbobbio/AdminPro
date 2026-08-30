import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ModuloService } from './modulo.service';
import { Modulo } from '../models/modulo.model';

describe('ModuloService', () => {
  let service: ModuloService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ModuloService, provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(ModuloService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('loadModulos populates the modulos signal from GET /api/modulos', async () => {
    const mockModulos: Modulo[] = [
      { id: 1, nombre: 'Gestión de Proyectos', icono: 'bi-kanban', rutaBase: 'proyectos', color: 'primary', orden: 0 },
    ];

    const loadPromise = service.loadModulos();

    const req = httpMock.expectOne('/api/modulos');
    expect(req.request.method).toBe('GET');
    req.flush(mockModulos);

    await loadPromise;

    expect(service.modulos()).toEqual(mockModulos);
  });
});
