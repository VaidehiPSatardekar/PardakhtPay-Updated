import { TestBed } from '@angular/core/testing';

import { TimeZoneService } from './time-zone.service';

describe('TimeZoneService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: TimeZoneService = TestBed.get(TimeZoneService);
    expect(service).toBeTruthy();
  });
});
