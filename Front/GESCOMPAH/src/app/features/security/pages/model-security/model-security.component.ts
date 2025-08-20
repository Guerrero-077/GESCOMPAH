import { Component } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { FormModuleComponent } from "../../components/form-module/form-module.component";
import { RolFormPermissionComponent } from "../../components/rol-form-permission/rol-form-permission.component";

@Component({
  selector: 'app-model-security',
  imports: [MatTabsModule, FormModuleComponent, RolFormPermissionComponent],
  templateUrl: './model-security.component.html',
  styleUrl: './model-security.component.css'
})
export class ModelSecurityComponent {

}
