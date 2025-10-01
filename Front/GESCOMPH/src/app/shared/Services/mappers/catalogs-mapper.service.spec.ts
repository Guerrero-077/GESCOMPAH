import { TestBed } from '@angular/core/testing';

import { CatalogsMapperService } from './catalogs-mapper.service';

describe('CatalogsMapperService', () => {
  let service: CatalogsMapperService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CatalogsMapperService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
