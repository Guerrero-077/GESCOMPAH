import { Component, inject, OnInit } from '@angular/core';
import { ModuleServices } from '../../../Services/Module/module-services';
import { ModulesModule } from '../../../Models/module.models';
import { TableColumn } from '../../../../../shared/models/TableColumn.models';
import { GenericTableComponents } from "../../../../../shared/components/generic-table-components/generic-table-components";

@Component({
  selector: 'app-module-componenet',
  imports: [GenericTableComponents],
  templateUrl: './module-componenet.html',
  styleUrl: './module-componenet.css'
})
export class ModuleComponenet implements OnInit {
  private readonly ModuleService = inject(ModuleServices);
  Forms: ModulesModule[] = [];

  columns: TableColumn<ModulesModule>[] = [];

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' }
    ];
    this.load();
  }

  load() {
    this.ModuleService.getAll("module").subscribe({
      next: (data) => (this.Forms = data),
      error: (err) => console.error('Error al cargar formularios:', err)
    })
  }

  onEdit(row: ModulesModule) {
    console.log('Editar:', row);
  }

  onDelete(row: ModulesModule) {
    console.log('Eliminar:', row);
  }

  onView(row: ModulesModule) {
    console.log('Ver:', row);
  }
}
