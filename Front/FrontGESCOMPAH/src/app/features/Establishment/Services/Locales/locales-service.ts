import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../../environments/environment.development';
import { LocalesModel, LocalCreateModel, LocalUpdateModel } from '../../../../shared/components/Models/card/card.models';

@Injectable({ providedIn: 'root' })
export class LocalesService {
  private urlBase = environment.apiURL + '/Establishments';

  constructor(private http: HttpClient) { }

  getLocalById(id: number): Observable<LocalesModel> {
    const url = `${this.urlBase}/${id}`;
    return this.http.get<LocalesModel>(url);
  }
  getLocales(): Observable<LocalesModel[]> {
    return this.http.get<LocalesModel[]>(this.urlBase);
  }

  // createLocal(local: LocalCreateModel): Observable<LocalesModel> {
  //   const formData = new FormData();
  //   formData.append('Name', local.name);
  //   formData.append('Description', local.description);
  //   formData.append('AreaM2', local.areaM2.toString());
  //   formData.append('RentValueBase', local.rentValueBase.toString());

  //   if (local.files?.length) {
  //     for (let file of local.files) {
  //       formData.append('Files', file);
  //     }
  //   }

  //   return this.http.post<LocalesModel>(this.urlBase, formData);
  // }

  // updateLocal(local: LocalUpdateModel): Observable<LocalesModel> {
  //   const formData = new FormData();

  //   formData.append('Id', local.id.toString());
  //   formData.append('Name', local.name);
  //   formData.append('Description', local.description);
  //   formData.append('AreaM2', local.areaM2.toString());
  //   formData.append('RentValueBase', local.rentValueBase.toString());

  //   if (local.files?.length) {
  //     for (const file of local.files) {
  //       formData.append('Files', file);
  //     }
  //   }

  //   return this.http.put<LocalesModel>(this.urlBase, formData);
  // }



  // deleteLocal(id: number, forceDelete = false): Observable<void> {
  //   const url = `${this.urlBase}/${id}`;
  //   const options = {
  //     params: {
  //       forceDelete: forceDelete.toString()
  //     }
  //   };

  //   return this.http.delete<void>(url, options);
  // }
  createLocal(data: LocalCreateModel): Observable<LocalesModel> {
    const formData = this.buildFormData(data);
    return this.http.post<LocalesModel>(this.urlBase, formData);
  }

  updateLocal(data: LocalUpdateModel): Observable<LocalesModel> {
    if (!data.id) throw new Error('ID obligatorio para actualizar el local');

    const formData = this.buildFormData(data);
    return this.http.put<LocalesModel>(`${this.urlBase}/${data.id}`, formData);
  }
  
  deleteLocal(id: number, hardDelete = false): Observable<void> {
    return this.http.delete<void>(`${this.urlBase}/${id}?hardDelete=${hardDelete}`);
  }

  private buildFormData(data: LocalCreateModel | LocalUpdateModel): FormData {
    const formData = new FormData();

    // Para el update, aseguramos que el ID se incluya
    if ('id' in data && data.id !== undefined && data.id !== null) {
      formData.append('id', data.id.toString());
    }

    formData.append('name', data.name);
    formData.append('description', data.description);
    formData.append('areaM2', data.areaM2.toString());
    formData.append('rentValueBase', data.rentValueBase.toString());

    if (data.files && data.files.length > 0) {
      data.files.forEach((file, index) => {
        formData.append('files', file, file.name);
      });
    }

    return formData;
  }

}
