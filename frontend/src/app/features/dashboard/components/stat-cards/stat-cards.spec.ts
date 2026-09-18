import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { StatCards } from './stat-cards';
import { DashboardSummary } from '../../../../shared/models/dashboard-summary.model';

const emptySummary: DashboardSummary = {
  totalProjects: 0,
  totalApplications: 0,
  recentApplications: [],
};

describe('StatCards', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideRouter([])],
    });
  });

  it('renders zero for every stat when summary is all-zero', () => {
    const fixture = TestBed.createComponent(StatCards);
    fixture.componentRef.setInput('summary', emptySummary);
    fixture.detectChanges();

    const values = Array.from(
      (fixture.nativeElement as HTMLElement).querySelectorAll('.stat-num'),
    ).map((el) => el.textContent?.trim());

    expect(values).toEqual(['0', '0']);
  });

  it('renders real counts from the summary', () => {
    const fixture = TestBed.createComponent(StatCards);
    fixture.componentRef.setInput('summary', {
      ...emptySummary,
      totalProjects: 12,
      totalApplications: 28,
    });
    fixture.detectChanges();

    const text = (fixture.nativeElement as HTMLElement).textContent ?? '';
    expect(text).toContain('12');
    expect(text).toContain('28');
  });

  it('links each arrow to its destination screen', () => {
    const fixture = TestBed.createComponent(StatCards);
    fixture.componentRef.setInput('summary', emptySummary);
    fixture.detectChanges();

    const arrows = Array.from(
      (fixture.nativeElement as HTMLElement).querySelectorAll<HTMLAnchorElement>('.stat-arrow'),
    ).map((a) => a.getAttribute('href'));

    expect(arrows).toEqual(['/proyectos', '/proyectos']);
  });
});
