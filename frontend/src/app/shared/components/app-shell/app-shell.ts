import { ChangeDetectionStrategy, Component, inject, input, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ProjectService } from '../../services/project.service';

export type ShellNavId = 'dashboard' | 'proyectos' | 'servicios' | 'buscar';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './app-shell.html',
  styleUrl: './app-shell.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppShell implements OnInit {
  protected readonly projectService = inject(ProjectService);

  readonly activeNav = input.required<ShellNavId>();

  ngOnInit(): void {
    void this.projectService.loadProjects();
  }
}
