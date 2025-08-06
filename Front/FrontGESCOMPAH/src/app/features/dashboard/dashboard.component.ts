import { Component } from '@angular/core';
import { LocalCard } from '../../shared/components/card/local-card/local-card';
import { CardInfoComponent } from "./components/card-info/card-info.component";

@Component({
  selector: 'app-dashboard',
  imports: [LocalCard, CardInfoComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {

}
