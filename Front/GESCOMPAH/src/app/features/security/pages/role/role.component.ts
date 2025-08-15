import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSlideToggle } from "@angular/material/slide-toggle";
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { ToggleButtonComponent } from "../../../../shared/components/toggle-button-component/toggle-button-component.component";
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { RoleSelectModel, RoleUpdateModel } from '../../models/role.models';
import { RoleStore } from '../../services/role/role.store';

@Component({
  selector: 'app-role',
  imports: [GenericTableComponent, CommonModule, ToggleButtonComponent],
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

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  constructor(private dialog: MatDialog) { }

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' },
      {
        key: 'active',
        header: 'Estado',
        type: 'custom',           // tu GenericTable debe renderizar template cuando type === 'custom'
        template: this.estadoTemplate
      }
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


  // ----- Toggle estado (activo/inactivo) -----
  onToggleActive(row: RoleSelectModel, e: { checked: boolean }) {
    const previous = row.active;
    row.active = e.checked; // Optimistic UI

    // Asegúrate de tener en tu UserStore un método:
    // changeActiveStatus(id: number, active: boolean): Observable<UserSelectModel>
    this.roleStore.changeActiveStatus(row.id, e.checked).subscribe({
      next: (updated) => {
        // sincronizar con lo que devuelve el backend
        row.active = updated.active ?? row.active;
        this.sweetAlertService.showNotification(
          'Éxito',
          `Rol ${row.active ? 'activado' : 'desactivado'} correctamente.`,
          'success'
        );
      },
      error: (err) => {
        // revertir si falla
        row.active = previous;
        this.sweetAlertService.showNotification(
          'Error',
          err?.error?.detail || 'No se pudo cambiar el estado.',
          'error'
        );
      }
    });
  }
}
