import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-card-feature',
  imports: [],
  templateUrl: './card-feature.component.html',
  styleUrl: './card-feature.component.css'
})
export class CardFeatureComponent {
  @Input() title: string = '';
  @Input() description: string = '';
}
