import { TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { ModuleList } from './module-list';
import { Modulo } from '../../../../shared/models/modulo.model';

describe('ModuleList', () => {
  const modulos: Modulo[] = [
    { id: 1, nombre: 'Gestión de Proyectos', icono: 'bi-kanban', rutaBase: 'proyectos', color: 'primary', orden: 0 },
    { id: 2, nombre: 'Catálogo de Servicios', icono: 'bi-hdd-network', rutaBase: 'servicios', color: 'success', orden: 1 },
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideRouter([{ path: 'proyectos', children: [] }])],
    });
  });

  it('renders modules plus the static Presupuesto tile', () => {
    const fixture = TestBed.createComponent(ModuleList);
    fixture.componentRef.setInput('modulos', modulos);
    fixture.detectChanges();

    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).toContain('Gestión de Proyectos');
    expect(text).toContain('Catálogo de Servicios');
    expect(text).toContain('Presupuesto');
  });

  it('navigates when an active module is clicked', async () => {
    const fixture = TestBed.createComponent(ModuleList);
    fixture.componentRef.setInput('modulos', modulos);
    fixture.detectChanges();

    const router = TestBed.inject(Router);
    const navigateSpy = vi.spyOn(router, 'navigateByUrl');

    const firstModule = (fixture.nativeElement as HTMLElement).querySelector<HTMLElement>(
      '[data-testid="modulo-card"]',
    );
    firstModule?.click();
    fixture.detectChanges();

    expect(navigateSpy).toHaveBeenCalled();
  });
});
