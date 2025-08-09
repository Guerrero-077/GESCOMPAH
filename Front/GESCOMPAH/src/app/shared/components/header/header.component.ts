import { Component } from '@angular/core';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-header',
  standalone: true,
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent {

  title: string = 'Gestión de Plazas y Locales';
  description: string = 'Sistema de Gestión Comercial - GESCOMPAH';
  username: string = 'Administrador';

  // constructor(private authService: AuthService) {
  //   this.authService.GetMe().subscribe((user: User) => {
  //     this.username = user.fullName;
  //   });
  // }
}
