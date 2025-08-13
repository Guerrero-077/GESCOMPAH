import { TestBed } from '@angular/core/testing';

import { RolFormPermissionService } from './rol-form-permission.service';

describe('RolFormPermissionService', () => {
  let service: RolFormPermissionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RolFormPermissionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
