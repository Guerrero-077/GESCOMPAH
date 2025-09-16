import {
  Component, EventEmitter, HostBinding, HostListener,
  Input, Output, ChangeDetectionStrategy, ChangeDetectorRef
} from '@angular/core';

@Component({
  selector: 'app-toggle-button-component',
  templateUrl: './toggle-button-component.component.html',
  styleUrls: ['./toggle-button-component.component.css'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ToggleButtonComponent {
  private _checked = false;

  @Input()
  get checked(): boolean { return this._checked; }
  set checked(val: boolean) {
    this._checked = !!val;
    // 👇 fuerza CD del componente (más robusto que markForCheck aquí)
    this.cdr.detectChanges();
  }

  @Input() disabled = false;
  @Input() size = 22;
  @Input() onColor = '#388E3C';
  @Input() offColor = '#9CA3AF';
  @Input() thumb   = '#ffffff';

  @Output() changed = new EventEmitter<{ checked: boolean } | boolean>();

  constructor(private cdr: ChangeDetectorRef) {}

  @HostBinding('attr.role') role = 'switch';
  @HostBinding('attr.aria-checked') get ariaChecked() { return String(this.checked); }
  @HostBinding('attr.tabindex') tabindex = 0;

  @HostBinding('class.tb-disabled') get disabledClass() { return this.disabled; }
  @HostBinding('style.--tb-h')     get h() { return `${this.size}px`; }
  @HostBinding('style.--tb-w')     get w() { return `${Math.round(this.size * 1.9)}px`; }
  @HostBinding('style.--tb-d')     get d() { return `${this.size - 4}px`; }
  @HostBinding('style.--tb-shift') get shift() { return `${Math.round(this.size * 0.9)}px`; }
  @HostBinding('style.--tb-thumb') get thumbColor() { return this.thumb; }

  @HostListener('click')
  onClick() {
    if (this.disabled) return;
    // ⚠️ no tocar el @Input; solo interno y emitir intención
    this.changed.emit({ checked: !this._checked });
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
