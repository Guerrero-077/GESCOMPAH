
import { Injectable } from '@angular/core';
import type SwalNS from 'sweetalert2';
import type { SweetAlertIcon } from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class SweetAlertService {

  constructor() { }

  private cssVar(name: string, fallback: string): string {
    try {
      const v = getComputedStyle(document.documentElement).getPropertyValue(name).trim();
      return v || fallback;
    } catch {
      return fallback;
    }
  }

  public async showNotification(title: string, text: string, icon: SweetAlertIcon): Promise<void> {
    const { default: Swal } = await import('sweetalert2');
    const fg = this.cssVar('--color-text', '#111827');

    // Selección semántica por tipo de icono
    let bg = this.cssVar('--color-surface', '#ffffff');
    let iconColor: string | undefined;
    switch (icon) {
      case 'success':
        bg = this.cssVar('--state-success-bg', this.cssVar('--color-surface', '#ffffff'));
        iconColor = this.cssVar('--state-success', '#16a34a');
        break;
      case 'warning':
        bg = this.cssVar('--state-warning-bg', this.cssVar('--color-surface', '#ffffff'));
        iconColor = this.cssVar('--state-warning', '#eab308');
        break;
      case 'error':
        bg = this.cssVar('--state-error-bg', this.cssVar('--color-surface', '#ffffff'));
        iconColor = this.cssVar('--state-error', '#ef4444');
        break;
      case 'info':
        bg = this.cssVar('--state-info-bg', this.cssVar('--color-surface', '#ffffff'));
        iconColor = this.cssVar('--state-info', '#3b82f6');
        break;
    }

    Swal.fire({
      title,
      text,
      icon,
      toast: true,
      position: 'top-end',
      showConfirmButton: false,
      timer: 3000,
      timerProgressBar: true,
      background: bg,
      color: fg,
      iconColor,
    });
  }

  public async showConfirm(
    title: string,
    text: string,
    confirmButtonText = 'Aceptar',
    cancelButtonText = 'Cancelar',
    icon: SweetAlertIcon = 'warning'
  ): Promise<any> {
    const { default: Swal } = await import('sweetalert2');
    // Base tokens
    const fg = this.cssVar('--color-text', '#111827');
    const bg = this.cssVar('--color-surface', '#ffffff');

    // Colores por icono
    let confirmColor = this.cssVar('--color-primary', '#16a34a');
    let cancelColor  = this.cssVar('--gray-400',   '#9ca3af');
    switch (icon) {
      case 'success':
        confirmColor = this.cssVar('--state-success', '#16a34a');
        break;
      case 'warning':
        confirmColor = this.cssVar('--state-warning', '#eab308');
        break;
      case 'error':
        confirmColor = this.cssVar('--state-error', '#ef4444');
        break;
      case 'info':
        confirmColor = this.cssVar('--state-info', '#3b82f6');
        break;
    }

    return Swal.fire({
      title,
      text,
      icon,
      showCancelButton: true,
      confirmButtonColor: confirmColor,
      cancelButtonColor: cancelColor,
      confirmButtonText,
      cancelButtonText,
      color: fg,
      background: bg,
      reverseButtons: true,
      focusCancel: true,
    });
  }

  public async showLoading(title: string, text: string): Promise<void> {
    const { default: Swal } = await import('sweetalert2');
    const fg = this.cssVar('--color-text', '#111827');
    const bg = this.cssVar('--color-surface', '#ffffff');
    Swal.fire({
      title,
      text,
      didOpen: () => {
        Swal.showLoading();
      },
      allowOutsideClick: false,
      allowEscapeKey: false,
      allowEnterKey: false,
      color: fg,
      background: bg,
    });
  }

  public async hideLoading(): Promise<void> {
    const { default: Swal } = await import('sweetalert2');
    Swal.close();
  }

  public async success(message: string, title: string = 'Éxito'): Promise<void> {
    await this.showNotification(title, message, 'success');
  }

  public async error(message: string, title: string = 'Error'): Promise<void> {
    await this.showNotification(title, message, 'error');
  }
}
