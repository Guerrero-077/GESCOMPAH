import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { AlertCardComponent } from '../alert-card/alert-card.component';
import { IAlert } from '../../services/alert';

@Component({
  selector: 'app-system-alert',
  imports: [CommonModule, AlertCardComponent],
  templateUrl: './system-alert.component.html',
  styleUrl: './system-alert.component.css'
})
export class SystemAlertComponent {
  // 🔹 En el futuro puedes recibirlo por Input:
  // @Input() alerta!: IAlerta;

  // 🔹 Por ahora usamos quemado:
  alertas: IAlert [] = [
    {
      title: 'Citas Pendientes',
      description: '1 solicitudes por revisar',
      type: 'warning',
      text: 'Revisar'
    },
    {
      title: 'Locales Disponibles',
      description: '2 locales sin arrendar',
      type: 'info',
      text: 'Gestionar'
    }
  ];
}
