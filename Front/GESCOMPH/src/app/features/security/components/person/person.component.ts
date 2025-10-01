import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { catchError, map, of, take } from 'rxjs';

import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
import { ToggleButtonComponent } from '../../../../shared/components/toggle-button-component/toggle-button-component.component';

import { TableColumn } from '../../../../shared/models/TableColumn.models';
// import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';

import { CityService } from '../../../setting/services/city/city.service';
import { PersonSelectModel } from '../../models/person.models';
import { PersonStore } from '../../services/person/person.store';

type SelectOption<T = number> = { value: T; label: string };

@Component({
  selector: 'app-person',
  standalone: true,
  imports: [CommonModule, GenericTableComponent, ToggleButtonComponent],
  templateUrl: './person.component.html',
  styleUrls: ['./person.component.css']
})
export class PersonComponent implements OnInit {

  // Inyección de dependencias
  private readonly personStore       = inject(PersonStore);
  // private readonly confirmDialog     = inject(ConfirmDialogService);
  private readonly sweetAlert        = inject(SweetAlertService);
  private readonly dialog            = inject(MatDialog);
  private readonly cityService       = inject(CityService);
  private readonly sweetAlertService = inject(SweetAlertService);

  // Estado
  readonly persons$ = this.personStore.persons$;
  columns: TableColumn<PersonSelectModel>[] = [];
  private readonly busyToggleIds = new Set<number>(); // evita dobles clics en toggle

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  // Ciclo de vida
  ngOnInit(): void {
    this.columns = [
      { key: 'firstName', header: 'Nombre' },
      { key: 'lastName', header: 'Apellido' },
      { key: 'document', header: 'Documento' },
      { key: 'address', header: 'Dirección' },
      { key: 'phone', header: 'Teléfono' },
      { key: 'cityName', header: 'Ciudad' },
      { key: 'active', header: 'Estado', type: 'custom', template: this.estadoTemplate }
    ];
  }

  // Utilidades UI
  trackById = (_: number, it: PersonSelectModel) => it.id;
  

  // Helpers
  private getCityOptions$() {
    return this.cityService.getAll().pipe(
      take(1),
      catchError(() => of([])),
      map((cities: any[]): SelectOption[] =>
        cities.map(c => ({ value: c.id, label: c.name ?? `Ciudad ${c.id}` }))
      )
    );
  }

  // Editar
  onEdit(row: PersonSelectModel) {
    this.getCityOptions$().subscribe((cityOptions) => {
      import('../../../../shared/components/form-dialog/form-dialog.component').then(m => {
      const dialogRef = this.dialog.open(m.FormDialogComponent, {
        width: '600px',
        data: {
          entity: { ...row }, // ideal: incluir cityId en tu SelectDto para preselección
          formType: 'Person',
          selectOptions: { cityId: cityOptions }
        }
      });

      dialogRef.afterClosed().pipe(take(1)).subscribe(result => {
        if (!result) return;
        this.personStore.update(row.id, result).pipe(take(1)).subscribe({
          next: () => this.sweetAlertService.showNotification('Actualización Exitosa', 'Persona actualizada correctamente.', 'success'),
          error: (err) => {
            console.error('Error actualizando persona:', err);
            this.sweetAlertService.showApiError(err, 'No se pudo actualizar la persona.');
          }
        });
      });
      });
    });
  }

  // Crear
  onCreateNew() {
    this.getCityOptions$().subscribe((cityOptions) => {
      import('../../../../shared/components/form-dialog/form-dialog.component').then(m => {
      const dialogRef = this.dialog.open(m.FormDialogComponent, {
        width: '600px',
        data: {
          entity: {} as PersonSelectModel,
          formType: 'Person',
          selectOptions: { cityId: cityOptions }
        }
      });

      dialogRef.afterClosed().pipe(take(1)).subscribe((result: any) => {
        if (!result) return;
        this.personStore.create(result).pipe(take(1)).subscribe({
          next: () => this.sweetAlertService.showNotification('Creación Exitosa', 'Persona creada exitosamente.', 'success'),
          error: (err) => {
            console.error('Error creando persona:', err);
            this.sweetAlertService.showApiError(err, 'No se pudo crear la persona.');
          }
        });
      });
      });
    });
  }

  // Eliminar
  async onDelete(row: PersonSelectModel) {
    const result = await this.sweetAlert.showConfirm(
      'Eliminar Persona',
      `¿Deseas eliminar a "${row.firstName} ${row.lastName}"?`,
      'Eliminar',
      'Cancelar',
      'warning'
    );
    if (!result.isConfirmed) return;

    this.personStore.deleteLogic(row.id).pipe(take(1)).subscribe({
      next: () => this.sweetAlertService.showNotification('Eliminación Exitosa', 'Persona eliminada exitosamente.', 'success'),
      error: (err) => {
        console.error('Error eliminando persona:', err);
        this.sweetAlertService.showApiError(err, 'No se pudo eliminar la persona.');
      }
    });
  }

  onView(row: PersonSelectModel) {
    // Ver detalle (diálogo o navegación)
  }

  // Toggle activo/inactivo (UI optimista + rollback)
  onToggleActive(row: PersonSelectModel, e: { checked: boolean } | boolean) {
    const nextValue = typeof e === 'boolean' ? e : !!e?.checked;

    if (this.busyToggleIds.has(row.id)) return; // evita doble click
    this.busyToggleIds.add(row.id);

    const prev = row.active;
    // UI optimista
    row.active = nextValue;

    this.personStore.changeActiveStatus(row.id, nextValue).pipe(take(1)).subscribe({
      next: (updated: Partial<PersonSelectModel> | void) => {
        // Si la API devuelve DTO con 'active', sincronizamos; si no, mantenemos optimista
        if (updated && typeof updated.active === 'boolean') {
          row.active = updated.active;
        }
        this.sweetAlertService.showNotification('Éxito', `Persona ${row.active ? 'activada' : 'desactivada'} correctamente.`, 'success');
        this.busyToggleIds.delete(row.id);
      },
      error: (err) => {
        // rollback
        row.active = prev;
        this.sweetAlertService.showApiError(err, 'No se pudo cambiar el estado.');
        this.busyToggleIds.delete(row.id);
      }
    });
  }
}
