import { Component, inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { RoleService } from '../../services/role/role.service';
import { RoleSelectModel, RoleUpdateModel } from '../../models/role.models';
import { RoleStore } from '../../services/role/role.store';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-role',
  imports: [GenericTableComponent, CommonModule],
  templateUrl: './role.component.html',
  styleUrl: './role.component.css',
  
})
export class RoleComponent implements OnInit {

  private readonly roleStore = inject(RoleStore);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);

  roles$ = this.roleStore.roles$;

  columns: TableColumn<RoleSelectModel>[] = [];
  selectedForm: any = null;

  constructor(private dialog: MatDialog) { }

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' },
      { key: 'active', header: 'Active', type: 'boolean' }
    ];
  }

  onEdit(row: RoleSelectModel) {
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
        const updateDto: RoleUpdateModel = { ...result, id };
        this.roleStore.update(updateDto).subscribe({
          next: () => {
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
        this.roleStore.create(result).subscribe(res => {
          this.sweetAlertService.showNotification('Creación Exitosa', 'Rol creado exitosamente.', 'success');
        }, err => {
          console.error('Error creando el rol:', err);
          this.sweetAlertService.showNotification('Error', 'No se pudo crear el rol.', 'error');
        });
      }
    });
  }


  async onDelete(row: RoleSelectModel) {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar rol',
      text: `¿Deseas eliminar el rol "${row.name}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });

    if (confirmed) {
      this.roleStore.deleteLogic(row.id).subscribe({
        next: () => {
          console.log('Rol eliminado correctamente');
          {
            this.sweetAlertService.showNotification('Eliminación Exitosa', 'Rol eliminado exitosamente.', 'success');
          };
        },
        error: (err) => {
          console.error('Error al eliminar el rol:', err);
          this.sweetAlertService.showNotification('Error', 'No se pudo eliminar el rol.', 'error');
        }
      });
    }
  }

  onView(row: RoleSelectModel) {
    console.log('Ver:', row);
  }

}