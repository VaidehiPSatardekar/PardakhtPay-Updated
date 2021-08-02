import { TestBed } from '@angular/core/testing';

import { TenantUrlConfigService } from './tenant-url-config.service';

describe('TenantUrlConfigService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: TenantUrlConfigService = TestBed.get(TenantUrlConfigService);
    expect(service).toBeTruthy();
  });
});
