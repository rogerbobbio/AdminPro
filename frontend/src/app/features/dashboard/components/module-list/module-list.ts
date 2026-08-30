import { ChangeDetectionStrategy, Component, inject, input } from '@angular/core';
import { Router } from '@angular/router';
import { Modulo } from '../../../../shared/models/modulo.model';

@Component({
  selector: 'app-module-list',
  standalone: true,
  templateUrl: './module-list.html',
  styleUrl: './module-list.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ModuleList {
  readonly modulos = input.required<Modulo[]>();

  private readonly router = inject(Router);

  onModuleClick(modulo: Modulo): void {
    this.router.navigateByUrl(`/${modulo.rutaBase}`);
  }
}
