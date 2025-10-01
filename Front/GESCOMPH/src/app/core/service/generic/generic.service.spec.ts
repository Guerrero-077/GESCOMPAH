import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { GenericService } from './generic.service';
import { environment } from '../../../../environments/environment.development';

interface DummyModel {
  id: number;
  name: string;
}

describe('GenericService', () => {
  let service: GenericService<DummyModel>;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [GenericService]
    });

    service = TestBed.inject(GenericService<DummyModel>);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('builds correct URL when requesting by id', () => {
    const id = 123;
    service.getById('Entidad', id).subscribe();

    const req = httpMock.expectOne(`${environment.apiURL}/Entidad/${id}`);
    expect(req.request.method).toBe('GET');
    req.flush({ id, name: 'test' });
  });
});

