import { Directive, ElementRef, HostListener, Optional, Self, OnInit, OnDestroy, Input } from '@angular/core';
import { NgControl } from '@angular/forms';
import { Subscription } from 'rxjs';

// Formatea números con separador de miles (es-CO) en inputs.
// Mantiene el FormControl con valor number (o null si vacío).

@Directive({
  selector: '[appThousandSeparator]',
  standalone: true
})
export class ThousandSeparatorDirective implements OnInit, OnDestroy {
  private sub?: Subscription;
  /** Máximo de dígitos (sin contar separadores ni el punto decimal). */
  @Input('appThousandSeparatorMax') maxDigits?: number;
  constructor(private el: ElementRef<HTMLInputElement>, @Self() @Optional() private ngControl?: NgControl) {}

  ngOnInit(): void {
    const ctrl = this.ngControl?.control;
    if (!ctrl) return;
    // Formato inicial si no está enfocado
    if (document.activeElement !== this.el.nativeElement) this.applyDisplayFromValue(ctrl.value);
    // Mantener formateado ante cambios externos
    this.sub = ctrl.valueChanges?.subscribe(v => {
      if (document.activeElement !== this.el.nativeElement) this.applyDisplayFromValue(v);
    });
  }

  ngOnDestroy(): void { this.sub?.unsubscribe(); }

  private unformat(v: string): string {
    if (!v) return '';
    return v.replace(/\./g, '').replace(',', '.');
  }

  private format(n: number): string {
    return n.toLocaleString('es-CO', { maximumFractionDigits: 2 });
  }

  private applyDisplayFromValue(v: any) {
    const input = this.el.nativeElement;
    if (v === null || v === undefined || v === '') { input.value = ''; return; }
    const n = typeof v === 'number' ? v : Number(this.unformat(String(v)));
    if (!Number.isNaN(n)) input.value = this.format(n);
  }

  @HostListener('focus') onFocus() {
    // Mantener SIEMPRE los puntos: no desformatear al enfocar
    const input = this.el.nativeElement;
    const len = input.value.length;
    setTimeout(() => input.setSelectionRange(len, len));
  }

  @HostListener('blur') onBlur() {
    const input = this.el.nativeElement;
    const raw = this.unformat(input.value);
    if (!raw) {
      this.updateControl(null);
      return;
    }
    const n = Number(raw);
    if (!Number.isNaN(n)) {
      input.value = this.format(n);
      this.updateControl(n);
      // Reaplicar tras otros handlers (p.ej., setValue con emitEvent:false)
      setTimeout(() => {
        if (document.activeElement !== input) this.applyDisplayFromValue(this.ngControl?.control?.value);
      });
    }
  }

  @HostListener('input') onInput() {
    const input = this.el.nativeElement;
    // Mantener solo dígitos y separadores
    const cleaned = input.value.replace(/[^\d.,]/g, '');
    let next = cleaned;

    // Limitar máximo de dígitos (sin separadores)
    if (this.maxDigits && this.maxDigits > 0) {
      // separar por decimal (coma o punto, según lo escrito)
      const parts = next.split(/[.,]/);
      const intPart = (parts[0] || '').replace(/\D/g, '');
      const decSep = next.includes('.') ? '.' : (next.includes(',') ? ',' : '');
      const decPart = parts.length > 1 ? (parts[1] || '').replace(/\D/g, '') : '';
      const limitedInt = intPart.slice(0, this.maxDigits);
      next = decSep ? `${limitedInt}${decSep}${decPart}` : limitedInt;
    }
    if (next !== input.value) input.value = next;

    // Convertir a número y reflejarlo con separadores (live-format)
    const raw = this.unformat(input.value);
    if (raw === '' || raw === '.' || raw === ',') {
      this.updateControl(null);
      return;
    }
    const n = Number(raw);
    if (Number.isNaN(n)) return;

    // Actualiza el control y formatea visualmente
    this.updateControl(n);
    const posToEnd = document.activeElement === input;
    input.value = this.format(n);
    if (posToEnd) {
      const len = input.value.length;
      setTimeout(() => input.setSelectionRange(len, len));
    }
  }

  private updateControl(n: number | null) {
    if (!this.ngControl?.control) return;
    this.ngControl.control.setValue(n, { emitEvent: true });
    this.ngControl.control.updateValueAndValidity({ emitEvent: false });
  }
}
