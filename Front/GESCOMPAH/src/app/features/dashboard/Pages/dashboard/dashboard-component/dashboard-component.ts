import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { CardInfoComponent } from '../../../components/card-info/card-info.component';
import { QuickActionComponent } from '../../../components/quick-action/quick-action.component';
import { SystemAlertComponent } from "../../../components/system-alert/system-alert.component";
import { PageHeaderService } from '../../../../../shared/Services/PageHeader/page-header.service';

@Component({
  selector: 'app-dashboard-component',
  standalone: true,
  imports: [CommonModule, CardInfoComponent, SystemAlertComponent, QuickActionComponent],
  templateUrl: './dashboard-component.html',
  styleUrl: './dashboard-component.css'
})
export class DashboardComponent implements OnInit {
  private readonly pageHeaderService = inject(PageHeaderService);

  ngOnInit(): void {
    this.pageHeaderService.setPageHeader('Inicio', 'Página Principal - GESCOMPAH');
  }
}

