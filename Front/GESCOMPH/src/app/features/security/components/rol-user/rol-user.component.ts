import { Component, OnInit, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { forkJoin, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
// import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { RolUserService } from '../../services/rol-user/rol-user.service';
import { UserService } from '../../services/user/user.service';
import { RoleService } from '../../services/role/role.service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { RolUserCreateModel, RolUserSelectModel } from '../../models/rol-user.models';

@Component({
  selector: 'app-rol-user',
  imports: [CommonModule, GenericTableComponent],
  templateUrl: './rol-user.component.html',
  styleUrl: './rol-user.component.css'
})
export class RolUserComponent implements OnInit {
  private readonly rolUserService = inject(RolUserService);
  private readonly userService = inject(UserService);
  // private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlert    = inject(SweetAlertService);


  private readonly dialog = inject(MatDialog);
  private readonly sweetAlertService = inject(SweetAlertService);

  rolUsers: RolUserSelectModel[] = [];
  columns: TableColumn<RolUserSelectModel>[] = [];

  ngOnInit(): void {
    this.columns = [
      { key: 'userEmail', header: 'Correo' },
      { key: 'rolName', header: 'Rol' },
      { key: 'active', header: 'Estado' }
    ];
    this.load();
  }

  load() {
    this.rolUserService.getAll().pipe().subscribe({
      next: data => (this.rolUsers = data),
      error: err => console.error('Error al cargar RolUser:', err)
    });
  }

  // Helpers para selects
  private getUserOptions$() {
    return this.userService.getAll().pipe(
      catchError(() => of([])),
      map((users: any[]) =>
        users.map(u => ({
          value: u.id,
          label: (u.personName ?? `${u.firstName ?? ''} ${u.lastName ?? ''}`)?.trim() || u.email
        }))
      )
    );
  }

  private getRoleOptions$() {
    return this.rolUserService.getAll().pipe(
      catchError(() => of([])),
      map((roles: any[]) => roles.map(r => ({ value: r.id, label: r.name })))
    );
  }

  // Crear
  onCreateNew() {
    forkJoin({
      userOpts: this.getUserOptions$(),
      roleOpts: this.getRoleOptions$()
    }).subscribe(({ userOpts, roleOpts }) => {
      if (!userOpts.length || !roleOpts.length) {
        console.error('No hay opciones disponibles para Usuario/Rol');
        return;
      }

      const initial: RolUserCreateModel = {
        userId: userOpts[0].value,
        rolId: roleOpts[0].value,
        active: true
      };

      import('../../../../shared/components/form-dialog/form-dialog.component').then(m => {
      const dialogRef = this.dialog.open(m.FormDialogComponent, {
        width: '600px',
        data: {
          entity: initial,
          formType: 'RolUser',
          selectOptions: {
            userId: userOpts,
            rolId: roleOpts
          }
        }
      });

      dialogRef.afterClosed().subscribe((result: any) => {
        if (!result) return;

        const payload: RolUserCreateModel = {
          userId: +result.userId,
          rolId: +result.rolId,
          active: !!result.active
        };

        this.rolUserService.create(payload).subscribe({
          next: () => {
            this.load()
            this.sweetAlertService.showNotification('Creación Exitosa', 'Rol-Usuario creado exitosamente.', 'success');
          },
          error: err => {
            console.error('Error creando RolUser:', err)
            this.sweetAlertService.showApiError(err, 'No se pudo crear la asignación Rol-Usuario.');
          }
        });
      });
      });
    });
  }

  // EDIT
  onEdit(row: RolUserSelectModel) {
    const id = row.id;

    forkJoin({
      userOpts: this.getUserOptions$(),
      roleOpts: this.getRoleOptions$(),
      raw: this.rolUserService.getById(id)
    })
      .pipe(
        map(({ userOpts, roleOpts, raw }) => {
          // Normaliza userId/rolId aunque vengan con otro casing o anidados
          const userId =
            raw.userId;
          const rolId =
            raw.rolId;

          return {
            userOpts,
            roleOpts,
            initial: {
              id: Number(raw.id),
              userId: Number(userId),
              rolId: Number(rolId),
              active: Boolean(raw.active)
            }
          };
        })
      )
      .subscribe(({ userOpts, roleOpts, initial }) => {
        if (!userOpts.length || !roleOpts.length) {
          console.error('No hay opciones disponibles para Usuario/Rol');
          return;
        }

        import('../../../../shared/components/form-dialog/form-dialog.component').then(m => {
        const dialogRef = this.dialog.open(m.FormDialogComponent, {
          width: '600px',
          data: {
            entity: initial,              // <- ahora seguro trae userId/rolId
            formType: 'RolUser',
            selectOptions: { userId: userOpts, rolId: roleOpts }
          }
        });

        // rol-user.component.ts
        dialogRef.afterClosed().subscribe(result => {
          if (!result) return;

          const payload = {
            id, // lo mandas también en el body
            rolId: +result.rolId,
            userId: +result.userId,
            active: !!result.active
          };

          this.rolUserService.update(id, payload).subscribe({
            next: () => {
              this.load()
              this.sweetAlertService.showNotification('Actualización Exitosa', 'Rol-Usuario actualizado exitosamente.', 'success');
            },
            error: err => {
              console.error('Error actualizando RolUser:', err)
              this.sweetAlertService.showApiError(err, 'No se pudo actualizar la asignación Rol-Usuario.');
            }
          });
        });
        });

      });
  }




  // DELETE
  async onDelete(row: RolUserSelectModel) {
    const result = await this.sweetAlertService.showConfirm(
      'Eliminar Asignación Rol-Usuario',
      `¿Deseas eliminar el rol "${row.rolName}" del usuario "${row.userEmail}"?`,
      'Eliminar',
      'Cancelar',
      'warning'
    );

    if (result.isConfirmed) {
      this.rolUserService.deleteLogic(row.id).subscribe({
        next: () => {
          this.load()
          this.sweetAlertService.showNotification('Eliminación Exitosa', 'Asignación Rol-Usuario eliminada exitosamente.', 'success');
        },
        error: err => {
          console.error('Error eliminando RolUser:', err)
          this.sweetAlertService.showApiError(err, 'No se pudo eliminar la asignación Rol-Usuario.');
        }
      });
    }
  }

  onView(row: RolUserSelectModel) {
  }
}
