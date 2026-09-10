import { ChangeDetectionStrategy, Component, inject, input, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ProjectService } from '../../services/project.service';
import { ThemeService } from '../../services/theme.service';

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
  protected readonly themeService = inject(ThemeService);

  readonly activeNav = input.required<ShellNavId>();
  protected readonly isSidebarCollapsed = signal(false);

  ngOnInit(): void {
    void this.projectService.loadProjects();
  }

  protected toggleSidebar(): void {
    this.isSidebarCollapsed.update((collapsed) => !collapsed);
  }

  protected toggleTheme(): void {
    this.themeService.toggle();
  }
}
