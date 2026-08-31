import { ChangeDetectionStrategy, Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProjectService } from '../../../../shared/services/project.service';

@Component({
  selector: 'app-project-detail',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './project-detail.html',
  styleUrl: './project-detail.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectDetail implements OnInit {
  private readonly route = inject(ActivatedRoute);
  protected readonly projectService = inject(ProjectService);

  protected projectId!: number;

  ngOnInit(): void {
    this.projectId = Number(this.route.snapshot.paramMap.get('id'));
    void this.projectService.getById(this.projectId);
  }
}
