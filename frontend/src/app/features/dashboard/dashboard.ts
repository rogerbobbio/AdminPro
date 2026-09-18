import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AppShell } from '../../shared/components/app-shell/app-shell';
import { DashboardService } from '../../shared/services/dashboard.service';
import { ProjectService } from '../../shared/services/project.service';
import { ApplicationService } from '../../shared/services/application.service';
import { StatCards } from './components/stat-cards/stat-cards';
import { RecentApplicationsTable } from './components/recent-applications-table/recent-applications-table';
import { ProjectOverview } from './components/project-overview/project-overview';
import { ApplicationOverview } from './components/application-overview/application-overview';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [AppShell, RouterLink, StatCards, RecentApplicationsTable, ProjectOverview, ApplicationOverview],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Dashboard implements OnInit {
  protected readonly dashboardService = inject(DashboardService);
  protected readonly projectService = inject(ProjectService);
  protected readonly applicationService = inject(ApplicationService);

  protected readonly selectedProjectId = signal<number | null>(null);
  protected readonly selectedApplicationId = signal<number | null>(null);

  ngOnInit(): void {
    void this.dashboardService.loadSummary();
    void this.projectService.loadProjects();
  }

  protected onProjectChange(value: string): void {
    this.selectedApplicationId.set(null);
    if (!value) {
      this.selectedProjectId.set(null);
      return;
    }
    const id = Number(value);
    this.selectedProjectId.set(id);
    void this.projectService.getById(id);
  }

  protected onApplicationChange(value: string): void {
    if (!value) {
      this.selectedApplicationId.set(null);
      return;
    }
    const id = Number(value);
    this.selectedApplicationId.set(id);
    void this.applicationService.getById(id);
  }

  protected onClearProject(): void {
    this.selectedProjectId.set(null);
    this.selectedApplicationId.set(null);
  }
}
