import { TestBed } from '@angular/core/testing';
import { StatCards } from './stat-cards';
import { DashboardSummary } from '../../../../shared/models/dashboard-summary.model';

const emptySummary: DashboardSummary = {
  totalProjects: 0,
  totalApplications: 0,
  totalAmbientes: 0,
  totalServiciosVinculados: 0,
  applicationsCreatedLast7Days: [0, 0, 0, 0, 0, 0, 0],
  recentApplications: [],
  statusBreakdown: { activo: 0, enProgreso: 0, pendiente: 0 },
};

describe('StatCards', () => {
  it('renders zero for every stat when summary is all-zero', () => {
    const fixture = TestBed.createComponent(StatCards);
    fixture.componentRef.setInput('summary', emptySummary);
    fixture.detectChanges();

    const values = Array.from(
      (fixture.nativeElement as HTMLElement).querySelectorAll('.stat-num'),
    ).map((el) => el.textContent?.trim());

    expect(values).toEqual(['0', '0', '0', '0']);
  });

  it('renders real counts from the summary', () => {
    const fixture = TestBed.createComponent(StatCards);
    fixture.componentRef.setInput('summary', {
      ...emptySummary,
      totalProjects: 12,
      totalApplications: 28,
      totalAmbientes: 54,
      totalServiciosVinculados: 9,
    });
    fixture.detectChanges();

    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).toContain('12');
    expect(text).toContain('28');
    expect(text).toContain('54');
    expect(text).toContain('9');
  });
});
