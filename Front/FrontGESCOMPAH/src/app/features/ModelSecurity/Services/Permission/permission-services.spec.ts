import { TestBed } from '@angular/core/testing';

import { PermissionServices } from './permission-services';

describe('PermissionServices', () => {
  let service: PermissionServices;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PermissionServices);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
