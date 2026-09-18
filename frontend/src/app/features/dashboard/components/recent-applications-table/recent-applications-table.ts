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

  protected relativeTime(isoDate: string): string {
    const diffMs = Date.now() - new Date(isoDate).getTime();
    const diffMinutes = Math.round(diffMs / 60_000);

    if (diffMinutes < 1) {
      return 'hace un momento';
    }
    if (diffMinutes < 60) {
      return `hace ${diffMinutes} min`;
    }

    const diffHours = Math.round(diffMinutes / 60);
    if (diffHours < 24) {
      return `hace ${diffHours} hora${diffHours === 1 ? '' : 's'}`;
    }

    const diffDays = Math.round(diffHours / 24);
    if (diffDays === 1) {
      return 'ayer';
    }
    if (diffDays < 7) {
      return `hace ${diffDays} días`;
    }

    const diffWeeks = Math.round(diffDays / 7);
    if (diffWeeks < 5) {
      return `hace ${diffWeeks} semana${diffWeeks === 1 ? '' : 's'}`;
    }

    const diffMonths = Math.round(diffDays / 30);
    return `hace ${diffMonths} mes${diffMonths === 1 ? '' : 'es'}`;
  }
}
