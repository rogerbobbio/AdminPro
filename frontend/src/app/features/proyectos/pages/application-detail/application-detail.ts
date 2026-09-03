import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ApplicationService } from '../../../../shared/services/application.service';
import { EnvironmentService } from '../../../../shared/services/environment.service';
import { ReporteService } from '../../../../shared/services/reporte.service';
import { NotaService } from '../../../../shared/services/nota.service';
import { DocumentoService } from '../../../../shared/services/documento.service';
import { FixDataService } from '../../../../shared/services/fixdata.service';
import { Ambiente, Documento, FixData, Nota, Reporte } from '../../../../shared/models/project.model';

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
  private readonly reporteService = inject(ReporteService);
  private readonly notaService = inject(NotaService);
  private readonly documentoService = inject(DocumentoService);
  private readonly fixDataService = inject(FixDataService);
  protected readonly applicationService = inject(ApplicationService);

  protected applicationId!: number;

  // Ambientes
  protected readonly showModal = signal(false);
  protected editingEnvironmentId: number | null = null;

  protected readonly environmentForm = new FormGroup({
    nombre: new FormControl('', { nonNullable: true }),
    url: new FormControl<string | null>(null),
    esWebApi: new FormControl(false, { nonNullable: true }),
    notas: new FormControl<string | null>(null),
  });

  protected readonly urlError = signal<string | null>(null);

  // Reportes
  protected readonly showReporteModal = signal(false);
  protected editingReporteId: number | null = null;

  protected readonly reporteForm = new FormGroup({
    reportCode: new FormControl('', { nonNullable: true }),
    reportName: new FormControl('', { nonNullable: true }),
    regionId: new FormControl<string | null>(null),
    reportPath: new FormControl<string | null>(null),
    spTranship: new FormControl<string | null>(null),
    spReportViewer: new FormControl<string | null>(null),
    notas: new FormControl<string | null>(null),
    parametrosEjemplo: new FormControl<string | null>(null),
  });

  protected readonly reportCodeError = signal<string | null>(null);

  // Notas
  protected readonly showNotaModal = signal(false);
  protected editingNotaId: number | null = null;
  protected readonly expandedNotaIds = signal<ReadonlySet<number>>(new Set());

  protected readonly notaForm = new FormGroup({
    titulo: new FormControl('', { nonNullable: true }),
    descripcion: new FormControl('', { nonNullable: true }),
  });

  // Documentos
  protected readonly showDocumentoModal = signal(false);
  protected editingDocumentoId: number | null = null;
  protected readonly tiposDocumento = ['manual', 'diagrama', 'codigo', 'otro'] as const;

  protected readonly documentoForm = new FormGroup({
    nombreArchivo: new FormControl('', { nonNullable: true }),
    urlOneDrive: new FormControl('', { nonNullable: true }),
    tipo: new FormControl('manual', { nonNullable: true }),
    descripcion: new FormControl<string | null>(null),
  });

  protected readonly documentoUrlError = signal<string | null>(null);

  // FixDatas
  protected readonly showFixDataModal = signal(false);
  protected editingFixDataId: number | null = null;

  protected readonly fixDataForm = new FormGroup({
    nombre: new FormControl('', { nonNullable: true }),
    descripcion: new FormControl<string | null>(null),
    script: new FormControl<string | null>(null),
  });

  ngOnInit(): void {
    this.applicationId = Number(this.route.snapshot.paramMap.get('id'));
    void this.applicationService.getById(this.applicationId);
  }

  private async reload(): Promise<void> {
    await this.applicationService.getById(this.applicationId);
  }

  // Ambientes
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
      await this.reload();
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
    await this.reload();
  }

  // Reportes
  openAddReporteModal(): void {
    this.editingReporteId = null;
    this.reporteForm.reset({
      reportCode: '',
      reportName: '',
      regionId: null,
      reportPath: null,
      spTranship: null,
      spReportViewer: null,
      notas: null,
      parametrosEjemplo: null,
    });
    this.reportCodeError.set(null);
    this.showReporteModal.set(true);
  }

  openEditReporteModal(reporte: Reporte): void {
    this.editingReporteId = reporte.id;
    this.reporteForm.setValue({
      reportCode: reporte.reportCode,
      reportName: reporte.reportName,
      regionId: reporte.regionId,
      reportPath: reporte.reportPath,
      spTranship: reporte.spTranship,
      spReportViewer: reporte.spReportViewer,
      notas: reporte.notas,
      parametrosEjemplo: reporte.parametrosEjemplo,
    });
    this.reportCodeError.set(null);
    this.showReporteModal.set(true);
  }

  closeReporteModal(): void {
    this.showReporteModal.set(false);
  }

  async onSaveReporte(): Promise<void> {
    this.reportCodeError.set(null);
    const value = this.reporteForm.getRawValue();
    try {
      if (this.editingReporteId !== null) {
        await this.reporteService.update(this.editingReporteId, { id: this.editingReporteId, ...value });
      } else {
        await this.reporteService.create(this.applicationId, value);
      }
      this.showReporteModal.set(false);
      await this.reload();
    } catch (err) {
      if (err instanceof HttpErrorResponse && err.status === 400) {
        const body = err.error as ValidationErrorBody;
        const detail = (body.details ?? []).find((d) => d.field.toLowerCase() === 'reportcode');
        this.reportCodeError.set(detail?.error ?? 'Datos inválidos.');
      } else {
        throw err;
      }
    }
  }

  async onDeleteReporte(id: number): Promise<void> {
    await this.reporteService.delete(id);
    await this.reload();
  }

  // Notas
  toggleNota(id: number): void {
    this.expandedNotaIds.update((current) => {
      const next = new Set(current);
      if (next.has(id)) {
        next.delete(id);
      } else {
        next.add(id);
      }
      return next;
    });
  }

  isNotaExpanded(id: number): boolean {
    return this.expandedNotaIds().has(id);
  }

  openAddNotaModal(): void {
    this.editingNotaId = null;
    this.notaForm.reset({ titulo: '', descripcion: '' });
    this.showNotaModal.set(true);
  }

  openEditNotaModal(nota: Nota): void {
    this.editingNotaId = nota.id;
    this.notaForm.setValue({ titulo: nota.titulo, descripcion: nota.descripcion });
    this.showNotaModal.set(true);
  }

  closeNotaModal(): void {
    this.showNotaModal.set(false);
  }

  async onSaveNota(): Promise<void> {
    const value = this.notaForm.getRawValue();
    if (this.editingNotaId !== null) {
      await this.notaService.update(this.editingNotaId, { id: this.editingNotaId, ...value, orden: 0 });
    } else {
      await this.notaService.create(this.applicationId, { ...value, orden: 0 });
    }
    this.showNotaModal.set(false);
    await this.reload();
  }

  async onDeleteNota(id: number): Promise<void> {
    await this.notaService.delete(id);
    await this.reload();
  }

  // Documentos
  openAddDocumentoModal(): void {
    this.editingDocumentoId = null;
    this.documentoForm.reset({ nombreArchivo: '', urlOneDrive: '', tipo: 'manual', descripcion: null });
    this.documentoUrlError.set(null);
    this.showDocumentoModal.set(true);
  }

  openEditDocumentoModal(documento: Documento): void {
    this.editingDocumentoId = documento.id;
    this.documentoForm.setValue({
      nombreArchivo: documento.nombreArchivo,
      urlOneDrive: documento.urlOneDrive,
      tipo: documento.tipo,
      descripcion: documento.descripcion,
    });
    this.documentoUrlError.set(null);
    this.showDocumentoModal.set(true);
  }

  closeDocumentoModal(): void {
    this.showDocumentoModal.set(false);
  }

  async onSaveDocumento(): Promise<void> {
    this.documentoUrlError.set(null);
    const value = this.documentoForm.getRawValue();
    try {
      if (this.editingDocumentoId !== null) {
        await this.documentoService.update(this.editingDocumentoId, { id: this.editingDocumentoId, ...value, orden: 0 });
      } else {
        await this.documentoService.create(this.applicationId, { ...value, orden: 0 });
      }
      this.showDocumentoModal.set(false);
      await this.reload();
    } catch (err) {
      if (err instanceof HttpErrorResponse && err.status === 400) {
        const body = err.error as ValidationErrorBody;
        const detail = (body.details ?? []).find((d) => d.field.toLowerCase() === 'urlonedrive');
        this.documentoUrlError.set(detail?.error ?? 'Datos inválidos.');
      } else {
        throw err;
      }
    }
  }

  async onDeleteDocumento(id: number): Promise<void> {
    await this.documentoService.delete(id);
    await this.reload();
  }

  // FixDatas
  openAddFixDataModal(): void {
    this.editingFixDataId = null;
    this.fixDataForm.reset({ nombre: '', descripcion: null, script: null });
    this.showFixDataModal.set(true);
  }

  openEditFixDataModal(fixData: FixData): void {
    this.editingFixDataId = fixData.id;
    this.fixDataForm.setValue({
      nombre: fixData.nombre,
      descripcion: fixData.descripcion,
      script: fixData.script,
    });
    this.showFixDataModal.set(true);
  }

  closeFixDataModal(): void {
    this.showFixDataModal.set(false);
  }

  async onSaveFixData(): Promise<void> {
    const value = this.fixDataForm.getRawValue();
    if (this.editingFixDataId !== null) {
      await this.fixDataService.update(this.editingFixDataId, { id: this.editingFixDataId, ...value, orden: 0 });
    } else {
      await this.fixDataService.create(this.applicationId, { ...value, orden: 0 });
    }
    this.showFixDataModal.set(false);
    await this.reload();
  }

  async onDeleteFixData(id: number): Promise<void> {
    await this.fixDataService.delete(id);
    await this.reload();
  }

  async copyScript(script: string | null): Promise<void> {
    if (script) {
      await navigator.clipboard.writeText(script);
    }
  }
}
