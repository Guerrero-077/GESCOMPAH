import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment.development';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  private urlBase = environment.apiURL + '/Establishments';

  constructor(private http: HttpClient) { }

  
}
