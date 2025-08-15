import { Component, EventEmitter, HostBinding, HostListener, Input, Output } from '@angular/core';

@Component({
  selector: 'app-toggle-button-component',
  imports: [],
  templateUrl: './toggle-button-component.component.html',
  styleUrl: './toggle-button-component.component.css'
})
export class ToggleButtonComponent {
  /** Estado actual */
  @Input() checked = false;
  /** Deshabilitado */
  @Input() disabled = false;

  /** Tamaño base en px (alto del switch). Ancho = 1.9 * size */
  @Input() size = 22;

  /** Colores */
  @Input() onColor = '#16a34a';  // verde ON
  @Input() offColor = '#ef4444';  // rojo OFF
  @Input() thumb = '#ffffff';  // color de la perilla

  /** Evento cuando cambia */
  @Output() changed = new EventEmitter<boolean>();

  /** Accesibilidad */
  @HostBinding('attr.role') role = 'switch';
  @HostBinding('attr.aria-checked') get ariaChecked() { return String(this.checked); }
  @HostBinding('attr.tabindex') tabindex = 0;
  @HostBinding('class.tb-disabled') get disabledClass() { return this.disabled; }

  /** CSS vars calculadas por tamaño */
  @HostBinding('style.--tb-h') get h() { return `${this.size}px`; }
  @HostBinding('style.--tb-w') get w() { return `${Math.round(this.size * 1.9)}px`; }
  @HostBinding('style.--tb-d') get d() { return `${this.size - 4}px`; }
  @HostBinding('style.--tb-shift') get shift() { return `${Math.round(this.size * 0.9)}px`; }
  @HostBinding('style.--tb-thumb') get thumbColor() { return this.thumb; }

  @HostListener('click')
  onClick() {
    if (this.disabled) return;
    this.checked = !this.checked;
    this.changed.emit(this.checked);
  }

  @HostListener('keydown', ['$event'])
  onKeydown(e: KeyboardEvent) {
    if (this.disabled) return;
    if (e.key === ' ' || e.key === 'Enter') {
      e.preventDefault();
      this.onClick();
    }
  }
}
