import { Component, inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { PermissionService } from '../../services/permission/permission.service';
import { PermissionSelectModel, PermissionUpdateModel } from '../../models/permission.models';
import { PermissionStore } from '../../services/permission/permission.store';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-permission',
  imports: [GenericTableComponent, CommonModule],
  templateUrl: './permission.component.html',
  styleUrl: './permission.component.css'
})
export class PermissionComponent implements OnInit {
  private readonly permissionStore = inject(PermissionStore);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);

  permissions$ = this.permissionStore.permissions$;

  columns: TableColumn<PermissionSelectModel>[] = [];
  selectedForm: any = null;

  constructor(private dialog: MatDialog) { }

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' },
      { key: 'active', header: 'Active', type: 'boolean' }
    ];
  }

  onEdit(row: PermissionSelectModel) {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: row,
        formType: 'Permission'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const id = row.id;
        const updateDto: PermissionUpdateModel = { ...result, id };
        this.permissionStore.update(updateDto).subscribe({
          next: () => {
            this.sweetAlertService.showNotification('Actualización Exitosa', 'Permiso actualizado exitosamente.', 'success');
          },
          error: (err: Error) => {
            console.error('Error actualizando el permiso:', err);
            this.sweetAlertService.showNotification('Error', 'No se pudo actualizar el permiso.', 'error');
          }
        });
      }
    });
  }

  onCreateNew() {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: {},
        formType: 'Permission'
      }
    });

    dialogRef.afterClosed().subscribe((result: any) => {
      if (result) {
        this.permissionStore.create(result).subscribe({
          next: () => {
            this.sweetAlertService.showNotification('Creación Exitosa', 'Permiso creado exitosamente.', 'success');
          },
          error: (err: Error) => {
            console.error('Error creando el permiso:', err);
            this.sweetAlertService.showNotification('Error', 'No se pudo crear el permiso.', 'error');
          }
        });
      }
    });
  }

  async onDelete(row: PermissionSelectModel) {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar permiso',
      text: `¿Deseas eliminar el permiso "${row.name}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });

    if (confirmed) {
      this.permissionStore.deleteLogic(row.id).subscribe({
        next: () => {
          this.sweetAlertService.showNotification('Eliminación Exitosa', 'Permiso eliminado exitosamente.', 'success');
        },
        error: (err: Error) => {
          console.error('Error al eliminar el permiso:', err);
          this.sweetAlertService.showNotification('Error', 'No se pudo eliminar el permiso.', 'error');
        }
      });
    }
  }

  onView(row: PermissionSelectModel) {
    console.log('Ver:', row);
  }


}

