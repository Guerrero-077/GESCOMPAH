import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { LocationSettingsComponent } from '../location-settings/location-settings.component';
import { FinanceComponent } from "../../components/finance/finance.component";

@Component({
  selector: 'app-main-settings',
  standalone: true,
  imports: [
    CommonModule,
    MatTabsModule,
    LocationSettingsComponent,
    FinanceComponent
],
  templateUrl: './main-settings.component.html',
  styleUrl: './main-settings.component.css'
})
export class MainSettingsComponent {

}
