import { TestBed } from '@angular/core/testing';

import { BlockedPhoneNumbersService } from './blocked-phone-numbers.service';

describe('BlockedPhoneNumbersService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: BlockedPhoneNumbersService = TestBed.get(BlockedPhoneNumbersService);
    expect(service).toBeTruthy();
  });
});
