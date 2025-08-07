import { Component, inject, OnInit } from '@angular/core';
import { PermissionServices } from '../../../Services/Permission/permission-services';
import { PermissionModule } from '../../../Models/Permission.Models';
import { TableColumn } from '../../../../../shared/models/TableColumn.models';
import { FormModule } from '../../../Models/form.model';
import { GenericTableComponents } from "../../../../../shared/components/generic-table-components/generic-table-components";
import { MatDialog } from '@angular/material/dialog';
import { FormDialogComponent } from '../../../../../shared/components/form-dialog/form-dialog.component';

@Component({
  selector: 'app-permission-component',
  imports: [GenericTableComponents],
  templateUrl: './permission-component.html',
  styleUrl: './permission-component.css'
})
export class PermissionComponent implements OnInit {
  private readonly permissionService = inject(PermissionServices);
  permissions: PermissionModule[] = [];

  columns: TableColumn<PermissionModule>[] = [];
  selectedForm: any = null;

  constructor(private dialog: MatDialog) {}

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
    // this.permissionService.getAll("permission").subscribe({
    //   next: (data) => (this.permissions = data),
    //   error: (err) => console.error('Error:', err)
    // })
    this.permissionService.getAllPruebas().subscribe({
      next: (data) => (this.permissions = data),
      error: (err) => console.error('Error:', err)
    })
  }

  onEdit(row: PermissionModule) {
    const dialogRef = this.dialog.open(FormDialogComponent,{
      width: '600px',
      data: {
        entity: row,
        formType: 'Permission'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      this.selectedForm = null;
      if (result) { //comentario
        this.load(); // Recarga si es necesario
      }
    });
  }

  onDelete(row: PermissionModule) {
    console.log('Eliminar:', row);
  }

  onView(row: PermissionModule) {
    console.log('Ver:', row);
  }


}
