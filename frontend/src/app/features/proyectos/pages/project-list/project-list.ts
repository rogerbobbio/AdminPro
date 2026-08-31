import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-project-list',
  standalone: true,
  templateUrl: './project-list.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectList {}
