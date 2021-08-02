import { TestBed } from '@angular/core/testing';

import { BankStatementService } from './bank-statement.service';

describe('BankStatementService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: BankStatementService = TestBed.get(BankStatementService);
    expect(service).toBeTruthy();
  });
});
