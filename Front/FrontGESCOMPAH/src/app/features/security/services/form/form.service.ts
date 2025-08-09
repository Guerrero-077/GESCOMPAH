import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/services/generic/generic.service';
import { FormModule } from '../../models/form.models';

@Injectable({
  providedIn: 'root'
})
export class FormService extends GenericService<FormModule>{
  
}
