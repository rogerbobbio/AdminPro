import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { DashboardService } from './dashboard.service';
import { DashboardSummary } from '../models/dashboard-summary.model';

describe('DashboardService', () => {
  let service: DashboardService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [DashboardService, provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(DashboardService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('loadSummary populates the summary signal from GET /api/dashboard/summary', async () => {
    const mockSummary: DashboardSummary = {
      totalProjects: 0,
      totalApplications: 0,
      totalAmbientes: 0,
      totalServiciosVinculados: 0,
      applicationsCreatedLast7Days: [0, 0, 0, 0, 0, 0, 0],
      recentApplications: [],
      statusBreakdown: { activo: 0, enProgreso: 0, pendiente: 0 },
    };

    expect(service.loading()).toBe(false);
    const loadPromise = service.loadSummary();
    expect(service.loading()).toBe(true);

    const req = httpMock.expectOne('/api/dashboard/summary');
    expect(req.request.method).toBe('GET');
    req.flush(mockSummary);

    await loadPromise;

    expect(service.summary()).toEqual(mockSummary);
    expect(service.loading()).toBe(false);
    expect(service.error()).toBeNull();
  });

  it('sets error signal when the request fails', async () => {
    const loadPromise = service.loadSummary();

    const req = httpMock.expectOne('/api/dashboard/summary');
    req.error(new ProgressEvent('error'));

    await loadPromise;

    expect(service.error()).not.toBeNull();
    expect(service.loading()).toBe(false);
  });
});
