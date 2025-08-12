import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { forkJoin, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';


import { UserCreateDto, UserListDto } from '../../models/user.models';
import { RoleService } from '../../services/role/role.service';
import { UserService } from '../../services/user/user.service';
import { PersonService } from '../../services/person/person.service';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [CommonModule, GenericTableComponent],
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css']
})
export class UserComponent implements OnInit {

  private readonly userService = inject(UserService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly personService = inject(PersonService);
  private readonly roleService = inject(RoleService);
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
      next: (data) => {
        console.log('Usuarios desde el servicio:', data);
        this.users = data;
      },
      error: err => console.error('Error al cargar usuarios:', err)
    });
  }

  onEdit(row: UserListDto) {
    const id = row.id;

    forkJoin({
      user: this.userService.getUserById(id),
      people: this.personService.getPersons().pipe(
        catchError(() => of([])),
        map(persons => persons.map(p => ({
          value: p.id,
          label: `${p.firstName} ${p.lastName}`
        })))
      ),
      roles: this.roleService.getAll('rol').pipe(
        catchError(() => of([])),
        map(roles => roles.map(r => ({
          value: r.id,
          label: r.name
        })))
      )
    }).subscribe(({ user, people, roles }) => {
      console.log('Datos cargados:', { user, people, roles });

      const dialogRef = this.dialog.open(FormDialogComponent, {
        width: '600px',
        data: {
          entity: user,
          formType: 'User',
          selectOptions: {
            personId: people,
            roleIds: roles
          }
        }
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          this.userService.updateUser(id, result).subscribe({
            next: () => {
              console.log('Usuario actualizado correctamente');
              this.load();
            },
            error: err => console.error('Error actualizando el usuario:', err)
          });
        }
      });
    });
  }


  onCreateNew() {
    forkJoin({
      people: this.personService.getPersons().pipe(
        catchError(() => of([])),
        map(persons => persons.map(p => ({
          value: p.id,
          label: `${p.firstName} ${p.lastName}`
        })))
      ),
      roles: this.roleService.getAll('rol').pipe(
        catchError(() => of([])),
        map(roles => roles.map(r => ({
          value: r.id,
          label: r.name
        })))
      )
    }).subscribe(({ people, roles }) => {
      const dialogRef = this.dialog.open(FormDialogComponent, {
        width: '600px',
        data: {
          entity: {} as UserCreateDto,
          formType: 'User',
          selectOptions: {
            personId: people,
            roleIds: roles
          }
        }
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          this.userService.createUser(result).subscribe({
            next: () => this.load(),
            error: err => console.error('Error creando usuario:', err)
          });
        }
      });
    });
  }


  async onDelete(row: UserListDto) {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar Usuario',
      text: `¿Deseas eliminar al usuario "${row.personName}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });

    if (confirmed) {
      this.userService.deleteUser(row.id).subscribe({
        next: () => {
          console.log('Usuario eliminado correctamente');
          this.load();
        },
        error: err => console.error('Error eliminando usuario:', err)
      });
    }
  }

  onView(row: UserListDto) {
    console.log('Ver usuario:', row);
  }
}
