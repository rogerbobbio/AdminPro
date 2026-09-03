import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { CreateDocumentoCommand, UpdateDocumentoCommand } from '../models/project.model';

@Injectable({ providedIn: 'root' })
export class DocumentoService {
  private readonly http = inject(HttpClient);

  async create(applicationId: number, command: CreateDocumentoCommand): Promise<number> {
    return firstValueFrom(this.http.post<number>(`/api/applications/${applicationId}/documentos`, command));
  }

  async update(id: number, command: UpdateDocumentoCommand): Promise<void> {
    await firstValueFrom(this.http.put(`/api/documentos/${id}`, command));
  }

  async delete(id: number): Promise<void> {
    await firstValueFrom(this.http.delete(`/api/documentos/${id}`));
  }
}
