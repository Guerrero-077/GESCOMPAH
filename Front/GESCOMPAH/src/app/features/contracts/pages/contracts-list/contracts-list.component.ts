import { Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { EMPTY } from 'rxjs';
import { catchError, filter, switchMap, take, tap } from 'rxjs/operators';

// Models
import { ContractCreateModel, ContractSelectModel } from '../../models/contract.models';
import { TableColumn } from '../../../../shared/models/TableColumn.models';

// Services
import { ContractService } from '../../services/contract/contract.service';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';

// Components
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { ToggleButtonComponent } from "../../../../shared/components/toggle-button-component/toggle-button-component.component";
import { FormContractComponent } from '../../components/form-contract/form-contract.component';

@Component({
  selector: 'app-contracts-list',
  standalone: true,
  imports: [
    CommonModule,
    GenericTableComponent,
    ToggleButtonComponent
  ],
  templateUrl: './contracts-list.component.html',
  styleUrls: ['./contracts-list.component.css']
})
export class ContractsListComponent implements OnInit {

  private readonly contractService = inject(ContractService);
  private readonly dialog = inject(MatDialog);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);

  contracts$ = this.contractService.contracts;

  columns: TableColumn<ContractSelectModel>[] = [];

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  ngOnInit(): void {
    this.loadContracts();
    this.setupColumns();
  }

  loadContracts(): void {
    this.contractService.getAllContracts({ force: true }).pipe(
      take(1),
      catchError(err => {
        console.error('Error loading contracts:', err);
        this.sweetAlertService.showNotification('Error', 'No se pudieron cargar los contratos.', 'error');
        return EMPTY;
      })
    ).subscribe();
  }

  setupColumns(): void {
    this.columns = [
      { key: 'index', header: 'Nº', type: 'index' },
      { key: 'fullName', header: 'Arrendatario' },
      { key: 'document', header: 'Documento' },
      { key: 'startDate', header: 'Fecha Inicio' },
      { key: 'endDate', header: 'Fecha Fin' },
      {
        key: 'active',
        header: 'Estado',
        type: 'custom',
        template: this.estadoTemplate
      }
    ];
  }

  onCreateNew() {
    const dialogRef = this.dialog.open(FormContractComponent, {
      width: '800px',
      data: { entity: null, formType: 'Contract' }
    });

    dialogRef.afterClosed().pipe(
      filter(result => !!result),
      switchMap(result => this.contractService.createContract(result)),
      take(1)
    ).subscribe({
      next: () => {
        this.sweetAlertService.showNotification(
          'Creación Exitosa',
          'Contrato creado exitosamente.',
          'success'
        );
      },
      error: (err) => {
        console.error('Error creando el contrato:', err);
        this.sweetAlertService.showNotification(
          'Error',
          'No se pudo crear el contrato.',
          'error'
        );
      }
    });
  }

  onEdit(row: ContractSelectModel) {
    const dialogRef = this.dialog.open(FormContractComponent, {
      width: '800px',
      data: { entity: row, formType: 'Contract' }
    });

    dialogRef.afterClosed().pipe(
      filter((result): result is Partial<ContractCreateModel> => !!result),
      switchMap(updateDto => this.contractService.updateContract(row.id, updateDto)),
      take(1),
      tap(() => {
        this.sweetAlertService.showNotification(
          'Actualización Exitosa',
          'Contrato actualizado exitosamente.',
          'success'
        );
      }),
      catchError(err => {
        console.error('Error actualizando el contrato:', err);
        this.sweetAlertService.showNotification(
          'Error',
          'No se pudo actualizar el contrato.',
          'error'
        );
        return EMPTY;
      })
    ).subscribe();
  }

  async onDelete(row: ContractSelectModel) {
    const confirmed = await this.confirmDialog.confirm({
      title: 'Eliminar Contrato',
      text: `¿Deseas eliminar el contrato para "${row.fullName}"?`,
      confirmButtonText: 'Eliminar',
      cancelButtonText: 'Cancelar',
    });

    if (confirmed) {
      this.contractService.deleteContract(row.id).pipe(take(1)).subscribe({
        next: () => {
          this.sweetAlertService.showNotification('Eliminación Exitosa', 'Contrato eliminado exitosamente.', 'success');
        },
        error: err => {
          console.error('Error eliminando el contrato:', err);
          this.sweetAlertService.showNotification('Error', 'No se pudo eliminar el contrato.', 'error');
        }
      });
    }
  }

  onView(row: ContractSelectModel) {
    this.contractService.downloadContractPdf(row.id).subscribe({
      next: blob => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Contrato_${row.fullName}.pdf`;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(url);
        document.body.removeChild(a);
        this.sweetAlertService.showNotification('Éxito', 'La descarga del contrato ha comenzado.', 'success');
      },
      error: err => {
        console.error('Error downloading PDF:', err);
        this.sweetAlertService.showNotification('Error', 'No se pudo descargar el contrato.', 'error');
      }
    });
  }

  // ----- Toggle estado (activo/inactivo) -----
  onToggleActive(row: ContractSelectModel, e: { checked: boolean }) {
    const previous = row.active;
    const nextValue = e.checked;

    // No toques row.active aquí; deja que el servicio + store lo actualicen
    this.contractService.updateContractActive(row.id, nextValue).subscribe({
      next: (updated) => {
        // Ya está sincronizado por el store; solo feedback
        this.sweetAlertService.showNotification(
          'Éxito',
          `Contrato ${updated.active ? 'activado' : 'desactivado'} correctamente.`,
          'success'
        );
      },
      error: (err) => {
        // Si por alguna razón tocaste row.active antes, reviértelo:
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
