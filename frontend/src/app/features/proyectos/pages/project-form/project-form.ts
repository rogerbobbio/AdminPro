import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { Location } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ProjectService } from '../../../../shared/services/project.service';

interface ValidationErrorBody {
  details?: { field: string; error: string }[];
}

@Component({
  selector: 'app-project-form',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './project-form.html',
  styleUrl: './project-form.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectForm implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly location = inject(Location);
  protected readonly projectService = inject(ProjectService);

  private projectId: number | null = null;

  protected readonly form = new FormGroup({
    nombre: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    descripcion: new FormControl<string | null>(null),
  });

  protected readonly fieldErrors = signal<Record<string, string>>({});
  protected readonly isEditMode = signal(false);

  async ngOnInit(): Promise<void> {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.projectId = Number(idParam);
      this.isEditMode.set(true);
      await this.projectService.getById(this.projectId);
      const project = this.projectService.selectedProject();
      if (project) {
        this.form.setValue({ nombre: project.nombre, descripcion: project.descripcion });
      }
    }
  }

  cancel(): void {
    this.location.back();
  }

  async onSubmit(): Promise<void> {
    this.fieldErrors.set({});
    const { nombre, descripcion } = this.form.getRawValue();

    try {
      if (this.isEditMode() && this.projectId !== null) {
        await this.projectService.update(this.projectId, { id: this.projectId, nombre, descripcion });
        await this.router.navigate(['/proyectos', this.projectId]);
      } else {
        const id = await this.projectService.create({ nombre, descripcion });
        await this.router.navigate(['/proyectos', id]);
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
