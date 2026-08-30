import { TestBed } from '@angular/core/testing';
import { WeeklyChart } from './weekly-chart';

describe('WeeklyChart', () => {
  it('renders 7 bars at minimum height when all values are 0', () => {
    const fixture = TestBed.createComponent(WeeklyChart);
    fixture.componentRef.setInput('values', [0, 0, 0, 0, 0, 0, 0]);
    fixture.detectChanges();

    const bars = (fixture.nativeElement as HTMLElement).querySelectorAll<HTMLElement>('.bar');

    expect(bars.length).toBe(7);
    bars.forEach((bar) => {
      const height = parseFloat(bar.style.height);
      expect(height).toBeGreaterThan(0);
      expect(height).toBeLessThanOrEqual(10);
    });
  });

  it('renders the tallest bar at 100% height', () => {
    const fixture = TestBed.createComponent(WeeklyChart);
    fixture.componentRef.setInput('values', [1, 2, 3, 10, 4, 2, 1]);
    fixture.detectChanges();

    const bars = (fixture.nativeElement as HTMLElement).querySelectorAll<HTMLElement>('.bar');
    const heights = Array.from(bars).map((bar) => parseFloat(bar.style.height));

    expect(Math.max(...heights)).toBe(100);
  });
});
