import { Component, inject, OnInit } from '@angular/core';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { SquareService } from '../../services/square/square.service';
import { SquareSelectModel } from '../../models/squares.models';

@Component({
  selector: 'app-square-list',
  imports: [GenericTableComponent],
  templateUrl: './square-list.component.html',
  styleUrl: './square-list.component.css'
})
export class SquareListComponent implements OnInit {
  private readonly SquaresService = inject(SquareService);
  Forms: SquareSelectModel[] = [];

  columns: TableColumn<SquareSelectModel>[] = [];

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
    this.SquaresService.getAll().subscribe({
      next: (data) => (this.Forms = data),
      error: (err) => console.error('Error al cargar formularios:', err)
    })
  }

  onEdit(row: SquareSelectModel) {
    console.log('Editar:', row);
  }

  onDelete(row: SquareSelectModel) {
    console.log('Eliminar:', row);
  }

  onView(row: SquareSelectModel) {
    console.log('Ver:', row);
  }




}
