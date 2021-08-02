import { TestBed } from '@angular/core/testing';

import { MobileTransferCardAccountService } from './mobile-transfer-card-account.service';

describe('MobileTransferCardAccountService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: MobileTransferCardAccountService = TestBed.get(MobileTransferCardAccountService);
    expect(service).toBeTruthy();
  });
});
