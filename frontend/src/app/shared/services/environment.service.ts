import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { CreateEnvironmentCommand, UpdateEnvironmentCommand } from '../models/project.model';

@Injectable({ providedIn: 'root' })
export class EnvironmentService {
  private readonly http = inject(HttpClient);

  async create(applicationId: number, command: CreateEnvironmentCommand): Promise<number> {
    return firstValueFrom(this.http.post<number>(`/api/applications/${applicationId}/ambientes`, command));
  }

  async update(id: number, command: UpdateEnvironmentCommand): Promise<void> {
    await firstValueFrom(this.http.put(`/api/ambientes/${id}`, command));
  }

  async delete(id: number): Promise<void> {
    await firstValueFrom(this.http.delete(`/api/ambientes/${id}`));
  }
}
