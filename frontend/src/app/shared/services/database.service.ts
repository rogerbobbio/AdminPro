import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { CreateBaseDeDatosCommand, UpdateBaseDeDatosCommand } from '../models/project.model';

@Injectable({ providedIn: 'root' })
export class DatabaseService {
  private readonly http = inject(HttpClient);

  async create(projectId: number, command: CreateBaseDeDatosCommand): Promise<number> {
    return firstValueFrom(this.http.post<number>(`/api/projects/${projectId}/basesdedatos`, command));
  }

  async update(id: number, command: UpdateBaseDeDatosCommand): Promise<void> {
    await firstValueFrom(this.http.put(`/api/basesdedatos/${id}`, command));
  }

  async delete(id: number): Promise<void> {
    await firstValueFrom(this.http.delete(`/api/basesdedatos/${id}`));
  }
}
