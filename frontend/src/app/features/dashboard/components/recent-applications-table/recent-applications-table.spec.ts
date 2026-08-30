import { TestBed } from '@angular/core/testing';
import { RecentApplicationsTable } from './recent-applications-table';
import { RecentApplication } from '../../../../shared/models/dashboard-summary.model';

describe('RecentApplicationsTable', () => {
  it('shows an empty-state message when there are no recent applications', () => {
    const fixture = TestBed.createComponent(RecentApplicationsTable);
    fixture.componentRef.setInput('applications', []);
    fixture.detectChanges();

    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text.toLowerCase()).toContain('no hay aplicaciones');
  });

  it('renders a row per recent application', () => {
    const apps: RecentApplication[] = [
      {
        id: 1,
        nombre: 'EIR',
        projectName: 'DOLE',
        tecnologiaFront: 'Angular 6',
        tecnologiaBack: '.NET 6',
        status: 'Activo',
      },
    ];

    const fixture = TestBed.createComponent(RecentApplicationsTable);
    fixture.componentRef.setInput('applications', apps);
    fixture.detectChanges();

    const rows = (fixture.nativeElement as HTMLElement).querySelectorAll('tbody tr');
    expect(rows.length).toBe(1);
    expect(rows[0].textContent).toContain('EIR');
    expect(rows[0].textContent).toContain('DOLE');
  });
});
