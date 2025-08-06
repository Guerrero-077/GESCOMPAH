import { Injectable } from '@angular/core';
import { GenericService } from '../../../../Core/Service/Generic/generic-service';
import { DepartmentModels } from '../../Models/department.Models';

@Injectable({
  providedIn: 'root'
})
export class DepartmentServices extends GenericService<DepartmentModels> {
  
}
