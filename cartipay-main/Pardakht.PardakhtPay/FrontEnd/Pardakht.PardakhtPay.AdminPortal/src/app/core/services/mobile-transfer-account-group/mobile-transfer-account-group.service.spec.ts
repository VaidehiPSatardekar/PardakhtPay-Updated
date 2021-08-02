import { TestBed } from '@angular/core/testing';

import { MobileTransferAccountGroupService } from './mobile-transfer-account-group.service';

describe('MobileTransferAccountGroupService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: MobileTransferAccountGroupService = TestBed.get(MobileTransferAccountGroupService);
    expect(service).toBeTruthy();
  });
});
