import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { CreateNotaCommand, UpdateNotaCommand } from '../models/project.model';

@Injectable({ providedIn: 'root' })
export class NotaService {
  private readonly http = inject(HttpClient);

  async create(applicationId: number, command: CreateNotaCommand): Promise<number> {
    return firstValueFrom(this.http.post<number>(`/api/applications/${applicationId}/notas`, command));
  }

  async update(id: number, command: UpdateNotaCommand): Promise<void> {
    await firstValueFrom(this.http.put(`/api/notas/${id}`, command));
  }

  async delete(id: number): Promise<void> {
    await firstValueFrom(this.http.delete(`/api/notas/${id}`));
  }
}
