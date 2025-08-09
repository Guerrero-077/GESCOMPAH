import { Component, inject, OnInit } from '@angular/core';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { CityModel } from '../../models/city.models';
import { CityService } from '../../services/city/city.service';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";

@Component({
  selector: 'app-city-list',
  imports: [GenericTableComponent],
  templateUrl: './city-list.component.html',
  styleUrl: './city-list.component.css'
})
export class CityListComponent implements OnInit {
  private readonly service = inject(CityService);
  Forms: CityModel[] = [];

  columns: TableColumn<CityModel>[] = [];

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

  onEdit(row: CityModel) {
    console.log('Editar:', row);
  }

  onDelete(row: CityModel) {
    console.log('Eliminar:', row);
  }

  onView(row: CityModel) {
    console.log('Ver:', row);
  }

}