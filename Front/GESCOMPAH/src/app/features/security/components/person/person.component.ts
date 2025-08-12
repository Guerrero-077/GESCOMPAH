import { Component, inject, OnInit } from '@angular/core';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { MatDialog } from '@angular/material/dialog';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { Person } from '../../models/user.models';
import { PersonService } from '../../services/person/person.service';
import { CommonModule } from '@angular/common';

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

  load() {
    this.personService.getPersons().subscribe({
      next: (data) => {
        console.log('Personas desde el servicio:', data);
        this.persons = data;
      },
      error: (err) => console.error('Error al cargar personas:', err)
    });
  }

  onEdit(row: Person) {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: row,
        formType: 'Person'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.personService.updatePerson(row.id, result).subscribe({
          next: () => {
            console.log('Persona actualizada correctamente');
            this.load();
          },
          error: err => console.error('Error actualizando persona:', err)
        });
      }
    });
  }

  onCreateNew() {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: {} as Person,
        formType: 'Person'
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
        next: () => {
          console.log('Persona eliminada correctamente');
          this.load();
        },
        error: err => console.error('Error eliminando persona:', err)
      });
    }
  }

  onView(row: Person) {
    console.log('Ver persona:', row);
  }
}
