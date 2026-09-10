import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { Location } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { FormArray, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ApplicationService } from '../../../../shared/services/application.service';
import { EnvironmentService } from '../../../../shared/services/environment.service';
import { NotaService } from '../../../../shared/services/nota.service';
import { ReporteService } from '../../../../shared/services/reporte.service';
import { DocumentoService } from '../../../../shared/services/documento.service';
import { FixDataService } from '../../../../shared/services/fixdata.service';
import { ProjectService } from '../../../../shared/services/project.service';
import { Ambiente, Documento, FixData, Nota, Reporte } from '../../../../shared/models/project.model';

interface ValidationErrorBody {
  details?: { field: string; error: string }[];
}

type AmbienteGroup = FormGroup<{
  id: FormControl<number | null>;
  nombre: FormControl<string>;
  url: FormControl<string>;
  esWebApi: FormControl<boolean>;
}>;

type NotaGroup = FormGroup<{
  id: FormControl<number | null>;
  titulo: FormControl<string>;
  descripcion: FormControl<string>;
}>;

type ReporteGroup = FormGroup<{
  id: FormControl<number | null>;
  reportCode: FormControl<string>;
  reportName: FormControl<string>;
  regionId: FormControl<string | null>;
  reportPath: FormControl<string | null>;
  spTranship: FormControl<string | null>;
  spReportViewer: FormControl<string | null>;
  notas: FormControl<string | null>;
  parametrosEjecucion: FormControl<string | null>;
}>;

type DocumentoGroup = FormGroup<{
  id: FormControl<number | null>;
  nombreArchivo: FormControl<string>;
  urlOneDrive: FormControl<string>;
  tipo: FormControl<string>;
  descripcion: FormControl<string | null>;
}>;

type FixDataGroup = FormGroup<{
  id: FormControl<number | null>;
  nombre: FormControl<string>;
  descripcion: FormControl<string | null>;
  script: FormControl<string | null>;
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
  private readonly notaService = inject(NotaService);
  private readonly reporteService = inject(ReporteService);
  private readonly documentoService = inject(DocumentoService);
  private readonly fixDataService = inject(FixDataService);

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

  protected readonly notasArray = new FormArray<NotaGroup>([]);
  private readonly removedNotaIds: number[] = [];

  protected readonly reportesArray = new FormArray<ReporteGroup>([]);
  private readonly removedReporteIds: number[] = [];

  protected readonly documentosArray = new FormArray<DocumentoGroup>([]);
  private readonly removedDocumentoIds: number[] = [];
  protected readonly tiposDocumento = ['manual', 'diagrama', 'codigo', 'otro'] as const;

  protected readonly fixDatasArray = new FormArray<FixDataGroup>([]);
  private readonly removedFixDataIds: number[] = [];

  protected readonly fieldErrors = signal<Record<string, string>>({});
  protected readonly submitError = signal<string | null>(null);
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
        for (const nota of application.notas) {
          this.notasArray.push(this.createNotaGroup(nota));
        }
        for (const reporte of application.reportes) {
          this.reportesArray.push(this.createReporteGroup(reporte));
        }
        for (const documento of application.documentos) {
          this.documentosArray.push(this.createDocumentoGroup(documento));
        }
        for (const fixData of application.fixDatas) {
          this.fixDatasArray.push(this.createFixDataGroup(fixData));
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
      nombre: new FormControl(ambiente?.nombre ?? '', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      url: new FormControl(ambiente?.url ?? '', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      esWebApi: new FormControl(ambiente?.esWebApi ?? false, { nonNullable: true }),
    });
  }

  addNota(): void {
    this.notasArray.push(this.createNotaGroup());
  }

  removeNota(index: number): void {
    const id = this.notasArray.at(index).controls.id.value;
    if (id !== null) {
      this.removedNotaIds.push(id);
    }
    this.notasArray.removeAt(index);
  }

  private createNotaGroup(nota?: Nota): NotaGroup {
    return new FormGroup({
      id: new FormControl<number | null>(nota?.id ?? null),
      titulo: new FormControl(nota?.titulo ?? '', { nonNullable: true }),
      descripcion: new FormControl(nota?.descripcion ?? '', { nonNullable: true }),
    });
  }

  addReporte(): void {
    this.reportesArray.push(this.createReporteGroup());
  }

  removeReporte(index: number): void {
    const id = this.reportesArray.at(index).controls.id.value;
    if (id !== null) {
      this.removedReporteIds.push(id);
    }
    this.reportesArray.removeAt(index);
  }

  private createReporteGroup(reporte?: Reporte): ReporteGroup {
    return new FormGroup({
      id: new FormControl<number | null>(reporte?.id ?? null),
      reportCode: new FormControl(reporte?.reportCode ?? '', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      reportName: new FormControl(reporte?.reportName ?? '', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      regionId: new FormControl<string | null>(reporte?.regionId ?? null),
      reportPath: new FormControl<string | null>(reporte?.reportPath ?? null),
      spTranship: new FormControl<string | null>(reporte?.spTranship ?? null),
      spReportViewer: new FormControl<string | null>(reporte?.spReportViewer ?? null),
      notas: new FormControl<string | null>(reporte?.notas ?? null),
      parametrosEjecucion: new FormControl<string | null>(reporte?.parametrosEjecucion ?? null),
    });
  }

  addDocumento(): void {
    this.documentosArray.push(this.createDocumentoGroup());
  }

  removeDocumento(index: number): void {
    const id = this.documentosArray.at(index).controls.id.value;
    if (id !== null) {
      this.removedDocumentoIds.push(id);
    }
    this.documentosArray.removeAt(index);
  }

  private createDocumentoGroup(documento?: Documento): DocumentoGroup {
    return new FormGroup({
      id: new FormControl<number | null>(documento?.id ?? null),
      nombreArchivo: new FormControl(documento?.nombreArchivo ?? '', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      urlOneDrive: new FormControl(documento?.urlOneDrive ?? '', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      tipo: new FormControl(documento?.tipo ?? 'manual', { nonNullable: true }),
      descripcion: new FormControl<string | null>(documento?.descripcion ?? null),
    });
  }

  addFixData(): void {
    this.fixDatasArray.push(this.createFixDataGroup());
  }

  removeFixData(index: number): void {
    const id = this.fixDatasArray.at(index).controls.id.value;
    if (id !== null) {
      this.removedFixDataIds.push(id);
    }
    this.fixDatasArray.removeAt(index);
  }

  private createFixDataGroup(fixData?: FixData): FixDataGroup {
    return new FormGroup({
      id: new FormControl<number | null>(fixData?.id ?? null),
      nombre: new FormControl(fixData?.nombre ?? '', {
        nonNullable: true,
        validators: [Validators.required],
      }),
      descripcion: new FormControl<string | null>(fixData?.descripcion ?? null),
      script: new FormControl<string | null>(fixData?.script ?? null),
    });
  }

  cancel(): void {
    this.location.back();
  }

  async onSubmit(): Promise<void> {
    this.fieldErrors.set({});
    this.submitError.set(null);

    if (this.ambientesArray.invalid) {
      this.ambientesArray.markAllAsTouched();
      this.submitError.set('Revisá los ambientes: nombre y URL son obligatorios.');
      return;
    }

    if (this.reportesArray.invalid) {
      this.reportesArray.markAllAsTouched();
      this.submitError.set('Revisá los reportes: código y nombre son obligatorios.');
      return;
    }

    if (this.documentosArray.invalid) {
      this.documentosArray.markAllAsTouched();
      this.submitError.set('Revisá los documentos: nombre y URL son obligatorios.');
      return;
    }

    if (this.fixDatasArray.invalid) {
      this.fixDatasArray.markAllAsTouched();
      this.submitError.set('Revisá los fix datas: el nombre es obligatorio.');
      return;
    }

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
        await this.syncNotas(this.applicationId);
        await this.syncReportes(this.applicationId);
        await this.syncDocumentos(this.applicationId);
        await this.syncFixDatas(this.applicationId);
        await this.router.navigate(['/proyectos/aplicaciones', this.applicationId], { queryParams: { saved: 'updated' } });
      } else {
        const id = await this.applicationService.create(this.projectId, command);
        await this.syncAmbientes(id);
        await this.syncNotas(id);
        await this.syncReportes(id);
        await this.syncDocumentos(id);
        await this.syncFixDatas(id);
        await this.router.navigate(['/proyectos/aplicaciones', id], { queryParams: { saved: 'created' } });
      }
    } catch (err) {
      if (err instanceof HttpErrorResponse && err.status === 400) {
        const body = err.error as ValidationErrorBody;
        const errors: Record<string, string> = {};
        for (const detail of body.details ?? []) {
          errors[detail.field.toLowerCase()] = detail.error;
        }
        if (Object.keys(errors).length > 0) {
          this.fieldErrors.set(errors);
        } else {
          this.submitError.set('No se pudo guardar la aplicación. Verificá los datos ingresados e intentá nuevamente.');
        }
      } else {
        this.submitError.set('No se pudo guardar la aplicación. Intentá nuevamente.');
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

  private async syncNotas(applicationId: number): Promise<void> {
    for (const id of this.removedNotaIds) {
      await this.notaService.delete(id);
    }

    const rows = this.notasArray.getRawValue();
    for (const [index, row] of rows.entries()) {
      if (!row.titulo.trim()) {
        continue;
      }
      if (row.id !== null) {
        await this.notaService.update(row.id, {
          id: row.id,
          titulo: row.titulo,
          descripcion: row.descripcion,
          orden: index,
        });
      } else {
        await this.notaService.create(applicationId, {
          titulo: row.titulo,
          descripcion: row.descripcion,
          orden: index,
        });
      }
    }
  }

  private async syncReportes(applicationId: number): Promise<void> {
    for (const id of this.removedReporteIds) {
      await this.reporteService.delete(id);
    }

    const rows = this.reportesArray.getRawValue();
    for (const row of rows) {
      if (!row.reportCode.trim() || !row.reportName.trim()) {
        continue;
      }
      if (row.id !== null) {
        await this.reporteService.update(row.id, {
          id: row.id,
          reportCode: row.reportCode,
          reportName: row.reportName,
          regionId: row.regionId,
          reportPath: row.reportPath,
          spTranship: row.spTranship,
          spReportViewer: row.spReportViewer,
          notas: row.notas,
          parametrosEjecucion: row.parametrosEjecucion,
        });
      } else {
        await this.reporteService.create(applicationId, {
          reportCode: row.reportCode,
          reportName: row.reportName,
          regionId: row.regionId,
          reportPath: row.reportPath,
          spTranship: row.spTranship,
          spReportViewer: row.spReportViewer,
          notas: row.notas,
          parametrosEjecucion: row.parametrosEjecucion,
        });
      }
    }
  }

  private async syncDocumentos(applicationId: number): Promise<void> {
    for (const id of this.removedDocumentoIds) {
      await this.documentoService.delete(id);
    }

    const rows = this.documentosArray.getRawValue();
    for (const [index, row] of rows.entries()) {
      if (!row.nombreArchivo.trim() || !row.urlOneDrive.trim()) {
        continue;
      }
      if (row.id !== null) {
        await this.documentoService.update(row.id, {
          id: row.id,
          nombreArchivo: row.nombreArchivo,
          urlOneDrive: row.urlOneDrive,
          tipo: row.tipo,
          descripcion: row.descripcion,
          orden: index,
        });
      } else {
        await this.documentoService.create(applicationId, {
          nombreArchivo: row.nombreArchivo,
          urlOneDrive: row.urlOneDrive,
          tipo: row.tipo,
          descripcion: row.descripcion,
          orden: index,
        });
      }
    }
  }

  private async syncFixDatas(applicationId: number): Promise<void> {
    for (const id of this.removedFixDataIds) {
      await this.fixDataService.delete(id);
    }

    const rows = this.fixDatasArray.getRawValue();
    for (const [index, row] of rows.entries()) {
      if (!row.nombre.trim()) {
        continue;
      }
      if (row.id !== null) {
        await this.fixDataService.update(row.id, {
          id: row.id,
          nombre: row.nombre,
          descripcion: row.descripcion,
          script: row.script,
          orden: index,
        });
      } else {
        await this.fixDataService.create(applicationId, {
          nombre: row.nombre,
          descripcion: row.descripcion,
          script: row.script,
          orden: index,
        });
      }
    }
  }
}
