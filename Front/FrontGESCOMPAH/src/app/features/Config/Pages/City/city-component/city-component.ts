import { Component, inject, OnInit } from '@angular/core';
import { CityServices } from '../../../Services/City/city-services';
import { TableColumn } from '../../../../../shared/models/TableColumn.models';
import { CityModels } from '../../../Models/City.Models';
import { GenericTableComponents } from "../../../../../shared/components/generic-table-components/generic-table-components";

@Component({
  selector: 'app-city-component',
  // imports: [GenericTableComponents],
  templateUrl: './city-component.html',
  styleUrl: './city-component.css',
  imports: [GenericTableComponents]
})
export class CityComponent implements OnInit {
  private readonly service = inject(CityServices);
  Forms: CityModels[] = [];

  columns: TableColumn<CityModels>[] = [];

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'name', header: 'Nombre' },
      { key: 'departmentName', header: 'Departamento' },
      { key: 'active', header: 'Active' }
    ];
    this.load();
  }

  load() {
    this.service.getAll("city").subscribe({
      next: (data) => (this.Forms = data),
      error: (err) => console.error('Error al cargar:', err)
    })
  }

  onEdit(row: CityModels) {
    console.log('Editar:', row);
  }

  onDelete(row: CityModels) {
    console.log('Eliminar:', row);
  }

  onView(row: CityModels) {
    console.log('Ver:', row);
  }




}
