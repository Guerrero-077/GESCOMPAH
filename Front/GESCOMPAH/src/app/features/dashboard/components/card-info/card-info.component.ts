import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-card-info',
  imports: [CommonModule],
  templateUrl: './card-info.component.html',
  styleUrl: './card-info.component.css'
})
export class CardInfoComponent {
  @Input() title: string = '';
  @Input() value: number = 0;  // valor actual
  @Input() total: number = 0; // total para el cálculo

  get percentage(): number {
    if (this.total > 0) {
      return Math.round((this.value / this.total) * 100);
    }
    return 0;
  }
}
