import { Injectable } from '@angular/core';

export interface FilePickerOptions {
  remaining: number;       // capacidad restante (p. ej. 5 - existentes - seleccionados)
  maxSizeBytes: number;    // tamaño máximo por archivo
  acceptImagesOnly?: boolean; // default true
  // Lista blanca opcional de extensiones permitidas (sin punto), ej.: ['jpg','jpeg','png','webp']
  allowedExtensions?: string[];
}

export interface FilePickerResult {
  accepted: File[];
  errors: string[];
  objectUrls: string[];    // URLs creadas con createObjectURL
}

@Injectable({ providedIn: 'root' })
export class FilePickerService {
  pick(files: File[], alreadySelected: File[], opts: FilePickerOptions): FilePickerResult {
    const { remaining, maxSizeBytes, acceptImagesOnly = true, allowedExtensions } = opts;

    const errors: string[] = [];
    const accepted: File[] = [];

    // Evitar duplicados comparando name+size+lastModified
    const isDup = (f: File) =>
      alreadySelected.some(a => a.name === f.name && a.size === f.size && a.lastModified === f.lastModified) ||
      accepted.some(a => a.name === f.name && a.size === f.size && a.lastModified === f.lastModified);

    for (const f of files) {
      if (accepted.length >= remaining) break;
      if (isDup(f)) continue;

      if (acceptImagesOnly) {
        if (!f.type.startsWith('image/')) {
          errors.push('"' + f.name + '" no es una imagen válida.');
          continue;
        }
        const ext = (f.name.split('.').pop() || '').toLowerCase();
        const allowed = (allowedExtensions && allowedExtensions.length)
          ? allowedExtensions.map(x => x.toLowerCase())
          : ['jpg','jpeg','png','webp'];
        const okExt = allowed.indexOf(ext) !== -1;
        const okMime = ['image/jpeg','image/png','image/webp'].indexOf((f.type || '').toLowerCase()) !== -1;
        if (!okExt || !okMime) {
          errors.push('"' + f.name + '" tiene un formato no permitido. Solo se permiten JPG, PNG o WEBP.');
          continue;
        }
      }
      if (f.size > maxSizeBytes) {
        const mb = (f.size / (1024 * 1024)).toFixed(2);
        const maxMb = (maxSizeBytes / (1024 * 1024)).toFixed(0);
        errors.push('"' + f.name + '" pesa ' + mb + ' MB (máx. ' + maxMb + ' MB).');
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
