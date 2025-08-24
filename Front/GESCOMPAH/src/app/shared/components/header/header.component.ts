import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-header',
  standalone: true,
  imports:[MatIconModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent {
  @Input() toggleDrawer!: () => void;

  title = 'Gestión de Plazas y Locales';
  description = 'Sistema de Gestión Comercial - GESCOMPAH';
  username = 'Administrador';
}
