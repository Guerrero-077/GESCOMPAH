import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { CardInfoComponent } from '../../../components/card-info/card-info.component';
import { DymanicFormsComponent } from "../../../../../shared/components/dymanic-forms/dymanic-forms.component";

@Component({
  selector: 'app-dashboard-component',
  imports: [CommonModule, CardInfoComponent],
  templateUrl: './dashboard-component.html',
  styleUrl: './dashboard-component.css'
})
export class DashboardComponent {

}
