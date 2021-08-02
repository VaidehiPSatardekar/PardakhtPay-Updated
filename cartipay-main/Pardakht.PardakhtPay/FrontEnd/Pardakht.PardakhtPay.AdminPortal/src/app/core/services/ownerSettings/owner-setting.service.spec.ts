import { TestBed } from '@angular/core/testing';

import { OwnerSettingService } from './owner-setting.service';

describe('OwnerSettingService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: OwnerSettingService = TestBed.get(OwnerSettingService);
    expect(service).toBeTruthy();
  });
});
