import { TestBed } from '@angular/core/testing';

import { DepartmentServices } from './department-services';

describe('DepartmentServices', () => {
  let service: DepartmentServices;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DepartmentServices);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
