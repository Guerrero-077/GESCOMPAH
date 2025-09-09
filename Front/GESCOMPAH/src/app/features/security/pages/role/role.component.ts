import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { take } from 'rxjs';

import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
import { ToggleButtonComponent } from '../../../../shared/components/toggle-button-component/toggle-button-component.component';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';

import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
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

  // ====== Inyección estilo Angular moderno ======
  private readonly dialog            = inject(MatDialog);
  private readonly roleStore         = inject(RoleStore);
  private readonly confirmDialog     = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);
  private readonly pageHeaderService = inject(PageHeaderService);

  // ====== Estado de datos ======
  readonly roles$ = this.roleStore.roles$;

  // ====== Columnas de la GenericTable ======
  columns: TableColumn<RoleSelectModel>[] = [];

  // Template para el toggle de estado
  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  // ====== Ciclo de vida ======
  ngOnInit(): void {
    this.pageHeaderService.setPageHeader('Roles', 'Gestión de Roles');

    // Se define aquí porque requiere el template ya resuelto
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
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

  // ====== Helpers ======
  trackById = (_: number, it: RoleSelectModel) => it.id;

  private notifySuccess(title: string, message: string) {
    this.sweetAlertService.showNotification(title, message, 'success');
  }
  private notifyError(message: string) {
    this.sweetAlertService.showNotification('Error', message, 'error');
  }

  // ====== CRUD: Crear ======
  onCreateNew(): void {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: {},
        formType: 'Rol'
      }
    });

    dialogRef.afterClosed().pipe(take(1)).subscribe((result: any) => {
      if (!result) return;
      this.roleStore.create(result).pipe(take(1)).subscribe({
        next: () => this.notifySuccess('Creación Exitosa', 'Rol creado exitosamente.'),
        error: (err) => this.notifyError(err?.error?.detail || 'No se pudo crear el rol.')
      });
    });
  }

  // ====== CRUD: Editar ======
  onEdit(row: RoleSelectModel): void {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: row,
        formType: 'Rol'
      }
    });

    dialogRef.afterClosed().pipe(take(1)).subscribe((result: Partial<RoleUpdateModel> | undefined) => {
      if (!result) return;

      const payload: RoleUpdateModel = { ...result, id: row.id } as RoleUpdateModel;

      this.roleStore.update(payload).pipe(take(1)).subscribe({
        next: () => this.notifySuccess('Actualización Exitosa', 'Rol actualizado exitosamente.'),
        error: (err) => this.notifyError(err?.error?.detail || 'No se pudo actualizar el rol.')
      });
    });
  }

  // ====== CRUD: Ver ======
  onView(row: RoleSelectModel): void {
    // Punto de extensión: puedes abrir un dialogo de detalle o navegar.
    console.log('Ver:', row);
  }

  // ====== CRUD: Eliminar (borrado lógico) ======
  async onDelete(row: RoleSelectModel): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar rol',
      text: `¿Deseas eliminar el rol "${row.name}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });

    if (!confirmed) return;

    this.roleStore.deleteLogic(row.id).pipe(take(1)).subscribe({
      next: () => this.notifySuccess('Eliminación Exitosa', 'Rol eliminado exitosamente.'),
      error: (err) => this.notifyError(err?.error?.detail || 'No se pudo eliminar el rol.')
    });
  }

  // ====== Toggle Activo/Inactivo (UI optimista + rollback) ======
  // En el HTML del template del toggle:
  // <app-toggle-button-component
  //   [checked]="row.active"
  //   (toggleChange)="onToggleActive(row, $event)">
  // </app-toggle-button-component>
  onToggleActive(row: IsActive, e: boolean | { checked: boolean }): void {
    const checked = typeof e === 'boolean' ? e : !!e?.checked;
    const prev = row.active;

    // UI optimista
    row.active = checked;

    this.roleStore.changeActiveStatus(row.id, checked).pipe(take(1)).subscribe({
      next: (updated: Partial<IsActive> | void) => {
        // Si la API retorna DTO, sincronizamos; si retorna 204, mantenemos el optimista
        row.active = (updated as IsActive)?.active ?? checked;
        this.notifySuccess('Éxito', `Rol ${row.active ? 'activado' : 'desactivado'} correctamente.`);
      },
      error: (err) => {
        // Rollback
        row.active = prev;
        this.notifyError(err?.error?.detail || 'No se pudo cambiar el estado del rol.');
      }
    });
  }
}
