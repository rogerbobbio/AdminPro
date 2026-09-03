import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { Location } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { FormArray, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApplicationService } from '../../../../shared/services/application.service';
import { EnvironmentService } from '../../../../shared/services/environment.service';
import { ProjectService } from '../../../../shared/services/project.service';
import { Ambiente } from '../../../../shared/models/project.model';

interface ValidationErrorBody {
  details?: { field: string; error: string }[];
}

type AmbienteGroup = FormGroup<{
  id: FormControl<number | null>;
  nombre: FormControl<string>;
  url: FormControl<string | null>;
  esWebApi: FormControl<boolean>;
}>;

const TIPOS_APLICACION = ['Web', 'API', 'Mobile'] as const;
type TipoAplicacion = (typeof TIPOS_APLICACION)[number];

@Component({
  selector: 'app-application-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './application-form.html',
  styleUrl: './application-form.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApplicationForm implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly location = inject(Location);
  protected readonly applicationService = inject(ApplicationService);
  protected readonly projectService = inject(ProjectService);
  private readonly environmentService = inject(EnvironmentService);

  private applicationId: number | null = null;
  private projectId!: number;

  protected readonly form = new FormGroup({
    nombre: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    descripcion: new FormControl<string | null>(null),
    tipo: new FormControl<TipoAplicacion>('Web', { nonNullable: true }),
    tecnologiaFront: new FormControl<string | null>(null),
    tecnologiaBack: new FormControl<string | null>(null),
    ramaDesarrollo: new FormControl<string | null>(null),
    rutaLocal: new FormControl<string | null>(null),
    rutaGit: new FormControl<string | null>(null),
    comoSeLevanta: new FormControl<string | null>(null),
  });

  protected readonly ambientesArray = new FormArray<AmbienteGroup>([]);
  private readonly removedAmbienteIds: number[] = [];

  protected readonly fieldErrors = signal<Record<string, string>>({});
  protected readonly isEditMode = signal(false);

  protected readonly tiposDisponibles = TIPOS_APLICACION;

  async ngOnInit(): Promise<void> {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.applicationId = Number(idParam);
      this.isEditMode.set(true);
      await this.applicationService.getById(this.applicationId);
      const application = this.applicationService.selectedApplication();
      if (application) {
        this.projectId = application.proyectoId;
        this.form.setValue({
          nombre: application.nombre,
          descripcion: application.descripcion,
          tipo: this.isTipoAplicacion(application.tipo) ? application.tipo : 'Web',
          tecnologiaFront: application.tecnologiaFront,
          tecnologiaBack: application.tecnologiaBack,
          ramaDesarrollo: application.ramaDesarrollo,
          rutaLocal: application.rutaLocal,
          rutaGit: application.rutaGit,
          comoSeLevanta: application.comoSeLevanta,
        });
        for (const ambiente of application.ambientes) {
          this.ambientesArray.push(this.createAmbienteGroup(ambiente));
        }
      }
    } else {
      this.projectId = Number(this.route.snapshot.queryParamMap.get('proyectoId'));
    }

    await this.projectService.getById(this.projectId);
  }

  selectTipo(tipo: TipoAplicacion): void {
    this.form.controls.tipo.setValue(tipo);
  }

  private isTipoAplicacion(value: string | null): value is TipoAplicacion {
    return (TIPOS_APLICACION as readonly string[]).includes(value ?? '');
  }

  addAmbiente(): void {
    this.ambientesArray.push(this.createAmbienteGroup());
  }

  removeAmbiente(index: number): void {
    const id = this.ambientesArray.at(index).controls.id.value;
    if (id !== null) {
      this.removedAmbienteIds.push(id);
    }
    this.ambientesArray.removeAt(index);
  }

  private createAmbienteGroup(ambiente?: Ambiente): AmbienteGroup {
    return new FormGroup({
      id: new FormControl<number | null>(ambiente?.id ?? null),
      nombre: new FormControl(ambiente?.nombre ?? '', { nonNullable: true }),
      url: new FormControl<string | null>(ambiente?.url ?? null),
      esWebApi: new FormControl(ambiente?.esWebApi ?? false, { nonNullable: true }),
    });
  }

  cancel(): void {
    this.location.back();
  }

  async onSubmit(): Promise<void> {
    this.fieldErrors.set({});
    const value = this.form.getRawValue();
    const command = {
      ...value,
      applicationName: value.nombre,
      notasCompilacion: value.comoSeLevanta,
      orden: 0,
    };

    try {
      if (this.isEditMode() && this.applicationId !== null) {
        await this.applicationService.update(this.applicationId, { id: this.applicationId, ...command });
        await this.syncAmbientes(this.applicationId);
        await this.router.navigate(['/proyectos/aplicaciones', this.applicationId]);
      } else {
        const id = await this.applicationService.create(this.projectId, command);
        await this.syncAmbientes(id);
        await this.router.navigate(['/proyectos/aplicaciones', id]);
      }
    } catch (err) {
      if (err instanceof HttpErrorResponse && err.status === 400) {
        const body = err.error as ValidationErrorBody;
        const errors: Record<string, string> = {};
        for (const detail of body.details ?? []) {
          errors[detail.field.toLowerCase()] = detail.error;
        }
        this.fieldErrors.set(errors);
      } else {
        throw err;
      }
    }
  }

  private async syncAmbientes(applicationId: number): Promise<void> {
    for (const id of this.removedAmbienteIds) {
      await this.environmentService.delete(id);
    }

    const rows = this.ambientesArray.getRawValue();
    for (const [index, row] of rows.entries()) {
      if (!row.nombre.trim()) {
        continue;
      }
      if (row.id !== null) {
        await this.environmentService.update(row.id, {
          id: row.id,
          nombre: row.nombre,
          url: row.url,
          esWebApi: row.esWebApi,
          orden: index,
        });
      } else {
        await this.environmentService.create(applicationId, {
          nombre: row.nombre,
          url: row.url,
          esWebApi: row.esWebApi,
          orden: index,
        });
      }
    }
  }
}
