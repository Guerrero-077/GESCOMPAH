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
    this.pageHeaderService.setPageHeader('Inicio', 'Página Principal - GESCOMPAH');
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

  contratos: ContractCard[] = [
    { id: 1, personId: 1, personFullName: 'Ana Pérez', personDocument: '123', personPhone: '300000001', personEmail: 'ana@correo.com', startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 1200, totalUvt: 0, active: true },
    { id: 2, personId: 2, personFullName: 'Luis Gómez', personDocument: '456', personPhone: '300000002', personEmail: 'luis@correo.com', startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 900, totalUvt: 0, active: true },
    { id: 3, personId: 3, personFullName: 'Marta Díaz', personDocument: '789', personPhone: '300000003', personEmail: null, startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 1500, totalUvt: 0, active: true },
    { id: 4, personId: 4, personFullName: 'Carlos Ruiz', personDocument: '101', personPhone: '300000004', personEmail: 'carlos@correo.com', startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 1800, totalUvt: 0, active: true },
    { id: 5, personId: 5, personFullName: 'Laura Torres', personDocument: '102', personPhone: '300000005', personEmail: 'laura@correo.com', startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 1000, totalUvt: 0, active: true },
    { id: 6, personId: 6, personFullName: 'Diego López', personDocument: '103', personPhone: '300000006', personEmail: null, startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 1350, totalUvt: 0, active: true },
    { id: 7, personId: 7, personFullName: 'Sofía Moreno', personDocument: '104', personPhone: '300000007', personEmail: 'sofia@correo.com', startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 950, totalUvt: 0, active: true },
    { id: 8, personId: 8, personFullName: 'Jorge Castillo', personDocument: '105', personPhone: '300000008', personEmail: 'jorge@correo.com', startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 1700, totalUvt: 0, active: true },
    { id: 9, personId: 9, personFullName: 'Isabel Romero', personDocument: '106', personPhone: '300000009', personEmail: null, startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 1100, totalUvt: 0, active: true },
    { id: 10, personId: 10, personFullName: 'Ricardo Mendoza', personDocument: '107', personPhone: '300000010', personEmail: 'ricardo@correo.com', startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 1450, totalUvt: 0, active: true },
    { id: 11, personId: 11, personFullName: 'Julia Naranjo', personDocument: '108', personPhone: '300000011', personEmail: 'julia@correo.com', startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 1050, totalUvt: 0, active: true },
    { id: 12, personId: 12, personFullName: 'Emilio Patiño', personDocument: '109', personPhone: '300000012', personEmail: null, startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 1300, totalUvt: 0, active: true },
    { id: 13, personId: 13, personFullName: 'Lucía Salazar', personDocument: '110', personPhone: '300000013', personEmail: 'lucia@correo.com', startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 1600, totalUvt: 0, active: true },
    { id: 14, personId: 14, personFullName: 'Tomás Herrera', personDocument: '111', personPhone: '300000014', personEmail: 'tomas@correo.com', startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 1250, totalUvt: 0, active: true },
    { id: 15, personId: 15, personFullName: 'Patricia León', personDocument: '112', personPhone: '300000015', personEmail: 'patricia@correo.com', startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 1400, totalUvt: 0, active: true },
    { id: 16, personId: 16, personFullName: 'Álvaro Cortés', personDocument: '113', personPhone: '300000016', personEmail: null, startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 950, totalUvt: 0, active: true },
    { id: 17, personId: 17, personFullName: 'Verónica Silva', personDocument: '114', personPhone: '300000017', personEmail: 'veronica@correo.com', startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 1600, totalUvt: 0, active: true },
    { id: 18, personId: 18, personFullName: 'Esteban Ríos', personDocument: '115', personPhone: '300000018', personEmail: 'esteban@correo.com', startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 1750, totalUvt: 0, active: true },
    { id: 19, personId: 19, personFullName: 'Natalia Prieto', personDocument: '116', personPhone: '300000019', personEmail: 'natalia@correo.com', startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 1400, totalUvt: 0, active: true },
    { id: 20, personId: 20, personFullName: 'Andrés Fernández', personDocument: '117', personPhone: '300000020', personEmail: 'andres@correo.com', startDate: '2025-01-01', endDate: '2025-12-31', totalBase: 1000, totalUvt: 0, active: true },
  ];

  labels = this.contratos.map(c => c.personFullName);
  prices = this.contratos.map(c => c.totalBase);
  min = Math.min(...this.prices);
  max = Math.max(...this.prices);

}

