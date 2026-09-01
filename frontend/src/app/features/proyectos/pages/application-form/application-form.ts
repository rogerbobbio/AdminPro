import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ApplicationService } from '../../../../shared/services/application.service';

interface ValidationErrorBody {
  details?: { field: string; error: string }[];
}

@Component({
  selector: 'app-application-form',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './application-form.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApplicationForm implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly applicationService = inject(ApplicationService);

  private applicationId: number | null = null;
  private projectId!: number;

  protected readonly form = new FormGroup({
    nombre: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    descripcion: new FormControl<string | null>(null),
    tecnologiaFront: new FormControl<string | null>(null),
    tecnologiaBack: new FormControl<string | null>(null),
    ramaDesarrollo: new FormControl<string | null>(null),
    applicationName: new FormControl<string | null>(null),
    tieneProyectoBD: new FormControl<string | null>(null),
    rutaLocal: new FormControl<string | null>(null),
    rutaGit: new FormControl<string | null>(null),
    comoSeLevanta: new FormControl<string | null>(null),
    notasCompilacion: new FormControl<string | null>(null),
  });

  protected readonly fieldErrors = signal<Record<string, string>>({});
  protected readonly isEditMode = signal(false);
  protected readonly showDetallesTecnicos = signal(false);

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
          tecnologiaFront: application.tecnologiaFront,
          tecnologiaBack: application.tecnologiaBack,
          ramaDesarrollo: application.ramaDesarrollo,
          applicationName: application.applicationName,
          tieneProyectoBD: application.tieneProyectoBD,
          rutaLocal: application.rutaLocal,
          rutaGit: application.rutaGit,
          comoSeLevanta: application.comoSeLevanta,
          notasCompilacion: application.notasCompilacion,
        });
      }
    } else {
      this.projectId = Number(this.route.snapshot.queryParamMap.get('proyectoId'));
    }
  }

  toggleDetallesTecnicos(): void {
    this.showDetallesTecnicos.update((value) => !value);
  }

  async onSubmit(): Promise<void> {
    this.fieldErrors.set({});
    const value = this.form.getRawValue();

    try {
      if (this.isEditMode() && this.applicationId !== null) {
        await this.applicationService.update(this.applicationId, { id: this.applicationId, ...value, orden: 0 });
        await this.router.navigate(['/proyectos/aplicaciones', this.applicationId]);
      } else {
        const id = await this.applicationService.create(this.projectId, { ...value, orden: 0 });
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
}
