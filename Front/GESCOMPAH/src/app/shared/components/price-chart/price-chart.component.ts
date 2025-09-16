// price-chart.component.ts
import { Component, Input, OnChanges } from '@angular/core';
import { BaseChartDirective } from 'ng2-charts';
import {
  Chart, ScatterController, PointElement,
  LinearScale, TimeScale, Tooltip, Legend, ChartConfiguration
} from 'chart.js';
import 'chartjs-adapter-date-fns'; // <-- necesario para eje tiempo
// import { ContractCard } from '../../../features/contracts/models/contract.models';

Chart.register(ScatterController, PointElement, LinearScale, TimeScale, Tooltip, Legend);

@Component({
  selector: 'app-price-chart',
  standalone: true,
  imports: [BaseChartDirective],
  templateUrl: './price-chart.component.html',
  styleUrls: ['./price-chart.component.css']
})
export class PriceChartComponent {
  @Input() title = 'Precios de contratos';
}
