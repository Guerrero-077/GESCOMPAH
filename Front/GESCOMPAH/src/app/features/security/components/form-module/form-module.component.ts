import { Component, OnInit, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { forkJoin, map, of, catchError } from 'rxjs';

import { FormModuleService } from '../../services/form-module/form-module.service';
import { FormService } from '../../services/form/form.service';
import { ModuleService } from '../../services/module/module.service';

import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { FormModuleSelectDto, FormModuleCreateDto, FormModuleUpdateDto } from '../../models/form-module..models';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";

@Component({
  selector: 'app-form-module',
  standalone: true,
  imports: [GenericTableComponent],
  templateUrl: './form-module.component.html',
  styleUrl: './form-module.component.css'
})
export class FormModuleComponent implements OnInit {
  private readonly formModuleService = inject(FormModuleService);
  private readonly formService = inject(FormService);
  private readonly moduleService = inject(ModuleService);
  private readonly dialog = inject(MatDialog);
  private readonly confirmDialog = inject(ConfirmDialogService);

  items: FormModuleSelectDto[] = [];

  columns: TableColumn<FormModuleSelectDto>[] = [
    { key: 'index', header: 'Nº', type: 'index' },
    { key: 'formName', header: 'Formulario' },
    { key: 'moduleName', header: 'Módulo' },
    { key: 'active', header: 'Estado', type: 'boolean' }
  ];

  ngOnInit(): void { this.load(); }

  load(): void {
    this.formModuleService.getAll().subscribe({
      next: data => (this.items = data),
      error: err => console.error('Error cargando FormModule:', err)
    });
  }

  // --- helpers para selects ---
  private getFormOptions$() {
    return this.formService.getAll("form").pipe(
      catchError(() => of([])),
      map((forms: any[]) => forms.map(f => ({ value: f.id, label: f.name ?? f.formName ?? `Form ${f.id}` })))
    );
  }

  private getModuleOptions$() {
    return this.moduleService.getAll("module").pipe(
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
          formType: 'FormModule', // clave para tu DynamicForm
          selectOptions: {
            formId: formOpts,
            moduleId: moduleOpts
          }
        }
      });

      dialogRef.afterClosed().subscribe((result: any) => {
        if (!result) return;

        // Normaliza el resultado del diálogo (depende de tu DynamicForm)
        const payload: FormModuleCreateDto = {
          formId: +result.formId,
          moduleId: +result.moduleId
        };

        this.formModuleService.create(payload).subscribe({
          next: () => this.load(),
          error: err => console.error('Error creando FormModule:', err)
        });
      });
    });
  }

  // --- EDIT ---
  onEdit(row: FormModuleSelectDto): void {
    // Necesitas el id del vínculo FormModule
    const id = row.id;

    forkJoin({
      formOpts: this.getFormOptions$(),
      moduleOpts: this.getModuleOptions$(),
      current: this.formModuleService.getById(id)
    })
      .pipe(
        map(({ formOpts, moduleOpts, current }) => {
          // current = FormModuleSelectDto (no trae ids), por lo general necesitarás un endpoint que devuelva formId/moduleId.
          // Si tu API de GET by id devuelve { id, formId, moduleId, active }, usa eso.
          // Aquí asumo que tienes ese endpoint; si no, debes resolver los IDs en el back o ampliar el SelectDto.
          const initial = {
            id: current.id,
            formId: (current as any).formId,       // <-- ideal si tu API los expone
            moduleId: (current as any).moduleId,   // <-- ideal si tu API los expone
            active: current.active
          };
          return { formOpts, moduleOpts, initial };
        })
      )
      .subscribe(({ formOpts, moduleOpts, initial }) => {
        if (!formOpts.length || !moduleOpts.length) {
          console.error('No hay opciones disponibles para Form/Module');
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

          const payload: FormModuleUpdateDto = {
            id,
            formId: +result.formId,
            moduleId: +result.moduleId
          };

          this.formModuleService.update(id, payload).subscribe({
            next: () => this.load(),
            error: err => console.error('Error actualizando FormModule:', err)
          });
        });
      });
  }

  // --- DELETE ---
  async onDelete(row: FormModuleSelectDto): Promise<void> {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar relación Form–Module',
      text: `¿Eliminar el formulario "${row.formName}" del módulo "${row.moduleName}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar'
    });

    if (!confirmed) return;

    this.formModuleService.deleteLogical(row.id).subscribe({
      next: () => this.load(),
      error: err => console.error('Error eliminando FormModule:', err)
    });
  }

  onView(row: FormModuleSelectDto): void {
    console.log('Detalle FormModule:', row);
  }
}
