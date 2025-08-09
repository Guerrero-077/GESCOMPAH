import { Component } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { RoleComponent } from "../../components/role/role.component";
import { FormComponent } from "../../components/form/form.component";
import { ModuleComponent } from "../../components/module/module.component";
import { PermissionComponent } from "../../components/permission/permission.component";

@Component({
  selector: 'app-model-security',
  imports: [MatTabsModule, RoleComponent, FormComponent, ModuleComponent, PermissionComponent],
  templateUrl: './model-security.component.html',
  styleUrl: './model-security.component.css'
})
export class ModelSecurityComponent {

}
