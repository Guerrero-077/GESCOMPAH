import { Component, inject, OnInit } from '@angular/core';
import { RolService } from '../../../Services/Rol/rol-service';
import { RolModule } from '../../../Models/rol.models';
import { GenericTableComponents } from "../../../../../shared/components/generic-table-components/generic-table-components";
import { TableColumn } from '../../../../../shared/models/TableColumn.models';
import { MatDialog } from '@angular/material/dialog';
import { FormDialogComponent } from '../../../../../shared/components/form-dialog/form-dialog.component';

@Component({
  selector: 'app-rol-component',
  imports: [GenericTableComponents],
  templateUrl: './rol-component.html',
  styleUrl: './rol-component.css'
})
export class RolComponent implements OnInit {


  private readonly rolService = inject(RolService);
  rols: RolModule[] = [];

  columns: TableColumn<RolModule>[] = [];
  selectedForm: any = null;

  constructor(private dialog: MatDialog) {}

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' }
    ];
    this.load();
  }

  load() {
    // this.rolService.getAll("rol").subscribe({
    //   next: (data) => (this.rols = data),
    //   error: (err) => console.error('Error al cargar roles:', err)
    // });
    this.rolService.getAllPruebas().subscribe({
      next: (data) => (this.rols = data),
      error: (err) => console.error('Error al cargar roles:', err)
    });

    console.log(this.rols);

  }

  onEdit(row: RolModule) {
    const dialogRef = this.dialog.open(FormDialogComponent,{
      width: '600px',
      data: {
        entity: row,
        formType: 'Rol'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      this.selectedForm = null;
      if (result) {
        this.load();
      }
    })
  }

  onDelete(row: RolModule) {
    console.log('Eliminar:', row);
  }

  onView(row: RolModule) {
    console.log('Ver:', row);
  }

}
