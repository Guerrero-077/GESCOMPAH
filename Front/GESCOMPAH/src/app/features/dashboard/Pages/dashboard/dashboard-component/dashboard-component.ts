import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { CardInfoComponent } from '../../../components/card-info/card-info.component';
import { SystemAlertComponent } from "../../../components/system-alert/system-alert.component";

@Component({
  selector: 'app-dashboard-component',
  imports: [CommonModule, CardInfoComponent, SystemAlertComponent],
  templateUrl: './dashboard-component.html',
  styleUrl: './dashboard-component.css'
})
export class DashboardComponent {
  
}
