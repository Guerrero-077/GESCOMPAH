import { TestBed } from '@angular/core/testing';

import { SharedEventsServiceService } from './shared-events-service.service';

describe('SharedEventsServiceService', () => {
  let service: SharedEventsServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SharedEventsServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
