import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-reminder-card',
  standalone: true,
  templateUrl: './reminder-card.html',
  styleUrl: './reminder-card.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ReminderCard {}
