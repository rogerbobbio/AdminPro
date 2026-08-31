import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DashboardSummary } from '../../../../shared/models/dashboard-summary.model';

@Component({
  selector: 'app-stat-cards',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './stat-cards.html',
  styleUrl: './stat-cards.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class StatCards {
  readonly summary = input.required<DashboardSummary>();
}
