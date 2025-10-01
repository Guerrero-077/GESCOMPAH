import { Injectable } from '@angular/core';
import type { SweetAlertIcon } from 'sweetalert2';

@Injectable({
  providedIn: 'root',
})
export class SweetAlertService {
  constructor() {}

  private cssVar(name: string, fallback: string): string {
    try {
      const v = getComputedStyle(document.documentElement)
        .getPropertyValue(name)
        .trim();
      return v || fallback;
    } catch {
      return fallback;
    }
  }

  public async showNotification(
    title: string,
    text: string,
    icon: SweetAlertIcon
  ): Promise<void> {
    const { default: Swal } = await import('sweetalert2');
    const fg = this.cssVar('--color-text', '#111827');

    // Selección semántica por tipo de icono
    let bg = this.cssVar('--color-surface', '#ffffff');
    let iconColor: string | undefined;
    switch (icon) {
      case 'success':
        bg = this.cssVar(
          '--state-success-bg',
          this.cssVar('--color-surface', '#ffffff')
        );
        iconColor = this.cssVar('--state-success', '#16a34a');
        break;
      case 'warning':
        bg = this.cssVar(
          '--state-warning-bg',
          this.cssVar('--color-surface', '#ffffff')
        );
        iconColor = this.cssVar('--state-warning', '#eab308');
        break;
      case 'error':
        bg = this.cssVar(
          '--state-error-bg',
          this.cssVar('--color-surface', '#ffffff')
        );
        iconColor = this.cssVar('--state-error', '#ef4444');
        break;
      case 'info':
        bg = this.cssVar(
          '--state-info-bg',
          this.cssVar('--color-surface', '#ffffff')
        );
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
    let cancelColor = this.cssVar('--gray-400', '#9ca3af');
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
      showConfirmButton: false,
      showCancelButton: false,
      focusConfirm: false,
      color: fg,
      background: bg,
    });
  }

  public async hideLoading(): Promise<void> {
    const { default: Swal } = await import('sweetalert2');
    Swal.close();
  }

  public async success(
    message: string,
    title: string = 'Éxito'
  ): Promise<void> {
    await this.showNotification(title, message, 'success');
  }

  public async error(message: string, title: string = 'Error'): Promise<void> {
    await this.showNotification(title, message, 'error');
  }

  // Extrae un mensaje amigable desde diferentes formas de error (HttpErrorResponse, strings, etc.)
  private extractErrorMessage(err: unknown, fallback?: string): string {
    try {
      // String directo
      if (typeof err === 'string') return err;

      // Error estándar
      if (err instanceof Error)
        return err.message || fallback || 'Ocurrió un error inesperado.';

      // Posible HttpErrorResponse (Angular)
      const anyErr: any = err as any;

      // Network error
      if (anyErr && anyErr.status === 0) {
        return fallback || 'No hay conexión con el servidor. Verifica tu red.';
      }

      // ModelState (errors: { field: [messages] })
      const errors = anyErr?.error?.errors;
      if (errors && typeof errors === 'object') {
        const msgs: string[] = [];
        for (const key of Object.keys(errors)) {
          const val = errors[key];
          if (Array.isArray(val)) msgs.push(...val);
          else if (typeof val === 'string') msgs.push(val);
        }
        if (msgs.length) return msgs.join('\n');
      }

      // Estructuras comunes
      const candidates = [
        anyErr?.error?.detail,
        anyErr?.error?.title,
        anyErr?.error?.message,
        anyErr?.message,
        anyErr?.error,
      ].filter(Boolean);

      if (candidates.length) {
        const first = candidates[0];
        if (typeof first === 'string') return first;
        if (typeof first === 'object') {
          // Si viene un objeto como error, intenta serializar brevemente
          return JSON.stringify(first);
        }
      }

      return fallback || 'Ocurrió un error inesperado.';
    } catch {
      return fallback || 'Ocurrió un error inesperado.';
    }
  }

  // Muestra un toast de error estandarizado a partir de un error/HttpErrorResponse
  public async showApiError(
    err: unknown,
    fallbackMessage?: string,
    title: string = 'Error'
  ): Promise<void> {
    try {
      const anyErr: any = err as any;

      // Si viene desde el interceptor como AppError, no volver a notificar (ya hay toast)
      if (
        anyErr &&
        typeof anyErr === 'object' &&
        'type' in anyErr &&
        'message' in anyErr
      ) {
        return;
      }

      // Si es validación (ModelState) o 404, dejar a los mensajes inline y no toastear
      const isValidation = !!anyErr?.error?.errors;
      const is404 = anyErr?.status === 404;
      if (isValidation || is404) {
        return;
      }

      const msg = this.extractErrorMessage(err, fallbackMessage);
      await this.showNotification(title, msg, 'error');
    } catch {
      const msg = fallbackMessage || 'Ocurrió un error inesperado.';
      await this.showNotification(title, msg, 'error');
    }
  }
}
