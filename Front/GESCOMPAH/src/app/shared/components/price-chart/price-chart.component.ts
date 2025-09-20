// price-chart.component.ts
import { Component, Input } from '@angular/core';
import {
  Chart,
  Legend,
  LinearScale,
  PointElement,
  ScatterController,
  TimeScale, Tooltip
} from 'chart.js';
import 'chartjs-adapter-date-fns';

Chart.register(ScatterController, PointElement, LinearScale, TimeScale, Tooltip, Legend);

@Component({
  selector: 'app-price-chart',
  standalone: true,
  imports: [],
  templateUrl: './price-chart.component.html',
  styleUrls: ['./price-chart.component.css']
})
export class PriceChartComponent {
  @Input() title = 'Precios de contratos';
}
