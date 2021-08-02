import { TestBed } from '@angular/core/testing';

import { ManualTransferService } from './manual-transfer.service';

describe('ManualTransferService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ManualTransferService = TestBed.get(ManualTransferService);
    expect(service).toBeTruthy();
  });
});
