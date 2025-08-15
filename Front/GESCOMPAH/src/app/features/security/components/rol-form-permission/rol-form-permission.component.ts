import { CommonModule } from '@angular/common';
import { AfterViewInit, ChangeDetectorRef, Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { combineLatest, take } from 'rxjs';

import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';

import {
  RolFormPermissionCreateModel,
  RolFormPermissionSelectModel,
  RolFormPermissionUpdateModel
} from '../../models/rol-form-permission.models';

import { FormStore } from '../../services/form/form.store';
import { PermissionStore } from '../../services/permission/permission.store';
import { RolFormPermissionStore } from '../../services/rol-form-permission/rol-form-permission.store';
import { RoleStore } from '../../services/role/role.store';

import { TableColumn } from '../../../../shared/models/TableColumn.models';

@Component({
  standalone: true,
  selector: 'app-rol-form-permission',
  templateUrl: './rol-form-permission.component.html',
  styleUrl: './rol-form-permission.component.css',
  imports: [
    CommonModule,
    GenericTableComponent,
    MatDialogModule
  ]
})
export class RolFormPermissionComponent implements OnInit, AfterViewInit {
  private readonly rolFormPermissionStore = inject(RolFormPermissionStore);
  private readonly roleStore = inject(RoleStore);
  private readonly formStore = inject(FormStore);
  private readonly permissionStore = inject(PermissionStore);
  private readonly dialog = inject(MatDialog);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);

  items$ = this.rolFormPermissionStore.rolFormPermissions$;


  columns: TableColumn<RolFormPermissionSelectModel>[] = [
    { key: 'index', header: 'Nº', type: 'index' },
    { key: 'rolName', header: 'Rol' },
    { key: 'formName', header: 'Formulario' },
    { key: 'permissionName', header: 'Permisos' }
  ];

  @ViewChild('estadoTemplate') estadoTemplate!: TemplateRef<any>;
  constructor(private cdr: ChangeDetectorRef) { }

  ngAfterViewInit(): void {
    // ahora sí podemos agregar la columna que usa el template
    this.columns = [
      ...this.columns,
      { key: 'active', header: 'Estado', template: this.estadoTemplate } // 👈 clave del campo boolean
    ];

    // Si la tabla ya se renderizó, forzamos detección para que vea la nueva columna
    this.cdr.detectChanges();
  }
  ngOnInit(): void {
    this.roleStore.loadAll();
    this.formStore.loadAll();
    this.permissionStore.loadAll();
    this.rolFormPermissionStore.loadAll();
  }

  onCreateNew(): void {
    combineLatest([
      this.roleStore.roles$,
      this.formStore.forms$,
      this.permissionStore.permissions$
    ])
      .pipe(take(1))
      .subscribe(([roles, forms, permissions]) => {
        const dialogRef = this.dialog.open(FormDialogComponent, {
          width: '600px',
          data: {
            entity: { active: true },
            formType: 'RolFormPermission',
            title: 'Nueva Asignación de Permisos',
            selectOptions: {
              rolId: roles.map(r => ({ value: r.id, label: r.name })),
              formId: forms.map(f => ({ value: f.id, label: f.name })),
              permissionId: permissions.map(p => ({ value: p.id, label: p.name }))
            }
          }
        });

        dialogRef.afterClosed().subscribe((result: any) => {
          if (!result) return;

          const payload: RolFormPermissionCreateModel = {
            rolId: +result.rolId,
            formId: +result.formId,
            permissionId: +result.permissionId
          };

          this.rolFormPermissionStore.create(payload).subscribe({
            next: () => {
              this.sweetAlertService.showNotification('Creado', 'Relación creada con éxito.', 'success');
            },
            error: err => {
              console.error('Error creando:', err);
              this.sweetAlertService.showNotification('Error', 'No se pudo crear.', 'error');
            }
          });
        });
      });
  }

  onEdit(row: RolFormPermissionUpdateModel): void {
    combineLatest([
      this.roleStore.roles$,
      this.formStore.forms$,
      this.permissionStore.permissions$
    ])
      .pipe(take(1))
      .subscribe(([roles, forms, permissions]) => {
        const dialogRef = this.dialog.open(FormDialogComponent, {
          width: '600px',
          data: {
            entity: row,
            formType: 'RolFormPermission',
            title: 'Editar Asignación de Permisos',
            selectOptions: {
              rolId: roles.map(r => ({ value: r.id, label: r.name })),
              formId: forms.map(f => ({ value: f.id, label: f.name })),
              permissionId: permissions.map(p => ({ value: p.id, label: p.name }))
            }
          }
        });

        dialogRef.afterClosed().subscribe((result: any) => {
          if (!result) return;

          const payload: RolFormPermissionUpdateModel = {
            id: row.id,
            rolId: +result.rolId,
            formId: +result.formId,
            permissionId: +result.permissionId,
            active: result.active !== undefined ? result.active : row.active // Mantener el estado actual si no se cambia
          };

          this.rolFormPermissionStore.update(payload).subscribe({
            next: () => {
              this.sweetAlertService.showNotification('Actualizado', 'Relación actualizada con éxito.', 'success');
            },
            error: err => {
              console.error('Error actualizando:', err);
              this.sweetAlertService.showNotification('Error', 'No se pudo actualizar.', 'error');
            }
          });
        });
      });
  }

  async onDelete(row: RolFormPermissionSelectModel): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar relación',
      text: `¿Eliminar el rol "${row.rolName}" del formulario "${row.formName}" con permiso "${row.permissionName}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar'
    });

    if (!confirmed) return;

    this.rolFormPermissionStore.deleteLogic(row.id).subscribe({
      next: () => {
        this.sweetAlertService.showNotification('Eliminado', 'Relación eliminada correctamente.', 'success');
      },
      error: err => {
        console.error('Error eliminando:', err);
        this.sweetAlertService.showNotification('Error', 'No se pudo eliminar.', 'error');
      }
    });
  }

  onView(row: RolFormPermissionSelectModel): void {
    console.log('Vista detalle:', row);
  }
}
