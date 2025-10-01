import { CommonModule } from '@angular/common';
import { Component, Input, computed, signal } from '@angular/core';
import { AbstractControl, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormErrorService, ErrorMessagesMap } from './form-error.service';

@Component({
  selector: 'app-form-error',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule],
  template: `
    <ng-container *ngIf="shouldShow() && message() as msg">
      <mat-error *ngIf="variant === 'mat'">{{ msg }}</mat-error>
      <div *ngIf="variant === 'plain'" class="field-error">{{ msg }}</div>
    </ng-container>
  `,
  styles: [
    `.field-error{color:#b91c1c;font-size:12px;line-height:1.25;margin:4px 0 0 0;}`
  ]
})
export class FormErrorComponent {
  constructor(private formatter: FormErrorService) {}

  @Input() control: AbstractControl | null | undefined;
  @Input() label?: string;
  @Input() showWhen: 'touched' | 'dirty' | 'always' = 'touched';
  @Input() messages?: ErrorMessagesMap;
  @Input() variant: 'mat' | 'plain' = 'mat';

  message = computed(() => this.formatter.format(this.control ?? null, this.label, this.messages));

  shouldShow = () => {
    if (!this.control) return false;
    if (this.showWhen === 'always') return this.control.invalid;
    if (this.showWhen === 'dirty') return this.control.dirty && this.control.invalid;
    return this.control.touched && this.control.invalid; // touched por defecto
  };
}
