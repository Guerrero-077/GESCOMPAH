import { TestBed } from '@angular/core/testing';

import { CityServices } from './city-services';

describe('CityServices', () => {
  let service: CityServices;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CityServices);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
