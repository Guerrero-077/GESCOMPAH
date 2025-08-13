import { Component, inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';

import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';

import { UserCreateDto, UserListDto, UserUpdatePayload } from '../../models/user.models';
import { UserService } from '../../services/user/user.service';
import { PersonService } from '../../services/person/person.service';
import { catchError, map } from 'rxjs/operators';
import { of } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user',
  imports: [CommonModule, GenericTableComponent],
  templateUrl: './user.component.html',
  styleUrl: './user.component.css'
})
export class UserComponent implements OnInit {
  private readonly userService = inject(UserService);
  private readonly personService = inject(PersonService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly dialog = inject(MatDialog);

  users: UserListDto[] = [];
  columns: TableColumn<UserListDto>[] = [];

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'personName', header: 'Nombre Completo' },
      { key: 'email', header: 'Correo Electrónico' },
      { key: 'active', header: 'Estado' }
    ];
    this.load();
  }

  load() {
    this.userService.getUsers().subscribe({
      next: data => (this.users = data),
      error: err => console.error('Error al cargar usuarios:', err)
    });
  }

  // EDIT
  onEdit(row: UserListDto) {
    const id = row.id;

    const initial = {
      id,                 // <-- IMPORTANTE para que password deje de ser requerida
      email: row.email,
      password: ''
    };

    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: initial,
        formType: 'User'   // <-- usar 'User'
        // sin selectOptions.personId en edición
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;
      const payload: UserUpdatePayload = {
        email: (result.email ?? initial.email)?.trim(),
        ...(result.password ? { password: result.password } : {})
      };
      this.userService.updateUser(id, payload).subscribe({ next: () => this.load() });
    });
  }

  // CREATE
  onCreateNew() {
    this.personService.getPersons().pipe(
      catchError(() => of([])),
      map((persons: any[]) => persons.map(p => ({ value: p.id, label: `${p.firstName} ${p.lastName}` })))
    ).subscribe(people => {
      if (!people.length) { console.error('No hay personas disponibles'); return; }

      const initial: UserCreateDto = {
        email: '',
        password: '',
        personId: people[0].value
      };

      const dialogRef = this.dialog.open(FormDialogComponent, {
        width: '600px',
        data: {
          entity: initial,
          formType: 'User',                 // <-- usar 'User' (no 'UserCreate')
          selectOptions: { personId: people }
        }
      });

      dialogRef.afterClosed().subscribe((result: any) => {
        if (!result) return;
        const payload: UserCreateDto = {
          email: (result.email ?? '').trim(),
          password: result.password,
          personId: +result.personId
        };
        this.userService.createUser(payload).subscribe({ next: () => this.load() });
      });
    });
  }

  
  // DELETE
  async onDelete(row: UserListDto) {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar Usuario',
      text: `¿Deseas eliminar al usuario "${row.personName}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar'
    });

    if (confirmed) {
      this.userService.deleteUser(row.id).subscribe({
        next: () => this.load(),
        error: err => console.error('Error eliminando el usuario:', err)
      });
    }
  }

  onView(row: UserListDto) {
    console.log('Ver usuario:', row);
  }

}
