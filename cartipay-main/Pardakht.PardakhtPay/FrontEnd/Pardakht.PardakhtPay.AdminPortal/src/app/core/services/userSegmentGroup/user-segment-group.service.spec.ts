import { TestBed } from '@angular/core/testing';

import { UserSegmentGroupService } from './user-segment-group.service';

describe('UserSegmentGroupService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: UserSegmentGroupService = TestBed.get(UserSegmentGroupService);
    expect(service).toBeTruthy();
  });
});
