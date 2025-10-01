import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'money',
  standalone: true,
  pure: true
})
export class MoneyPipe implements PipeTransform {
  transform(value: unknown, withSymbol = false, maxDecimals: 0): string {
    const n = typeof value === 'number' ? value : Number(value);
    if (Number.isNaN(n)) return '';
    try {
      if (withSymbol) {
        return new Intl.NumberFormat('es-CO', {
          style: 'currency',
          currency: 'COP',
          minimumFractionDigits: 0,
          maximumFractionDigits: Math.max(0, Math.min(2, maxDecimals)),
        }).format(n);
      }
      return new Intl.NumberFormat('es-CO', {
        minimumFractionDigits: 0,
        maximumFractionDigits: Math.max(0, Math.min(2, maxDecimals)),
      }).format(n);
    } catch {
      return String(n);
    }
  }
}

