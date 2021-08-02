import { TestBed } from '@angular/core/testing';

import { MerchantBankAccountService } from './merchant-bank-account.service';

describe('MerchantBankAccountService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: MerchantBankAccountService = TestBed.get(MerchantBankAccountService);
    expect(service).toBeTruthy();
  });
});
