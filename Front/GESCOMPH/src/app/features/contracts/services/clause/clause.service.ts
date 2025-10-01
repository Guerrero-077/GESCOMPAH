import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ClauseSelect } from '../../models/clause.models';
import { environment } from '../../../../../environments/environment.development';

@Injectable({ providedIn: 'root' })
export class ClauseService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiURL}/clause`;

  getAll(): Observable<ClauseSelect[]> {
    return this.http.get<ClauseSelect[]>(this.baseUrl);
  }
}

