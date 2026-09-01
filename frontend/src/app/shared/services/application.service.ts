import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import {
  ApplicationDetail,
  ApplicationSummary,
  CreateApplicationCommand,
  UpdateApplicationCommand,
} from '../models/project.model';

@Injectable({ providedIn: 'root' })
export class ApplicationService {
  private readonly http = inject(HttpClient);

  readonly applications = signal<ApplicationSummary[]>([]);
  readonly selectedApplication = signal<ApplicationDetail | null>(null);

  async loadByProject(projectId: number): Promise<void> {
    const data = await firstValueFrom(
      this.http.get<ApplicationSummary[]>(`/api/projects/${projectId}/applications`),
    );
    this.applications.set(data);
  }

  async getById(id: number): Promise<void> {
    const data = await firstValueFrom(this.http.get<ApplicationDetail>(`/api/applications/${id}`));
    this.selectedApplication.set(data);
  }

  async create(projectId: number, command: CreateApplicationCommand): Promise<number> {
    return firstValueFrom(this.http.post<number>(`/api/projects/${projectId}/applications`, command));
  }

  async update(id: number, command: UpdateApplicationCommand): Promise<void> {
    await firstValueFrom(this.http.put(`/api/applications/${id}`, command));
  }

  async delete(id: number): Promise<void> {
    await firstValueFrom(this.http.delete(`/api/applications/${id}`));
  }
}
