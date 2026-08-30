import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { DashboardSummary } from '../../../../shared/models/dashboard-summary.model';

@Component({
  selector: 'app-stat-cards',
  standalone: true,
  templateUrl: './stat-cards.html',
  styleUrl: './stat-cards.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StatCards {
  readonly summary = input.required<DashboardSummary>();
}
