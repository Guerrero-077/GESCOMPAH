import { TestBed } from '@angular/core/testing';

import { ContractsRealtimeService } from './contracts-realtime.service';

describe('ContractsRealtimeService', () => {
  let service: ContractsRealtimeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ContractsRealtimeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
