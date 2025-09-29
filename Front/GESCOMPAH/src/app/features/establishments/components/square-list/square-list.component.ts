import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild, inject, effect } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';

import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
import { ToggleButtonComponent } from '../../../../shared/components/toggle-button-component/toggle-button-component.component';
import { TableColumn } from '../../../../shared/models/TableColumn.models';

import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { HasRoleAndPermissionDirective } from '../../../../core/security/directives/HasRoleAndPermission.directive';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { SquareSelectModel, SquareUpdateModel } from '../../models/squares.models';
import { SharedEventsServiceService } from '../../services/shared/shared-events-service.service';
import { SquareStore } from '../../services/square/square.store';

@Component({
  selector: 'app-square-list',
  imports: [GenericTableComponent, CommonModule, ToggleButtonComponent, HasRoleAndPermissionDirective, MatProgressSpinnerModule],
  templateUrl: './square-list.component.html',
  styleUrl: './square-list.component.css'
})
export class SquareListComponent implements OnInit {
  private readonly squaresStore = inject(SquareStore);
  private readonly sweetAlert = inject(SweetAlertService);
  private readonly sharedEvents = inject(SharedEventsServiceService);
  private readonly dialog = inject(MatDialog);

  readonly squares = this.squaresStore.items;
  readonly loading = this.squaresStore.loading;
  readonly error = this.squaresStore.error;

  selectedSquare: SquareSelectModel | null = null;
  columns: TableColumn<SquareSelectModel>[] = [];

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  async ngOnInit(): Promise<void> {
    this.columns = [
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' },
      { key: 'location', header: 'Ubicación' },
      { key: 'active', header: 'Estado', type: 'custom', template: this.estadoTemplate }
    ];

    await this.squaresStore.loadAll();
  }

  // Notificación de error estandarizada
  private readonly errorToast = effect(() => {
    const err = this.error();
    if (err) {
      this.sweetAlert.showApiError(err, 'No se pudieron cargar las plazas.');
    }
  });

  onView(row: SquareSelectModel) { }

  onCreate(): void {
    import('../../../../shared/components/form-dialog/form-dialog.component').then(m => {
      const ref = this.dialog.open(m.FormDialogComponent, {
        width: '600px',
        data: { entity: {}, formType: 'Plaza' }
      });

      ref.afterClosed().subscribe(async result => {
        if (!result) return;
        try {
          await this.squaresStore.create(result);
          this.sweetAlert.showNotification('Creación Exitosa', 'Plaza creada exitosamente.', 'success');
        } catch (err) {
          this.sweetAlert.showApiError(err, 'No se pudo crear la plaza.');
        }
      });
    });
  }

  onEdit(row: SquareUpdateModel): void {
    import('../../../../shared/components/form-dialog/form-dialog.component').then(m => {
      const ref = this.dialog.open(m.FormDialogComponent, {
        width: '600px',
        data: { entity: row, formType: 'Plaza' }
      });

      ref.afterClosed().subscribe(async (partial: Partial<SquareUpdateModel> | undefined) => {
        if (!partial) return;
        const dto: SquareUpdateModel = { ...row, ...partial, id: row.id };
        try {
          await this.squaresStore.update(dto.id, dto);
          this.sweetAlert.showNotification('Actualización Exitosa', 'Plaza actualizada exitosamente.', 'success');
        } catch (err) {
          this.sweetAlert.showApiError(err, 'No se pudo actualizar la plaza.');
        }
      });
    });
  }

  async onDelete(row: SquareSelectModel): Promise<void> {
    const result = await this.sweetAlert.showConfirm(
      'Eliminar plaza',
      `¿Deseas eliminar la plaza "${row.name}"?`,
      'Eliminar',
      'Cancelar',
      'warning'
    );
    if (!result.isConfirmed) return;
    try {
      await this.squaresStore.deleteLogic(row.id);
      this.sweetAlert.showNotification('Eliminación exitosa', 'Plaza eliminada correctamente.', 'success');
    } catch (err) {
      this.sweetAlert.showApiError(err, 'No se pudo eliminar la plaza.');
    }
  }

  async onToggleActive(
    id: number | null | undefined,
    e: { checked: boolean } | boolean | null | undefined
  ): Promise<void> {
    if (id == null) {
      this.sweetAlert.showNotification('Sin plaza', 'No se pudo obtener el ID de la plaza.', 'warning');
      return;
    }
    const checked = typeof e === 'boolean' ? e : !!e?.checked;
    try {
      const res = await this.squaresStore.changeActiveStatusRemote(id, checked);
      if (!res?.ok) {
        const msg = res?.message || 'No se pudo cambiar el estado de la plaza.';
        this.sweetAlert.showNotification('Operación no permitida', msg, 'warning');
        return;
      }
      this.sharedEvents.notifyPlazaStateChanged(id);
      this.sweetAlert.showNotification('Éxito', `Plaza ${checked ? 'activada' : 'desactivada'} correctamente.`, 'success');
    } catch (err: any) {
      const detail = err?.error?.detail || err?.error?.message || err?.error?.title || err?.message;
      const msg = detail || 'No se pudo cambiar el estado de la plaza.';
      this.sweetAlert.showNotification('Operación no permitida', msg, 'warning');
    }
  }

  trackById = (_: number, item: SquareSelectModel) => item.id;

  isBusy(id: number | null | undefined): boolean {
    return typeof id === 'number' ? this.squaresStore.isBusy(id) : false;
  }
}
