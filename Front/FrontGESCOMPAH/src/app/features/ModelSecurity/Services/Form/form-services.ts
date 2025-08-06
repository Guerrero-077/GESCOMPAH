import { FormModule } from './../../Models/form.model';
import { Injectable } from '@angular/core';
import { GenericService } from '../../../../Core/Service/Generic/generic-service';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FormServices extends GenericService<FormModule> {
  private form: FormModule[] = [
    {
      "id": 1,
      "name": "Usuarios",
      "description": "Gestión de usuarios"
    },
    {
      "id": 2,
      "name": "Roles",
      "description": "Gestión de roles"
    }
  ];

  getAllPruebas(): Observable<FormModule[]> {
    return of(this.form);
  }
  getByIdPruebas(id: number) {
    const form = this.form.find(x => x.id === id);
    return new Observable<FormModule | undefined>(observer => {
      observer.next(form);
      observer.complete();
    });
  }

  AddPruebas(form: FormModule){
    
  }
}
