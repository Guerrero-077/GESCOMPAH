import { Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { TableColumn } from '../../../../shared/models/TableColumn.models';
import { SquareService } from '../../services/square/square.service';
import { SquareSelectModel, SquareUpdateModel } from '../../models/squares.models';
import { FormDialogComponent } from '../../../../shared/components/form-dialog/form-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { SquareStore } from '../../services/square/square.store';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';
import { catchError, EMPTY, filter, map, switchMap, take, tap } from 'rxjs';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { CommonModule } from '@angular/common';
import { ToggleButtonComponent } from "../../../../shared/components/toggle-button-component/toggle-button-component.component";
import { SharedEventsServiceService } from '../../services/shared/shared-events-service.service';

@Component({
  selector: 'app-square-list',
  imports: [GenericTableComponent, CommonModule, ToggleButtonComponent],
  templateUrl: './square-list.component.html',
  styleUrl: './square-list.component.css'
})
export class SquareListComponent implements OnInit {

  //Servicios
  private readonly squaresStore = inject(SquareStore);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);
  private sharedEvents = inject(SharedEventsServiceService);



  // Variables
  squares$ = this.squaresStore.squares$;
  selectedSquare: any = null;
  columns: TableColumn<SquareSelectModel>[] = [];

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;
  constructor(private dialog: MatDialog) { }



  //Inicializador
  ngOnInit(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'name', header: 'Nombre' },
      { key: 'description', header: 'Descripción' },
      { key: 'location', header: 'Ubicación' },
      {
        key: 'active',
        header: 'Estado',
        type: 'custom',
        template: this.estadoTemplate
      }
    ];
  }




  onView(row: SquareSelectModel) {
    console.log('Ver:', row);
  }

  onCreate() {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: {},
        formType: 'Plaza'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (!result) return;


      this.squaresStore.create(result).pipe(take(1)).subscribe({
        next: () => {
          this.sweetAlertService.showNotification(
            'Creación Exitosa',
            'Plaza creada exitosamente.',
            'success'
          );
        },
        error: (err) => {
          console.error('Error creando la plaza:', err);
          this.sweetAlertService.showNotification(
            'Error',
            'No se pudo crear la plaza.',
            'error'
          );
        }
      });
    });

  }



  onEdit(row: SquareUpdateModel) {
    const dialogRef = this.dialog.open(FormDialogComponent, {
      width: '600px',
      data: {
        entity: row,
        formType: 'Plaza'
      }
    });

    dialogRef.afterClosed().pipe(
      filter((result): result is Partial<SquareUpdateModel> => !!result),

      map(result => ({ id: row.id, ...result } as SquareUpdateModel)),

      switchMap(dto => this.squaresStore.update(dto.id, dto)),

      take(1),

      tap(() => {
        this.sweetAlertService.showNotification(
          'Actualización Exitosa',
          'Plaza actualizada exitosamente.',
          'success'
        );
      }),

      catchError(err => {
        console.error('Error actualizando la plaza:', err);
        this.sweetAlertService.showNotification(
          'Error',
          'No se pudo actualizar la plaza.',
          'error'
        );
        return EMPTY;
      })
    ).subscribe();
  }

  async onDelete(row: SquareSelectModel) {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar plaza',
      text: `¿Deseas eliminar la plaza "${row.name}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });

    if (confirmed) {
      this.squaresStore.deleteLogical(row.id).subscribe({
        next: () => {
          this.sweetAlertService.showNotification('Eliminación exitosa', 'Plaza eleminada correctamente.', 'success')
        },
        error: err => {
          console.log('Error elimnando plaza: ', err);
          this.sweetAlertService.showNotification('Error', 'No se pudo eliminar la plaza.', 'error')
        }
      })
    }
  }


  // ----- Toggle estado (activo/inactivo) -----
  onToggleActive(row: SquareSelectModel, e: { checked: boolean }) {
    const previous = row.active;
    row.active = e.checked;
    this.squaresStore.changeActiveStatus(row.id, e.checked).subscribe({
      next: (updated) => {
        // sincronizar con lo que devuelve el backend
        row.active = updated.active ?? row.active;
        this.sharedEvents.notifyPlazaStateChanged(row.id);
        this.sweetAlertService.showNotification(
          'Éxito',
          `Plaza ${row.active ? 'activado' : 'desactivado'} correctamente.`,
          'success'
        );
      },
      error: (err) => {
        row.active = previous;
        this.sweetAlertService.showNotification(
          'Error',
          err?.error?.detail || 'No se pudo cambiar el estado.',
          'error'
        );
      }
    });
  }


}
