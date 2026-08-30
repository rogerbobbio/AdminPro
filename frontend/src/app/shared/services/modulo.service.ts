import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Modulo } from '../models/modulo.model';

@Injectable({ providedIn: 'root' })
export class ModuloService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/modulos';

  readonly modulos = signal<Modulo[]>([]);

  async loadModulos(): Promise<void> {
    const data = await firstValueFrom(this.http.get<Modulo[]>(this.apiUrl));
    this.modulos.set(data);
  }
}
