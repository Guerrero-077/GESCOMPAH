import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatStepperModule } from '@angular/material/stepper';
import { Observable } from 'rxjs';
import { CardComponent } from "../../../../shared/components/card/card.component";
import { EstablishmentSelect } from '../../models/establishment.models';
import { EstablishmentStore } from '../../services/establishment/establishment.store';
import { FormEstablishmentComponent } from '../form-establishment/form-establishment.component';
import { SweetAlertService } from '../../../../shared/Services/sweet-alert/sweet-alert.service'


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
    MatProgressSpinnerModule,
  ],
  templateUrl: './establishments-list.component.html',
  styleUrl: './establishments-list.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EstablishmentsListComponent {
  establishments$: Observable<EstablishmentSelect[]>;

  constructor(
    private dialog: MatDialog,
    private store: EstablishmentStore,
    private sweetAlert: SweetAlertService
  ) {
    this.establishments$ = this.store.establishments$;
  }

  openCreateDialog(): void {
    this.dialog.open(FormEstablishmentComponent, {
      width: '600px',
      data: null
    });
  }

  openEditDialog(est: EstablishmentSelect): void {
    this.dialog.open(FormEstablishmentComponent, {
      width: '600px',
      data: est
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
}
