import { CommonModule } from '@angular/common';
import { Component, OnInit, TemplateRef, ViewChild, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { BehaviorSubject, catchError, forkJoin, map, of, take } from 'rxjs';

import { GenericTableComponent } from '../../../../shared/components/generic-table/generic-table.component';
import { ToggleButtonComponent } from '../../../../shared/components/toggle-button-component/toggle-button-component.component';

import { TableColumn } from '../../../../shared/models/TableColumn.models';
// import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';

import { HasRoleAndPermissionDirective } from '../../../../core/security/directives/HasRoleAndPermission.directive';
import {
  FormModuleCreateModel,
  FormModuleSelectModel,
  FormModuleUpdateModel
} from '../../models/form-module.model';

import { FormModuleService } from '../../services/form-module/form-module.service';
import { FormService } from '../../services/form/form.service';
import { ModuleService } from '../../services/module/module.service';

type SelectOption<T = number> = { value: T; label: string };

@Component({
  selector: 'app-form-module',
  standalone: true,
  imports: [CommonModule, GenericTableComponent, ToggleButtonComponent, HasRoleAndPermissionDirective],
  templateUrl: './form-module.component.html',
  styleUrls: ['./form-module.component.css']
})
export class FormModuleComponent implements OnInit {

  // Inyección de dependencias
  private readonly formService = inject(FormService);
  private readonly moduleService = inject(ModuleService);
  private readonly formModuleService = inject(FormModuleService);
  private readonly dialog = inject(MatDialog);
  // private readonly confirmDialog      = inject(ConfirmDialogService);
  private readonly sweetAlert = inject(SweetAlertService);
  private readonly sweetAlertService = inject(SweetAlertService);

  // Estado
  private readonly formModulesSubject = new BehaviorSubject<FormModuleSelectModel[]>([]);
  readonly formModules$ = this.formModulesSubject.asObservable();
  private readonly busyToggleIds = new Set<number>(); // evita dobles clics en toggle

  // Tabla
  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;
  columns!: TableColumn<FormModuleSelectModel>[];
  trackById = (_: number, it: FormModuleSelectModel) => it.id;

  // Ciclo de vida
  ngOnInit(): void {
    this.columns = [
      { key: 'formName', header: 'Formulario' },
      { key: 'moduleName', header: 'Módulo' },
      { key: 'active', header: 'Estado', type: 'custom', template: this.estadoTemplate }
    ];
    this.loadFormModules();
  }

  // Utilidades UI
  

  // Datos
  private loadFormModules(): void {
    this.formModuleService.getAll().pipe(take(1)).subscribe({
      next: (data) => this.formModulesSubject.next(data as FormModuleSelectModel[]),
      error: (err) => {
        console.error('Error loading form modules:', err);
        this.sweetAlertService.showApiError(err, 'No se pudieron cargar las relaciones Form–Module.');
      }
    });
  }

  // Helpers para selects
  private getFormOptions$() {
    return this.formService.getAll().pipe(
      take(1),
      catchError(() => of([])),
      map((forms: any[]): SelectOption[] =>
        forms.map(f => ({ value: f.id, label: f.name ?? f.formName ?? `Form ${f.id}` }))
      )
    );
  }

  private getModuleOptions$() {
    return this.moduleService.getAll().pipe(
      take(1),
      catchError(() => of([])),
      map((mods: any[]): SelectOption[] =>
        mods.map(m => ({ value: m.id, label: m.name ?? m.moduleName ?? `Module ${m.id}` }))
      )
    );
  }

  // Crear
  onCreateNew(): void {
    forkJoin({
      formOpts: this.getFormOptions$(),
      moduleOpts: this.getModuleOptions$()
    }).pipe(take(1)).subscribe(({ formOpts, moduleOpts }) => {
      if (!formOpts.length || !moduleOpts.length) {
        console.warn('No hay opciones disponibles para Form/Module');
        this.sweetAlertService.showNotification('Advertencia', 'No hay opciones disponibles para Formularios o Módulos. Asegúrese de que existan.', 'warning');
        return;
      }

      import('../../../../shared/components/form-dialog/form-dialog.component').then(m => {
        const dialogRef = this.dialog.open(m.FormDialogComponent, {
          width: '600px',
          data: {
            entity: {
              formId: formOpts[0].value,
              moduleId: moduleOpts[0].value,
              active: true
            },
            formType: 'FormModule',
            selectOptions: {
              formId: formOpts,
              moduleId: moduleOpts
            }
          }
        });

        dialogRef.afterClosed().pipe(take(1)).subscribe((result: any) => {
          if (!result) return;

          const payload: FormModuleCreateModel = {
            formId: +result.formId,
            moduleId: +result.moduleId
          };

          this.formModuleService.create(payload).pipe(take(1)).subscribe({
            next: () => {
              this.sweetAlertService.showNotification('Creación Exitosa', 'Relación Form–Module creada exitosamente.', 'success');
              this.loadFormModules();
            },
            error: (err) => {
              console.error('Error creando FormModule:', err);
              this.sweetAlertService.showApiError(err, 'No se pudo crear la relación Form–Module.');
            }
          });
        });
      });
    });
  }

  // ===== EDIT =====
  onEdit(row: FormModuleSelectModel): void {
    const id = row.id;

    forkJoin({
      formOpts: this.getFormOptions$(),
      moduleOpts: this.getModuleOptions$(),
      current: this.formModuleService.getById(id).pipe(take(1))
    })
      .pipe(
        map(({ formOpts, moduleOpts, current }: { formOpts: SelectOption[]; moduleOpts: SelectOption[]; current: any }) => ({
          formOpts,
          moduleOpts,
          initial: {
            id: current?.id ?? id,
            formId: current?.formId,
            moduleId: current?.moduleId,
            active: current?.active
          }
        })),
        catchError(err => {
          console.error('Error al obtener datos para edición:', err);
          this.sweetAlertService.showApiError(err, 'No se pudieron cargar los datos para editar la relación Form–Module.');
          return of({ formOpts: [], moduleOpts: [], initial: {} as any });
        }),
        take(1)
      )
      .subscribe(({ formOpts, moduleOpts, initial }) => {
        if (!formOpts.length || !moduleOpts.length) {
          console.warn('No hay opciones disponibles para Form/Module');
          this.sweetAlertService.showNotification('Advertencia', 'No hay opciones disponibles para Formularios o Módulos. Asegúrese de que existan.', 'warning');
          return;
        }

        import('../../../../shared/components/form-dialog/form-dialog.component').then(m => {
          const dialogRef = this.dialog.open(m.FormDialogComponent, {
            width: '600px',
            data: {
              entity: initial,
              formType: 'FormModule',
              selectOptions: {
                formId: formOpts,
                moduleId: moduleOpts
              }
            }
          });

          dialogRef.afterClosed().pipe(take(1)).subscribe((result: any) => {
            if (!result) return;

            const payload: FormModuleUpdateModel = {
              id,
              formId: +result.formId,
              moduleId: +result.moduleId
            };

            this.formModuleService.update(id, payload).pipe(take(1)).subscribe({
              next: () => {
                this.sweetAlertService.showNotification('Actualización Exitosa', 'Relación Form–Module actualizada exitosamente.', 'success');
                this.loadFormModules();
              },
              error: (err) => {
                console.error('Error actualizando FormModule:', err);
                this.sweetAlertService.showApiError(err, 'No se pudo actualizar la relación Form–Module.');
              }
            });
          });
        });
      });
  }

  // Eliminar
  async onDelete(row: FormModuleSelectModel): Promise<void> {
    const result = await this.sweetAlert.showConfirm(
      'Eliminar relación Form–Module',
      `¿Eliminar el formulario "${row.formName}" del módulo "${row.moduleName}"?`,
      'Eliminar',
      'Cancelar',
      'warning'
    );
    if (!result.isConfirmed) return;

    this.formModuleService.delete(row.id).pipe(take(1)).subscribe({
      next: () => {
        this.sweetAlertService.showNotification('Eliminación Exitosa', 'Relación Form–Module eliminada exitosamente.', 'success');
        this.loadFormModules();
      },
      error: (err) => {
        console.error('Error eliminando FormModule:', err);
        this.sweetAlertService.showApiError(err, 'No se pudo eliminar la relación Form–Module.');
      }
    });
  }

  onView(row: FormModuleSelectModel): void {
    // Ver detalle (diálogo o navegación)
  }

  // Toggle activo/inactivo (UI optimista + rollback)
  onToggleActive(row: FormModuleSelectModel, e: { checked: boolean } | boolean): void {
    const nextValue = typeof e === 'boolean' ? e : !!e?.checked;

    if (this.busyToggleIds.has(row.id)) return; // evita doble click
    this.busyToggleIds.add(row.id);

    const original = this.formModulesSubject.getValue();
    // UI optimista
    this.formModulesSubject.next(
      original.map(it => it.id === row.id ? { ...it, active: nextValue } : it)
    );

    this.formModuleService.changeActiveStatus(row.id, nextValue).pipe(take(1)).subscribe({
      next: (updated: Partial<FormModuleSelectModel> | void) => {
        // Si la API retorna DTO, sincronizamos; si retorna 204/void, mantenemos optimista
        if (updated && typeof updated.active === 'boolean') {
          const curr = this.formModulesSubject.getValue();
          this.formModulesSubject.next(
            curr.map(it => it.id === row.id ? { ...it, active: updated.active! } : it)
          );
        }
        this.sweetAlertService.showNotification('Éxito', 'Estado de la relación actualizado correctamente.', 'success');
        this.busyToggleIds.delete(row.id);
      },
      error: (err) => {
        // rollback
        this.formModulesSubject.next(original);
        this.sweetAlertService.showApiError(err, 'No se pudo cambiar el estado.');
        this.busyToggleIds.delete(row.id);
      }
    });
  }
}
