import { TestBed } from '@angular/core/testing';

import { TenantsServices } from './tenants-services';

describe('TenantsServices', () => {
  let service: TenantsServices;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TenantsServices);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
