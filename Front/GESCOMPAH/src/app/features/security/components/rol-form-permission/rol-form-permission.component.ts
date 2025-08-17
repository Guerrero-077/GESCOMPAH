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
  RolFormPermissionGroupedModel, // <--- USAR EL MODELO AGRUPADO
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

  // El store ahora debería devolver los items agrupados
  items$ = this.rolFormPermissionStore.rolFormPermissions$;

  // ViewChild para las plantillas de la tabla
  @ViewChild('permissionsTemplate') permissionsTemplate!: TemplateRef<any>;
  @ViewChild('estadoTemplate') estadoTemplate!: TemplateRef<any>;

  // Las columnas ahora se basan en el modelo agrupado
  columns: TableColumn<RolFormPermissionGroupedModel>[] = [
    { key: 'index', header: 'Nº', type: 'index' },
    { key: 'rolName', header: 'Rol' },
    { key: 'formName', header: 'Formulario' },
    // La columna de permisos usará un template
    { key: 'permissions', header: 'Permisos' }
  ];

  constructor(private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    // Cargar datos necesarios para los diálogos
    this.roleStore.loadAll();
    this.formStore.loadAll();
    this.permissionStore.loadAll();
    // Esto debería llamar al endpoint /grouped
    this.rolFormPermissionStore.loadAll();
  }

  ngAfterViewInit(): void {
    // Asignar los templates a las columnas después de que la vista se inicialice
    this.columns = [
      ...this.columns.filter(c => c.key !== 'permissions'),
      { key: 'permissions', header: 'Permisos', template: this.permissionsTemplate },
      { key: 'active', header: 'Estado', template: this.estadoTemplate }
    ];
    this.cdr.detectChanges();
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
              permissionIds: permissions.map(p => ({ value: p.id, label: p.name }))
            }
          }
        });

        dialogRef.afterClosed().subscribe((result: any) => {
          if (!result) return;

          const payload: RolFormPermissionCreateModel = {
            rolId: +result.rolId,
            formId: +result.formId,
            permissionIds: result.permissionIds
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

  onEdit(row: RolFormPermissionGroupedModel): void {
    combineLatest([
      this.roleStore.roles$,
      this.formStore.forms$,
      this.permissionStore.permissions$
    ])
      .pipe(take(1))
      .subscribe(([roles, forms, permissions]) => {
        // Pre-seleccionar los permisos que el grupo ya tiene
        const entityForDialog = {
          rolId: row.rolId,
          formId: row.formId,
          active: row.active,
          permissionIds: row.permissions.map(p => p.permissionId)
        };

        const dialogRef = this.dialog.open(FormDialogComponent, {
          width: '600px',
          data: {
            entity: entityForDialog,
            formType: 'RolFormPermission',
            title: 'Editar Asignación de Permisos',
            selectOptions: {
              rolId: roles.map(r => ({ value: r.id, label: r.name })),
              formId: forms.map(f => ({ value: f.id, label: f.name })),
              permissionIds: permissions.map(p => ({ value: p.id, label: p.name }))
            }
          }
        });

        dialogRef.afterClosed().subscribe((result: any) => {
          if (!result) return;

          const payload: RolFormPermissionUpdateModel = {
            id: 0, // El ID ya no es relevante para un grupo, pero el DTO lo requiere
            rolId: +result.rolId,
            formId: +result.formId,
            permissionIds: result.permissionIds,
            active: result.active
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

  async onDelete(row: RolFormPermissionGroupedModel): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar Grupo de Permisos',
      text: `¿Eliminar todos los permisos del rol "${row.rolName}" para el formulario "${row.formName}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar'
    });

    if (!confirmed) return;

    // Llamar al nuevo método del store para borrar por grupo
    this.rolFormPermissionStore.deleteByGroup(row.rolId, row.formId).subscribe({
      next: () => {
        this.sweetAlertService.showNotification('Eliminado', 'Grupo de permisos eliminado correctamente.', 'success');
      },
      error: err => {
        console.error('Error eliminando:', err);
        this.sweetAlertService.showNotification('Error', 'No se pudo eliminar el grupo.', 'error');
      }
    });
  }

  onView(row: RolFormPermissionGroupedModel): void {
    console.log('Vista detalle:', row);
  }
}