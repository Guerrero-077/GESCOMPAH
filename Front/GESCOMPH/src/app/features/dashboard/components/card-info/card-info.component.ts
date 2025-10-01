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
  @Input() value: number = 0;

}
