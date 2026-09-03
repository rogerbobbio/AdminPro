import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { CreateFixDataCommand, UpdateFixDataCommand } from '../models/project.model';

@Injectable({ providedIn: 'root' })
export class FixDataService {
  private readonly http = inject(HttpClient);

  async create(applicationId: number, command: CreateFixDataCommand): Promise<number> {
    return firstValueFrom(this.http.post<number>(`/api/applications/${applicationId}/fixdatas`, command));
  }

  async update(id: number, command: UpdateFixDataCommand): Promise<void> {
    await firstValueFrom(this.http.put(`/api/fixdatas/${id}`, command));
  }

  async delete(id: number): Promise<void> {
    await firstValueFrom(this.http.delete(`/api/fixdatas/${id}`));
  }
}
