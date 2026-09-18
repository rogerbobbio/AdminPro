import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ProjectDetail } from '../../../../shared/models/project.model';

@Component({
  selector: 'app-project-overview',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './project-overview.html',
  styleUrl: './project-overview.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectOverview {
  readonly project = input.required<ProjectDetail>();
}
