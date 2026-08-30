import { TestBed } from '@angular/core/testing';
import { StatusDonut } from './status-donut';

describe('StatusDonut', () => {
  it('renders a 100% conic-gradient and center label for the all-active placeholder', () => {
    const fixture = TestBed.createComponent(StatusDonut);
    fixture.componentRef.setInput('breakdown', { activo: 3, enProgreso: 0, pendiente: 0 });
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const donut = compiled.querySelector<HTMLElement>('.donut');
    const label = compiled.querySelector<HTMLElement>('.donut-inner strong');

    expect(donut?.style.background).toContain('100%');
    expect(label?.textContent?.trim()).toBe('100%');
  });

  it('renders 0% when there is no data yet', () => {
    const fixture = TestBed.createComponent(StatusDonut);
    fixture.componentRef.setInput('breakdown', { activo: 0, enProgreso: 0, pendiente: 0 });
    fixture.detectChanges();

    const label = (fixture.nativeElement as HTMLElement).querySelector<HTMLElement>(
      '.donut-inner strong',
    );

    expect(label?.textContent?.trim()).toBe('0%');
  });
});
