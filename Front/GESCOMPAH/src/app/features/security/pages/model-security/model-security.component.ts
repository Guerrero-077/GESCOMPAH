import { Component, inject, OnInit } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { FormModuleComponent } from "../../components/form-module/form-module.component";
import { RolFormPermissionComponent } from "../../components/rol-form-permission/rol-form-permission.component";
import { PageHeaderService } from '../../../../shared/Services/PageHeader/page-header.service';

@Component({
  selector: 'app-model-security',
  standalone: true,
  imports: [MatTabsModule, FormModuleComponent, RolFormPermissionComponent],
  templateUrl: './model-security.component.html',
  styleUrl: './model-security.component.css'
})
export class ModelSecurityComponent implements OnInit {
  private readonly pageHeaderService = inject(PageHeaderService);

  ngOnInit(): void {
    this.pageHeaderService.setPageHeader('Formularios y Módulos', 'Asignación de formularios a módulos');
  }

  onTabChange(index: number): void {
    if (index === 0) {
      this.pageHeaderService.setPageHeader('Formularios y Módulos', 'Asignación de formularios a módulos');
    } else if (index === 1) {
      this.pageHeaderService.setPageHeader('Roles, Formularios y Permisos', 'Asignación de permisos a roles por formulario');
    }
  }
}

