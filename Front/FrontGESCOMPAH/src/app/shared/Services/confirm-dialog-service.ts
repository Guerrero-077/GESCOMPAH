import { Injectable } from '@angular/core';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class ConfirmDialogService {
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

    const result = await Swal.fire({
      title,
      text,
      icon,
      showCancelButton: true,
      confirmButtonText,
      cancelButtonText,
      reverseButtons: true,
      focusCancel: true,
    });

    return result.isConfirmed;
  }
}
