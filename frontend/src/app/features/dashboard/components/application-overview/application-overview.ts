import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { ApplicationDetail } from '../../../../shared/models/project.model';

@Component({
  selector: 'app-application-overview',
  standalone: true,
  imports: [],
  templateUrl: './application-overview.html',
  styleUrl: './application-overview.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApplicationOverview {
  readonly application = input.required<ApplicationDetail>();
}
