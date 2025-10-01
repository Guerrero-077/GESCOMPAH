import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal, computed} from '@angular/core';
import { CardInfoComponent } from '../../../components/card-info/card-info.component';
import { QuickActionComponent } from '../../../components/quick-action/quick-action.component';
import { SystemAlertComponent } from "../../../components/system-alert/system-alert.component";
import { PageHeaderService } from '../../../../../shared/Services/PageHeader/page-header.service';
import { BaseChartDirective } from 'ng2-charts';
import { Chart, DoughnutController, ArcElement, Tooltip, Legend, ChartOptions} from 'chart.js';
import { CircleChartComponent } from "../../../../../shared/components/circle-chart/circle-chart.component";
import { LineChartComponent } from "../../../../../shared/components/line-chart/line-chart.component";
import { EstablishmentService } from '../../../../establishments/services/establishment/establishment.service';
import { EstablishmentSelect } from '../../../../establishments/models/establishment.models';
import { take } from 'rxjs/operators';

import { ContractService } from '../../../../contracts/services/contract/contract.service';
import { ContractCard } from '../../../../contracts/models/contract.models';
import { PriceChartComponent } from "../../../../../shared/components/price-chart/price-chart.component";

Chart.register(DoughnutController, ArcElement, Tooltip, Legend);

@Component({
  selector: 'app-dashboard-component',
  standalone: true,
  imports: [CommonModule, CardInfoComponent, SystemAlertComponent, QuickActionComponent, BaseChartDirective, CircleChartComponent, LineChartComponent, PriceChartComponent],
  templateUrl: './dashboard-component.html',
  styleUrl: './dashboard-component.css'
})
export class DashboardComponent implements OnInit {

  private readonly pageHeaderService = inject(PageHeaderService);
  private readonly establishmentService = inject(EstablishmentService);
  private readonly contractService = inject(ContractService);

  readonly establishments = signal<readonly EstablishmentSelect[]>([]);
  readonly contract = signal<readonly ContractCard[]>([]);

  readonly loading = signal<boolean>(false);
  readonly error = signal<string | null>(null);

  // Derivados como en Establishment-list
  readonly activeEstablishment = computed(() =>
    this.establishments().filter(e => e.active).length
  );

  readonly inactiveEstablishment  = computed(() =>
    this.establishments().filter(e => !e.active).length
  );

  // Derivados como en Establishment-list
  readonly activeContract = computed(() =>
    this.contract().filter(e => e.active).length
  );
  readonly inactiveContract  = computed(() =>
    this.contract().filter(e => !e.active).length
  );


  ngOnInit(): void {
    this.pageHeaderService.setPageHeader('Inicio', 'Página Principal - GESCOMPH');
    this.loadEstablishments();
    this.loadContract();
  }

  private loadEstablishments(): void {
    this.loading.set(true);
    this.error.set(null);

    this.establishmentService.getAll().pipe(take(1)).subscribe({
      next: (list) => this.establishments.set(list),
      error: (err) => this.error.set(err?.message || 'Error al cargar establecimientos'),
      complete: () => this.loading.set(false),
    });
  }

  private loadContract(): void {
    this.loading.set(true);
    this.error.set(null);

    this.contractService.getAll().pipe(take(1)).subscribe({
      next: (list) => this.contract.set(list),
      error: (err) => this.error.set(err?.message || 'Error al cargar establecimientos'),
      complete: () => this.loading.set(false),
    });
  }

}

