import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";
import { catchError, EMPTY, filter, map, switchMap, take, tap } from 'rxjs';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { TableColumn } from '../../../../shared/models/TableColumn.models';
// import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { PageHeaderService } from '../../../../shared/Services/PageHeader/page-header.service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { StandardButtonComponent } from "../../../../shared/components/standard-button/standard-button.component";
import { AppointmentSelect, AppointmentUpdateModel } from '../../models/appointment.models';
import { AppointmentStore } from '../../services/appointment/appointment.store';

import { HasRoleAndPermissionDirective } from '../../../../core/security/directives/HasRoleAndPermission.directive';


@Component({
  selector: 'app-appointment-list',
  imports: [CommonModule, GenericTableComponent, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './appointment-list.component.html',
  styleUrl: './appointment-list.component.css'
})
export class AppointmentListComponent implements OnInit {

  // Dependencias inyectadas
  private readonly appointmentStore = inject(AppointmentStore);
  // private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);
  private readonly pageHeaderService = inject(PageHeaderService);


  // Observable de formularios
  appointments$ = this.appointmentStore.appointments$;
  selectedForm: any = null;

  pendingId: number | null = null;
  desiredState: boolean | null = null;

  columns: TableColumn<AppointmentSelect>[] = [];

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;
  @ViewChild('userTemplate', { static: true }) userTemplate!: TemplateRef<any>;
  @ViewChild('onlyDateTpl', { static: true }) onlyDateTpl!: TemplateRef<any>;
  @ViewChild('dateTimeTpl', { static: true }) dateTimeTpl!: TemplateRef<any>;

  constructor(private dialog: MatDialog) { }

  ngOnInit(): void {
    this.pageHeaderService.setPageHeader('Citas', 'Gestión de Citas');
    this.columns = [
      { key: 'personName', header: 'User', template: this.userTemplate },
      { key: 'establishmentName', header: 'Local' },
      { key: 'description', header: 'Descripcion' },
      { key: 'requestDate', header: 'Fecha de solicitud', template: this.onlyDateTpl },
      { key: 'dateTimeAssigned', header: 'Fecha de asignacion', template: this.dateTimeTpl },
      {
        key: 'active',
        header: 'Estado',
        type: 'custom',
        template: this.estadoTemplate
      }
    ];
  }

  onCreateNew() {

    import('../../components/form-appointment/form-appointment.component').then(m => {
      const ref = this.dialog.open(m.FormAppointmentComponent, {
        width: '800px',
        disableClose: true,
        autoFocus: true,
        data: null,
      });

      ref.afterClosed().pipe(take(1)).subscribe(async (created: boolean) => {
        if (!created) return;
        this.sweetAlertService.showNotification('Éxito', 'Formulario creado correctamente.', 'success');
        await this.appointmentStore.loadAll()
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


  async onDelete(row: AppointmentSelect) {
    const result = await this.sweetAlertService.showConfirm(
      'Eliminar form',
      `¿Deseas eliminar el form "${row.personName}"?`,
      'Eliminar',
      'Cancelar',
      'warning'
    );

    if (result.isConfirmed) {
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

  onView(row: AppointmentSelect): void {
    import('../../components/appointment-detail-dialog/appointment-detail-dialog.component').then(m => {
      this.dialog.open(m.AppointmentDetailDialogComponent, {
        width: '900px',
        maxWidth: '95vw',
        data: { id: row.id },
        autoFocus: false,
        disableClose: false,
      });
    });
  }



  // Toggle activo/inactivo (UI optimista + rollback)
  onToggleActive(row: AppointmentSelect, e: { checked: boolean }) {
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
