import { Component, Input, OnChanges } from '@angular/core';
import { BaseChartDirective } from 'ng2-charts';
import {
  Chart, LineController, LineElement, PointElement, LinearScale, CategoryScale, Tooltip, Legend,
  ChartConfiguration
} from 'chart.js';

Chart.register(LineController, LineElement, PointElement, LinearScale, CategoryScale, Tooltip, Legend);


@Component({
  selector: 'app-price-chart',
  imports: [BaseChartDirective],
  templateUrl: './price-chart.component.html',
  styleUrl: './price-chart.component.css'
})
export class PriceChartComponent implements OnChanges {
  @Input() labels: string[] = [];       // nombres o ids de los contratos
  @Input() prices: number[] = [];       // totalBase de cada contrato
  @Input() activeFlags: boolean[] = []; // array con true/false según contrato activo
  @Input() title = 'Precios de contratos';

  chartData: ChartConfiguration<'bar'>['data'] = { labels: [], datasets: [] };
  chartOptions: ChartConfiguration<'bar'>['options'] = { responsive: true };

  ngOnChanges(): void {
    // Dataset único, con color dependiendo si el contrato está activo o no
    const backgroundColors = this.activeFlags.map(active => active ? 'rgba(34,197,94,0.7)' : 'rgba(209,213,219,0.7)');
    const borderColors = this.activeFlags.map(active => active ? 'rgba(22,101,52,1)' : 'rgba(156,163,175,1)');

    this.chartData = {
      labels: this.labels,
      datasets: [
        {
          label: this.title,
          data: this.prices,
          backgroundColor: backgroundColors,
          borderColor: borderColors,
          borderWidth: 1
        }
      ]
    };

    this.chartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      scales: {
        x: {
          title: { display: true, text: 'Contrato' }
        },
        y: {
          beginAtZero: true,
          title: { display: true, text: 'Valor Base' }
        }
      },
      plugins: {
        legend: { display: true },
        tooltip: { enabled: true }
      }
    };
  }
}
