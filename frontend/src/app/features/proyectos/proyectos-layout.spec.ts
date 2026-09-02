import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { ProyectosLayout } from './proyectos-layout';

describe('ProyectosLayout', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProyectosLayout],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();
  });

  it('renders AppShell with activeNav="proyectos"', () => {
    const fixture = TestBed.createComponent(ProyectosLayout);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const activeItems = compiled.querySelectorAll('.nav-item.active');

    expect(activeItems.length).toBe(1);
    expect(activeItems[0].textContent).toContain('Proyectos');
  });

  it('renders a router-outlet for child routes', () => {
    const fixture = TestBed.createComponent(ProyectosLayout);
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('router-outlet')).toBeTruthy();
  });
});
