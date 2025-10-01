import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-quick-action',
  imports: [CommonModule, RouterLink],
  templateUrl: './quick-action.component.html',
  styleUrl: './quick-action.component.css'
})
export class QuickActionComponent {
  @Input() actions = [
    { label: 'Nuevo Contrato', style: 'btn-outline-success', RouterLink: '../contracts'},
    { label: 'Nuevo Establecimiento', style: 'btn-outline-success', RouterLink: '../establishments/main'},
    { label: 'Nueva Plaza', style: 'btn-outline-success', RouterLink: '../establishments/main'},
    { label: 'Nuevo Usuario', style: 'btn-outline-success', RouterLink: '../tenants'},
    { label: 'Gestionar Citas', style: 'btn-outline-success', RouterLink: '../appointment'},
  ];
}
