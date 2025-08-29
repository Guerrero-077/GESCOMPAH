// src/app/features/contracts/components/contracts-list/contracts-list.component.ts
import { Component, inject, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { EMPTY, of } from 'rxjs';
import { catchError, take, finalize, switchMap, tap } from 'rxjs/operators'; // 👈 añade tap

// Models
import { ContractSelectModel, ContractCard } from '../../models/contract.models';
import { TableColumn } from '../../../../shared/models/TableColumn.models';

// Services
import { ContractService } from '../../services/contract/contract.service';
import { ConfirmDialogService } from '../../../../shared/Services/confirm-dialog-service';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service';

// Components
import { GenericTableComponent } from "../../../../shared/components/generic-table/generic-table.component";
import { ToggleButtonComponent } from "../../../../shared/components/toggle-button-component/toggle-button-component.component";

// Angular Material
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FormContractComponent } from '../../components/form-contract/form-contract.component';

type ViewMode = 'mine' | 'all';

@Component({
  selector: 'app-contracts-list',
  standalone: true,
  imports: [
    CommonModule,
    GenericTableComponent,
    ToggleButtonComponent,
    MatProgressSpinnerModule,
    MatDialogModule,
  ],
  templateUrl: './contracts-list.component.html',
  styleUrls: ['./contracts-list.component.css']
})
export class ContractsListComponent implements OnInit {

  private readonly contractService = inject(ContractService);
  private readonly dialog = inject(MatDialog);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly sweetAlertService = inject(SweetAlertService);

  /** Cambia a 'all' si quieres usar la lista completa */
  viewMode: ViewMode = 'mine';

  // Data reactivas (signals expuestas por el service/store)
  contracts$ = this.contractService.contracts; // lista completa
  cards$     = this.contractService.cards;     // lista “mine”

  // Columnas por modo
  columnsAll:  TableColumn<ContractSelectModel>[] = [];
  columnsMine: TableColumn<ContractCard>[] = [];

  /** Flag de loading mientras se genera/descarga el PDF */
  isDownloadingPdf = false;

  @ViewChild('estadoTemplate', { static: true }) estadoTemplate!: TemplateRef<any>;

  ngOnInit(): void {
    this.loadData();
    this.setupColumns();
  }

  private loadData(): void {
    if (this.viewMode === 'mine') {
      this.contractService.getMineCards({ force: true }).pipe(
        take(1),
        catchError(err => {
          console.error('Error loading my contracts:', err);
          this.sweetAlertService.showNotification('Error', 'No se pudieron cargar tus contratos.', 'error');
          return EMPTY;
        })
      ).subscribe();
    } else {
      this.contractService.getAllContracts({ force: true }).pipe(
        take(1),
        catchError(err => {
          console.error('Error loading contracts:', err);
          this.sweetAlertService.showNotification('Error', 'No se pudieron cargar los contratos.', 'error');
          return EMPTY;
        })
      ).subscribe();
    }
  }

  private setupColumns(): void {
    // Lista COMPLETA (Admin)
    this.columnsAll = [
      { key: 'index',     header: 'Nº', type: 'index' },
      { key: 'fullName',  header: 'Arrendatario' },
      { key: 'document',  header: 'Documento' },
      { key: 'startDate', header: 'Fecha Inicio' },
      { key: 'endDate',   header: 'Fecha Fin' },
      { key: 'active',    header: 'Estado', type: 'custom', template: this.estadoTemplate }
    ];

    // Lista “MÍOS” (o TODOS si Admin, pero con read-model ligero)
    this.columnsMine = [
      { key: 'index',     header: 'Nº', type: 'index' },
      { key: 'personId',  header: 'PersonaId' },
      { key: 'startDate', header: 'Fecha Inicio' },
      { key: 'endDate',   header: 'Fecha Fin' },
      { key: 'totalBase', header: 'Total Base' },
      { key: 'totalUvt',  header: 'Total UVT' },
      { key: 'active',    header: 'Estado', type: 'custom', template: this.estadoTemplate }
    ];
  }

  /** ✅ Crea: abre diálogo, crea contrato y recarga según modo */
  onCreateNew() {
    const ref = this.dialog.open(FormContractComponent, {
      width: '800px',
      disableClose: true,
      autoFocus: true,
      data: null
    });

    ref.afterClosed().pipe(take(1)).subscribe((created: boolean) => {
      if (created) {
        this.sweetAlertService.showNotification('Éxito', 'Contrato creado correctamente.', 'success');
        this.loadData();
      }
    });
  }

  onEdit(row: ContractSelectModel) {
    // (tu lógica actual para la lista completa)
  }

  async onDelete(row: ContractSelectModel) {
    // (tu lógica actual para la lista completa)
  }

  /** Descargar PDF (funciona con ambos modelos) */
  onView(row: ContractSelectModel | ContractCard) {
    if (this.isDownloadingPdf) {
      this.sweetAlertService.showNotification('Información', 'Ya hay una descarga en curso.', 'info');
      return;
    }

    this.isDownloadingPdf = true;
    const id = row.id;

    // si es modelo completo, usar fullName; si es card, consulta detalle para bonito
    const fileName$ = ('fullName' in row && !!row.fullName)
      ? of(`Contrato_${row.fullName}.pdf`)
      : this.contractService.getContractById(id).pipe(
          take(1),
          catchError(() => of(null)),
          switchMap(det => of(`Contrato_${det?.fullName ?? id}.pdf`))
        );

    this.contractService.downloadContractPdf(id).pipe(
      take(1),
      switchMap(blob => fileName$.pipe(
        take(1),
        tap((fileName: string) => { // 👈 tip explícito opcional
          const url = window.URL.createObjectURL(blob);
          const a = document.createElement('a');
          a.href = url;
          a.download = fileName;
          document.body.appendChild(a);
          a.click();
          window.URL.revokeObjectURL(url);
          document.body.removeChild(a);
        })
      )),
      finalize(() => this.isDownloadingPdf = false)
    ).subscribe({
      next: () => this.sweetAlertService.showNotification('Éxito', 'La descarga del contrato ha comenzado.', 'success'),
      error: err => {
        console.error('Error downloading PDF:', err);
        this.sweetAlertService.showNotification('Error', 'No se pudo descargar el contrato.', 'error');
      }
    });
  }

  // ----- Toggle estado (activo/inactivo) -----
  onToggleActive(row: ContractSelectModel | ContractCard, e: { checked: boolean }) {
    const nextValue = e.checked;
    this.contractService.updateContractActive(row.id, nextValue).subscribe({
      next: (updated) => {
        this.sweetAlertService.showNotification(
          'Éxito',
          `Contrato ${updated.active ? 'activado' : 'desactivado'} correctamente.`,
          'success'
        );
      },
      error: (err) => {
        this.sweetAlertService.showNotification(
          'Error',
          err?.error?.detail || 'No se pudo cambiar el estado.',
          'error'
        );
      }
    });
  }
}
