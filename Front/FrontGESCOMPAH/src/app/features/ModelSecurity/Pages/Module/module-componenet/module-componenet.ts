import { Component, inject, OnInit } from '@angular/core';
import { ModuleServices } from '../../../Services/Module/module-services';
import { ModulesModule } from '../../../Models/module.models';
import { TableColumn } from '../../../../../shared/models/TableColumn.models';
import { GenericTableComponents } from "../../../../../shared/components/generic-table-components/generic-table-components";
import { MatDialog } from '@angular/material/dialog';
import { FormDialogComponent } from '../../../../../shared/components/form-dialog/form-dialog.component';

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
  selectedForm: any = null;

  constructor(private dialog: MatDialog) { }

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
    // this.ModuleService.getAllPruebas().subscribe({
    //   next: (data) => (this.Forms = data),
    //   error: (err) => console.error('Error al cargar formularios:', err)
    // })
  }

  onEdit(row: ModulesModule) {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: row,
        formType: 'Module' // el tipo de schema a cargar
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const id = row.id;
        this.ModuleService.Update("module", id, result).subscribe({
          next: () => {
            console.log('Module actualizado correctamente');
            this.load();
          },
          error: err => console.error('Error actualizando el Module:', err)
        });
      }
    });
  }
  onDelete(row: ModulesModule) {
    console.log('Eliminar:', row);
  }

  onView(row: ModulesModule) {
    console.log('Ver:', row);
  }
}
