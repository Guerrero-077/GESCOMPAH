import { Injectable } from '@angular/core';
import { AbstractControl, ValidationErrors } from '@angular/forms';

export type ErrorMessagesMap = Record<string, string | ((e: any, label?: string) => string)>;

@Injectable({ providedIn: 'root' })
export class FormErrorService {
  private defaultMessages: ErrorMessagesMap = {
    required: (e, label) => `${label ?? 'Este campo'} es obligatorio.`,
    minlength: (e) => `Debe tener al menos ${e?.requiredLength ?? ''} caracteres.`,
    maxlength: (e) => `No puede superar ${e?.requiredLength ?? ''} caracteres.`,
    email: () => 'Ingresa un correo válido (ej. usuario@dominio.com).',
    pattern: () => 'El formato no es válido.',
    min: (e, label) => `${label ?? 'El valor'} no puede ser menor que ${e?.min}.`,
    max: (e, label) => `${label ?? 'El valor'} no puede ser mayor que ${e?.max}.`,
    onlySpaces: (e, label) => `${label ?? 'El valor'} no puede ser solo espacios.`,
    NaN: (e, label) => `${label ?? 'El valor'} debe ser numérico.`,
    numberInvalid: (e, label) => `${label ?? 'El valor'} debe ser numérico.`,
    invalidNotation: () => 'No uses notación científica (e/E) ni signos +/−.',
    decimalScale: () => 'Cantidad de decimales excedida.',
    addressInvalid: () => 'Usa solo letras, números, espacios y # - , .',
    dateRange: () => 'La fecha final debe ser posterior a la inicial.',
    mismatch: () => 'Los valores no coinciden.',
    incorrect: () => 'Valor incorrecto.',
  };

  format(control: AbstractControl | null | undefined, label?: string, overrides?: ErrorMessagesMap): string | null {
    if (!control || !control.errors) return null;
    const errors: ValidationErrors = control.errors;
    const keys = Object.keys(errors);
    if (keys.length === 0) return null;

    const key = keys[0];
    const errVal = (errors as any)[key];
    const messages: ErrorMessagesMap = { ...this.defaultMessages, ...(overrides || {}) };
    const msg = messages[key];
    if (!msg) return 'Valor inválido.';
    return typeof msg === 'function' ? msg(errVal, label) : msg;
  }
}
