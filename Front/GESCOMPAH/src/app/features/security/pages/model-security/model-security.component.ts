import { Component } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { RoleComponent } from "../../components/role/role.component";
import { FormComponent } from "../../components/form/form.component";
import { ModuleComponent } from "../../components/module/module.component";
import { PermissionComponent } from "../../components/permission/permission.component";
import { UserComponent } from "../../components/user/user.component";
import { PersonComponent } from "../../components/person/person.component";

@Component({
  selector: 'app-model-security',
  imports: [MatTabsModule, RoleComponent, FormComponent, ModuleComponent, PermissionComponent, UserComponent, PersonComponent],
  templateUrl: './model-security.component.html',
  styleUrl: './model-security.component.css'
})
export class ModelSecurityComponent {

}
