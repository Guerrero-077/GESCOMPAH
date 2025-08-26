import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-alert-card',
  imports: [CommonModule],
  templateUrl: './alert-card.component.html',
  styleUrl: './alert-card.component.css'
})
export class AlertCardComponent {
  @Input() title!: string;
  @Input() description!: string;
  @Input() type: 'warning' | 'info' = 'info';
  @Input() text!: string;
}
