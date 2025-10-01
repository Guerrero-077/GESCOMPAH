import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';

import { of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
// import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';

import { UserCreateModel, UserSelectModel, UserUpdateModel } from '../../models/user.models';
import { PersonService } from '../../services/person/person.service';
import { UserStore } from '../../services/user/user.store';
import { PageHeaderService } from '../../../../shared/Services/PageHeader/page-header.service';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [CommonModule, GenericTableComponent, MatSlideToggleModule],
  templateUrl: './user.component.html',
  styleUrl: './user.component.css'
})
export class UserComponent implements OnInit {
  private readonly userStore = inject(UserStore);
  private readonly personService = inject(PersonService);
  // private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly dialog = inject(MatDialog);
  private readonly sweetAlertService = inject(SweetAlertService);
  private readonly pageHeaderService = inject(PageHeaderService);

  @ViewChild('estadoTemplate', { static: true }) estadoTpl!: TemplateRef<any>;

  users$ = this.userStore.users$;
  columns: TableColumn<UserSelectModel>[] = [];

  ngOnInit(): void {
    this.pageHeaderService.setPageHeader('Usuarios', 'Gestión de Usuarios');
    this.columns = [
      { key: 'personName', header: 'Nombre Completo' },
      { key: 'email', header: 'Correo Electrónico' },
      {
        key: 'active',
        header: 'Estado',
        type: 'custom',           // tu GenericTable debe renderizar template cuando type === 'custom'
        template: this.estadoTpl  // <-- TemplateRef (no string)
      }
    ];
  }

  // Toggle activo/inactivo (UI optimista + rollback)
  onToggleActive(row: UserSelectModel, e: MatSlideToggleChange) {
    const previous = row.active;
    row.active = e.checked; // Optimistic UI

    // Asegúrate de tener en tu UserStore un método:
    // changeActiveStatus(id: number, active: boolean): Observable<UserSelectModel>
    this.userStore.changeActiveStatus(row.id, e.checked).subscribe({
      next: (updated) => {
        // sincronizar con lo que devuelve el backend
        row.active = updated.active ?? row.active;
        this.sweetAlertService.showNotification(
          'Éxito',
          `Usuario ${row.active ? 'activado' : 'desactivado'} correctamente.`,
          'success'
        );
      },
      error: (err) => {
        // revertir si falla
        row.active = previous;
        this.sweetAlertService.showApiError(err, 'No se pudo cambiar el estado.');
      }
    });
  }

  // Editar
  onEdit(row: UserSelectModel) {
    const id = row.id;
    const initial = { email: row.email, password: '' };

    import('../../../../shared/components/form-dialog/form-dialog.component').then(m => {
    const dialogRef = this.dialog.open(m.FormDialogComponent, {
      width: '600px',
      data: {
        entity: initial,
        formType: 'User'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;
      const payload: UserUpdateModel = {
        email: (result.email ?? initial.email)?.trim(),
        ...(result.password ? { password: result.password } : {})
      };
      this.userStore.update(id, payload).subscribe({
        next: () => {
          this.sweetAlertService.showNotification(
            'Actualización Exitosa',
            'Usuario actualizado exitosamente.',
            'success'
          );
        },
        error: err => {
          console.error('Error al actualizar el usuario:', err);
        }
      });
    });
    });
  }

  // Crear
  onCreateNew() {
    this.personService.getAll().pipe(
      catchError(() => of([])),
      map((persons: any[]) => persons.map(p => ({ value: p.id, label: `${p.firstName} ${p.lastName}` })))
    ).subscribe(people => {
      if (!people.length) { console.error('No hay personas disponibles'); return; }

      const initial: UserCreateModel = {
        email: '',
        password: '',
        personId: people[0].value
      };

      import('../../../../shared/components/form-dialog/form-dialog.component').then(m => {
      const dialogRef = this.dialog.open(m.FormDialogComponent, {
        width: '600px',
        data: {
          entity: initial,
          formType: 'User',
          selectOptions: { personId: people }
        }
      });

      dialogRef.afterClosed().subscribe((result: any) => {
        if (!result) return;
        const payload: UserCreateModel = {
          email: (result.email ?? '').trim(),
          password: result.password,
          personId: +result.personId
        };
        this.userStore.create(payload).subscribe({
          next: () => {
            this.sweetAlertService.showNotification(
              'Creación Exitosa',
              'Usuario creado exitosamente.',
              'success'
            );
          },
          error: err => {
            console.error('Error al crear el usuario:', err);
          }
        });
      });
      });
    });
  }

  // ----- DELETE (soft delete) -----
  async onDelete(row: UserSelectModel) {
    const result = await this.sweetAlertService.showConfirm(
      'Eliminar Usuario',
      `¿Deseas eliminar al usuario "${row.personName}"?`,
      'Eliminar',
      'Cancelar',
      'warning'
    );

    if (result.isConfirmed) {
      this.userStore.deleteLogic(row.id).subscribe({
        next: () => {
          this.sweetAlertService.showNotification(
            'Eliminación Exitosa',
            'Usuario eliminado exitosamente.',
            'success'
          );
        },
        error: err => {
          console.error('Error eliminando el usuario:', err);
          this.sweetAlertService.showApiError(err, 'No se pudo eliminar el usuario.');
        }
      });
    }
  }

  onView(row: UserSelectModel) {
  }
}
