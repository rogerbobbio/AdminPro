import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

export type ShellNavId = 'dashboard' | 'proyectos' | 'servicios' | 'buscar';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './app-shell.html',
  styleUrl: './app-shell.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppShell {
  readonly activeNav = input.required<ShellNavId>();
}
