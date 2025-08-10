import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CardComponent } from "../../../../shared/components/card/card.component";
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatStepperModule } from '@angular/material/stepper';
import { EstablishmentSelect } from '../../models/establishment.models';
import { FormEstablishmentComponent } from '../form-establishment/form-establishment.component';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { EstablishmentStore } from '../../services/establishment/establishment.store';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

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
    MatSnackBarModule
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
    private snackbar: MatSnackBar
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
    this.store.delete(id).subscribe({
      next: () => {
        this.snackbar.open('Local eliminado exitosamente', 'Cerrar', { duration: 2000 });
      }
    });
  }
}