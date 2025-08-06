import { Component } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { RolComponent } from "../../Rol/rol-component/rol-component";
import { FormComponent } from "../../Form/form-component/form-component";
import { ModuleComponenet } from "../../Module/module-componenet/module-componenet";
import { PermissionComponent } from "../../Permission/permission-component/permission-component";

@Component({
  selector: 'app-main-sistem-segurity',
  imports: [MatTabsModule, RolComponent, FormComponent, ModuleComponenet, PermissionComponent],
  templateUrl: './main-sistem-segurity.html',
  styleUrl: './main-sistem-segurity.css'
})
export class MainSistemSegurity {

}
