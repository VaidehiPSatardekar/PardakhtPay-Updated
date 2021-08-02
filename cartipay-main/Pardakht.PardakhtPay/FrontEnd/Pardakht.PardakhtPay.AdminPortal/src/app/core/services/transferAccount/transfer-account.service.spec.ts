import { TestBed } from '@angular/core/testing';

import { TransferAccountService } from './transfer-account.service';

describe('TransferAccountService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: TransferAccountService = TestBed.get(TransferAccountService);
    expect(service).toBeTruthy();
  });
});
