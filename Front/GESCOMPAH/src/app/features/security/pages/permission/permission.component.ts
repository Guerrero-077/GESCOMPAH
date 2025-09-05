import { CommonModule } from '@angular/common';
import { AfterViewInit, ChangeDetectorRef, Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { PermissionSelectModel, PermissionUpdateModel } from '../../models/permission.models';
import { PermissionStore } from '../../services/permission/permission.store';
import { ToggleButtonComponent } from "../../../../shared/components/toggle-button-component/toggle-button-component.component";
import { PageHeaderService } from '../../../../shared/Services/PageHeader/page-header.service';

@Component({
  selector: 'app-permission',
  imports: [GenericTableComponent, CommonModule, ToggleButtonComponent],
  templateUrl: './permission.component.html',
  styleUrl: './permission.component.css'
})
export class PermissionComponent implements OnInit {
  private readonly permissionStore = inject(PermissionStore);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);
  private readonly pageHeaderService = inject(PageHeaderService);

  permissions$ = this.permissionStore.permissions$;

  columns: TableColumn<PermissionSelectModel>[] = [];
  selectedForm: any = null;

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  constructor(private dialog: MatDialog) { }

    ngOnInit(): void {
    this.pageHeaderService.setPageHeader('Permisos', 'Gestión de Permisos');
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

  onEdit(row: PermissionSelectModel) {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: row,
        formType: 'Permission'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        const id = row.id;
        const updateDto: PermissionUpdateModel = { ...result, id };
        this.permissionStore.update(updateDto).subscribe({
          next: () => {
            this.sweetAlertService.showNotification('Actualización Exitosa', 'Permiso actualizado exitosamente.', 'success');
          },
          error: (err: Error) => {
            console.error('Error actualizando el permiso:', err);
            this.sweetAlertService.showNotification('Error', 'No se pudo actualizar el permiso.', 'error');
          }
        });
      }
    });
  }

  onCreateNew() {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: {},
        formType: 'Permission'
      }
    });

    dialogRef.afterClosed().subscribe((result: any) => {
      if (result) {
        this.permissionStore.create(result).subscribe({
          next: () => {
            this.sweetAlertService.showNotification('Creación Exitosa', 'Permiso creado exitosamente.', 'success');
          },
          error: (err: Error) => {
            console.error('Error creando el permiso:', err);
            this.sweetAlertService.showNotification('Error', 'No se pudo crear el permiso.', 'error');
          }
        });
      }
    });
  }

  async onDelete(row: PermissionSelectModel) {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar permiso',
      text: `¿Deseas eliminar el permiso "${row.name}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });

    if (confirmed) {
      this.permissionStore.deleteLogic(row.id).subscribe({
        next: () => {
          this.sweetAlertService.showNotification('Eliminación Exitosa', 'Permiso eliminado exitosamente.', 'success');
        },
        error: (err: Error) => {
          console.error('Error al eliminar el permiso:', err);
          this.sweetAlertService.showNotification('Error', 'No se pudo eliminar el permiso.', 'error');
        }
      });
    }
  }

  onView(row: PermissionSelectModel) {
    console.log('Ver:', row);
  }


  onToggleActive(row: PermissionSelectModel, e: { checked: boolean }) {
    const previous = row.active;
    row.active = e.checked; // Optimistic UI

    this.permissionStore.changeActiveStatus(row.id, e.checked).subscribe({
      next: (updated) => {
        // sincronizar con lo que devuelve el backend
        row.active = updated.active ?? row.active;
        this.sweetAlertService.showNotification(
          'Éxito',
          `Permiso ${row.active ? 'activado' : 'desactivado'} correctamente.`,
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

