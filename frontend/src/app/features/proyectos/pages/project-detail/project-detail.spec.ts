import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { ProjectDetail } from './project-detail';
import { ProjectDetail as ProjectDetailModel } from '../../../../shared/models/project.model';

describe('ProjectDetail', () => {
  let httpMock: HttpTestingController;

  const detail: ProjectDetailModel = {
    id: 1,
    nombre: 'Acme Corp',
    descripcion: 'Sistema',
    activo: true,
    createdAt: '',
    updatedAt: '',
    basesDeDatos: [
      { id: 10, nombre: 'SalesDb', servidor: null, databaseId: null, loginName: null, password: null, ambiente: 'desarrollo', notas: null, activo: true },
    ],
    applications: [],
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProjectDetail],
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
    const fixture = TestBed.createComponent(ProjectDetail);
    fixture.detectChanges();
    httpMock.expectOne('/api/projects/1').flush(detail);
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();
    return fixture;
  }

  it('renders the databases list and an empty Aplicaciones section', async () => {
    const fixture = await createAndLoad();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('SalesDb');
    expect(compiled.querySelector('[data-testid="btn-nueva-aplicacion"]')).toBeNull();
    expect(compiled.textContent?.toLowerCase()).toContain('no hay aplicaciones');
  });

  it('adding a database updates the list without navigating away', async () => {
    const fixture = await createAndLoad();
    const compiled = fixture.nativeElement as HTMLElement;

    (compiled.querySelector('[data-testid="btn-agregar-bd"]') as HTMLButtonElement).click();
    fixture.detectChanges();

    const nombreInput = compiled.querySelector('[data-testid="modal-input-nombre"]') as HTMLInputElement;
    nombreInput.value = 'AuthDb';
    nombreInput.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    (compiled.querySelector('[data-testid="modal-btn-guardar"]') as HTMLButtonElement).click();

    const createReq = httpMock.expectOne('/api/projects/1/basesdedatos');
    createReq.flush(11);
    await new Promise((resolve) => setTimeout(resolve, 0));

    httpMock.expectOne('/api/projects/1').flush({
      ...detail,
      basesDeDatos: [...detail.basesDeDatos, { id: 11, nombre: 'AuthDb', servidor: null, databaseId: null, loginName: null, password: null, ambiente: null, notas: null, activo: true }],
    });
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();

    expect(compiled.textContent).toContain('AuthDb');
    expect(compiled.querySelector('[data-testid="modal-input-nombre"]')).toBeNull();
  });

  it('toggles the password field visibility', async () => {
    const fixture = await createAndLoad();
    const compiled = fixture.nativeElement as HTMLElement;

    (compiled.querySelector('[data-testid="btn-agregar-bd"]') as HTMLButtonElement).click();
    fixture.detectChanges();

    const passwordInput = compiled.querySelector<HTMLInputElement>('[data-testid="modal-input-password"]')!;
    expect(passwordInput.type).toBe('password');

    (compiled.querySelector('[data-testid="btn-toggle-password"]') as HTMLButtonElement).click();
    fixture.detectChanges();

    expect(passwordInput.type).toBe('text');

    (compiled.querySelector('[data-testid="btn-toggle-password"]') as HTMLButtonElement).click();
    fixture.detectChanges();

    expect(passwordInput.type).toBe('password');
  });
});
