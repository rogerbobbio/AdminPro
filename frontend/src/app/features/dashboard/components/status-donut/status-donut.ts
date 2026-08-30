import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { ApplicationStatusBreakdown } from '../../../../shared/models/dashboard-summary.model';

@Component({
  selector: 'app-status-donut',
  standalone: true,
  templateUrl: './status-donut.html',
  styleUrl: './status-donut.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StatusDonut {
  readonly breakdown = input.required<ApplicationStatusBreakdown>();

  private readonly total = computed(() => {
    const b = this.breakdown();
    return b.activo + b.enProgreso + b.pendiente;
  });

  readonly activoPercent = computed(() => this.percentOf(this.breakdown().activo));
  readonly enProgresoPercent = computed(() => this.percentOf(this.breakdown().enProgreso));

  readonly gradient = computed(() => {
    const activoEnd = this.activoPercent();
    const enProgresoEnd = activoEnd + this.enProgresoPercent();
    return `conic-gradient(var(--ap-g-900) 0% ${activoEnd}%, var(--ap-g-500) ${activoEnd}% ${enProgresoEnd}%, #E9EDEB ${enProgresoEnd}% 100%)`;
  });

  private percentOf(value: number): number {
    const total = this.total();
    return total === 0 ? 0 : Math.round((value / total) * 100);
  }
}
