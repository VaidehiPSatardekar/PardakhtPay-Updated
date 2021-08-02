import { TestBed } from '@angular/core/testing';

import { InvoiceOwnerSettingService } from './invoice-owner-setting.service';

describe('InvoiceOwnerSettingService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: InvoiceOwnerSettingService = TestBed.get(InvoiceOwnerSettingService);
    expect(service).toBeTruthy();
  });
});
