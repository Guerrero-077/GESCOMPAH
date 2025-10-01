import { Injectable } from '@angular/core';

export interface FilePickerOptions {
  remaining: number;       // capacidad restante (p. ej. 5 - existentes - seleccionados)
  maxSizeBytes: number;    // tamaño máximo por archivo
  acceptImagesOnly?: boolean; // default true
}

export interface FilePickerResult {
  accepted: File[];
  errors: string[];
  objectUrls: string[];    // URLs creadas con createObjectURL
}

@Injectable({ providedIn: 'root' })
export class FilePickerService {
  pick(files: File[], alreadySelected: File[], opts: FilePickerOptions): FilePickerResult {
    const { remaining, maxSizeBytes, acceptImagesOnly = true } = opts;

    const errors: string[] = [];
    const accepted: File[] = [];

    // Evitar duplicados comparando name+size+lastModified
    const isDup = (f: File) =>
      alreadySelected.some(a => a.name === f.name && a.size === f.size && a.lastModified === f.lastModified) ||
      accepted.some(a => a.name === f.name && a.size === f.size && a.lastModified === f.lastModified);

    for (const f of files) {
      if (accepted.length >= remaining) break;
      if (isDup(f)) continue;

      if (acceptImagesOnly && !f.type.startsWith('image/')) {
        errors.push(`"${f.name}" no es una imagen válida.`);
        continue;
      }
      if (f.size > maxSizeBytes) {
        const mb = (f.size / (1024 * 1024)).toFixed(2);
        errors.push(`"${f.name}" pesa ${mb} MB (máx. ${(maxSizeBytes / (1024 * 1024)).toFixed(0)} MB).`);
        continue;
      }
      accepted.push(f);
    }

    const objectUrls = accepted.map(f => URL.createObjectURL(f));
    return { accepted, errors, objectUrls };
  }

  revoke(url: string) { try { URL.revokeObjectURL(url); } catch { /* noop */ } }

  revokeAll(urls: string[]) {
    for (const u of urls) this.revoke(u);
  }
}
