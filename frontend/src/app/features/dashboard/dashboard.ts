import { ChangeDetectionStrategy, Component } from '@angular/core';
import { AppShell } from '../../shared/components/app-shell/app-shell';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [AppShell],
  templateUrl: './dashboard.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Dashboard {}
