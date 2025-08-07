import { Component, inject, OnInit } from '@angular/core';
import { SquaresService } from '../../Service/squares-service';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { SquaresModels } from '../../Models/squares.models';
import { GenericTableComponents } from "../../../../shared/components/generic-table-components/generic-table-components";

@Component({
  selector: 'app-squares-component',
  // imports: [GenericTableComponents],
  templateUrl: './squares-component.html',
  styleUrl: './squares-component.css',
  imports: [GenericTableComponents]
})
export class SquaresComponent implements OnInit {
  private readonly SquaresService = inject(SquaresService);
  Forms: SquaresModels[] = [];

  columns: TableColumn<SquaresModels>[] = [];

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' },
      { key: 'location', header: 'Ubicación' },
      { key: 'capacity', header: 'Capacidad' },
      { key: 'active', header: 'Active' }
    ];
    this.load();
  }

  load() {
    this.SquaresService.getAll("Plaza").subscribe({
      next: (data) => (this.Forms = data),
      error: (err) => console.error('Error al cargar formularios:', err)
    })
  }

  onEdit(row: SquaresModels) {
    console.log('Editar:', row);
  }

  onDelete(row: SquaresModels) {
    console.log('Eliminar:', row);
  }

  onView(row: SquaresModels) {
    console.log('Ver:', row);
  }




}
