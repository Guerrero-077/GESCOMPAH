import { TestBed } from '@angular/core/testing';

import { FormModuleService } from './form-module.service';

describe('FormModuleService', () => {
  let service: FormModuleService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FormModuleService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
