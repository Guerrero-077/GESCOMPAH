import { Injectable } from '@angular/core';
import {
  AbstractControl,
  ValidationErrors,
  ValidatorFn,
  Validators,
  FormControl
} from '@angular/forms';

@Injectable({ providedIn: 'root' })
export class FormUtilsService {
  /**
   * Colapsa espacios múltiples a uno y hace trim.
   * No emite evento para evitar ciclos de validación innecesarios.
   */
  normalizeWhitespace(control: AbstractControl | null): void {
    if (!control) return;
    const raw = control.value;
    if (raw == null) return;
    const s = String(raw);
    const normalized = s.trim().replace(/\s+/g, ' ');
    if (normalized !== s) control.setValue(normalized, { emitEvent: false });
  }

  /**
   * Normaliza un email y garantiza TLD básico:
   * - lower case
   * - si hay dominio sin punto -> añade ".com"
   * - si falta dominio después de @ -> mantiene "local@" para que el validador marque error
   */
  coerceEmailTld(control: AbstractControl | null): void {
    if (!control) return;
    let v = String(control.value ?? '').trim().toLowerCase();
    if (!v) return;

    const at = v.indexOf('@');
    if (at < 0) {
      control.setValue(v, { emitEvent: false });
      return;
    }

    const local = v.slice(0, at);
    const domain = v.slice(at + 1);

    if (!domain) {
      control.setValue(`${local}@`, { emitEvent: false });
      return;
    }
    if (!domain.includes('.')) {
      control.setValue(`${local}@${domain}.com`, { emitEvent: false });
      return;
    }
    control.setValue(`${local}@${domain}`, { emitEvent: false });
  }

  /**
   * Split de nombre completo (es-ES/LA) en firstName/lastName.
   * Intenta respetar partículas comunes de apellidos compuestos.
   */
  splitNameEs(full: string): { firstName: string; lastName: string } {
    const clean = String(full ?? '').trim().replace(/\s+/g, ' ');
    if (!clean) return { firstName: '', lastName: '' };

    const parts = clean.split(' ');
    if (parts.length === 1) return { firstName: parts[0], lastName: '' };
    if (parts.length === 2) return { firstName: parts[0], lastName: parts[1] };

    const particles = new Set(['de', 'del', 'la', 'las', 'los', 'san', 'santa', 'y', "d'", 'da', 'do', 'dos']);
    const surnames: string[] = [];
    let taken = 0;

    for (let i = parts.length - 1; i >= 0; i--) {
      const w = parts[i];
      surnames.unshift(w);
      taken++;
      const prev = parts[i - 1]?.toLowerCase();
      if (taken >= 2 && (!prev || !particles.has(prev))) {
        const firsts = parts.slice(0, i).join(' ').trim();
        return { firstName: firsts, lastName: surnames.join(' ').trim() };
      }
    }
    return { firstName: parts[0], lastName: parts.slice(1).join(' ') };
  }
}

/* ============================
 *  VALIDADORES (FUNCIONES PURAS)
 *  ============================ */

/**
 * Validador ligero que complementa a Validators.email con chequeos de dominio/TLD.
 * NO modifica el control, solo retorna errores.
 */
export function emailWithDotTld(): ValidatorFn {
  return (c: AbstractControl): ValidationErrors | null => {
    const raw = String(c.value ?? '').trim();
    if (!raw) return null; // "required" decide vacío
    const at = raw.indexOf('@');
    if (at < 0) return null; // deja que Validators.email marque el formato base

    const domain = raw.slice(at + 1);
    if (!domain) return { domainMissing: true };
    if (!/^[A-Za-z0-9.-]+$/.test(domain)) return { domainInvalid: true };
    if (!domain.includes('.')) return { tldMissing: true };

    const tld = domain.split('.').pop() ?? '';
    if (tld.length < 2 || tld.length > 24) return { tldInvalid: true };

    return null;
  };
}

/** Arreglo estándar de validadores para email */
export function buildEmailValidators(required = true): ValidatorFn[] {
  const v: ValidatorFn[] = [Validators.email, emailWithDotTld()];
  if (required) v.unshift(Validators.required);
  return v;
}

/** FormControl listo si prefieres construirlo directo */
export function buildEmailControl(initial = '', required = true): FormControl<string | null> {
  return new FormControl<string | null>(initial, buildEmailValidators(required));
}
