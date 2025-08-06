import { FormModule } from './../../Models/form.model';
import { Injectable } from '@angular/core';
import { GenericService } from '../../../../Core/Service/Generic/generic-service';

@Injectable({
  providedIn: 'root'
})
export class FormServices extends GenericService<FormModule> {

}
