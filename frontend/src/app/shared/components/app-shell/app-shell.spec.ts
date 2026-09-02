import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';
import { AppShell } from './app-shell';

describe('AppShell', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AppShell],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();
  });

  it('marks only the nav item matching activeNav as active', () => {
    const fixture = TestBed.createComponent(AppShell);
    fixture.componentRef.setInput('activeNav', 'dashboard');
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const activeItems = compiled.querySelectorAll('.nav-item.active');

    expect(activeItems.length).toBe(1);
    expect(activeItems[0].textContent).toContain('Dashboard');
  });

  it('marks the proyectos nav item active when activeNav is proyectos', () => {
    const fixture = TestBed.createComponent(AppShell);
    fixture.componentRef.setInput('activeNav', 'proyectos');
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const activeItems = compiled.querySelectorAll('.nav-item.active');

    expect(activeItems.length).toBe(1);
    expect(activeItems[0].textContent).toContain('Proyectos');
  });

  it('projects page content into the shell content area', () => {
    const fixture = TestBed.createComponent(AppShell);
    fixture.componentRef.setInput('activeNav', 'dashboard');
    const projected = document.createElement('div');
    projected.textContent = 'Projected Content';
    fixture.nativeElement.appendChild(projected);
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelector('.shell-content')).toBeTruthy();
  });
});
