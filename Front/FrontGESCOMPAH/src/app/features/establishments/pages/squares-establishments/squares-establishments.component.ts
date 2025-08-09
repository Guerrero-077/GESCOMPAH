import { Component } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { SquareListComponent } from "../../components/square-list/square-list.component";
import { EstablishmentsListComponent } from "../../components/establishments-list/establishments-list.component";

@Component({
  selector: 'app-squares-establishments',
  imports: [MatTabsModule, SquareListComponent, EstablishmentsListComponent],
  templateUrl: './squares-establishments.component.html',
  styleUrl: './squares-establishments.component.css'
})
export class SquaresEstablishmentsComponent {

}
