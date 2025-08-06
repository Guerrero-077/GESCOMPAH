import { Component } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { DepartmentsCitiesComponent } from "../departments-cities-component/departments-cities-component";

@Component({
  selector: 'app-main-config',
  imports: [MatTabsModule, DepartmentsCitiesComponent],
  templateUrl: './main-config.html',
  styleUrl: './main-config.css'
})
export class MainConfig {

}
