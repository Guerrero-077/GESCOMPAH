import { CommonModule } from '@angular/common';
import { Component, Inject, OnInit, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { finalize, take } from 'rxjs/operators';
import { forkJoin } from 'rxjs';

import { ContractSelectModel, MonthlyObligation } from '../../models/contract.models';
import { ContractService } from '../../services/contract/contract.service';
import { StandardButtonComponent } from '../../../../shared/components/standard-button/standard-button.component';
import { ContractStore } from '../../services/contract/contract.store';
import { MoneyPipe } from '../../../../shared/pipes/money.pipe';

@Component({
  selector: 'app-contract-detail-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    StandardButtonComponent,
    MoneyPipe,
  ],
  templateUrl: './contract-detail-dialog.component.html',
  styleUrls: ['./contract-detail-dialog.component.css'],
})
export class ContractDetailDialogComponent implements OnInit {
  private readonly svc = inject(ContractService);
  private readonly store = inject(ContractStore);

  contract: ContractSelectModel | null = null;
  obligations: MonthlyObligation[] = [];
  loading = false;
  error: string | null = null;

  // Mejor performance en *ngFor
  trackByObligationId = (_: number, item: MonthlyObligation) => item.id;

  // --- Mapa tipado y type-guard para evitar TS7053 ---
  private static readonly STATUS_LABEL = {
    PAID: 'Pagado',
    PENDING: 'Pendiente',
    OVERDUE: 'Vencido',
  } as const;

  private static isStatusKey(s: string): s is keyof typeof ContractDetailDialogComponent.STATUS_LABEL {
    return Object.prototype.hasOwnProperty.call(ContractDetailDialogComponent.STATUS_LABEL, s);
  }

  getStatusText(status: string): string {
    return ContractDetailDialogComponent.isStatusKey(status)
      ? ContractDetailDialogComponent.STATUS_LABEL[status]
      : status; // fallback si backend envía algo no mapeado
  }
  // ---------------------------------------------------

  constructor(
    private readonly dialogRef: MatDialogRef<ContractDetailDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { id: number }
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  private loadData(): void {
    this.loading = true;
    this.error = null;

    const id = this.data?.id;
    if (!id) {
      this.error = 'ID de contrato no proporcionado';
      this.loading = false;
      return;
    }

    forkJoin({
      contract: this.svc.getById(id).pipe(take(1)),
      obligations: this.svc.getMonthlyObligations(id).pipe(take(1)),
    })
      .pipe(finalize(() => (this.loading = false)))
      .subscribe({
        next: ({ contract, obligations }) => {
          // Sincroniza estado activo con la grilla si hubiese cambio reciente
          const current = this.store.getById(contract.id);
          this.contract = current ? { ...contract, active: current.active } : contract;
          this.obligations = (obligations || []).sort((a, b) => {
            // sort por año desc y mes desc
            if (a.year !== b.year) return b.year - a.year;
            return b.month - a.month;
          });
        },
        error: (err) => {
          console.error('Error loading contract detail:', err);
          this.error = 'No se pudo cargar el detalle del contrato.';
        },
      });
  }

  close(): void {
    this.dialogRef.close();
  }
}
