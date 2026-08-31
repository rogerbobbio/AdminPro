import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AppShell } from '../../shared/components/app-shell/app-shell';

@Component({
  selector: 'app-proyectos-layout',
  standalone: true,
  imports: [AppShell, RouterOutlet],
  templateUrl: './proyectos-layout.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProyectosLayout {}
