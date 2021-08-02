import { TestBed } from '@angular/core/testing';

import { LoginDeviceStatusService } from './login-device-status.service';

describe('LoginDeviceStatusService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
      const service: LoginDeviceStatusService = TestBed.get(LoginDeviceStatusService);
    expect(service).toBeTruthy();
  });
});
