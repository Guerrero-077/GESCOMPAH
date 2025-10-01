import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

/* ========= Helpers internos (no export) ========= */

const isEmpty = (v: unknown) => v === null || v === undefined || v === '';

/** Normaliza separador decimal (., ,) según configuración */
function normalizeDecimal(input: string, separators: 'dot' | 'comma' | 'both'): string {
  const s = input.trim();
  if (separators === 'dot')   return s.replace(',', '.');
  if (separators === 'comma') return s.replace('.', ','); // si quisieras “coma” como estándar
  // both: elegimos '.' como estándar interno
  // si hay ambos, prioriza el último separador presente
  const lastDot   = s.lastIndexOf('.');
  const lastComma = s.lastIndexOf(',');
  if (lastDot > lastComma) return s.replace(',', ''); // “1,234.56” -> “1234.56”
  if (lastComma > lastDot) return s.replace(/\./g, '').replace(',', '.'); // “1.234,56” -> “1234.56”
  return s.replace(',', '.'); // solo coma -> punto
}

/** Parsea número plano según opciones (no científica si no está permitida) */
function parseNumber(
  raw: string,
  {
    allowSign = false,
    allowScientific = false,
    separators = 'both' as 'dot' | 'comma' | 'both',
  } = {}
): { n: number | null; error?: ValidationErrors } {
  let s = normalizeDecimal(raw, separators);

  // Notación científica / signo
  if (!allowScientific && /[eE]/.test(s)) return { n: null, error: { numberNotation: true } };
  if (!allowSign && /[+\-]/.test(s))     return { n: null, error: { numberNotation: true } };

  // Solo dígitos con parte decimal opcional (sin separadores de miles)
  // Permitimos signo y/o científica si las opciones lo permiten
  const signPart = allowSign ? '[+-]?' : '';
  const sciPart  = allowScientific ? '(?:[eE][+-]?\\d+)?' : '';
  const re = new RegExp(`^${signPart}\\d+(?:\\.\\d+)?${sciPart}$`);
  if (!re.test(s)) return { n: null, error: { numberInvalid: true } };

  const n = Number(s);
  if (Number.isNaN(n)) return { n: null, error: { nan: true } };
  return { n };
}

/* ========= API pública (estandarizada) ========= */

export const AppValidators = {
  /**
   * Rechaza valores con solo espacios. Respeta null/undefined/'' como válidos para combinar con required.
   * error: { onlySpaces: true }
   */
  notOnlySpaces(): ValidatorFn {
    return (c: AbstractControl): ValidationErrors | null => {
      const v = c.value;
      if (isEmpty(v)) return null;
      return String(v).trim().length === 0 ? { onlySpaces: true } : null;
    };
  },

  /**
   * Valida decimal simple con opciones.
   * - decimals: máximo de decimales (p.ej. 2)
   * - allowSign / allowScientific
   * - separators: 'dot' | 'comma' | 'both' (por defecto 'both')
   * errores: { numberNotation }, { numberInvalid }, { nan }, { decimalScale: { max } }
   */
  decimal(opts: {
    decimals?: number;
    allowSign?: boolean;
    allowScientific?: boolean;
    separators?: 'dot' | 'comma' | 'both';
  } = {}): ValidatorFn {
    const { decimals, allowSign = false, allowScientific = false, separators = 'both' } = opts;

    return (c: AbstractControl): ValidationErrors | null => {
      const raw = c.value;
      if (isEmpty(raw)) return null;

      const sNorm = normalizeDecimal(String(raw), separators);
      const parsed = parseNumber(sNorm, { allowSign, allowScientific, separators: 'dot' });
      if (parsed.error) return parsed.error;

      if (typeof decimals === 'number') {
        const [, dec = ''] = sNorm.split('.');
        if (dec.length > decimals) return { decimalScale: { max: decimals } };
      }

      return null;
    };
  },

  /**
   * Valida rango numérico usando el mismo parser que decimal().
   * - min/max: límites inclusivos
   * - allowSign/allowScientific/separators igual que decimal()
   * errores: { numberNotation }, { numberInvalid }, { nan }, { min: { min, actual } }, { max: { max, actual } }
   */
  numberRange(opts: {
    min?: number;
    max?: number;
    allowSign?: boolean;
    allowScientific?: boolean;
    separators?: 'dot' | 'comma' | 'both';
  } = {}): ValidatorFn {
    const { min, max, allowSign = false, allowScientific = false, separators = 'both' } = opts;

    return (c: AbstractControl): ValidationErrors | null => {
      const raw = c.value;
      if (isEmpty(raw)) return null;

      const sNorm = normalizeDecimal(String(raw), separators);
      const parsed = parseNumber(sNorm, { allowSign, allowScientific, separators: 'dot' });
      if (parsed.error) return parsed.error;

      const n = parsed.n as number;
      if (min !== undefined && n < min) return { min: { min, actual: n } };
      if (max !== undefined && n > max) return { max: { max, actual: n } };
      return null;
    };
  },

  /**
   * Valida una dirección básica (Unicode letters, dígitos, espacios y # - , .)
   * error: { addressInvalid: true }
   */
  address(): ValidatorFn {
    const regex = /^[\p{L}\d\s#\-,.]+$/u;
    return (c: AbstractControl): ValidationErrors | null => {
      const v = (c.value ?? '') as string;
      if (!v) return null;
      return regex.test(v) ? null : { addressInvalid: true };
    };
  },

  /**
   * Valida nombre “humano” con letras y separadores comunes (apócopes, apóstrofes, guiones, espacios).
   * error: { alphaHuman: true }
   */
  alphaHumanName(): ValidatorFn {
    const rx = /^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+(?:[ '’-][A-Za-zÁÉÍÓÚÜÑáéíóúüñ]+)*$/;
    return (c: AbstractControl): ValidationErrors | null => {
      const v = String(c.value ?? '').trim();
      if (!v) return null;
      return rx.test(v) ? null : { alphaHuman: true };
    };
  },

  /**
   * Documento colombiano (7-10 dígitos).
   * error: { colombianDocument: true }
   */
  colombianDocument(): ValidatorFn {
    const rx = /^\d{7,10}$/;
    return (c: AbstractControl): ValidationErrors | null => {
      const v = String(c.value ?? '').trim();
      if (!v) return null;
      return rx.test(v) ? null : { colombianDocument: true };
    };
  },

  /**
   * Celular colombiano (10 dígitos, empieza en 3).
   * error: { colombianPhone: true }
   */
  colombianPhone(): ValidatorFn {
    const rx = /^3\d{9}$/;
    return (c: AbstractControl): ValidationErrors | null => {
      const v = String(c.value ?? '').trim();
      if (!v) return null;
      return rx.test(v) ? null : { colombianPhone: true };
    };
  },

  /**
   * Email con TLD (complementa Validators.email).
   * errores: { domainMissing }, { domainInvalid }, { tldMissing }, { tldInvalid }
   */
  emailWithDotTld(): ValidatorFn {
    return (c: AbstractControl): ValidationErrors | null => {
      const v = String(c.value ?? '').trim();
      if (!v) return null;
      const at = v.indexOf('@');
      if (at < 0) return null; // Angular Validators.email se encarga del formato general
      const domain = v.slice(at + 1);
      if (!domain) return { domainMissing: true };
      if (!/^[A-Za-z0-9.-]+$/.test(domain)) return { domainInvalid: true };
      if (!domain.includes('.')) return { tldMissing: true };
      const lastLabel = domain.split('.').pop() || '';
      if (lastLabel.length < 2 || lastLabel.length > 24) return { tldInvalid: true };
      return null;
    };
  },
};
