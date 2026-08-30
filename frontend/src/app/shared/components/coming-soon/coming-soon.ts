import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { AppShell, ShellNavId } from '../app-shell/app-shell';

@Component({
  selector: 'app-coming-soon',
  standalone: true,
  imports: [AppShell],
  templateUrl: './coming-soon.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ComingSoon {
  readonly activeNav = input.required<ShellNavId>();
  readonly title = input.required<string>();
}
