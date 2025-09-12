import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
import { ToggleButtonComponent } from '../../../../shared/components/toggle-button-component/toggle-button-component.component';
import { TableColumn } from '../../../../shared/models/TableColumn.models';

// import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { PageHeaderService } from '../../../../shared/Services/PageHeader/page-header.service';

import { TenantsFormDialogComponent } from '../../components/tenants-form-dialog/tenants-form-dialog.component';
import {
  TenantFormData,
  TenantsSelectModel,
  TenantsUpdateModel,
  TenantsCreateModel,
} from '../../models/tenants.models';
import { TenantStore } from '../../services/tenants/tenants.store';
import { HasRoleAndPermissionDirective } from '../../../../core/Directives/HasRoleAndPermission.directive';

@Component({
  selector: 'app-tenants-list',
  standalone: true,
  imports: [
    CommonModule,
    GenericTableComponent,
    ToggleButtonComponent,
    HasRoleAndPermissionDirective,
    MatProgressSpinnerModule
  ],
  templateUrl: './tenants-list.component.html',
  styleUrls: ['./tenants-list.component.css'],
})
export class TenantsListComponent implements OnInit {
  // Services / stores
  private readonly store = inject(TenantStore);
  // private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlert = inject(SweetAlertService);
  private readonly dialog = inject(MatDialog);
  private readonly pageHeader = inject(PageHeaderService);

  // Signals (para la plantilla)
  readonly tenants = this.store.items;
  readonly loading = this.store.loading;
  readonly error = this.store.error;

  // Tabla
  columns: TableColumn<TenantsSelectModel>[] = [];

  @ViewChild('userTemplate', { static: true }) userTemplate!: TemplateRef<any>;
  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  async ngOnInit(): Promise<void> {
    this.pageHeader.setPageHeader('Arrendatarios', 'Gestión de Arrendatarios');

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
        render: (r) => (Array.isArray(r.roles) ? r.roles.join(', ') : ''),
      },
      { key: 'active', header: 'Estado', template: this.estadoTemplate },
    ];

    await this.store.loadAll();
  }

  // -------- CRUD --------
  onCreate(): void {
    const data: TenantFormData = { mode: 'create' };
    const ref = this.dialog.open(TenantsFormDialogComponent, { width: '720px', data });

    ref.afterClosed().subscribe(async (payload?: TenantsCreateModel) => {
      if (!payload) return;
      try {
        await this.store.create(payload);
        this.sweetAlert.toast('Éxito', 'Usuario creado exitosamente.', 'success');
      } catch {
        this.sweetAlert.toast('Error', 'No se pudo crear el usuario.', 'error');
      }
    });
  }

  onEdit(row: TenantsSelectModel): void {
    const data: TenantFormData = { mode: 'edit', tenant: row };
    const ref = this.dialog.open(TenantsFormDialogComponent, { width: '720px', data });

    ref.afterClosed().subscribe(async (partial?: Partial<TenantsUpdateModel>) => {
      if (!partial) return;

      const dto = this.toUpdateDto(row, partial);
      if (!dto) {
        this.sweetAlert.toast(
          'Datos incompletos',
          'Faltan datos obligatorios para actualizar el usuario.',
          'warning'
        );
        return;
      }

      try {
        await this.store.update(dto.id, dto);
        this.sweetAlert.toast('Éxito', 'Usuario actualizado exitosamente.', 'success');
      } catch {
        this.sweetAlert.toast('Error', 'No se pudo actualizar el usuario.', 'error');
      }
    });
  }

  async onDelete(row: TenantsSelectModel): Promise<void> {
    const confirmed = await this.sweetAlert.confirm({
      text: `¿Deseas eliminar el Usuario "${row.personName}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });
    if (!confirmed) return;

    try {
      await this.store.delete(row.id); // si tu back es lógico: this.store.deleteLogic(row.id)
      this.sweetAlert.toast('Eliminación Exitosa', 'Usuario eliminado exitosamente.', 'success');
    } catch {
      this.sweetAlert.toast('Error', 'No se pudo eliminar al Usuario.', 'error');
    }
  }

  onView(_row: TenantsSelectModel) {
    // (opcional) ver detalle
  }

  // Toggle estado → usa store (optimista + rollback si falla)
  async onToggleActive(
    id: number | null | undefined,
    e: { checked: boolean } | boolean | null | undefined
  ): Promise<void> {
    if (id == null) {
      this.sweetAlert.toast('Sin usuario', 'No se pudo obtener el ID del usuario.', 'warning');
      return;
    }

    const checked = typeof e === 'boolean' ? e : !!e?.checked;

    try {
      await this.store.changeActiveStatusRemote(id, checked);
      this.sweetAlert.toast(
        'Éxito',
        `Usuario ${checked ? 'activado' : 'desactivado'} correctamente.`,
        'success'
      );
    } catch (err: any) {
      this.sweetAlert.toast('Error', err?.message || 'No se pudo cambiar el estado.', 'error');
    }
  }

  // -------- Helpers --------
  private toUpdateDto(
    row: TenantsSelectModel,
    partial: Partial<TenantsUpdateModel>
  ): TenantsUpdateModel | null {
    const dto: TenantsUpdateModel = {
      id: row.id,
      firstName: partial.firstName ?? (row as any).firstName ?? '',
      lastName:  partial.lastName  ?? (row as any).lastName  ?? '',
      email:     partial.email     ?? row.email,
      phone:     partial.phone     ?? row.personPhone ?? '',
      address:   partial.address   ?? row.personAddress ?? '',
      cityId:    partial.cityId    ?? (row as any).cityId,
      roleIds:   partial.roleIds   ?? (row as any).roleIds ?? [],
      active:    partial.active    ?? row.active,
    };

    if (!dto.firstName || !dto.lastName || !dto.email || dto.cityId == null) return null;
    return dto;
  }

  trackById = (_: number, item: TenantsSelectModel) => item.id;
}
