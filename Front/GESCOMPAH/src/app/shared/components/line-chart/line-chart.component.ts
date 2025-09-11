import { Component, Input } from '@angular/core';
import {
  Chart,
  LineController,
  LineElement,
  PointElement,
  LinearScale,  
  CategoryScale,
  Tooltip,
  Legend,
  Title,
  Filler,
  ChartConfiguration
} from 'chart.js';
import { CommonModule } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';

Chart.register(
  LineController,
  LineElement,
  PointElement,
  LinearScale,
  CategoryScale,
  Tooltip,
  Legend,
  Title,
  Filler // para el área bajo la línea
);

@Component({
  selector: 'app-line-chart',
  imports: [CommonModule, BaseChartDirective],
  templateUrl: './line-chart.component.html',
  styleUrl: './line-chart.component.css'
})
export class LineChartComponent {
  @Input() title: string = 'Gráfico de Líneas';
  @Input() labels: string[] = [];
  @Input() datasets: ChartConfiguration<'line'>['data']['datasets'] = [];

  public lineChartOptions: ChartConfiguration<'line'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    elements: {
      line: {
        tension: 0.3
      }
    },
    scales: {
      x: {},
      y: {}
    },
    plugins: {
      legend: {
        display: true,
        position: 'bottom'
      }
    }
  };

  public lineChartType: ChartConfiguration<'line'>['type'] = 'line';
}