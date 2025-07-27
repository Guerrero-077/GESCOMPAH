import { Injectable } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PlazaModel } from '../../Models/Plaza.models';

@Injectable({
  providedIn: 'root'
})
export class PlazasService {

  private urlBase = environment.apiURL + '/Plaza';
  /**
   *
   */
  constructor(private http: HttpClient) { }


  getAll(): Observable<PlazaModel[]> {
    return this.http.get<PlazaModel[]>(this.urlBase);
  }
}
