import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import {
  IonButtons,
  IonContent,
  IonHeader,
  IonMenu,
  IonMenuButton,
  IonTitle,
  IonToolbar,
  IonButton,
  IonMenuToggle, IonList, IonItem, IonIcon, IonLabel, IonRouterOutlet
} from '@ionic/angular/standalone';


@Component({
  selector: 'app-menu',
  templateUrl: './menu.component.html',
  styleUrls: ['./menu.component.scss'],
  imports: [IonRouterOutlet, IonLabel,
    RouterModule,               // ⬅️ NECESARIO para routerLink
    IonIcon,
    IonItem,
    IonList,
    IonButtons,
    IonContent,
    IonHeader,
    IonMenu,
    IonMenuButton,
    IonTitle,
    IonToolbar,
    IonMenuToggle
  ],
})
export class MenuComponent {
  constructor() {}
}
