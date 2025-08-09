import { Injectable } from '@angular/core';
import { FormModule } from '../../models/form.models';
import { GenericService } from '../../../../core/service/generic/generic.service';

@Injectable({
  providedIn: 'root'
})
export class FormService extends GenericService<FormModule>{
  
}
