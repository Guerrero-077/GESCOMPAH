import { TestBed } from '@angular/core/testing';

import { FilePickerService } from './file-picker.service';

describe('FilePickerService', () => {
  let service: FilePickerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FilePickerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
