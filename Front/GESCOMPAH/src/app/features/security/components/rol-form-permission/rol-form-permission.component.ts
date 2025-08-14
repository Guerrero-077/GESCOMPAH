import { Component, inject, OnInit } from '@angular/core';
import { FormService } from '../../services/form/form.service';
import { RoleService } from '../../services/role/role.service';
import { RolFormPermissionService } from '../../services/rol-form-permission/rol-form-permission.service';
import { PermissionService } from '../../services/permission/permission.service';
import { MatDialog } from '@angular/material/dialog';
import { catchError, of, map, forkJoin } from 'rxjs';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { RolFormPermissionCreateModel, RolFormPermissionSelectModel, RolFormPermissionUpdateModel } from '../../models/rol-form-permission.models';

@Component({
  selector: 'app-rol-form-permission',
  imports: [GenericTableComponent],
  templateUrl: './rol-form-permission.component.html',
  styleUrl: './rol-form-permission.component.css'
})
export class RolFormPermissionComponent implements OnInit {
  private readonly rolFormPermissionService = inject(RolFormPermissionService);
  private readonly rolService = inject(RoleService);
  private readonly formService = inject(FormService);
  private readonly permissionService = inject(PermissionService);
  private readonly dialog = inject(MatDialog);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);

  items: RolFormPermissionSelectModel[] = [];

  columns: TableColumn<RolFormPermissionSelectModel>[] = [
    { key: 'index', header: 'Nº', type: 'index' },
    { key: 'rolName', header: 'Rol' },
    { key: 'formName', header: 'Formulario' },
    { key: 'permissionName', header: 'Permisos' },
    { key: 'active', header: 'Estado', type: 'boolean' }
  ];

  ngOnInit(): void { this.load(); }

  load(): void {
    this.rolFormPermissionService.getAll().subscribe({
      next: data => (this.items = data),
      error: err => console.error('Error cargando RolFormPermission:', err)
    });
  }

  // --- helpers para selects ---
  private getRolOptions$() {
    return this.rolService.getAll().pipe(
      catchError(() => of([])),
      map((mods: any[]) => mods.map(m => ({ value: m.id, label: m.name ?? m.rolName ?? `Rol ${m.id}` })))
    );
  }
  private getFormOptions$() {
    return this.formService.getAll().pipe(
      catchError(() => of([])),
      map((forms: any[]) => forms.map(f => ({ value: f.id, label: f.name ?? f.formName ?? `Form ${f.id}` })))
    );
  }

  private getPermissionOptions$() {
    return this.permissionService.getAll().pipe(
      catchError(() => of([])),
      map((mods: any[]) => mods.map(m => ({ value: m.id, label: m.name ?? m.permissionName ?? `Permission ${m.id}` })))
    );
  }

  // --- CREATE ---
  onCreateNew(): void {
    forkJoin({
      rolOpts: this.getRolOptions$(),
      formOpts: this.getFormOptions$(),
      permissionOpts: this.getPermissionOptions$()
    }).subscribe(({ rolOpts, formOpts, permissionOpts }) => {
      if (!rolOpts.length || !formOpts.length || !permissionOpts.length) {
        console.error('No hay opciones disponibles para Form/Module');
        return;
      }

      const initial: { rolId: number; formId: number; permissionId: number; active: boolean } = {
        rolId: rolOpts[0].value,
        formId: formOpts[0].value,
        permissionId: permissionOpts[0].value,
        active: true
      };

      const dialogRef = this.dialog.open(FormDialogComponent, {
        width: '600px',
        data: {
          entity: initial,
          formType: 'RolFormPermission',
          selectOptions: {
            rolId: rolOpts,
            formId: formOpts,
            permissionId: permissionOpts
          }
        }
      });

      dialogRef.afterClosed().subscribe(async (result: any) => {
        if (!result) return;

        const payload: RolFormPermissionCreateModel = {
          rolId: +result.rolId,
          formId: +result.formId,
          permissionId: +result.permissionId
        };

        this.rolFormPermissionService.create(payload).subscribe({
          next: () => {
            this.load();
            this.sweetAlertService.showNotification('Creación Exitosa', 'La relación Rol-Formulario-Permiso ha sido creada.', 'success');
          },
          error: err => console.error('Error creando RolFormPermission:', err)
        });
      });
    });
  }

  // --- EDIT ---
  onEdit(row: RolFormPermissionSelectModel): void {
    const id = row.id;

    forkJoin({
      rolOpts: this.getRolOptions$(),
      formOpts: this.getFormOptions$(),
      permissionOpts: this.getPermissionOptions$(),
      current: this.rolFormPermissionService.getById(id)
    })
      .pipe(
        map(({ rolOpts, formOpts, permissionOpts, current }) => {
          const initial = {
            id: current.id,
            rolId: (current as any).rolId,
            formId: (current as any).formId,
            permissionId: (current as any).permissionId,
            active: current.active
          };
          return { rolOpts, formOpts, permissionOpts, initial };
        })
      )
      .subscribe(({ rolOpts, formOpts, permissionOpts, initial }) => {
        if (!formOpts.length || !permissionOpts.length) {
          console.error('No hay opciones disponibles para Form/Module');
          return;
        }

        const dialogRef = this.dialog.open(FormDialogComponent, {
          width: '600px',
          data: {
            entity: initial,
            formType: 'RolFormPermission',
            selectOptions: {
              rolId: rolOpts,
              formId: formOpts,
              permissionId: permissionOpts
            }
          }
        });

        dialogRef.afterClosed().subscribe(async (result: any) => {
          if (!result) return;

          const payload: RolFormPermissionUpdateModel = {
            id,
            rolId: +result.rolId,
            formId: +result.formId,
            permissionId: +result.permissionId
          };

          this.rolFormPermissionService.update(id, payload).subscribe({
            next: () => {
              this.load();
              this.sweetAlertService.showNotification('Actualización Exitosa', 'La relación Rol-Formulario-Permiso ha sido actualizada.', 'success');
            },
            error: err => console.error('Error actualizando RolFormPermission:', err)
          });
        });
      });
  }

  // --- DELETE ---
  async onDelete(row: RolFormPermissionSelectModel): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar relación Form–Module',
      text: `¿Eliminar el roL "${row.rolName}" del módulo "${row.formName} de ${row.permissionName}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar'
    });

    if (!confirmed) return;

    this.rolFormPermissionService.deleteLogic(row.id).subscribe({
      next: () => this.load(),
      error: err => console.error('Error eliminando RolFormPermission:', err)
    });
  }

  onView(row: RolFormPermissionSelectModel): void {
    console.log('Detalle RolFormPermission:', row);
  }
}
