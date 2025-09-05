import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { catchError, EMPTY, filter, map, switchMap, take, tap } from 'rxjs';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { AppointmentSelectModel, AppointmentUpdateModel } from '../../models/appointment.models';
import { AppointmentStore } from '../../services/appointment/appointment.store';
import { PageHeaderService } from '../../../../shared/Services/PageHeader/page-header.service';

@Component({
  selector: 'app-appointment-list',
  imports: [CommonModule, GenericTableComponent, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './appointment-list.component.html',
  styleUrl: './appointment-list.component.css'
})
export class AppointmentListComponent implements OnInit {

  // Dependencias inyectadas
  private readonly appointmentStore = inject(AppointmentStore);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);
  private readonly pageHeaderService = inject(PageHeaderService);


  // Observable de formularios
  appointments$ = this.appointmentStore.appointments$;
  selectedForm: any = null;

  pendingId: number | null = null;
  desiredState: boolean | null = null;

  columns: TableColumn<AppointmentSelectModel>[] = [];

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;
  @ViewChild('userTemplate', { static: true }) userTemplate!: TemplateRef<any>;

  constructor(private dialog: MatDialog) { }

  ngOnInit(): void {
    this.pageHeaderService.setPageHeader('Citas', 'Gestión de Citas');
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'email', header: 'User', template: this.userTemplate },
      { key: 'phone', header: 'Telefono' },
      { key: 'establishmentName', header: 'Local' },
      {
        key: 'active',
        header: 'Estado',
        type: 'custom',
        template: this.estadoTemplate
      }
    ];
  }

  onCreateNew() {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: { entity: {}, formType: 'Form' }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;

      this.appointmentStore.create(result).pipe(take(1)).subscribe({
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
  onEdit(row: AppointmentUpdateModel) {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: { entity: row, formType: 'Form' }
    });

    dialogRef.afterClosed().pipe(
      filter((result): result is Partial<AppointmentUpdateModel> => !!result),

      map(result => ({ id: row.id, ...result } as AppointmentUpdateModel)),

      switchMap(dto => this.appointmentStore.update(dto.id, dto)),

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


  async onDelete(row: AppointmentSelectModel) {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar form',
      text: `¿Deseas eliminar el form "${row.fullName}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });

    if (confirmed) {
      this.appointmentStore.deleteLogic(row.id).subscribe({
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

  onView(row: AppointmentSelectModel) {
    console.log('Ver:', row);
  }



  // ----- Toggle estado (activo/inactivo) -----
  onToggleActive(row: AppointmentSelectModel, e: { checked: boolean }) {
    const previous = row.active;
    row.active = e.checked;
    this.appointmentStore.changeActiveStatus(row.id, e.checked).subscribe({
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



  confirmState(row: any, newState: boolean) {
    // Abre tu modal/diálogo de confirmación aquí
    // Ejemplo con MatDialog (ajusta a tu propio componente de modal)
  }

}
