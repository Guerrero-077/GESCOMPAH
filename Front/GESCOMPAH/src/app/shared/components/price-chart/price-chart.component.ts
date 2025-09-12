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
  @Input() labels: string[] = [];
  @Input() prices: number[] = [];
  @Input() min = 0;
  @Input() max = 0;
  @Input() title = 'Precios de contratos';

  chartData: ChartConfiguration<'line'>['data'] = { labels: [], datasets: [] };
  chartOptions: ChartConfiguration<'line'>['options'] = { responsive: true };

  ngOnChanges(): void {
    this.chartData = {
      labels: this.labels,
      datasets: [
        { label: this.title, data: this.prices, tension: 0.2, fill: false, pointRadius: 3 },
        { label: `Mínimo (${this.min.toLocaleString()})`, data: this.repeat(this.min, this.labels.length), borderDash: [6,6], pointRadius: 0 },
        { label: `Máximo (${this.max.toLocaleString()})`, data: this.repeat(this.max, this.labels.length), borderDash: [6,6], pointRadius: 0 },
      ],
    };
    const same = this.min === this.max;
    const padding = same ? (this.min || 1) * 0.1 : (this.max - this.min) * 0.1;
    this.chartOptions = {
      responsive: true,
      maintainAspectRatio: false,
      scales: {
        x: { position: 'center' },
        y: { position: 'center', suggestedMin: this.min - padding, suggestedMax: this.max + padding }
      },
      plugins: { legend: { display: true }, tooltip: { enabled: true } }
    };
  }

  private repeat(v: number, n: number): number[] {
    return Array.from({ length: Math.max(n, 1) }, () => v);
  }
}
