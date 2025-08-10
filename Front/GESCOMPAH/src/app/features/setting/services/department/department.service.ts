import { Injectable } from '@angular/core';
import { GenericService } from '../../../../core/service/generic/generic.service';
import { DepartmentModel } from '../../models/department.models';

@Injectable({
  providedIn: 'root'
})
export class DepartmentService extends GenericService<DepartmentModel> {

}