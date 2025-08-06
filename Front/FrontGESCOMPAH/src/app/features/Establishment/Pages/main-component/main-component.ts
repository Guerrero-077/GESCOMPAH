import { Component } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { ListEstablishmentComponent } from "../list-establishment-component/list-establishment-component";

@Component({
  selector: 'app-main-component',
  imports: [MatTabsModule, ListEstablishmentComponent],
  templateUrl: './main-component.html',
  styleUrl: './main-component.css'
})
export class MainComponent {

}
