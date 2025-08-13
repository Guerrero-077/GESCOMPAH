import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { firstValueFrom } from 'rxjs';

import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';

import { PersonService } from '../../services/person/person.service';
import { Person } from '../../models/person.models';
// ⚠ Ajusta la ruta según tu proyecto
import { CityService } from '../../../setting/services/city/city.service';

@Component({
  selector: 'app-person',
  imports: [CommonModule, GenericTableComponent],
  templateUrl: './person.component.html',
  styleUrl: './person.component.css'
})
export class PersonComponent implements OnInit {

  private readonly personService = inject(PersonService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly dialog = inject(MatDialog);
  private readonly cityService = inject(CityService);

  persons: Person[] = [];
  columns: TableColumn<Person>[] = [];

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
    this.load();
  }

  load(): void {
    this.personService.getPersons().subscribe({
      next: (data) => this.persons = data,
      error: (err) => console.error('Error al cargar personas:', err)
    });
  }

  // --- helpers ---
  private async getCityOptions() {
    const cities = await firstValueFrom(this.cityService.getAll("city"));
    return cities.map((c: any) => ({ value: c.id, label: c.name }));
  }

  async onEdit(row: Person) {
    const cityOptions = await this.getCityOptions();

    // Si tu SelectDto aún no trae cityId, no habrá pre-selección (recomendado: incluirlo).
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: { ...row },               // idealmente: { ...row, cityId: row.cityId }
        formType: 'Person',
        selectOptions: { cityId: cityOptions }
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.personService.updatePerson(row.id, result).subscribe({
          next: () => this.load(),
          error: err => console.error('Error actualizando persona:', err)
        });
      }
    });
  }

  async onCreateNew() {
    const cityOptions = await this.getCityOptions();

    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: {} as Person,
        formType: 'Person',
        selectOptions: { cityId: cityOptions }
      }
    });

    dialogRef.afterClosed().subscribe((result: any) => {
      if (result) {
        this.personService.createPerson(result).subscribe({
          next: () => this.load(),
          error: err => console.error('Error creando persona:', err)
        });
      }
    });
  }

  async onDelete(row: Person) {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar Persona',
      text: `¿Deseas eliminar a "${row.firstName} ${row.lastName}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });

    if (confirmed) {
      this.personService.deletePerson(row.id).subscribe({
        next: () => this.load(),
        error: err => console.error('Error eliminando persona:', err)
      });
    }
  }

  onView(row: Person) {
    console.log('Ver persona:', row);
  }
}
