import { ChangeDetectionStrategy, Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AppShell } from '../../shared/components/app-shell/app-shell';
import { ModuloService } from '../../shared/services/modulo.service';
import { DashboardService } from '../../shared/services/dashboard.service';
import { StatCards } from './components/stat-cards/stat-cards';
import { WeeklyChart } from './components/weekly-chart/weekly-chart';
import { ReminderCard } from './components/reminder-card/reminder-card';
import { ModuleList } from './components/module-list/module-list';
import { RecentApplicationsTable } from './components/recent-applications-table/recent-applications-table';
import { StatusDonut } from './components/status-donut/status-donut';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    AppShell,
    RouterLink,
    StatCards,
    WeeklyChart,
    ReminderCard,
    ModuleList,
    RecentApplicationsTable,
    StatusDonut,
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Dashboard implements OnInit {
  protected readonly moduloService = inject(ModuloService);
  protected readonly dashboardService = inject(DashboardService);

  ngOnInit(): void {
    void this.moduloService.loadModulos();
    void this.dashboardService.loadSummary();
  }
}
