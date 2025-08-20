import { Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { TenantsService } from '../../services/tenants/tenants.service';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { TenantFormData, TenantsSelectModel } from '../../models/tenants.models';
import { TenantsFormDialogComponent } from '../../components/tenants-form-dialog/tenants-form-dialog.component';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { Title } from '@angular/platform-browser';
import { ToggleButtonComponent } from "../../../../shared/components/toggle-button-component/toggle-button-component.component";
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-tenants-list',
  standalone: true,
  imports: [CommonModule,GenericTableComponent, ToggleButtonComponent],
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
      { key: 'email', header: 'User', template: this.userTemplate },
      { key: 'personDocument', header: 'N° Documento' },
      { key: 'personPhone', header: 'Teléfono' },
      { key: 'personAddress', header: 'Dirección' },
      { key: 'cityName', header: 'Ciudad' },
      {
        key: 'roles', header: 'Rol',
        render: r => Array.isArray((r as any).roles)
          ? (r as any).roles.map((x: any) => x.name ?? x).join(', ')
          : (r as any).roles ?? ''
      },
      { key: 'active', header: 'Estado', template: this.estadoTemplate }
    ];
    this.load();
  }

  private num(v: any): number | null {
    const n = Number(v);
    return Number.isFinite(n) && n > 0 ? n : null;
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
        next: () => this.load(),
        error: err => console.error('Error al crear:', err)
      });
    });
  }

  onEdit(row: TenantsSelectModel) {
    const id = (row as any).id;

    this.tenantsService.getById(id).subscribe({
      next: (detail: any) => {
        const p = detail?.person ?? {};
        // Fallbacks: persona → raíz del detalle → fila
        const cityId =
          this.num(p?.cityId) ??
          this.num(p?.city?.id) ??
          this.num(detail?.cityId) ??
          this.num(detail?.city?.id) ??
          this.num((row as any)?.cityId);

        const roleNameFromList = Array.isArray((row as any).roles) ? (row as any).roles[0] : null;
        const roleNameFromDetail = Array.isArray((detail as any).roles) ? (detail as any).roles[0] : null;

        const fullName = String((row as any).personName ?? '').trim();
        const [firstFromFull, ...rest] = fullName.split(/\s+/);
        const lastFromFull = rest.join(' ').trim() || null;

        const data: TenantFormData = {
          mode: 'edit',
          tenant: {
            id,
            personId: Number(p?.id ?? detail?.personId ?? 0) || undefined,

            // ubicación (sin departamento en edición)
            departmentId: null,
            cityId: cityId ?? null,

            // persona
            firstName: p?.firstName ?? firstFromFull ?? null,
            lastName: p?.lastName ?? lastFromFull ?? null,
            document: p?.document ?? (row as any).personDocument ?? null,
            phone: p?.phone ?? (row as any).personPhone ?? null,
            address: p?.address ?? (row as any).personAddress ?? null,

            // cuenta
            email: (detail?.email ?? (row as any).email) || '',

            // rol
            roleName: (roleNameFromDetail ?? roleNameFromList) || null,

            active: detail?.active ?? (row as any).active
          }
        };

        const ref = this.dialog.open(TenantsFormDialogComponent, { width: '720px', data });
        ref.afterClosed().subscribe(payload => {
          if (!payload) return;
          this.tenantsService.update(id, payload).subscribe({
            next: () => this.load(),
            error: e => console.error('Error al actualizar:', e)
          });
        });
      },
      error: e => console.error('No se pudo cargar el detalle del usuario:', e)
    });
  }

  async onDelete(row: TenantsSelectModel) {

    const confirmed = await this.confirmDialog.confirm({
      text: `¿Deseas eliminar el Usuario "${row.personName}" ?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar'
    })

    if (confirmed) {
      this.tenantsService.delete(row.id).subscribe({
        next: () => {
          this.load()
          this.sweetAlertService.showNotification('Eliminación Exitosa', 'Usuario eliminado exitosamente.', 'success');
        },
        error: err => {
          console.error('Error eliminando User:', err)
          this.sweetAlertService.showNotification('Error', 'No se pudo eliminar al Usuario.', 'error');
        }
      })
    }
  }

  onView(row: TenantsSelectModel) { /* opcional */ }

  // ----- Toggle estado (activo/inactivo) -----
  onToggleActive(row: TenantsSelectModel, e: { checked: boolean }) {
    const previous = row.active;
    row.active = e.checked;
    this.tenantsService.changeActiveStatus(row.id, e.checked).subscribe({
      next: (updated) => {
        // sincronizar con lo que devuelve el backend
        row.active = updated.active ?? row.active;
        this.sweetAlertService.showNotification(
          'Éxito',
          `Usuario ${row.active ? 'activado' : 'desactivado'} correctamente.`,
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
