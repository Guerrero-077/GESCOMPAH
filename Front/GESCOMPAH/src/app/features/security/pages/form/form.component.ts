import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { catchError, EMPTY, filter, map, switchMap, take, tap } from 'rxjs';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { ToggleButtonComponent } from "../../../../shared/components/toggle-button-component/toggle-button-component.component";
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { FormSelectModel, FormUpdateModel } from '../../models/form.models';
import { FormStore } from '../../services/form/form.store';

@Component({
  selector: 'app-form',
  imports: [GenericTableComponent, CommonModule, ToggleButtonComponent],
  templateUrl: './form.component.html',
  styleUrl: './form.component.css',

})
export class FormComponent implements OnInit {

  // Dependencias inyectadas
  private readonly formStore = inject(FormStore);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);

  // Observable de formularios
  forms$ = this.formStore.forms$;
  selectedForm: any = null;

  columns: TableColumn<FormSelectModel>[] = [];

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;
  constructor(private dialog: MatDialog) { }

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' },
      { key: 'route', header: 'Route' },
      {
        key: 'active',
        header: 'Estado',
        type: 'custom',           // tu GenericTable debe renderizar template cuando type === 'custom'
        template: this.estadoTemplate
      }
    ];
  }

  onEdit(row: FormUpdateModel) {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: { entity: row, formType: 'Form' }
    });

    dialogRef.afterClosed().pipe(
      filter((result): result is Partial<FormUpdateModel> => !!result),

      map(result => ({ id: row.id, ...result } as FormUpdateModel)),

      switchMap(dto => this.formStore.update(dto.id, dto)),

      take(1),

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
  }


  async onDelete(row: FormSelectModel) {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar form',
      text: `¿Deseas eliminar el form "${row.name}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });

    if (confirmed) {
      this.formStore.deleteLogic(row.id).subscribe({
        next: () => {
          this.sweetAlertService.showNotification('Eliminación Exitosa', 'Formulario eliminado exitosamente.', 'success');
        },
        error: err => {
          console.error('Error eliminando el formulario:', err);
          this.sweetAlertService.showNotification('Error', 'No se pudo eliminar el formulario.', 'error');
        }
      });
    }
  }

  onView(row: FormSelectModel) {
    console.log('Ver:', row);
  }

  onCreateNew() {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: { entity: {}, formType: 'Form' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;

      this.formStore.create(result).pipe(take(1)).subscribe({
        next: () => {
          this.sweetAlertService.showNotification(
            'Creación Exitosa',
            'Formulario creado exitosamente.',
            'success'
          );
        },
        error: (err) => {
          console.error('Error creando el formulario:', err);
          this.sweetAlertService.showNotification(
            'Error',
            'No se pudo crear el formulario.',
            'error'
          );
        }
      });
    });
  }


  // ----- Toggle estado (activo/inactivo) -----
  onToggleActive(row: FormSelectModel, e: { checked: boolean }) {
    const previous = row.active;
    row.active = e.checked;
    this.formStore.changeActiveStatus(row.id, e.checked).subscribe({
      next: (updated) => {
        // sincronizar con lo que devuelve el backend
        row.active = updated.active ?? row.active;
        this.sweetAlertService.showNotification(
          'Éxito',
          `Formulario ${row.active ? 'activado' : 'desactivado'} correctamente.`,
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
