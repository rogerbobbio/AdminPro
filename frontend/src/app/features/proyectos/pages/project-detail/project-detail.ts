import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProjectService } from '../../../../shared/services/project.service';
import { DatabaseService } from '../../../../shared/services/database.service';
import { BaseDeDatos } from '../../../../shared/models/project.model';

@Component({
  selector: 'app-project-detail',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './project-detail.html',
  styleUrl: './project-detail.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectDetail implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly databaseService = inject(DatabaseService);
  protected readonly projectService = inject(ProjectService);

  protected projectId!: number;

  protected readonly showModal = signal(false);
  protected editingDatabaseId: number | null = null;

  protected readonly databaseForm = new FormGroup({
    nombre: new FormControl('', { nonNullable: true }),
    ambiente: new FormControl<string | null>(null),
    servidor: new FormControl<string | null>(null),
    databaseId: new FormControl<number | null>(null),
    loginName: new FormControl<string | null>(null),
    password: new FormControl<string | null>(null),
  });

  ngOnInit(): void {
    this.projectId = Number(this.route.snapshot.paramMap.get('id'));
    void this.projectService.getById(this.projectId);
  }

  openAddDatabaseModal(): void {
    this.editingDatabaseId = null;
    this.databaseForm.reset();
    this.showModal.set(true);
  }

  openEditDatabaseModal(database: BaseDeDatos): void {
    this.editingDatabaseId = database.id;
    this.databaseForm.setValue({
      nombre: database.nombre,
      ambiente: database.ambiente,
      servidor: database.servidor,
      databaseId: database.databaseId,
      loginName: database.loginName,
      password: database.password,
    });
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  async onSaveDatabase(): Promise<void> {
    const value = this.databaseForm.getRawValue();
    if (this.editingDatabaseId !== null) {
      await this.databaseService.update(this.editingDatabaseId, { id: this.editingDatabaseId, ...value });
    } else {
      await this.databaseService.create(this.projectId, value);
    }
    this.showModal.set(false);
    await this.projectService.getById(this.projectId);
  }

  async onDeleteDatabase(id: number): Promise<void> {
    await this.databaseService.delete(id);
    await this.projectService.getById(this.projectId);
  }
}
