import { TestBed } from '@angular/core/testing';

import { BlockedCardNumberService } from './blocked-card-number.service';

describe('BlockedCardNumberService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: BlockedCardNumberService = TestBed.get(BlockedCardNumberService);
    expect(service).toBeTruthy();
  });
});
