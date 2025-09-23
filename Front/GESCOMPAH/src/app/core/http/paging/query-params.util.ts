import { HttpParams } from '@angular/common/http';
import { PageQuery, Primitive } from './page.types';

// Serializa PageQuery al contrato de tu backend: Page, Size, Search, Sort, Desc, Filters[key]
export function toHttpParams(q: PageQuery): HttpParams {
  let params = new HttpParams();

  if (q.page && q.page > 0) params = params.set('Page', String(q.page));
  if (q.size && q.size > 0) params = params.set('Size', String(q.size));
  if (q.search != null && String(q.search).trim() !== '') params = params.set('Search', String(q.search).trim());
  if (q.sort != null && String(q.sort).trim() !== '') params = params.set('Sort', String(q.sort).trim());
  if (typeof q.desc === 'boolean') params = params.set('Desc', String(q.desc));

  if (q.filters) {
    for (const [key, raw] of Object.entries(q.filters)) {
      if (raw === undefined || raw === null) continue;
      const values = Array.isArray(raw) ? raw : [raw];
      for (const v of values) {
        params = params.append(`Filters[${key}]`, String(v as Primitive));
      }
    }
  }

  return params;
}
