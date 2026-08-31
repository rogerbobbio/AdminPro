import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-project-form',
  standalone: true,
  templateUrl: './project-form.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectForm {}
