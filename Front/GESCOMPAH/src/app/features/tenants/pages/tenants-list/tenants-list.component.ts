import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { ToggleButtonComponent } from "../../../../shared/components/toggle-button-component/toggle-button-component.component";
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { TenantsFormDialogComponent } from '../../components/tenants-form-dialog/tenants-form-dialog.component';
import { TenantFormData, TenantsSelectModel } from '../../models/tenants.models';
import { TenantsService } from '../../services/tenants/tenants.service';

@Component({
  selector: 'app-tenants-list',
  standalone: true,
  imports: [CommonModule, GenericTableComponent, ToggleButtonComponent],
  templateUrl: './tenants-list.component.html',
  styleUrls: ['./tenants-list.component.css']
})
export class TenantsListComponent implements OnInit {
  private readonly tenantsService = inject(TenantsService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);
  private readonly dialog = inject(MatDialog);

  tenants: TenantsSelectModel[] = [];
  columns: TableColumn<TenantsSelectModel>[] = [];

  @ViewChild('userTemplate', { static: true }) userTemplate!: TemplateRef<any>;
  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'email', header: 'Usuario', template: this.userTemplate },
      { key: 'personDocument', header: 'N° Documento' },
      { key: 'personPhone', header: 'Teléfono' },
      { key: 'personAddress', header: 'Dirección' },
      { key: 'cityName', header: 'Ciudad' },
      {
        key: 'roles',
        header: 'Rol',
        render: r => Array.isArray(r.roles) ? r.roles.join(', ') : ''
      },
      { key: 'active', header: 'Estado', template: this.estadoTemplate }
    ];
    this.load();
  }

  load() {
    this.tenantsService.getAll().subscribe({
      next: data => this.tenants = data,
      error: err => console.error('Error al cargar:', err)
    });
  }

  onCreate() {
    const data: TenantFormData = { mode: 'create' };
    const ref = this.dialog.open(TenantsFormDialogComponent, { width: '720px', data });

    ref.afterClosed().subscribe(payload => {
      if (!payload) return;
      this.tenantsService.create(payload).subscribe({
        next: () => {
          this.load();
          this.sweetAlertService.showNotification(
            'Éxito',
            'Usuario creado exitosamente.',
            'success'
          );
        },
        error: err => {
          console.error('Error al crear:', err);
          this.sweetAlertService.showNotification(
            'Error',
            'No se pudo crear el usuario.',
            'error'
          );
        }
      });
    });
  }

  onEdit(row: TenantsSelectModel) {
    const data: TenantFormData = {
      mode: 'edit',
      tenant: row
    };

    const ref = this.dialog.open(TenantsFormDialogComponent, { width: '720px', data });
    ref.afterClosed().subscribe(payload => {
      if (!payload) return;
      this.tenantsService.update(row.id, payload).subscribe({
        next: () => {
          this.load();
          this.sweetAlertService.showNotification(
            'Éxito',
            'Usuario actualizado exitosamente.',
            'success'
          );
        },
        error: err => {
          console.error('Error al actualizar:', err);
          this.sweetAlertService.showNotification(
            'Error',
            'No se pudo actualizar el usuario.',
            'error'
          );
        }
      });
    });
  }

  async onDelete(row: TenantsSelectModel) {
    const confirmed = await this.confirmDialog.confirm({
      text: `¿Deseas eliminar el Usuario "${row.personName}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar'
    });

    if (confirmed) {
      this.tenantsService.delete(row.id).subscribe({
        next: () => {
          this.load();
          this.sweetAlertService.showNotification(
            'Eliminación Exitosa',
            'Usuario eliminado exitosamente.',
            'success'
          );
        },
        error: err => {
          console.error('Error eliminando User:', err);
          this.sweetAlertService.showNotification(
            'Error',
            'No se pudo eliminar al Usuario.',
            'error'
          );
        }
      });
    }
  }

  onView(row: TenantsSelectModel) {
    // Opcional: lógica para visualizar detalle si se desea
  }

  onToggleActive(row: TenantsSelectModel, e: { checked: boolean }) {
    const previous = row.active;
    row.active = e.checked;
    this.tenantsService.changeActiveStatus(row.id, e.checked).subscribe({
      next: (updated) => {
        row.active = updated.active ?? row.active;
        this.sweetAlertService.showNotification(
          'Éxito',
          `Usuario ${row.active ? 'activado' : 'desactivado'} correctamente.`,
          'success'
        );
      },
      error: (err) => {
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
