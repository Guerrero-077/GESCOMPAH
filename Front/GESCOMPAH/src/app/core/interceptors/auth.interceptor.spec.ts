import { TestBed } from '@angular/core/testing';
import {
  HTTP_INTERCEPTORS,
  HttpClient,
  HttpInterceptorFn
} from '@angular/common/http';
import {
  HttpClientTestingModule,
  HttpTestingController
} from '@angular/common/http/testing';

import { authInterceptor } from './auth.interceptor';
import { environment } from '../../../environments/environment';

describe('authInterceptor', () => {
  const interceptor: HttpInterceptorFn = (req, next) =>
    TestBed.runInInjectionContext(() => authInterceptor(req, next));

  let httpMock: HttpTestingController;
  let httpClient: HttpClient;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        { provide: HTTP_INTERCEPTORS, useValue: authInterceptor, multi: true }
      ]
    });

    httpMock = TestBed.inject(HttpTestingController);
    httpClient = TestBed.inject(HttpClient);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(interceptor).toBeTruthy();
  });

  it('should set withCredentials for API requests', () => {
    const apiUrl = `${environment.apiURL}/test`;

    httpClient.get(apiUrl).subscribe();

    const req = httpMock.expectOne(apiUrl);
    expect(req.request.withCredentials).toBeTrue();
    req.flush({});
  });

  it('should not modify non API requests', () => {
    const externalUrl = 'https://example.com';

    httpClient.get(externalUrl).subscribe();

    const req = httpMock.expectOne(externalUrl);
    expect(req.request.withCredentials).toBeFalse();
    req.flush({});
  });
});
