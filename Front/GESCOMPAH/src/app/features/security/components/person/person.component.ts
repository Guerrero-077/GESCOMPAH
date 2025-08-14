import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { firstValueFrom } from 'rxjs';

import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';

import { PersonService } from '../../services/person/person.service';
import { CityService } from '../../../setting/services/city/city.service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { PersonSelectModel } from '../../models/person.models';
import { PersonStore } from '../../services/person/person.store';

@Component({
  selector: 'app-person',
  imports: [CommonModule, GenericTableComponent],
  templateUrl: './person.component.html',
  styleUrl: './person.component.css'
})
export class PersonComponent implements OnInit {

  private readonly personStore = inject(PersonStore);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly dialog = inject(MatDialog);
  private readonly cityService = inject(CityService);
  private readonly sweetAllertService = inject(SweetAlertService);


  persons$ = this.personStore.persons$;
  columns: TableColumn<PersonSelectModel>[] = [];

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'firstName', header: 'Nombre' },
      { key: 'lastName', header: 'Apellido' },
      { key: 'document', header: 'Documento' },
      { key: 'address', header: 'Dirección' },
      { key: 'phone', header: 'Teléfono' },
      { key: 'cityName', header: 'Ciudad' }
    ];
  }

  // --- helpers ---
  private async getCityOptions() {
    const cities = await firstValueFrom(this.cityService.getAll());
    return cities.map((c: any) => ({ value: c.id, label: c.name }));
  }

  async onEdit(row: PersonSelectModel) {
    const cityOptions = await this.getCityOptions();

    // Si tu SelectDto aún no trae cityId, no habrá pre-selección (recomendado: incluirlo).
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: { ...row },
        formType: 'Person',
        selectOptions: { cityId: cityOptions }
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.personStore.update(row.id, result).subscribe({
          next: () => {
            this.sweetAllertService.showNotification('Actualización Exitosa', 'Persona actualizada correctamente.', 'success');
          },
          error: err => {
            console.error('Error actualizando persona:', err)
            this.sweetAllertService.showNotification('Error', 'No se pudo actualizar la persona.', 'error');
          }
        });
      }
    });
  }

  async onCreateNew() {
    const cityOptions = await this.getCityOptions();

    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: {} as PersonSelectModel,
        formType: 'Person',
        selectOptions: { cityId: cityOptions }
      }
    });

    dialogRef.afterClosed().subscribe((result: any) => {
      if (result) {
        this.personStore.create(result).subscribe({
          next: () => {
            this.sweetAllertService.showNotification('Creación Exitosa', 'Persona creada exitosamente.', 'success');
          },
          error: err => {
            console.error('Error creando persona:', err)
            this.sweetAllertService.showNotification('Error', 'No se pudo crear la persona.', 'error');
          }
        });
      }
    });
  }

  async onDelete(row: PersonSelectModel) {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar Persona',
      text: `¿Deseas eliminar a "${row.firstName} ${row.lastName}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });

    if (confirmed) {
      this.personStore.deleteLogic(row.id).subscribe({
        next: () => {
          this.sweetAllertService.showNotification('Eliminación Exitosa', 'Persona eliminada exitosamente.', 'success');
        },
        error: err => {
          console.error('Error eliminando persona:', err)
          this.sweetAllertService.showNotification('Error', 'No se pudo eliminar la persona.', 'error');
        }
      });
    }
  }

  onView(row: PersonSelectModel) {
    console.log('Ver persona:', row);
  }
}
