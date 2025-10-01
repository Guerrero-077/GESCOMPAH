import { Component } from '@angular/core';
import { IonApp, IonRouterOutlet } from '@ionic/angular/standalone';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  imports: [IonApp, IonRouterOutlet],
})
export class AppComponent {
  constructor() {
    document.body.classList.remove('light'); // 👈 fuerza modo light
    document.body.classList.add('dark'); // 👈 asegura light al iniciar
  }


}
