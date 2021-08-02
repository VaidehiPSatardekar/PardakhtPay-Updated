import { TestBed } from '@angular/core/testing';

import { QRCodeRegisterService } from './qrcode-register.service';

describe('BankLoginService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
      const service: QRCodeRegisterService = TestBed.get(QRCodeRegisterService);
    expect(service).toBeTruthy();
  });
});
