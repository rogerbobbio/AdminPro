import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import {
  CreateProjectCommand,
  ProjectDetail,
  ProjectSummary,
  UpdateProjectCommand,
} from '../models/project.model';

@Injectable({ providedIn: 'root' })
export class ProjectService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/projects';

  readonly projects = signal<ProjectSummary[]>([]);
  readonly selectedProject = signal<ProjectDetail | null>(null);

  async loadProjects(): Promise<void> {
    const data = await firstValueFrom(this.http.get<ProjectSummary[]>(this.apiUrl));
    this.projects.set(data);
  }

  async getById(id: number): Promise<void> {
    const data = await firstValueFrom(this.http.get<ProjectDetail>(`${this.apiUrl}/${id}`));
    this.selectedProject.set(data);
  }

  async create(command: CreateProjectCommand): Promise<number> {
    const id = await firstValueFrom(this.http.post<number>(this.apiUrl, command));
    await this.loadProjects();
    return id;
  }

  async update(id: number, command: UpdateProjectCommand): Promise<void> {
    await firstValueFrom(this.http.put(`${this.apiUrl}/${id}`, command));
    await this.loadProjects();
  }

  async delete(id: number): Promise<void> {
    await firstValueFrom(this.http.delete(`${this.apiUrl}/${id}`));
    this.projects.update((list) => list.filter((p) => p.id !== id));
  }
}
