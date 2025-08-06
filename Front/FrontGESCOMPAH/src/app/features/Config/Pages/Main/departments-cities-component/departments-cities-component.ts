import { Component } from '@angular/core';
import { MatTabGroup, MatTabsModule } from "@angular/material/tabs";
import { MatTab } from "../../../../../../../node_modules/@angular/material/tabs/index";
import { CityComponent } from "../../City/city-component/city-component";
import { DepartmentComponent } from "../../Department/department-component/department-component";

@Component({
  selector: 'app-departments-cities-component',
  imports: [MatTabsModule, CityComponent, DepartmentComponent],
  templateUrl: './departments-cities-component.html',
  styleUrl: './departments-cities-component.css'
})
export class DepartmentsCitiesComponent {

}
