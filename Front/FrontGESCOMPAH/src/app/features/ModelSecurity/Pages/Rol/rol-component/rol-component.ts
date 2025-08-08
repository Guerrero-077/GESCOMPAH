import { Component, inject, OnInit } from '@angular/core';
import { RolService } from '../../../Services/Rol/rol-service';
import { RolModule } from '../../../Models/rol.models';
import { GenericTableComponents } from "../../../../../shared/components/generic-table-components/generic-table-components";
import { TableColumn } from '../../../../../shared/models/TableColumn.models';
import { MatDialog } from '@angular/material/dialog';
import { FormDialogComponent } from '../../../../../shared/components/form-dialog/form-dialog.component';
import { ConfirmDialogService } from '../../../../../shared/Services/confirm-dialog-service';

@Component({
  selector: 'app-rol-component',
  imports: [GenericTableComponents],
  templateUrl: './rol-component.html',
  styleUrl: './rol-component.css'
})
export class RolComponent implements OnInit {


  private readonly rolService = inject(RolService);
  private readonly confirmDialog = inject(ConfirmDialogService);

  rols: RolModule[] = [];

  columns: TableColumn<RolModule>[] = [];
  selectedForm: any = null;

  constructor(private dialog: MatDialog) { }

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' },
      { key: 'active', header: 'Active' }
    ];
    this.load();
  }

  load() {
    this.rolService.getAll("rol").subscribe({
      next: (data) => {
        console.log("Roles desde el servicio:", data);
        this.rols = data;
      }
    });

    // this.rolService.getAllPruebas().subscribe({
    //   next: (data) => (this.rols = data),
    //   error: (err) => console.error('Error al cargar roles:', err)
    // });


  }

  onEdit(row: RolModule) {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: row,
        formType: 'Rol'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const id = row.id;

        this.rolService.Update("rol", id, result).subscribe({
          next: () => {
            console.log('Rol actualizado correctamente');
            this.load();
          },
          error: err => console.error('Error actualizando el rol:', err)
        });
      }
    });
  }


  onCreateNew() {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: {},
        formType: 'Rol'
      }
    });

    dialogRef.afterClosed().subscribe((result: any) => {
      if (result) {
        this.rolService.Add("rol", result).subscribe(res => {
          this.load();
        });
      }
    });
  }


  async onDelete(row: RolModule) {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar rol',
      text: `¿Deseas eliminar el rol "${row.name}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });

    if (confirmed) {
      this.rolService.DeleteLogical('Rol', row.id).subscribe({
        next: () => {
          console.log('Rol eliminado correctamente');
          this.load();
        },
        error: err => console.error('Error eliminando el rol:', err)
      });
    }
  }

  onView(row: RolModule) {
    console.log('Ver:', row);
  }

}
