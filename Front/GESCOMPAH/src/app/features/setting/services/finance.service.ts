import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { FinanceSelectModel } from '../models/finance.model';

@Injectable({
  providedIn: 'root'
})
export class FinanceService {
  private apiUrl = `${environment.apiURL}/finances`; // Assuming endpoint is /finances

  constructor(private http: HttpClient) { }

  changeActiveStatus(id: number, active: boolean): Observable<FinanceSelectModel> {
    return this.http.patch<FinanceSelectModel>(`${this.apiUrl}/${id}/status`, { active });
  }
}
