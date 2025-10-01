import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { ChartConfiguration, ChartOptions } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';

@Component({
  selector: 'app-circle-chart',
  standalone: true,
  imports: [CommonModule, BaseChartDirective],
  templateUrl: './circle-chart.component.html',
  styleUrls: ['./circle-chart.component.css']
})
export class CircleChartComponent {
  @Input() title: string = 'Título por defecto';

  @Input() data: number[] = [];
  @Input() labels: string[] = [];
  @Input() colors: string[] = ['#10b981', '#f87171']; // Verde y rojo por defecto

  public chartData: ChartConfiguration<'doughnut'>['data'] = {
    labels: [],
    datasets: []
  };

  public chartOptions: ChartOptions<'doughnut'> = {
    responsive: true,
    cutout: '70%',
    plugins: {
      legend: {
        position: 'bottom'
      }
    }
  };

  ngOnChanges() {
    this.updateChart();
  }

  ngOnInit() {
    this.updateChart();
  }

  private updateChart() {
    this.chartData = {
      labels: this.labels,
      datasets: [
        {
          data: this.data,
          backgroundColor: this.colors,
          borderWidth: 1
        }
      ]
    };
  }
}
