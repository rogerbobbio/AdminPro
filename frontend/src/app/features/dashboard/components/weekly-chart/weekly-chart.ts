import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

const DAY_LABELS = ['L', 'M', 'M', 'J', 'V', 'S', 'D'];
const MIN_HEIGHT_PERCENT = 6;

@Component({
  selector: 'app-weekly-chart',
  standalone: true,
  templateUrl: './weekly-chart.html',
  styleUrl: './weekly-chart.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class WeeklyChart {
  readonly values = input.required<number[]>();

  readonly bars = computed(() => {
    const values = this.values();
    const max = Math.max(...values, 0);

    return values.map((value, index) => ({
      label: DAY_LABELS[index] ?? '',
      height: max === 0 ? MIN_HEIGHT_PERCENT : Math.max((value / max) * 100, MIN_HEIGHT_PERCENT),
      filled: value > 0,
      strong: max > 0 && value === max,
    }));
  });
}
