// core/services/base-crud.service.ts
import { inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
// Ajusta este import a tu proyecto si no usas alias
import { environment } from '../../../../environments/environment';

export abstract class GenericService<
  TModel,
  TCreate = unknown,
  TUpdate = Partial<TCreate>,
  ID = number
> {
  protected readonly http = inject(HttpClient);
  protected readonly baseUrl = (environment.apiURL as string).replace(/\/+$/, '');
  protected abstract resource: string;

  protected url(...segments: (string | number | undefined)[]) {
    const segs = [this.baseUrl, this.resource, ...segments.filter(s => s !== undefined && s !== '')];
    return segs.join('/').replace(/([^:]\/)\/+/g, '$1');
  }

  // GET api/{resource}
  getAll(): Observable<TModel[]> {
    return this.http.get<TModel[]>(this.url());
  }

  // GET api/{resource}/{id}
  getById(id: ID): Observable<TModel> {
    return this.http.get<TModel>(this.url(id as any));
  }

  // POST api/{resource}
  create(dto: TCreate): Observable<TModel> {
    return this.http.post<TModel>(this.url(), dto);
  }

  // PUT api/{resource}/{id}
  update(id: ID, dto: TUpdate): Observable<TModel> {
    return this.http.put<TModel>(this.url(id as any), dto);
  }

  // DELETE api/{resource}/{id}
  delete(id: ID): Observable<void> {
    return this.http.delete<void>(this.url(id as any));
  }

  // PATCH api/{resource}/{id}  (delete lógico SIN body)
  deleteLogic(id: ID): Observable<void> {
    return this.http.patch<void>(this.url(id as any), null);
  }
}
