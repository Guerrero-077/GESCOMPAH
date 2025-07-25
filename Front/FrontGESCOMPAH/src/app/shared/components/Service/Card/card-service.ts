import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../../environments/environment';
import { LocalesModel } from '../../Models/card/card.models';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CardService {
  
  private http = inject(HttpClient);
  private urlBase = environment.apiURL + '/Auth/Establishments/';

  GetAll(): Observable<LocalesModel> {
    return this.http.get<LocalesModel>(this.urlBase);
  }
}
