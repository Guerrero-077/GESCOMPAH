import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, TemplateRef, ViewChild, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { Observable, EMPTY } from 'rxjs';
import { take, tap, catchError, finalize } from 'rxjs/operators';

import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { DepartmentStore } from '../../services/department/department.store';
import { DepartmentSelectModel } from '../../models/department.models';
import { ToggleButtonComponent } from '../../../../shared/components/toggle-button-component/toggle-button-component.component';
import { HasRoleAndPermissionDirective } from '../../../../core/Directives/HasRoleAndPermission.directive';

@Component({
  selector: 'app-department',
  standalone: true,
  imports: [
    CommonModule,
    GenericTableComponent,
    MatButtonModule,
    MatIconModule,
    ToggleButtonComponent,
    HasRoleAndPermissionDirective
  ],
  templateUrl: './department.component.html',
  styleUrls: ['./department.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DepartmentComponent implements OnInit {
  departments$: Observable<DepartmentSelectModel[]>;

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  columns: TableColumn<DepartmentSelectModel>[] = [
    { key: 'id', header: 'ID' },
    { key: 'name', header: 'Nombre' }
  ];

  // ---- Lock por ítem (evita doble clic mientras llama al backend)
  private busyIds = new Set<number>();
  isBusy = (id: number) => this.busyIds.has(id);

  // DI
  private readonly cdr = inject(ChangeDetectorRef);

  constructor(
    private store: DepartmentStore,
    private dialog: MatDialog,
    private sweetAlert: SweetAlertService
  ) {
    this.departments$ = this.store.departments$;
  }

  ngOnInit(): void {
    this.columns = [
      ...this.columns,
      { key: 'active', header: 'Estado', type: 'custom', template: this.estadoTemplate }
    ];
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '400px',
      data: { entity: {}, formType: 'Department' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;
      this.store.create(result).pipe(take(1)).subscribe({
        next: () => {
          this.sweetAlert.showNotification('Éxito', 'Departamento creado exitosamente', 'success');
          this.cdr.markForCheck();
        }
      });
    });
  }

  openEditDialog(row: DepartmentSelectModel): void {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '400px',
      data: { entity: row, formType: 'Department' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;
      this.store.update(result).pipe(take(1)).subscribe({
        next: () => {
          this.sweetAlert.showNotification('Éxito', 'Departamento actualizado exitosamente', 'success');
          this.cdr.markForCheck();
        }
      });
    });
  }

  handleDelete(row: DepartmentSelectModel): void {
    if (!row.id) return;
    this.sweetAlert.showConfirm('¿Está seguro?', 'Esta acción no se puede deshacer').then(res => {
      if (!res.isConfirmed) return;
      this.store.delete(row.id!).pipe(take(1)).subscribe({
        next: () => {
          this.sweetAlert.showNotification('Éxito', 'Departamento eliminado exitosamente', 'success');
          this.cdr.markForCheck();
        }
      });
    });
  }

  // ===== Toggle Activo/Inactivo =====
  /**
   * Acepta boolean o {checked:boolean}. Con OnPush, marcamos la vista tras cambios.
   * En el HTML: (toggleChange)="onToggleActive(row, $event)"  [disabled]="isBusy(row.id)"
   */
  onToggleActive(row: DepartmentSelectModel, e: boolean | { checked: boolean }) {
    if (this.isBusy(row.id)) return;

    const checked = typeof e === 'boolean' ? e : !!e?.checked;
    const previous = row.active;

    // Optimistic UI + lock
    this.busyIds.add(row.id);
    row.active = checked;
    this.cdr.markForCheck();

    this.store.changeActiveStatus(row.id, checked).pipe(
      take(1),
      tap(updated => {
        // Si la API devuelve 204 No Content, updated puede ser undefined
        row.active = updated?.active ?? checked;
        this.sweetAlert.showNotification(
          'Éxito',
          `Departamento ${row.active ? 'activado' : 'desactivado'} correctamente.`,
          'success'
        );
        this.cdr.markForCheck();
      }),
      catchError(err => {
        // revertimos
        row.active = previous;
        this.sweetAlert.showNotification(
          'Error',
          err?.error?.detail || 'No se pudo cambiar el estado.',
          'error'
        );
        this.cdr.markForCheck();
        return EMPTY;
      }),
      finalize(() => {
        this.busyIds.delete(row.id);
        this.cdr.markForCheck();
      })
    ).subscribe();
  }
}
