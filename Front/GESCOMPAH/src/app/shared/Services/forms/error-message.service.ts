import { Injectable } from '@angular/core';
import { AbstractControl } from '@angular/forms';

type MsgMap = Record<
  string,
  string | ((v: any) => string)
>;

@Injectable({ providedIn: 'root' })
export class ErrorMessageService {
  /** Mensajes estándar + formateadores para errores con payload */
  private readonly map: MsgMap = {
    required: 'Requerido',
    email: 'Correo inválido',

    // Email con TLD
    domainMissing: 'Falta el dominio después de @',
    domainInvalid: 'Dominio inválido',
    tldMissing: 'Falta el TLD (por ej. .com)',
    tldInvalid: 'TLD inválido',

    // Longitudes / patrones
    minlength: (v: any) => `Mínimo ${v?.requiredLength} caracteres`,
    maxlength: (v: any) => `Máximo ${v?.requiredLength} caracteres`,
    pattern: 'Formato inválido',

    // Numéricos
    min: (v: any) => `El mínimo permitido es ${v?.min}`,
    max: (v: any) => `El máximo permitido es ${v?.max}`,
    numberInvalid: 'Número inválido',
    numberNotation: 'Notación no permitida (e/E/±)',
    nan: 'Debe ser un número',
    decimalScale: (v: any) => `Máximo ${v?.max} decimales`,

    // Custom app
    onlySpaces: 'No puede ser solo espacios',
    alphaHuman: 'Solo letras y separadores válidos',
    colombianDocument: 'La cédula debe ser un número de 7 a 10 dígitos.',
    colombianPhone: 'El celular debe ser un número de 10 dígitos que empiece por 3.',
    addressInvalid: 'Dirección inválida',
  };

  /** Devuelve el primer error legible según un orden opcional */
  firstError(control: AbstractControl | null, order: string[] = []): string | null {
    if (!control || !control.errors) return null;

    const keys = order.length ? order : Object.keys(control.errors);
    for (const key of keys) {
      if (control.hasError(key)) {
        const def = this.map[key];
        const payload = (control.errors as any)[key];
        if (typeof def === 'function') return def(payload);
        if (typeof def === 'string') return def;
        // Fallback genérico si no hay mensaje mapeado
        return key;
      }
    }
    // Si no coincide el orden, tomamos el primero que haya
    const firstKey = Object.keys(control.errors)[0];
    if (!firstKey) return null;
    const def = this.map[firstKey];
    const payload = (control.errors as any)[firstKey];
    return typeof def === 'function' ? def(payload) : (def ?? firstKey);
  }
}
