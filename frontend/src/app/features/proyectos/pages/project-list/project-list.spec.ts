import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { ProjectList } from './project-list';
import { ProjectSummary } from '../../../../shared/models/project.model';

describe('ProjectList', () => {
  const projects: ProjectSummary[] = [
    { id: 1, nombre: 'Acme Corp', descripcion: 'Sistema de gestión', activo: true, createdAt: '', updatedAt: '' },
    { id: 2, nombre: 'Globex Corp', descripcion: 'Nuevo cliente', activo: true, createdAt: '', updatedAt: '' },
  ];

  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProjectList],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
    TestBed.resetTestingModule();
  });

  it('renders a card per project', async () => {
    const fixture = TestBed.createComponent(ProjectList);
    fixture.detectChanges();
    httpMock.expectOne('/api/projects').flush(projects);
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();

    const cards = (fixture.nativeElement as HTMLElement).querySelectorAll('[data-testid="project-card"]');
    expect(cards.length).toBe(2);
  });

  it('filters the visible list when searching', async () => {
    const fixture = TestBed.createComponent(ProjectList);
    fixture.detectChanges();
    httpMock.expectOne('/api/projects').flush(projects);
    await new Promise((resolve) => setTimeout(resolve, 0));
    fixture.detectChanges();

    const input = (fixture.nativeElement as HTMLElement).querySelector<HTMLInputElement>(
      '[data-testid="search-input"]',
    )!;
    input.value = 'acme';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();

    const cards = (fixture.nativeElement as HTMLElement).querySelectorAll('[data-testid="project-card"]');
    expect(cards.length).toBe(1);
    expect(cards[0].textContent).toContain('Acme Corp');
  });
});
