import { TestBed } from '@angular/core/testing';

import { BankLoginService } from './bank-login.service';

describe('BankLoginService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: BankLoginService = TestBed.get(BankLoginService);
    expect(service).toBeTruthy();
  });
});
