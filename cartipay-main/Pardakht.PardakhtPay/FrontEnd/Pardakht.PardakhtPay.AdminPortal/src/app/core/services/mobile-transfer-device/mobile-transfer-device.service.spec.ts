import { TestBed } from '@angular/core/testing';

import { MobileTransferDeviceService } from './mobile-transfer-device.service';

describe('MobileTransferDeviceService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: MobileTransferDeviceService = TestBed.get(MobileTransferDeviceService);
    expect(service).toBeTruthy();
  });
});
