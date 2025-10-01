import { Directive, ElementRef, HostListener, Renderer2, forwardRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

const MAX_DOCUMENT_DIGITS = 10;

/**
 * Formatea documentos colombianos en el input mostrando separadores de miles con puntos
 * mientras mantiene solo dígitos en el control reactivo.
 */
@Directive({
  selector: '[appDocumentFormat]',
  standalone: true,
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => DocumentFormatDirective),
    multi: true,
  }],
})
export class DocumentFormatDirective implements ControlValueAccessor {
  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};

  constructor(
    private readonly el: ElementRef<HTMLInputElement>,
    private readonly renderer: Renderer2,
  ) {}

  writeValue(value: string | null): void {
    const digits = this.strip(value);
    this.renderer.setProperty(this.el.nativeElement, 'value', this.format(digits));
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.renderer.setProperty(this.el.nativeElement, 'disabled', isDisabled);
  }

  @HostListener('input', ['$event'])
  handleInput(event: Event): void {
    const target = event.target instanceof HTMLInputElement ? event.target : null;
    if (!target) return;

    const digits = this.strip(target.value);
    this.renderer.setProperty(this.el.nativeElement, 'value', this.format(digits));
    this.onChange(digits);
  }

  @HostListener('blur')
  handleBlur(): void {
    const digits = this.strip(this.el.nativeElement.value);
    this.renderer.setProperty(this.el.nativeElement, 'value', this.format(digits));
    this.onChange(digits);
    this.onTouched();
  }

  private strip(value: unknown): string {
    return String(value ?? '')
      .replace(/\D+/g, '')
      .slice(0, MAX_DOCUMENT_DIGITS);
  }

  private format(digits: string): string {
    return digits.replace(/\B(?=(\d{3})+(?!\d))/g, '.');
  }
}

