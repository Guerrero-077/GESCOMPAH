import { Component, inject, OnInit } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { SquareListComponent } from "../../components/square-list/square-list.component";
import { EstablishmentsListComponent } from "../../components/establishments-list/establishments-list.component";
import { PageHeaderService } from '../../../../shared/Services/PageHeader/page-header.service';
import { HasRoleAndPermissionDirective } from '../../../../core/security/directives/HasRoleAndPermission.directive';

@Component({
  selector: 'app-squares-establishments',
  standalone: true,
  imports: [MatTabsModule, SquareListComponent, EstablishmentsListComponent, HasRoleAndPermissionDirective],
  templateUrl: './squares-establishments.component.html',
  styleUrl: './squares-establishments.component.css'
})
export class SquaresEstablishmentsComponent implements OnInit {
  private readonly pageHeaderService = inject(PageHeaderService);

  ngOnInit(): void {
    this.pageHeaderService.setPageHeader('Plazas', 'Gestión de Plazas');
  }

  onTabChange(index: number): void {
    if (index === 0) {
      this.pageHeaderService.setPageHeader('Plazas', 'Gestión de Plazas');
    } else if (index === 1) {
      this.pageHeaderService.setPageHeader('Establecimientos', 'Gestión de Establecimientos');
    }
  }
}

