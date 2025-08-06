import { TestBed } from '@angular/core/testing';

import { ModuleServices } from './module-services';

describe('ModuleServices', () => {
  let service: ModuleServices;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ModuleServices);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
