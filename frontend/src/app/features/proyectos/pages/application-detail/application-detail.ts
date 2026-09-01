import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApplicationService } from '../../../../shared/services/application.service';
import { EnvironmentService } from '../../../../shared/services/environment.service';
import { Ambiente } from '../../../../shared/models/project.model';

interface ValidationErrorBody {
  details?: { field: string; error: string }[];
}

@Component({
  selector: 'app-application-detail',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './application-detail.html',
  styleUrl: './application-detail.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApplicationDetail implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly environmentService = inject(EnvironmentService);
  protected readonly applicationService = inject(ApplicationService);

  protected applicationId!: number;

  protected readonly showModal = signal(false);
  protected editingEnvironmentId: number | null = null;

  protected readonly environmentForm = new FormGroup({
    nombre: new FormControl('', { nonNullable: true }),
    url: new FormControl<string | null>(null),
    esWebApi: new FormControl(false, { nonNullable: true }),
    notas: new FormControl<string | null>(null),
  });

  protected readonly urlError = signal<string | null>(null);

  ngOnInit(): void {
    this.applicationId = Number(this.route.snapshot.paramMap.get('id'));
    void this.applicationService.getById(this.applicationId);
  }

  openAddEnvironmentModal(): void {
    this.editingEnvironmentId = null;
    this.environmentForm.reset({ nombre: '', url: null, esWebApi: false, notas: null });
    this.urlError.set(null);
    this.showModal.set(true);
  }

  openEditEnvironmentModal(ambiente: Ambiente): void {
    this.editingEnvironmentId = ambiente.id;
    this.environmentForm.setValue({
      nombre: ambiente.nombre,
      url: ambiente.url,
      esWebApi: ambiente.esWebApi,
      notas: ambiente.notas,
    });
    this.urlError.set(null);
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  async onSaveEnvironment(): Promise<void> {
    this.urlError.set(null);
    const value = this.environmentForm.getRawValue();
    try {
      if (this.editingEnvironmentId !== null) {
        await this.environmentService.update(this.editingEnvironmentId, {
          id: this.editingEnvironmentId,
          ...value,
          orden: 0,
        });
      } else {
        await this.environmentService.create(this.applicationId, { ...value, orden: 0 });
      }
      this.showModal.set(false);
      await this.applicationService.getById(this.applicationId);
    } catch (err) {
      if (err instanceof HttpErrorResponse && err.status === 400) {
        const body = err.error as ValidationErrorBody;
        const urlDetail = (body.details ?? []).find((d) => d.field.toLowerCase() === 'url');
        this.urlError.set(urlDetail?.error ?? 'Datos inválidos.');
      } else {
        throw err;
      }
    }
  }

  async onDeleteEnvironment(id: number): Promise<void> {
    await this.environmentService.delete(id);
    await this.applicationService.getById(this.applicationId);
  }
}
