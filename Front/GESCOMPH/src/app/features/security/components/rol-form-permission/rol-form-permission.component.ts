import { CommonModule } from '@angular/common';
import { AfterViewInit, ChangeDetectorRef, Component, OnInit, TemplateRef, ViewChild, inject } from '@angular/core';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { combineLatest, EMPTY } from 'rxjs';
import { catchError, finalize, filter, map, switchMap, take, tap } from 'rxjs/operators';

import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
// import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';

import {
  RolFormPermissionCreateModel,
  RolFormPermissionGroupedModel,
  RolFormPermissionUpdateModel
} from '../../models/rol-form-permission.models';

import { FormStore } from '../../services/form/form.store';
import { PermissionStore } from '../../services/permission/permission.store';
import { RolFormPermissionStore } from '../../services/rol-form-permission/rol-form-permission.store';
import { RoleStore } from '../../services/role/role.store';

import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ToggleButtonComponent } from '../../../../shared/components/toggle-button-component/toggle-button-component.component';
import { HasRoleAndPermissionDirective } from '../../../../core/security/directives/HasRoleAndPermission.directive';

@Component({
  standalone: true,
  selector: 'app-rol-form-permission',
  templateUrl: './rol-form-permission.component.html',
  styleUrls: ['./rol-form-permission.component.css'],
  imports: [
    CommonModule,
    GenericTableComponent,
    MatDialogModule,
    ToggleButtonComponent,
    HasRoleAndPermissionDirective
  ]
})
export class RolFormPermissionComponent implements OnInit, AfterViewInit {
  // Inyección de dependencias
  private readonly rolFormPermissionStore = inject(RolFormPermissionStore);
  private readonly roleStore = inject(RoleStore);
  private readonly formStore = inject(FormStore);
  private readonly permissionStore = inject(PermissionStore);
  private readonly dialog = inject(MatDialog);
  // private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlert = inject(SweetAlertService);
  private readonly sweetAlertService = inject(SweetAlertService);

  // Estado
  items$ = this.rolFormPermissionStore.rolFormPermissions$;
  columns!: TableColumn<RolFormPermissionGroupedModel>[];

  // Lock por ítem para evitar doble clic y pérdida de feedback
  private busyIds = new Set<number>();
  isBusy = (id: number) => this.busyIds.has(id);

  @ViewChild('permissionsTemplate') permissionsTemplate!: TemplateRef<any>;
  @ViewChild('estadoTemplate') estadoTemplate!: TemplateRef<any>;

  constructor(private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.roleStore.loadAll();
    this.formStore.loadAll();
    this.permissionStore.loadAll();
    this.rolFormPermissionStore.loadAll();
  }

  ngAfterViewInit(): void {
    this.columns = [
      { key: 'rolName', header: 'Rol' },
      { key: 'formName', header: 'Formulario' },
      { key: 'permissions', header: 'Permisos', template: this.permissionsTemplate },
      { key: 'active', header: 'Estado', template: this.estadoTemplate }
    ];
    this.cdr.detectChanges();
  }

  // Crear
  onCreateNew(): void {
    combineLatest([this.roleStore.roles$, this.formStore.forms$, this.permissionStore.permissions$])
      .pipe(take(1))
      .subscribe(([roles, forms, permissions]) => {
        import('../../../../shared/components/form-dialog/form-dialog.component').then(m => {
          const dialogRef = this.dialog.open(m.FormDialogComponent, {
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

          dialogRef.afterClosed().pipe(
            filter(Boolean),
            map((result: any) => ({
              rolId: +result.rolId,
              formId: +result.formId,
              permissionIds: result.permissionIds
            }) as RolFormPermissionCreateModel),
            switchMap(payload =>
              this.rolFormPermissionStore.create(payload).pipe(
                tap(() => this.sweetAlertService.showNotification('Creado', 'Relación creada con éxito.', 'success')),
                catchError(err => {
                  console.error('Error creando:', err);
                  this.sweetAlertService.showApiError(err, 'No se pudo crear.');
                  return EMPTY;
                })
              )
            )
          ).subscribe();
        });
      });
  }

  // Editar
  onEdit(row: RolFormPermissionGroupedModel): void {
    combineLatest([this.roleStore.roles$, this.formStore.forms$, this.permissionStore.permissions$])
      .pipe(take(1))
      .subscribe(([roles, forms, permissions]) => {
        const entityForDialog = {
          rolId: row.rolId,
          formId: row.formId,
          active: row.active,
          permissionIds: row.permissions.map(p => p.permissionId)
        };

        import('../../../../shared/components/form-dialog/form-dialog.component').then(m => {
          const dialogRef = this.dialog.open(m.FormDialogComponent, {
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

          dialogRef.afterClosed().pipe(
            filter(Boolean),
            map((result: any) => ({
              id: row.id, // si tu backend usa clave compuesta, cambia a método por grupo
              rolId: +result.rolId,
              formId: +result.formId,
              permissionIds: result.permissionIds,
              active: result.active
            }) as RolFormPermissionUpdateModel),
            switchMap(payload =>
              this.rolFormPermissionStore.update(payload).pipe(
                tap(() => this.sweetAlertService.showNotification('Actualizado', 'Relación actualizada con éxito.', 'success')),
                catchError(err => {
                  console.error('Error actualizando:', err);
                  this.sweetAlertService.showApiError(err, 'No se pudo actualizar.');
                  return EMPTY;
                })
              )
            )
          ).subscribe();
        });
      });
  }

  // Eliminar (grupo)
  async onDelete(row: RolFormPermissionGroupedModel): Promise<void> {
    const result = await this.sweetAlertService.showConfirm(
      'Eliminar Grupo de Permisos',
      `¿Eliminar todos los permisos del rol "${row.rolName}" para el formulario "${row.formName}"?`,
      'Eliminar',
      'Cancelar',
      'warning'
    );
    if (!result.isConfirmed) return;

    this.rolFormPermissionStore.deleteByGroup(row.rolId, row.formId).pipe(take(1)).subscribe({
      next: () => this.sweetAlertService.showNotification('Eliminado', 'Grupo de permisos eliminado correctamente.', 'success'),
      error: err => {
        console.error('Error eliminando:', err);
        this.sweetAlertService.showApiError(err, 'No se pudo eliminar el grupo.');
      }
    });
  }

  onView(row: RolFormPermissionGroupedModel): void {
  }

  // Toggle activo/inactivo (UI optimista + rollback)
  onToggleActive(row: RolFormPermissionGroupedModel, e: boolean | { checked: boolean }): void {
    if (this.isBusy(row.id)) return;

    const checked = typeof e === 'boolean' ? e : !!e?.checked;
    const previous = row.active;

    // Optimistic UI + lock por ítem
    this.busyIds.add(row.id);
    row.active = checked;

    this.rolFormPermissionStore.changeActiveStatus(row.id, checked).pipe(
      take(1),
      tap(updated => {
        // Si la API devuelve 204, updated puede venir undefined
        row.active = updated?.active ?? checked;
        this.sweetAlertService.showNotification(
          'Éxito',
          `Permisos del grupo ${row.active ? 'activados' : 'desactivados'} correctamente.`,
          'success'
        );
      }),
      catchError(err => {
        console.error('Error cambiando estado:', err);
        row.active = previous; // revertir
        this.sweetAlertService.showApiError(err, 'No se pudo cambiar el estado.');
        return EMPTY;
      }),
      finalize(() => this.busyIds.delete(row.id))
    ).subscribe();
  }
}
