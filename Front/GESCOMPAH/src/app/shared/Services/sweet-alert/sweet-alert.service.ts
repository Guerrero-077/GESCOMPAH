
import { Injectable } from '@angular/core';
import Swal, { SweetAlertIcon } from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class SweetAlertService {

  constructor() { }

  public showNotification(title: string, text: string, icon: SweetAlertIcon): void {
    Swal.fire({
      title,
      text,
      icon,
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
    cancelButtonText = 'Cancelar'
  ): Promise<any> {
    return Swal.fire({
      title,
      text,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText,
      cancelButtonText,
    });
  }

  public showLoading(title: string, text: string): void {
    Swal.fire({
      title,
      text,
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
}
