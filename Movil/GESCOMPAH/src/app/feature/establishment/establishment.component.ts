// import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { CardComponent } from "src/app/shared/card/card.component";
import { IonGrid, IonRow, IonCol, IonContent } from "@ionic/angular/standalone";

@Component({
  selector: 'app-establishment',
  templateUrl: './establishment.component.html',
  styleUrls: ['./establishment.component.scss'],
  imports: [CardComponent, IonGrid, IonRow, IonCol, IonContent],
})
export class EstablishmentComponent  implements OnInit {

  constructor() { }

  ngOnInit() {}

}
