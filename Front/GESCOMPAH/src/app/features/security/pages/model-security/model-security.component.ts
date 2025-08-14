import { Component } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { FormModuleComponent } from "../../components/form-module/form-module.component";
import { PersonComponent } from "../../components/person/person.component";
import { RolFormPermissionComponent } from "../../components/rol-form-permission/rol-form-permission.component";
import { RolUserComponent } from "../../components/rol-user/rol-user.component";
import { UserComponent } from "../../components/user/user.component";

@Component({
  selector: 'app-model-security',
  imports: [MatTabsModule, UserComponent, PersonComponent, RolUserComponent, FormModuleComponent, RolFormPermissionComponent],
  templateUrl: './model-security.component.html',
  styleUrl: './model-security.component.css'
})
export class ModelSecurityComponent {

}
