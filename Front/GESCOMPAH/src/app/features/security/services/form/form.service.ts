import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { FormCreateModel, FormSelectModel, FormUpdateModel } from '../../models/form.models';

@Injectable({
  providedIn: 'root'
})
export class FormService extends GenericService<FormSelectModel, FormCreateModel, FormUpdateModel> {
  protected override resource = 'form';

}
