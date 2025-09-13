import { Component, OnInit, TemplateRef, ViewChild, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';

import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
import { ToggleButtonComponent } from '../../../../shared/components/toggle-button-component/toggle-button-component.component';
import { TableColumn } from '../../../../shared/models/TableColumn.models';

import { SquareSelectModel, SquareUpdateModel } from '../../models/squares.models';
import { SquareStore } from '../../services/square/square.store';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { SharedEventsServiceService } from '../../services/shared/shared-events-service.service';
import { HasRoleAndPermissionDirective } from '../../../../core/Directives/HasRoleAndPermission.directive';
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";

@Component({
  selector: 'app-square-list',
  imports: [GenericTableComponent, CommonModule, ToggleButtonComponent, HasRoleAndPermissionDirective, MatProgressSpinnerModule],
  templateUrl: './square-list.component.html',
  styleUrl: './square-list.component.css'
})
export class SquareListComponent implements OnInit {
  // Servicios
  private readonly squaresStore = inject(SquareStore);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlert = inject(SweetAlertService);
  private readonly sharedEvents = inject(SharedEventsServiceService);
  private readonly dialog = inject(MatDialog);

  // Señales (para tabla/plantilla)
  readonly squares = this.squaresStore.items;
  readonly loading = this.squaresStore.loading;
  readonly error = this.squaresStore.error;

  selectedSquare: SquareSelectModel | null = null;
  columns: TableColumn<SquareSelectModel>[] = [];

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  async ngOnInit(): Promise<void> {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' },
      { key: 'location', header: 'Ubicación' },
      { key: 'active', header: 'Estado', type: 'custom', template: this.estadoTemplate }
    ];

    await this.squaresStore.loadAll();
  }

  onView(row: SquareSelectModel) {
  }

  onCreate(): void {
    const ref = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: { entity: {}, formType: 'Plaza' }
    });

    ref.afterClosed().subscribe(async result => {
      if (!result) return;
      try {
        await this.squaresStore.create(result);
        this.sweetAlert.showNotification('Creación Exitosa', 'Plaza creada exitosamente.', 'success');
      } catch (err) {
        this.sweetAlert.showNotification('Error', 'No se pudo crear la plaza.', 'error');
      }
    });
  }

  onEdit(row: SquareUpdateModel): void {
    const ref = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: { entity: row, formType: 'Plaza' }
    });

    ref.afterClosed().subscribe(async (partial: Partial<SquareUpdateModel> | undefined) => {
      if (!partial) return;

      // ✅ Merge: todos los campos requeridos quedan definidos
      const dto: SquareUpdateModel = { ...row, ...partial, id: row.id };

      try {
        await this.squaresStore.update(dto.id, dto);
        this.sweetAlert.showNotification('Actualización Exitosa', 'Plaza actualizada exitosamente.', 'success');
      } catch (err) {
        this.sweetAlert.showNotification('Error', 'No se pudo actualizar la plaza.', 'error');
      }
    });
  }


  async onDelete(row: SquareSelectModel): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar plaza',
      text: `¿Deseas eliminar la plaza "${row.name}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });
    if (!confirmed) return;

    try {
      await this.squaresStore.deleteLogic(row.id);
      this.sweetAlert.showNotification('Eliminación exitosa', 'Plaza eliminada correctamente.', 'success');
    } catch (err) {
      this.sweetAlert.showNotification('Error', 'No se pudo eliminar la plaza.', 'error');
    }
  }

  // Toggle estado (optimista + confirmación en el store)
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
      await this.squaresStore.changeActiveStatusRemote(id, checked);
      this.sharedEvents.notifyPlazaStateChanged(id);
      this.sweetAlert.showNotification('Éxito', `Plaza ${checked ? 'activada' : 'desactivada'} correctamente.`, 'success');
    } catch (err: any) {
      this.sweetAlert.showNotification('Error', err?.message || 'No se pudo cambiar el estado.', 'error');
    }
  }

  trackById = (_: number, item: SquareSelectModel) => item.id;
}
