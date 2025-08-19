import { Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { TenantsService } from '../../services/tenants/tenants.service';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { TenantsSelectModel } from '../../models/tenants.models';

@Component({
  selector: 'app-tenants-list',
  imports: [GenericTableComponent],
  templateUrl: './tenants-list.component.html',
  styleUrl: './tenants-list.component.css'
})
export class TenantsListComponent implements OnInit {
  private readonly tenantsService = inject(TenantsService);
  tenants: TenantsSelectModel[] = [];

  columns: TableColumn<TenantsSelectModel>[] = [];
  @ViewChild('userTemplate', { static: true }) userTemplate!: TemplateRef<any>;

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      {
        key: 'email',
        header: 'User',
        template: this.userTemplate
      },
      { key: 'personDocument', header: 'N° Documento' },

      { key: 'personPhone', header: 'Telefono' },
      { key: 'personAddress', header: 'Dirección' },
      { key: 'roles', header: 'Rol' },
      { key: 'active', header: 'Active' }
    ];
    this.load();
  }

  load() {
    this.tenantsService.getAll().subscribe({
      next: (data) => (
        this.tenants = data,
        console.log(data)
      ),
      error: (err) => console.error('Error al cargar:', err)
    })

  }

  onEdit(row: TenantsSelectModel) {
    console.log('Editar:', row);
  }

  onDelete(row: TenantsSelectModel) {
    console.log('Eliminar:', row);
  }

  onView(row: TenantsSelectModel) {
    console.log('Ver:', row);
  }

}

