import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RolFormPermissionGroupedModel, RolFormPermissionSelectModel, RolFormPermissionCreateModel, RolFormPermissionUpdateModel } from '../../models/rol-form-permission.models';
import { environment } from '../../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RolFormPermissionService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = (environment.apiURL as string).replace(/\/+$/, '');
  private resource = 'rolFormPermission';

  private url(...segments: (string | number | undefined)[]) {
    const segs = [this.baseUrl, this.resource, ...segments.filter(s => s !== undefined && s !== '')];
    return segs.join('/').replace(/([^:]\/)\/+/g, '$1');
  }

  // --- NUEVOS MÉTODOS ---
  getAllGrouped(): Observable<RolFormPermissionGroupedModel[]> {
    return this.http.get<RolFormPermissionGroupedModel[]>(this.url('grouped'));
  }

  deleteByGroup(rolId: number, formId: number): Observable<void> {
    return this.http.delete<void>(this.url('group', rolId, formId));
  }
  // --- FIN DE NUEVOS MÉTODOS ---

  getAll(): Observable<RolFormPermissionSelectModel[]> {
    return this.http.get<RolFormPermissionSelectModel[]>(this.url());
  }

  getById(id: number): Observable<RolFormPermissionSelectModel> {
    return this.http.get<RolFormPermissionSelectModel>(this.url(id));
  }

  create(dto: RolFormPermissionCreateModel): Observable<RolFormPermissionSelectModel> {
    return this.http.post<RolFormPermissionSelectModel>(this.url(), dto);
  }

  update(id: number, dto: RolFormPermissionUpdateModel): Observable<RolFormPermissionSelectModel> {
    return this.http.put<RolFormPermissionSelectModel>(this.url(id), dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(this.url(id));
  }

  deleteLogic(id: number): Observable<void> {
    return this.http.patch<void>(this.url(id), null);
  }
}