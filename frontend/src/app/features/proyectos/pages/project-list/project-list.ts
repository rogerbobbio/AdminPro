import { ChangeDetectionStrategy, Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ProjectService } from '../../../../shared/services/project.service';

@Component({
  selector: 'app-project-list',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './project-list.html',
  styleUrl: './project-list.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectList implements OnInit {
  protected readonly projectService = inject(ProjectService);

  protected readonly searchTerm = signal('');

  protected readonly filteredProjects = computed(() => {
    const term = this.searchTerm().trim().toLowerCase();
    const projects = this.projectService.projects();
    if (!term) {
      return projects;
    }
    return projects.filter((p) => p.nombre.toLowerCase().includes(term));
  });

  ngOnInit(): void {
    void this.projectService.loadProjects();
  }

  onSearchInput(event: Event): void {
    this.searchTerm.set((event.target as HTMLInputElement).value);
  }
}
