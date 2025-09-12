import { inject, Injectable } from '@angular/core';
import { SweetAlertService } from './sweet-alert/sweet-alert.service';

@Injectable({ providedIn: 'root' })
export class ConfirmDialogService {
  private readonly sweet = inject(SweetAlertService);
  async confirm(options: {
    title?: string;
    text?: string;
    confirmButtonText?: string;
    cancelButtonText?: string;
    icon?: 'warning' | 'info' | 'question' | 'error' | 'success';
  }): Promise<boolean> {
    const {
      title = '¿Estás seguro?',
      text = 'Esta acción no se puede deshacer.',
      confirmButtonText = 'Sí, continuar',
      cancelButtonText = 'Cancelar',
      icon = 'warning',
    } = options;

    const result = await this.sweet.showConfirm(title, text, confirmButtonText, cancelButtonText, icon);

    return result.isConfirmed;
  }
}
