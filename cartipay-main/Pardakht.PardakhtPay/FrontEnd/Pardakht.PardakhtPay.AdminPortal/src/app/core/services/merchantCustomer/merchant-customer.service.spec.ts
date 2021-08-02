import { TestBed } from '@angular/core/testing';

import { MerchantCustomerService } from './merchant-customer.service';

describe('MerchantCustomerService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: MerchantCustomerService = TestBed.get(MerchantCustomerService);
    expect(service).toBeTruthy();
  });
});
