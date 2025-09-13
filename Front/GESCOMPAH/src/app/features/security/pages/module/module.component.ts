import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { filter, switchMap, take, tap, catchError, EMPTY, finalize, map } from 'rxjs';

import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
import { ToggleButtonComponent } from '../../../../shared/components/toggle-button-component/toggle-button-component.component';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';

import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { PageHeaderService } from '../../../../shared/Services/PageHeader/page-header.service';

import { ModuleStore } from '../../services/module/module.store';
import { ModuleSelectModel, ModuleUpdateModel } from '../../models/module.models';

@Component({
  selector: 'app-module',
  standalone: true,
  imports: [GenericTableComponent, CommonModule, ToggleButtonComponent],
  templateUrl: './module.component.html',
  styleUrls: ['./module.component.css']
})
export class ModuleComponent implements OnInit {
  // Inyección de dependencias
  private readonly moduleStore = inject(ModuleStore);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);
  private readonly pageHeaderService = inject(PageHeaderService);
  constructor(private dialog: MatDialog) {}

  // Estado
  modules$ = this.moduleStore.modules$;
  columns: TableColumn<ModuleSelectModel>[] = [];
  private busyIds = new Set<number>();
  isBusy = (id: number) => this.busyIds.has(id);

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  ngOnInit(): void {
    this.pageHeaderService.setPageHeader('Módulos', 'Gestión de Módulos');

    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' },
      { key: 'route', header: 'Route' },
      { key: 'active', header: 'Estado', type: 'custom', template: this.estadoTemplate }
    ];
  }

  // Crear
  onCreateNew(): void {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: { entity: {}, formType: 'Module' }
    });

    dialogRef.afterClosed().pipe(
      filter(Boolean),
      switchMap(result => this.moduleStore.create(result).pipe(take(1))),
      tap(() => this.sweetAlertService.showNotification('Creación Exitosa', 'Módulo creado exitosamente.', 'success')),
      catchError(err => {
        console.error('Error creando el módulo:', err);
        this.sweetAlertService.showNotification('Error', 'No se pudo crear el módulo.', 'error');
        return EMPTY;
      })
    ).subscribe();
  }

  // Editar
  onEdit(row: ModuleUpdateModel): void {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: { entity: row, formType: 'Module' }
    });

    dialogRef.afterClosed().pipe(
      filter((result): result is Partial<ModuleUpdateModel> => !!result),
      map(result => ({ id: row.id, ...result } as ModuleUpdateModel)),
      switchMap(dto => this.moduleStore.update(dto.id, dto).pipe(take(1))),
      tap(() => this.sweetAlertService.showNotification('Actualización Exitosa', 'Módulo actualizado exitosamente.', 'success')),
      catchError(err => {
        console.error('Error actualizando el módulo:', err);
        this.sweetAlertService.showNotification('Error', 'No se pudo actualizar el módulo.', 'error');
        return EMPTY;
      })
    ).subscribe();
  }

  // Eliminar (lógico)
  async onDelete(row: ModuleSelectModel): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar módulo',
      text: `¿Deseas eliminar el módulo "${row.name}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });
    if (!confirmed) return;

    this.moduleStore.deleteLogic(row.id).pipe(take(1)).subscribe({
      next: () => this.sweetAlertService.showNotification('Eliminación Exitosa', 'Módulo eliminado exitosamente.', 'success'),
      error: err => {
        console.error('Error eliminando el módulo:', err);
        this.sweetAlertService.showNotification('Error', 'No se pudo eliminar el módulo.', 'error');
      }
    });
  }

  onView(row: ModuleSelectModel): void {
  }

  // Toggle activo/inactivo (UI optimista + rollback)
  onToggleActive(row: ModuleSelectModel, e: boolean | { checked: boolean }): void {
    if (this.isBusy(row.id)) return;

    const checked = typeof e === 'boolean' ? e : !!e?.checked;
    const previous = row.active;

    // Optimistic UI + lock por ítem
    this.busyIds.add(row.id);
    row.active = checked;

    this.moduleStore.changeActiveStatus(row.id, checked).pipe(
      take(1),
      tap(updated => {
        // Si la API devuelve 204 No Content, updated puede ser undefined.
        row.active = updated?.active ?? checked;
        this.sweetAlertService.showNotification(
          'Éxito',
          `Módulo ${row.active ? 'activado' : 'desactivado'} correctamente.`,
          'success'
        );
      }),
      catchError(err => {
        console.error('Error cambiando estado:', err);
        row.active = previous; // revertir
        this.sweetAlertService.showNotification(
          'Error',
          err?.error?.detail || 'No se pudo cambiar el estado.',
          'error'
        );
        return EMPTY;
      }),
      finalize(() => this.busyIds.delete(row.id))
    ).subscribe();
  }
}
