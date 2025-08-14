import { Component, inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { PermissionService } from '../../services/permission/permission.service';
import { PermissionSelectModel, PermissionUpdateModel } from '../../models/permission.models';

@Component({
  selector: 'app-permission',
  imports: [GenericTableComponent],
  templateUrl: './permission.component.html',
  styleUrl: './permission.component.css'
})
export class PermissionComponent implements OnInit {
  private readonly permissionService = inject(PermissionService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);

  permissions: PermissionSelectModel[] = [];

  columns: TableColumn<PermissionSelectModel>[] = [];
  selectedForm: any = null;

  constructor(private dialog: MatDialog) { }

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' },
      { key: 'active', header: 'Active' }
    ];
    this.load();
  }

  load() {
    this.permissionService.getAll().subscribe({
      next: (data) => (this.permissions = data),
      error: (err) => console.error('Error:', err)
    })
    // this.permissionService.getAllPruebas().subscribe({
    //   next: (data) => (this.permissions = data),
    //   error: (err) => console.error('Error:', err)
    // })
  }

  onEdit(row: PermissionUpdateModel) {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: row,
        formType: 'Permission'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      this.selectedForm = null;
      if (result) {
        const id = row.id;
        this.permissionService.update( id, result).subscribe({
          next: () => {
            console.log('permission actualizado correctamente');
            this.load();
            this.sweetAlertService.showNotification('Actualización Exitosa', 'Permission actualizado correctamente.', 'success');
          },
          error: err => {
            console.error('Error actualizando el permission:', err)
            this.sweetAlertService.showNotification('Error', 'No se pudo actualizar el permission.', 'error');
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
        formType: 'Permission' // o 'User', 'Product', etc.
      }
    });

    dialogRef.afterClosed().subscribe((result: any) => {
      if (result) {
        this.permissionService.create(result).subscribe(res => {
          this.load();
          this.sweetAlertService.showNotification('Creación Exitosa', 'Permission creado exitosamente.', 'success');
        }, err => {
          console.error('Error creando el permission:', err);
          this.sweetAlertService.showNotification('Error', 'No se pudo crear el permission.', 'error');
        });
      }
    });
  }




  async onDelete(row: PermissionSelectModel) {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar permission',
      text: `¿Deseas eliminar el permission "${row.name}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });

    if (confirmed) {
      this.permissionService.deleteLogic(row.id).subscribe({
        next: () => {
          console.log('Permission eliminado correctamente');
          this.load();
          this.sweetAlertService.showNotification('Eliminación Exitosa', 'Permission eliminado exitosamente.', 'success');
        },
        error: err => {
          console.error('Error eliminando el permission:', err)
          this.sweetAlertService.showNotification('Error', 'No se pudo eliminar el permission.', 'error');

        }
      });
    }
  }

  onView(row: PermissionSelectModel) {
    console.log('Ver:', row);
  }


}

