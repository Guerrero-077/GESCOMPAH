import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-quick-action',
  imports: [CommonModule],
  templateUrl: './quick-action.component.html',
  styleUrl: './quick-action.component.css'
})
export class QuickActionComponent {
  @Input() actions = [
    { label: 'Nuevo Contrato', style: 'btn-success' },
    { label: 'Nuevo Usuario', style: 'btn-outline-success' },
  ];
}
