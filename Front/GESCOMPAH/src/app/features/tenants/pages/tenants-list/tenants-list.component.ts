import { Component, inject, OnInit } from '@angular/core';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { TenantsService } from '../../services/tenants/tenants.service';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { TenantsModel } from '../../models/tenants.models';

@Component({
  selector: 'app-tenants-list',
  imports: [GenericTableComponent],
  templateUrl: './tenants-list.component.html',
  styleUrl: './tenants-list.component.css'
})
export class TenantsListComponent implements OnInit {
  private readonly tenantsService = inject(TenantsService);
  tenants: TenantsModel[] = [];

  columns: TableColumn < TenantsModel > [] =[];

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

  onEdit(row: TenantsModel) {
    console.log('Editar:', row);
  }

  onDelete(row: TenantsModel) {
    console.log('Eliminar:', row);
  }

  onView(row: TenantsModel) {
    console.log('Ver:', row);
  }

}

