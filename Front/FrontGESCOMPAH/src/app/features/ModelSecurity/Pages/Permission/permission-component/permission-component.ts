import { Component, inject, OnInit } from '@angular/core';
import { PermissionServices } from '../../../Services/Permission/permission-services';
import { PermissionModule } from '../../../Models/Permission.Models';
import { TableColumn } from '../../../../../shared/models/TableColumn.models';
import { FormModule } from '../../../Models/form.model';
import { GenericTableComponents } from "../../../../../shared/components/generic-table-components/generic-table-components";

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
  }

  onEdit(row: PermissionModule) {
    console.log('Editar:', row);
  }

  onDelete(row: PermissionModule) {
    console.log('Eliminar:', row);
  }

  onView(row: PermissionModule) {
    console.log('Ver:', row);
  }


}
