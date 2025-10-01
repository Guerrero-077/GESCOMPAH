import { TestBed } from '@angular/core/testing';

import { RolUserService } from './rol-user.service';

describe('RolUserService', () => {
  let service: RolUserService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RolUserService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
