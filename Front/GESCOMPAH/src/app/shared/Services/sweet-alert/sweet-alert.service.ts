
import { Injectable } from '@angular/core';
import Swal, { SweetAlertIcon } from 'sweetalert2';

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

  private baseColors() {
    return {
      // Superficies y texto
      surface: this.cssVar('--color-surface', '#ffffff'),
      text: this.cssVar('--color-text', '#1f2937'),
      // Botones
      primary: this.cssVar('--color-primary', '#16a34a'),
      primaryContrast: this.cssVar('--color-primary-contrast', '#ffffff'),
      // Estados
      success: this.cssVar('--state-success', '#16a34a'),
      warning: this.cssVar('--state-warning', '#eab308'),
    } as const;
  }

  public showNotification(title: string, text: string, icon: SweetAlertIcon): void {
    const c = this.baseColors();
    const iconColor = icon === 'success' ? c.success : icon === 'warning' ? c.warning : undefined;

    Swal.fire({
      title,
      text,
      icon,
      iconColor,
      color: c.text,
      background: c.surface,
      toast: true,
      position: 'top-end',
      showConfirmButton: false,
      timer: 3000,
      timerProgressBar: true,
    });
  }

  public showConfirm(
    title: string,
    text: string,
    confirmButtonText = 'Aceptar',
    cancelButtonText = 'Cancelar',
    icon: SweetAlertIcon = 'warning'
  ): Promise<any> {
    const c = this.baseColors();
    const iconColor = icon === 'success' ? c.success : icon === 'warning' ? c.warning : undefined;
    return Swal.fire({
      title,
      text,
      icon,
      iconColor,
      color: c.text,
      background: c.surface,
      showCancelButton: true,
      confirmButtonColor: c.primary,
      cancelButtonColor: this.cssVar('--gray-400', '#9ca3af'),
      confirmButtonText,
      cancelButtonText,
    });
  }

  public showLoading(title: string, text: string): void {
    const c = this.baseColors();
    Swal.fire({
      title,
      text,
      color: c.text,
      background: c.surface,
      didOpen: () => {
        Swal.showLoading();
      },
      allowOutsideClick: false,
      allowEscapeKey: false,
      allowEnterKey: false
    });
  }

  public hideLoading(): void {
    Swal.close();
  }

  public success(message: string, title: string = 'Éxito'): void {
    this.showNotification(title, message, 'success');
  }

  public error(message: string, title: string = 'Error'): void {
    this.showNotification(title, message, 'error');
  }

  public info(message: string, title: string = 'Información'): void {
    this.showNotification(title, message, 'info');
  }

  public warning(message: string, title: string = 'Advertencia'): void {
    this.showNotification(title, message, 'warning');
  }

  // Alias semántico para notificaciones tipo toast
  public toast(title: string, text: string, icon: SweetAlertIcon): void {
    this.showNotification(title, text, icon);
  }

  public async confirm(options: {
    title?: string;
    text?: string;
    confirmButtonText?: string;
    cancelButtonText?: string;
    icon?: SweetAlertIcon;
  } = {}): Promise<boolean> {
    const {
      title = '¿Estás seguro?',
      text = 'Esta acción no se puede deshacer.',
      confirmButtonText = 'Aceptar',
      cancelButtonText = 'Cancelar',
      icon = 'warning',
    } = options;
    const result = await this.showConfirm(title, text, confirmButtonText, cancelButtonText, icon);
    return !!result?.isConfirmed;
  }
}
