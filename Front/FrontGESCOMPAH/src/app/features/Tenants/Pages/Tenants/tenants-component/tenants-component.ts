import { Component, inject, OnInit } from '@angular/core';
import { TenantsServices } from '../../../Services/Tenants/tenants-services';
import { TableColumn } from '../../../../../shared/models/TableColumn.models';
import { TenantsModels } from '../../../Models/Tenants.mode';
// import { GenericTableComponents } from "../../../../../shared/components/generic-table-components/generic-table-components";

@Component({
  selector: 'app-tenants-component',
  // imports: [GenericTableComponents],
  templateUrl: './tenants-component.html',
  styleUrl: './tenants-component.css'
})
export class TenantsComponent implements OnInit {
  private readonly tenantsService = inject(TenantsServices);
  tenants: TenantsModels[] = [];

  columns: TableColumn<TenantsModels>[] = [];

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'personName', header: 'Nombre' },
      { key: 'email', header: 'Correo' },
      { key: 'active', header: 'Active' }
    ];
    this.load();
  }

  load() {
    this.tenantsService.getAll("user").subscribe({
      next: (data) => (this.tenants = data),
      error: (err) => console.error('Error al cargar:', err)
    })
    
  }

  onEdit(row: TenantsModels) {
    console.log('Editar:', row);
  }

  onDelete(row: TenantsModels) {
    console.log('Eliminar:', row);
  }

  onView(row: TenantsModels) {
    console.log('Ver:', row);
  }




}
