import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { CreateReporteCommand, UpdateReporteCommand } from '../models/project.model';

@Injectable({ providedIn: 'root' })
export class ReporteService {
  private readonly http = inject(HttpClient);

  async create(applicationId: number, command: CreateReporteCommand): Promise<number> {
    return firstValueFrom(this.http.post<number>(`/api/applications/${applicationId}/reportes`, command));
  }

  async update(id: number, command: UpdateReporteCommand): Promise<void> {
    await firstValueFrom(this.http.put(`/api/reportes/${id}`, command));
  }

  async delete(id: number): Promise<void> {
    await firstValueFrom(this.http.delete(`/api/reportes/${id}`));
  }
}
