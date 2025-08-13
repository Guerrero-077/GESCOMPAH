import { Component, inject, OnInit } from '@angular/core';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { MatDialog } from '@angular/material/dialog';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { PermissionModule } from '../../models/permission.models';
import { PermissionService } from '../../services/permission/permission.service';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';

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

  permissions: PermissionModule[] = [];

  columns: TableColumn<PermissionModule>[] = [];
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
    this.permissionService.getAll("permission").subscribe({
      next: (data) => (this.permissions = data),
      error: (err) => console.error('Error:', err)
    })
    // this.permissionService.getAllPruebas().subscribe({
    //   next: (data) => (this.permissions = data),
    //   error: (err) => console.error('Error:', err)
    // })
  }

  onEdit(row: PermissionModule) {
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
        this.permissionService.Update("permission", id, result).subscribe({
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
        this.permissionService.Add("permission", result).subscribe(res => {
          this.load();
          this.sweetAlertService.showNotification('Creación Exitosa', 'Permission creado exitosamente.', 'success');
        }, err => {
          console.error('Error creando el permission:', err);
          this.sweetAlertService.showNotification('Error', 'No se pudo crear el permission.', 'error');
        });
      }
    });
  }




  async onDelete(row: PermissionModule) {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar permission',
      text: `¿Deseas eliminar el permission "${row.name}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });

    if (confirmed) {
      this.permissionService.DeleteLogical('Permission', row.id).subscribe({
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

  onView(row: PermissionModule) {
    console.log('Ver:', row);
  }


}

