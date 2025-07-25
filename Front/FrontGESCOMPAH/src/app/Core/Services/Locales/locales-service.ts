import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { LocalesModel } from '../../../shared/components/Models/card/card.models';
import { LocalCreateModel } from '../../../shared/components/Models/card/card.models';
import { environment } from '../../../../environments/environment.development';

@Injectable({ providedIn: 'root' })
export class LocalesService {
  private urlBase = environment.apiURL + '/Establishments';

  constructor(private http: HttpClient) { }

  getLocales(): Observable<LocalesModel[]> {
    return this.http.get<LocalesModel[]>(this.urlBase);
  }

  createLocal(local: LocalCreateModel): Observable<LocalesModel> {
    const formData = new FormData();
    formData.append('Name', local.name);
    formData.append('Description', local.description);
    formData.append('AreaM2', local.areaM2.toString());
    formData.append('RentValueBase', local.rentValueBase.toString());

    if (local.files?.length) {
      for (let file of local.files) {
        formData.append('Files', file);
      }
    }

    return this.http.post<LocalesModel>(this.urlBase, formData);
  }

  
  deleteLocal(id: number, forceDelete = false): Observable<void> {
    const url = `${this.urlBase}/${id}`;
    const options = {
      params: {
        forceDelete: forceDelete.toString()
      }
    };

    return this.http.delete<void>(url, options);
  }

}
