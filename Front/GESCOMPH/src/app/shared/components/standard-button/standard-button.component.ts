import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

export type ButtonVariant = 'raised' | 'stroked' | 'flat' | 'icon' | 'fab' | 'mini-fab' | 'cancel';
export type ButtonColor = 'primary' | 'accent' | 'warn' | 'yellow' | '';
export type ButtonSize = 'small' | 'medium' | 'large';

@Component({
  selector: 'app-standard-button',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './standard-button.component.html',
  styleUrl: './standard-button.component.css'
})
export class StandardButtonComponent {
  @Input() text: string = '';
  @Input() variant: ButtonVariant = 'raised';
  @Input() color: ButtonColor = 'primary';
  @Input() size: ButtonSize = 'medium';
  @Input() icon: string = '';
  @Input() iconPosition: 'left' | 'right' = 'left';
  @Input() disabled: boolean = false;
  @Input() loading: boolean = false;
  @Input() type: 'button' | 'submit' | 'reset' = 'button';
  @Input() fullWidth: boolean = false;
  @Input() ariaLabel: string = '';
  @Input() title: string = '';
  @Input() class: string = '';

  @Output() clicked = new EventEmitter<void>();

  onClick(): void {
    if (!this.disabled && !this.loading) {
      this.clicked.emit();
    }
  }

  get buttonClasses(): string {
    const classes = ['standard-button'];

    if (this.variant) classes.push(`variant-${this.variant}`);
    if (this.color) classes.push(`color-${this.color}`);
    if (this.size) classes.push(`size-${this.size}`);
    if (this.fullWidth) classes.push('full-width');
    if (this.class) classes.push(this.class);

    return classes.join(' ');
  }

  get displayText(): string {
    return this.text;
  }

  get effectiveAriaLabel(): string {
    return this.ariaLabel || this.title || this.text;
  }
}
