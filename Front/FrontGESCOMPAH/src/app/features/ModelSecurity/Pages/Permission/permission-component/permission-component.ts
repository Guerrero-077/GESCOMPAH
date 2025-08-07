import { Component, inject, OnInit } from '@angular/core';
import { PermissionServices } from '../../../Services/Permission/permission-services';
import { PermissionModule } from '../../../Models/Permission.Models';
import { TableColumn } from '../../../../../shared/models/TableColumn.models';
import { FormModule } from '../../../Models/form.model';
import { GenericTableComponents } from "../../../../../shared/components/generic-table-components/generic-table-components";
import { MatDialog } from '@angular/material/dialog';
import { FormDialogComponent } from '../../../../../shared/components/form-dialog/form-dialog.component';
import { ConfirmDialogService } from '../../../../../shared/Services/confirm-dialog-service';

@Component({
  selector: 'app-permission-component',
  // imports: [GenericTableComponents],
  templateUrl: './permission-component.html',
  styleUrl: './permission-component.css',
  imports: [GenericTableComponents]
})
export class PermissionComponent implements OnInit {
  private readonly permissionService = inject(PermissionServices);
    private readonly confirmDialog = inject(ConfirmDialogService);

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
          },
          error: err => console.error('Error actualizando el permission:', err)
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
        },
        error: err => console.error('Error eliminando el permission:', err)
      });
    }
  }

  onView(row: PermissionModule) {
    console.log('Ver:', row);
  }


}
