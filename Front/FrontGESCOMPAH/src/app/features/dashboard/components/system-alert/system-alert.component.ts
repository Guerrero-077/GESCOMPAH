import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-system-alert',
  imports: [],
  templateUrl: './system-alert.component.html',
  styleUrl: './system-alert.component.css'
})
export class SystemAlertComponent {
  @Input() title: string = '';
  @Input() message: string = '';
}
