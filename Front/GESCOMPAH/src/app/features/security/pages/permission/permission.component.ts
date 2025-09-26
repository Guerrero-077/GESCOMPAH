import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { filter, switchMap, take, tap, catchError, EMPTY, finalize, map } from 'rxjs';

import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
import { ToggleButtonComponent } from '../../../../shared/components/toggle-button-component/toggle-button-component.component';

import { TableColumn } from '../../../../shared/models/TableColumn.models';
// import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { PageHeaderService } from '../../../../shared/Services/PageHeader/page-header.service';

import { PermissionStore } from '../../services/permission/permission.store';
import { PermissionSelectModel, PermissionUpdateModel } from '../../models/permission.models';

@Component({
  selector: 'app-permission',
  standalone: true,
  imports: [GenericTableComponent, CommonModule, ToggleButtonComponent],
  templateUrl: './permission.component.html',
  styleUrls: ['./permission.component.css']
})
export class PermissionComponent implements OnInit {
  // Inyección de dependencias
  private readonly permissionStore = inject(PermissionStore);
  // private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlert    = inject(SweetAlertService);
  private readonly sweetAlertService = inject(SweetAlertService);
  private readonly pageHeaderService = inject(PageHeaderService);
  constructor(private dialog: MatDialog) {}

  // Estado
  permissions$ = this.permissionStore.permissions$;
  columns: TableColumn<PermissionSelectModel>[] = [];
  private busyIds = new Set<number>();
  isBusy = (id: number) => this.busyIds.has(id);

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  ngOnInit(): void {
    this.pageHeaderService.setPageHeader('Permisos', 'Gestión de Permisos');
    this.columns = [
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' },
      { key: 'active', header: 'Estado', type: 'custom', template: this.estadoTemplate }
    ];
  }

  // Crear
  onCreateNew(): void {
    import('../../../../shared/components/form-dialog/form-dialog.component').then(m => {
      const dialogRef = this.dialog.open(m.FormDialogComponent, {
        width: '600px',
        data: { entity: {}, formType: 'Permission' }
      });

      dialogRef.afterClosed().pipe(
      filter(Boolean),
      switchMap(result => this.permissionStore.create(result).pipe(take(1))),
      tap(() => this.sweetAlertService.showNotification('Creación Exitosa', 'Permiso creado exitosamente.', 'success')),
      catchError(err => {
        console.error('Error creando el permiso:', err);
        this.sweetAlertService.showApiError(err, 'No se pudo crear el permiso.');
        return EMPTY;
      })
    ).subscribe();
    });
  }

  // Editar
  onEdit(row: PermissionSelectModel): void {
    import('../../../../shared/components/form-dialog/form-dialog.component').then(m => {
      const dialogRef = this.dialog.open(m.FormDialogComponent, {
        width: '600px',
        data: { entity: row, formType: 'Permission' }
      });

      dialogRef.afterClosed().pipe(
      filter((result): result is Partial<PermissionUpdateModel> => !!result),
      map(result => ({ id: row.id, ...result } as PermissionUpdateModel)),
      switchMap(dto => this.permissionStore.update(dto).pipe(take(1))),
      tap(() => this.sweetAlertService.showNotification('Actualización Exitosa', 'Permiso actualizado exitosamente.', 'success')),
      catchError(err => {
        console.error('Error actualizando el permiso:', err);
        this.sweetAlertService.showApiError(err, 'No se pudo actualizar el permiso.');
        return EMPTY;
      })
    ).subscribe();
    });
  }

  // Eliminar (lógico)
  async onDelete(row: PermissionSelectModel): Promise<void> {
    const result = await this.sweetAlert.showConfirm(
      'Eliminar permiso',
      `¿Deseas eliminar el permiso "${row.name}"?`,
      'Eliminar',
      'Cancelar',
      'warning'
    );
    if (!result.isConfirmed) return;

    this.permissionStore.deleteLogic(row.id).pipe(take(1)).subscribe({
      next: () => this.sweetAlertService.showNotification('Eliminación Exitosa', 'Permiso eliminado exitosamente.', 'success'),
      error: err => {
        console.error('Error al eliminar el permiso:', err);
        this.sweetAlertService.showApiError(err, 'No se pudo eliminar el permiso.');
      }
    });
  }

  onView(row: PermissionSelectModel): void {
  }

  // Toggle activo/inactivo (UI optimista + rollback)
  onToggleActive(row: PermissionSelectModel, e: boolean | { checked: boolean }): void {
    if (this.isBusy(row.id)) return;

    const checked = typeof e === 'boolean' ? e : !!e?.checked;
    const previous = row.active;

    // Optimistic UI + lock por ítem
    this.busyIds.add(row.id);
    row.active = checked;

    this.permissionStore.changeActiveStatus(row.id, checked).pipe(
      take(1),
      tap(updated => {
        // Si la API responde 204, updated puede venir undefined
        row.active = updated?.active ?? checked;
        this.sweetAlertService.showNotification(
          'Éxito',
          `Permiso ${row.active ? 'activado' : 'desactivado'} correctamente.`,
          'success'
        );
      }),
      catchError(err => {
        console.error('Error cambiando estado:', err);
        row.active = previous; // revertir
        this.sweetAlertService.showApiError(err, 'No se pudo cambiar el estado.');
        return EMPTY;
      }),
      finalize(() => this.busyIds.delete(row.id))
    ).subscribe();
  }
}
