import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { BehaviorSubject, catchError, forkJoin, map, Observable, of } from 'rxjs';

import { FormModuleService } from '../../services/form-module/form-module.service';
import { FormService } from '../../services/form/form.service';
import { ModuleService } from '../../services/module/module.service';

import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { ToggleButtonComponent } from "../../../../shared/components/toggle-button-component/toggle-button-component.component";
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { FormModuleCreateModel, FormModuleSelectModel, FormModuleUpdateModel } from '../../models/form-module.model';
import { HasRoleAndPermissionDirective } from '../../../../core/Directives/HasRoleAndPermission.directive';

@Component({
  selector: 'app-form-module',
  standalone: true,
  imports: [GenericTableComponent, CommonModule, ToggleButtonComponent, HasRoleAndPermissionDirective],
  templateUrl: './form-module.component.html',
  styleUrl: './form-module.component.css'
})
export class FormModuleComponent implements OnInit {
  private readonly formService = inject(FormService);
  private readonly moduleService = inject(ModuleService);
  private readonly formModuleService = inject(FormModuleService);
  private readonly dialog = inject(MatDialog);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);

  private formModulesSubject = new BehaviorSubject<FormModuleSelectModel[]>([]);
  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  formModules$ = this.formModulesSubject.asObservable();

  columns!: TableColumn<FormModuleSelectModel>[];

  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'formName', header: 'Formulario' },
      { key: 'moduleName', header: 'Módulo' },
      { key: 'active', header: 'Estado', type: 'custom', template: this.estadoTemplate }
    ];
    this.loadFormModules();
  }

  private loadFormModules(): void {
    this.formModuleService.getAll().subscribe({
      next: (data) => {
        this.formModulesSubject.next(data as FormModuleSelectModel[]);
      },
      error: (err) => {
        console.error('Error loading form modules:', err);
        this.sweetAlertService.showNotification('Error', 'No se pudieron cargar las relaciones Form–Module.', 'error');
      }
    });
  }

  // --- helpers para selects ---
  private getFormOptions$(): Observable<any[]> {
    return this.formService.getAll().pipe(
      catchError(() => of([])),
      map((forms: any[]) => forms.map(f => ({ value: f.id, label: f.name ?? f.formName ?? `Form ${f.id}` })))
    );
  }

  private getModuleOptions$(): Observable<any[]> {
    return this.moduleService.getAll().pipe(
      catchError(() => of([])),
      map((mods: any[]) => mods.map(m => ({ value: m.id, label: m.name ?? m.moduleName ?? `Module ${m.id}` })))
    );
  }

  // --- CREATE ---
  onCreateNew(): void {
    forkJoin({
      formOpts: this.getFormOptions$(),
      moduleOpts: this.getModuleOptions$()
    }).subscribe(({ formOpts, moduleOpts }) => {
      if (!formOpts.length || !moduleOpts.length) {
        console.error('No hay opciones disponibles para Form/Module');
        this.sweetAlertService.showNotification('Advertencia', 'No hay opciones disponibles para Formularios o Módulos. Asegúrese de que existan.', 'warning');
        return;
      }

      const initial: { formId: number; moduleId: number; active: boolean } = {
        formId: formOpts[0].value,
        moduleId: moduleOpts[0].value,
        active: true
      };

      const dialogRef = this.dialog.open(FormDialogComponent, {
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

      dialogRef.afterClosed().subscribe((result: any) => {
        if (!result) return;

        const payload: FormModuleCreateModel = {
          formId: +result.formId,
          moduleId: +result.moduleId
        };

        this.formModuleService.create(payload).subscribe({
          next: () => {
            this.sweetAlertService.showNotification('Creación Exitosa', 'Relación Form–Module creada exitosamente.', 'success');
            this.loadFormModules(); // Recargar la tabla
          },
          error: err => {
            console.error('Error creando FormModule:', err);
            this.sweetAlertService.showNotification('Error', 'No se pudo crear la relación Form–Module.', 'error');
          }
        });
      });
    });
  }

  // --- EDIT ---
  onEdit(row: FormModuleSelectModel): void {
    const id = row.id;

    forkJoin({
      formOpts: this.getFormOptions$(),
      moduleOpts: this.getModuleOptions$(),
      current: this.formModuleService.getById(id)
    })
      .pipe(
        map(({ formOpts, moduleOpts, current }) => {
          const initial = {
            id: current.id,
            formId: (current as any).formId,
            moduleId: (current as any).moduleId,
            active: current.active
          };
          return { formOpts, moduleOpts, initial };
        }),
        catchError(err => {
          console.error('Error al obtener datos para edición:', err);
          this.sweetAlertService.showNotification('Error', 'No se pudieron cargar los datos para editar la relación Form–Module.', 'error');
          return of({ formOpts: [], moduleOpts: [], initial: {} as any }); // Return empty to prevent app crash
        })
      )
      .subscribe(({ formOpts, moduleOpts, initial }) => {
        if (!formOpts.length || !moduleOpts.length) {
          console.error('No hay opciones disponibles para Form/Module');
          this.sweetAlertService.showNotification('Advertencia', 'No hay opciones disponibles para Formularios o Módulos. Asegúrese de que existan.', 'warning');
          return;
        }

        const dialogRef = this.dialog.open(FormDialogComponent, {
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

        dialogRef.afterClosed().subscribe((result: any) => {
          if (!result) return;

          const payload: FormModuleUpdateModel = {
            id,
            formId: +result.formId,
            moduleId: +result.moduleId
          };

          this.formModuleService.update(id, payload).subscribe({
            next: () => {
              this.sweetAlertService.showNotification('Actualización Exitosa', 'Relación Form–Module actualizada exitosamente.', 'success');
              this.loadFormModules(); // Recargar la tabla
            },
            error: err => {
              console.error('Error actualizando FormModule:', err)
              this.sweetAlertService.showNotification('Error', 'No se pudo actualizar la relación Form–Module.', 'error');
            }
          });
        });
      });
  }

  // --- DELETE ---
  async onDelete(row: FormModuleSelectModel): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar relación Form–Module',
      text: `¿Eliminar el formulario "${row.formName}" del módulo "${row.moduleName}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar'
    });

    if (!confirmed) return;

    this.formModuleService.delete(row.id).subscribe({
      next: () => {
        this.sweetAlertService.showNotification('Eliminación Exitosa', 'Relación Form–Module eliminada exitosamente.', 'success');
        this.loadFormModules(); // Recargar la tabla
      },
      error: err => {
        console.error('Error eliminando FormModule:', err);
        this.sweetAlertService.showNotification('Error', 'No se pudo eliminar la relación Form–Module.', 'error');
      }
    });
  }

  onView(row: FormModuleSelectModel): void {
    console.log('Detalle FormModule:', row);
  }


  // Toggle activo/inactivo
  onToggleActive(row: FormModuleSelectModel, e: { checked: boolean }) {
    const nextValue = e.checked;

    // Store original data for potential revert
    const originalModules = this.formModulesSubject.getValue();

    // Optimistic UI update
    const updatedModules = originalModules.map(item =>
      item.id === row.id ? { ...item, active: nextValue } : item
    );
    this.formModulesSubject.next(updatedModules);

    this.formModuleService.changeActiveStatus(row.id, nextValue).subscribe({
      next: () => {
        this.sweetAlertService.showNotification(
          'Éxisto',
          `Estado de la relación actualizado correctamente.`,
          'success'
        );
      },
      error: (err) => {
        // Revert UI on error
        this.formModulesSubject.next(originalModules);

        this.sweetAlertService.showNotification(
          'Error',
          err?.error?.detail || 'No se pudo cambiar el estado.',
          'error'
        );
      }
    });
  }
}
