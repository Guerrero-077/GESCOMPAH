import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatStepperModule } from '@angular/material/stepper';
import { Observable, tap } from 'rxjs';
import { CardComponent } from "../../../../shared/components/card/card.component";
import { EstablishmentSelect } from '../../models/establishment.models';
import { EstablishmentStore } from '../../services/establishment/establishment.store';
import { FormEstablishmentComponent } from '../form-establishment/form-establishment.component';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service'
import { EstablishmentDetailDialogComponent } from '../establishment-detail-dialog/establishment-detail-dialog.component';
import { SharedEventsServiceService } from '../../services/shared/shared-events-service.service';


@Component({
  selector: 'app-establishments-list',
  imports: [
    CommonModule,
    CardComponent,
    ReactiveFormsModule,
    MatDialogModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatStepperModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './establishments-list.component.html',
  styleUrl: './establishments-list.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EstablishmentsListComponent {
  establishments$: Observable<EstablishmentSelect[]>;
  establishments: EstablishmentSelect[] = [];

  private readonly dialog = inject(MatDialog);
  private readonly store = inject(EstablishmentStore);
  private readonly sweetAlert = inject(SweetAlertService);
  private readonly sharedEvents = inject(SharedEventsServiceService);


  constructor() {
    this.establishments$ = this.store.establishments$.pipe(
      tap(establishments => this.establishments = establishments)
    );

    this.sharedEvents.plazaStateChanged$.subscribe(() => {
      this.store.loadAll();
    });
  }

  openCreateDialog(): void {
    const ref = this.dialog.open(FormEstablishmentComponent, {
      width: '600px',
      data: null
    });
    ref.afterClosed().subscribe(ok => {
      if (ok) this.store.loadAll();
    });
  }

  openEditDialog(est: EstablishmentSelect): void {
    const ref = this.dialog.open(FormEstablishmentComponent, {
      width: '600px',
      data: est
    });
    ref.afterClosed().subscribe(ok => {
      if (ok) this.store.loadAll();
    });
  }

  handleDelete(id: number): void {
    this.sweetAlert.showConfirm('¿Está seguro?', 'Esta acción no se puede deshacer').then(result => {
      if (result.isConfirmed) {
        this.store.delete(id).subscribe({
          next: () => {
            this.sweetAlert.showNotification('Éxito', 'Local eliminado exitosamente', 'success');
          }
        });
      }
    });
  }

  onCardUpdated(): void {
    this.store.loadAll();
  }


  onView(id: number) {
    const row = this.establishments.find(e => e.id === id);
    if (row) { // Asegurarse de que el establecimiento fue encontrado
      this.dialog.open(EstablishmentDetailDialogComponent, {
        width: '900px',
        data: row
      });
    } else {
      console.warn(`Establecimiento con ID ${id} no encontrado.`);
      // Opcional: mostrar una alerta al usuario
      this.sweetAlert.showNotification('Error', 'Establecimiento no encontrado', 'error');
    }
  }

}
