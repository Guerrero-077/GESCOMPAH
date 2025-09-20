import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Inject, OnInit, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { finalize, take } from 'rxjs/operators';

import { StandardButtonComponent } from '../../../../shared/components/standard-button/standard-button.component';
import { AppointmentStore } from '../../services/appointment/appointment.store';
import { AppointmentService } from '../../services/appointment/appointment.service';
import { AppointmentSelect } from '../../models/appointment.models';

type ComputedStatus = 'SCHEDULED' | 'OVERDUE' | 'CLOSED';

@Component({
  selector: 'app-appointment-detail-dialog',
  standalone: true,
  imports: [
    CommonModule,
    MatDialogModule,
    MatIconModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    StandardButtonComponent,
  ],
  templateUrl: './appointment-detail-dialog.component.html',
  styleUrls: ['./appointment-detail-dialog.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppointmentDetailDialogComponent implements OnInit {
  // Inyección
  private readonly store = inject(AppointmentStore);
  private readonly svc = inject(AppointmentService);

  // Estado local
  appointment: AppointmentSelect | null = null;
  loading = false;
  error: string | null = null;

  // Etiquetas de estado (calculado en front)
  private static readonly STATUS_LABEL = {
    SCHEDULED: 'Programada',
    OVERDUE: 'Vencida',
    CLOSED: 'Cerrada',
  } as const;

  constructor(
    private readonly dialogRef: MatDialogRef<AppointmentDetailDialogComponent>,
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
      this.error = 'ID de cita no proporcionado';
      this.loading = false;
      return;
    }

    // 1) Intento rápido desde el Store (memoria)
    const fromStore = this.store.loadById(id);
    if (fromStore) {
      this.appointment = fromStore;
      this.loading = false;
      return;
    }

    // 2) Fallback al servicio (asegúrate de exponer getById en AppointmentService)
    this.svc.getById(id)
      .pipe(finalize(() => (this.loading = false)), take(1))
      .subscribe({
        next: (appointment) => {
          // Re-sincroniza con estado actual del Store si aplica
          const current = this.store.loadById(appointment.id);
          this.appointment = current ?? appointment;
        },
        error: (err) => {
          console.error('Error loading appointment detail:', err);
          this.error = 'No se pudo cargar el detalle de la cita.';
        },
      });
  }

  // Estado calculado:
  // - CLOSED: !active
  // - OVERDUE: active && dateTimeAssigned en el pasado
  // - SCHEDULED: active && dateTimeAssigned en el futuro/ahora
  getComputedStatus(a: AppointmentSelect | null): ComputedStatus {
    if (!a) return 'CLOSED';
    if (!a.active) return 'CLOSED';
    const now = new Date();
    const assigned = new Date(a.dateTimeAssigned);
    return assigned.getTime() < now.getTime() ? 'OVERDUE' : 'SCHEDULED';
  }

  getStatusText(status: ComputedStatus): string {
    return AppointmentDetailDialogComponent.STATUS_LABEL[status];
  }

  close(): void {
    this.dialogRef.close();
  }
}
