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
      "description": "Gestión de usuarios",
      "route" : "/admin/users",
      "active" : true
    },
    {
      "id": 2,
      "name": "Roles",
      "description": "Gestión de roles",
      "route" : "/admin/rols",
      "active" : true
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

  AddPruebas(form: FormModule): Observable<FormModule> {
    const newForm = {
      ...form,
      id: this.form.length + 1
    };
    console.log(newForm);
    this.form.push(newForm);
    console.log(this.form)
    return of(newForm);
  }
}
