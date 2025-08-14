import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RoleCreateModel, RoleSelectModel, RoleUpdateModel } from '../../models/role.models';
import { environment } from '../../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = (environment.apiURL as string).replace(/\/+$/, '');
  private resource = 'rol';

  private url(...segments: (string | number | undefined)[]) {
    const segs = [this.baseUrl, this.resource, ...segments.filter(s => s !== undefined && s !== '')];
    return segs.join('/').replace(/([^:]\/)\/+/g, '$1');
  }

  getAll(): Observable<RoleSelectModel[]> {
    return this.http.get<RoleSelectModel[]>(this.url());
  }

  getById(id: number): Observable<RoleSelectModel> {
    return this.http.get<RoleSelectModel>(this.url(id));
  }

  create(dto: RoleCreateModel): Observable<RoleSelectModel> {
    return this.http.post<RoleSelectModel>(this.url(), dto);
  }

  update(id: number, dto: RoleUpdateModel): Observable<RoleSelectModel> {
    return this.http.put<RoleSelectModel>(this.url(id), dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(this.url(id));
  }

  deleteLogic(id: number): Observable<void> {
    return this.http.patch<void>(this.url(id), null);
  }
}
