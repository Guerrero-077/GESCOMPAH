import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { catchError, EMPTY, filter, finalize, map, switchMap, take, tap } from 'rxjs';

import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
import { ToggleButtonComponent } from '../../../../shared/components/toggle-button-component/toggle-button-component.component';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
// import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { FormSelectModel, FormUpdateModel } from '../../models/form.models';
import { FormStore } from '../../services/form/form.store';
import { PageHeaderService } from '../../../../shared/Services/PageHeader/page-header.service';

@Component({
  selector: 'app-form',
  standalone: true,
  imports: [GenericTableComponent, CommonModule, ToggleButtonComponent],
  templateUrl: './form.component.html',
  styleUrls: ['./form.component.css'] // 👈 plural correcto
})
export class FormComponent implements OnInit {

  // Inyección de dependencias
  private readonly formStore = inject(FormStore);
  // private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlert    = inject(SweetAlertService);
  private readonly sweetAlertService = inject(SweetAlertService);
  private readonly pageHeaderService = inject(PageHeaderService);
  constructor(private dialog: MatDialog) {}

  // Estado / datos
  forms$ = this.formStore.forms$;
  selectedForm: FormSelectModel | null = null;
  columns: TableColumn<FormSelectModel>[] = [];

  // Control de concurrencia por ítem (evita doble click mientras llama al backend)
  private busyIds = new Set<number>();
  isBusy = (id: number) => this.busyIds.has(id);

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  ngOnInit(): void {
    this.pageHeaderService.setPageHeader('Formularios', 'Gestión de Formularios');

    this.columns = [
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' },
      { key: 'route', header: 'Route' },
      { key: 'active', header: 'Estado', type: 'custom', template: this.estadoTemplate }
    ];
  }

  // Crear
  onCreateNew(): void {
    import('../../../../shared/components/form-dialog/form-dialog.component').then(m => {
      const dialogRef = this.dialog.open(m.FormDialogComponent, {
        width: '600px',
        data: { entity: {}, formType: 'Form' }
      });

    dialogRef.afterClosed().pipe(
      filter(Boolean),
      switchMap(result => this.formStore.create(result).pipe(take(1))),
      tap(() => {
        this.sweetAlertService.showNotification(
          'Creación Exitosa',
          'Formulario creado exitosamente.',
          'success'
        );
      }),
      catchError(err => {
        console.error('Error creando el formulario:', err);
        this.sweetAlertService.showNotification(
          'Error',
          'No se pudo crear el formulario.',
          'error'
        );
        return EMPTY;
      })
    ).subscribe();
    });
  }

  // Editar
  onEdit(row: FormUpdateModel): void {
    import('../../../../shared/components/form-dialog/form-dialog.component').then(m => {
      const dialogRef = this.dialog.open(m.FormDialogComponent, {
        width: '600px',
        data: { entity: row, formType: 'Form' }
      });

    dialogRef.afterClosed().pipe(
      filter((result): result is Partial<FormUpdateModel> => !!result),
      map(result => ({ id: row.id, ...result } as FormUpdateModel)),
      switchMap(dto => this.formStore.update(dto.id, dto).pipe(take(1))),
      tap(() => {
        this.sweetAlertService.showNotification(
          'Actualización Exitosa',
          'Formulario actualizado exitosamente.',
          'success'
        );
      }),
      catchError(err => {
        console.error('Error actualizando el formulario:', err);
        this.sweetAlertService.showNotification(
          'Error',
          'No se pudo actualizar el formulario.',
          'error'
        );
        return EMPTY;
      })
    ).subscribe();
    });
  }

  // Eliminar (lógico)
  async onDelete(row: FormSelectModel): Promise<void> {
    const result = await this.sweetAlert.showConfirm(
      'Eliminar form',
      `¿Deseas eliminar el form "${row.name}"?`,
      'Eliminar',
      'Cancelar',
      'warning'
    );

    if (!result.isConfirmed) return;

    this.formStore.deleteLogic(row.id).pipe(take(1)).subscribe({
      next: () => {
        this.sweetAlertService.showNotification(
          'Eliminación Exitosa',
          'Formulario eliminado exitosamente.',
          'success'
        );
      },
      error: err => {
        console.error('Error eliminando el formulario:', err);
        this.sweetAlertService.showNotification(
          'Error',
          'No se pudo eliminar el formulario.',
          'error'
        );
      }
    });
  }

  onView(row: FormSelectModel): void {
  }

  // Toggle activo/inactivo (UI optimista + rollback)
  onToggleActive(row: FormSelectModel, e: boolean | { checked: boolean }): void {
    if (this.isBusy(row.id)) return;

    const checked = typeof e === 'boolean' ? e : !!e?.checked; // 👈 normaliza el evento
    const previous = row.active;

    // Optimistic UI + bloquear ítem
    this.busyIds.add(row.id);
    row.active = checked;

    this.formStore.changeActiveStatus(row.id, checked).pipe(
      take(1),
      tap(updated => {
        // Si tu API responde 204 No Content, 'updated' puede ser undefined.
        // Asegura sincronizar el estado visible.
        row.active = updated?.active ?? checked;

        this.sweetAlertService.showNotification(
          'Éxito',
          `Formulario ${row.active ? 'activado' : 'desactivado'} correctamente.`,
          'success'
        );
      }),
      catchError(err => {
        console.error('Error cambiando estado:', err);
        // Revertir estado si falla
        row.active = previous;

        this.sweetAlertService.showNotification(
          'Error',
          err?.error?.detail || 'No se pudo cambiar el estado.',
          'error'
        );
        return EMPTY;
      }),
      finalize(() => {
        this.busyIds.delete(row.id);
      })
    ).subscribe();
  }
}
