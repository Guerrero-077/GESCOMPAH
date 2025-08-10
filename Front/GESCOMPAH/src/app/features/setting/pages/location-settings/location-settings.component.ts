import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { CityComponent } from '../../components/city/city.component';
import { DepartmentComponent } from '../../components/department/department.component';

@Component({
  selector: 'app-location-settings',
  standalone: true,
  imports: [
    CommonModule,
    MatTabsModule,
    CityComponent,
    DepartmentComponent
  ],
  templateUrl: './location-settings.component.html',
  styleUrl: './location-settings.component.css'
})
export class LocationSettingsComponent {

}
