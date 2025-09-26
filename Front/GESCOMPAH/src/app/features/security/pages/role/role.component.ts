import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { take } from 'rxjs';

import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
import { ToggleButtonComponent } from '../../../../shared/components/toggle-button-component/toggle-button-component.component';

import { TableColumn } from '../../../../shared/models/TableColumn.models';
// import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { PageHeaderService } from '../../../../shared/Services/PageHeader/page-header.service';

import { RoleSelectModel, RoleUpdateModel } from '../../models/role.models';
import { RoleStore } from '../../services/role/role.store';
import { IsActive } from '../../../../core/models/IsAcitve.models';

@Component({
  selector: 'app-role',
  standalone: true,
  imports: [CommonModule, GenericTableComponent, ToggleButtonComponent],
  templateUrl: './role.component.html',
  styleUrls: ['./role.component.css'],
})
export class RoleComponent implements OnInit {

  // Inyección de dependencias
  private readonly dialog = inject(MatDialog);
  private readonly roleStore = inject(RoleStore);
  // private readonly confirmDialog     = inject(ConfirmDialogService);
  private readonly sweetAlert = inject(SweetAlertService);
  private readonly sweetAlertService = inject(SweetAlertService);
  private readonly pageHeaderService = inject(PageHeaderService);

  // Estado de datos
  readonly roles$ = this.roleStore.roles$;

  // Columnas de la tabla
  columns: TableColumn<RoleSelectModel>[] = [];

  // Template para el toggle de estado
  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  // Ciclo de vida
  ngOnInit(): void {
    this.pageHeaderService.setPageHeader('Roles', 'Gestión de Roles');

    // Se define aquí porque requiere el template ya resuelto
    this.columns = [
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' },
      {
        key: 'active',
        header: 'Estado',
        type: 'custom',
        template: this.estadoTemplate
      }
    ];
  }

  // Helpers
  trackById = (_: number, it: RoleSelectModel) => it.id;

  

  // Crear
  onCreateNew(): void {
    const open = (m: any) => this.dialog.open(m.FormDialogComponent, {
      width: '600px',
      data: {
        entity: {},
        formType: 'Rol'
      }
    });
    import('../../../../shared/components/form-dialog/form-dialog.component').then(m => {
      const dialogRef = open(m);

      dialogRef.afterClosed().pipe(take(1)).subscribe((result: any) => {
        if (!result) return;
        this.roleStore.create(result).pipe(take(1)).subscribe({
          next: () => this.sweetAlertService.showNotification('Creación Exitosa', 'Rol creado exitosamente.', 'success'),
          error: (err) => this.sweetAlertService.showApiError(err, 'No se pudo crear el rol.')
        });
      });
    });
  }

  // Editar
  onEdit(row: RoleSelectModel): void {
    const open = (m: any) => this.dialog.open(m.FormDialogComponent, {
      width: '600px',
      data: {
        entity: row,
        formType: 'Rol'
      }
    });
    import('../../../../shared/components/form-dialog/form-dialog.component').then(m => {
      const dialogRef = open(m);

      dialogRef.afterClosed().pipe(take(1)).subscribe((result: Partial<RoleUpdateModel> | undefined) => {
        if (!result) return;

        const payload: RoleUpdateModel = { ...result, id: row.id } as RoleUpdateModel;

        this.roleStore.update(payload).pipe(take(1)).subscribe({
          next: () => this.sweetAlertService.showNotification('Actualización Exitosa', 'Rol actualizado exitosamente.', 'success'),
          error: (err) => this.sweetAlertService.showApiError(err, 'No se pudo actualizar el rol.')
        });
      });
    });
  }

  // Ver
  onView(row: RoleSelectModel): void {
    // Ver detalle (diálogo o navegación)
  }

  // Eliminar (borrado lógico)
  async onDelete(row: RoleSelectModel): Promise<void> {
    const result = await this.sweetAlertService.showConfirm(
      'Eliminar rol',
      `¿Deseas eliminar el rol "${row.name}"?`,
      'Eliminar',
      'Cancelar',
      'warning'
    );

    if (!result.isConfirmed) return;

    this.roleStore.deleteLogic(row.id).pipe(take(1)).subscribe({
      next: () => this.sweetAlertService.showNotification('Eliminación Exitosa', 'Rol eliminado exitosamente.', 'success'),
      error: (err) => this.sweetAlertService.showApiError(err, 'No se pudo eliminar el rol.')
    });
  }

  // Toggle activo/inactivo (UI optimista + rollback)
  onToggleActive(row: IsActive, e: boolean | { checked: boolean }): void {
    const checked = typeof e === 'boolean' ? e : !!e?.checked;
    const prev = row.active;

    // UI optimista
    row.active = checked;

    this.roleStore.changeActiveStatus(row.id, checked).pipe(take(1)).subscribe({
      next: (updated: Partial<IsActive> | void) => {
        // Si la API retorna DTO, sincronizamos; si retorna 204, mantenemos el optimista
        row.active = (updated as IsActive)?.active ?? checked;
        this.sweetAlertService.showNotification('Éxito', `Rol ${row.active ? 'activado' : 'desactivado'} correctamente.`, 'success');
      },
      error: (err) => {
        // Rollback
        row.active = prev;
        this.sweetAlertService.showApiError(err, 'No se pudo cambiar el estado del rol.');
      }
    });
  }
}
