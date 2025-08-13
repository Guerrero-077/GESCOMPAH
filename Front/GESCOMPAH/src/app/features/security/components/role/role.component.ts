import { Component, inject, OnInit } from '@angular/core';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { MatDialog } from '@angular/material/dialog';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { RoleModule } from '../../models/role.models';
import { RoleService } from '../../services/role/role.service';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';

@Component({
  selector: 'app-role',
  imports: [GenericTableComponent],
  templateUrl: './role.component.html',
  styleUrl: './role.component.css'
})
export class RoleComponent implements OnInit {


  private readonly rolService = inject(RoleService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);

  rols: RoleModule[] = [];

  columns: TableColumn<RoleModule>[] = [];
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

  onEdit(row: RoleModule) {
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
            this.load();
            this.sweetAlertService.showNotification('Actualización Exitosa', 'Rol actualizado exitosamente.', 'success');
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
          this.sweetAlertService.showNotification('Creación Exitosa', 'Rol creado exitosamente.', 'success');
        }, err => {
          console.error('Error creando el rol:', err);
          this.sweetAlertService.showNotification('Error', 'No se pudo crear el rol.', 'error');
        });
      }
    });
  }


  async onDelete(row: RoleModule) {
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
          {
            this.load()
            this.sweetAlertService.showNotification('Eliminación Exitosa', 'Rol eliminado exitosamente.', 'success');
          };
        },
        error: err => {
          console.error('Error eliminando el rol:', err)
          this.sweetAlertService.showNotification('Error', 'No se pudo eliminar el rol.', 'error');
        }
      });
    }
  }

  onView(row: RoleModule) {
    console.log('Ver:', row);
  }

}
