import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment.development';
import { LocalesModel, LocalCreateModel, LocalUpdateModel } from '../../../../shared/components/Models/card/card.models';
import { EstablishmentCreate, EstablishmentSelect } from '../../Models/Establishment.models';

@Injectable({ providedIn: 'root' })
export class LocalesService {

  private urlBase = environment.apiURL + '/Establishments';

  constructor(private http: HttpClient) { }

  getAll(): Observable<EstablishmentSelect[]> {
    return this.http.get<EstablishmentSelect[]>(this.urlBase);
  }

  getById(id: number): Observable<EstablishmentSelect> {
    return this.http.get<EstablishmentSelect>(`${this.urlBase}/${id}`);
  }

  create(establishment: EstablishmentCreate): Observable<EstablishmentSelect> {
    const formData = this.buildFormData(establishment);
    return this.http.post<EstablishmentSelect>(this.urlBase, formData);
  }

  update(establishment: EstablishmentCreate): Observable<EstablishmentSelect> {
    if (!establishment.id) throw new Error('Establishment ID is required for update');
    const formData = this.buildFormData(establishment);
    return this.http.put<EstablishmentSelect>(`${this.urlBase}/${establishment.id}`, formData);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.urlBase}/${id}`);
  }

  private buildFormData(est: EstablishmentCreate): FormData {
    const form = new FormData();
    form.append('id', est.id?.toString() || '0');
    form.append('name', est.name);
    form.append('description', est.description);
    form.append('areaM2', est.areaM2.toString());
    form.append('rentValueBase', est.rentValueBase.toString());
    form.append('plazaId', est.plazaId.toString());

    if (est.files?.length) {
      for (const file of est.files.slice(0, 5)) {
        form.append('files', file, file.name);
      }
    }

    return form;
  }



  // createLocal(data: LocalCreateModel): Observable<LocalesModel> {
  //   const formData = this.buildFormData(data);
  //   return this.http.post<LocalesModel>(this.urlBase, formData);
  // }

  // updateLocal(data: LocalUpdateModel): Observable<LocalesModel> {
  //   if (!data.id) throw new Error('ID obligatorio para actualizar el local');

  //   const formData = this.buildFormData(data);
  //   return this.http.put<LocalesModel>(`${this.urlBase}/${data.id}`, formData);
  // }

  // deleteLocal(id: number, hardDelete = false): Observable<void> {
  //   return this.http.delete<void>(`${this.urlBase}/${id}?hardDelete=${hardDelete}`);
  // }

  // private buildFormData(data: LocalCreateModel | LocalUpdateModel): FormData {
  //   const formData = new FormData();

  //   // Para el update, aseguramos que el ID se incluya
  //   if ('id' in data && data.id !== undefined && data.id !== null) {
  //     formData.append('id', data.id.toString());
  //   }

  //   formData.append('name', data.name);
  //   formData.append('description', data.description);
  //   formData.append('areaM2', data.areaM2.toString());
  //   formData.append('rentValueBase', data.rentValueBase.toString());

  //   if (data.files && data.files.length > 0) {
  //     data.files.forEach((file, index) => {
  //       formData.append('files', file, file.name);
  //     });
  //   }

  //   return formData;
  // }

}
