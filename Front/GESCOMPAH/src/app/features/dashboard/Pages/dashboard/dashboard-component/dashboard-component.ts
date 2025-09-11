import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { CardInfoComponent } from '../../../components/card-info/card-info.component';
import { QuickActionComponent } from '../../../components/quick-action/quick-action.component';
import { SystemAlertComponent } from "../../../components/system-alert/system-alert.component";
import { PageHeaderService } from '../../../../../shared/Services/PageHeader/page-header.service';
import { BaseChartDirective } from 'ng2-charts';
import { Chart, DoughnutController, ArcElement, Tooltip, Legend, ChartOptions} from 'chart.js';
import { CircleChartComponent } from "../../../../../shared/components/circle-chart/circle-chart.component";
import { LineChartComponent } from "../../../../../shared/components/line-chart/line-chart.component";

Chart.register(DoughnutController, ArcElement, Tooltip, Legend);

@Component({
  selector: 'app-dashboard-component',
  standalone: true,
  imports: [CommonModule, CardInfoComponent, SystemAlertComponent, QuickActionComponent, BaseChartDirective, CircleChartComponent, LineChartComponent],
  templateUrl: './dashboard-component.html',
  styleUrl: './dashboard-component.css'
})
export class DashboardComponent implements OnInit {

  private readonly pageHeaderService = inject(PageHeaderService);

  ngOnInit(): void {
    this.pageHeaderService.setPageHeader('Inicio', 'Página Principal - GESCOMPAH');
  }

  chartType: 'doughnut' = 'doughnut';

  chartData = {
    labels: ['Ocupado', 'Disponible', 'Inhabilitado'],
    datasets: [{
      data: [10, 25, 5],  // Ejemplo: 10 ocupados, 25 disponibles, 5 inhabilitados
      backgroundColor: [
        '#22c55e',  // verde vibrante (ocupado)
        '#fbbf24',  // amarillo vibrante (disponible)
        '#dc2626'   // rojo (inhabilitado)
      ],
      hoverOffset: 10
    }]
  };

  chartOptions: ChartOptions<'doughnut'> = {
    responsive: true,
    plugins: {
      legend: {
        position: 'bottom',
        labels: {
          usePointStyle: true,
          pointStyle: 'circle',
        }
      }
    }
  };
}

