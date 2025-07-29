import { Component } from '@angular/core';
import { LocalCard } from '../../shared/components/card/local-card/local-card';

@Component({
  selector: 'app-dashboard',
  imports: [LocalCard],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {

}
