import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RecentApplication } from '../../../../shared/models/dashboard-summary.model';

@Component({
  selector: 'app-recent-applications-table',
  standalone: true,
  templateUrl: './recent-applications-table.html',
  styleUrl: './recent-applications-table.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RecentApplicationsTable {
  readonly applications = input.required<RecentApplication[]>();
}
