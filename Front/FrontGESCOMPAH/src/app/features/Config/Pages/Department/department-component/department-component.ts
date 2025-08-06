import { Component, inject, OnInit } from '@angular/core';
import { DepartmentServices } from '../../../Services/Department/department-services';
import { TableColumn } from '../../../../../shared/models/TableColumn.models';
import { DepartmentModels } from '../../../Models/department.Models';
import { GenericTableComponents } from "../../../../../shared/components/generic-table-components/generic-table-components";

@Component({
  selector: 'app-department-component',
  imports: [GenericTableComponents],
  templateUrl: './department-component.html',
  styleUrl: './department-component.css'
})
export class DepartmentComponent implements OnInit {
  private readonly service = inject(DepartmentServices);
  Department: DepartmentModels[] = [];

  columns: TableColumn<DepartmentModels>[] = [];

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'name', header: 'Nombre' },
      { key: 'active', header: 'Active' }
    ];
    this.load();
  }

  load() {
    this.service.getAll("city").subscribe({
      next: (data) => (this.Department = data),
      error: (err) => console.error('Error al cargar:', err)
    })
  }

  onEdit(row: DepartmentModels) {
    console.log('Editar:', row);
  }

  onDelete(row: DepartmentModels) {
    console.log('Eliminar:', row);
  }

  onView(row: DepartmentModels) {
    console.log('Ver:', row);
  }




}
