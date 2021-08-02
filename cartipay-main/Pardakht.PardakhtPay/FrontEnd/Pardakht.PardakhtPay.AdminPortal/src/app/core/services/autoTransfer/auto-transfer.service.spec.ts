import { TestBed } from '@angular/core/testing';

import { AutoTransferService } from './auto-transfer.service';

describe('AutoTransferService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AutoTransferService = TestBed.get(AutoTransferService);
    expect(service).toBeTruthy();
  });
});
