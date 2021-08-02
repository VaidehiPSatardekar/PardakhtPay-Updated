import { TestBed } from '@angular/core/testing';

import { RiskyKeywordService } from './risky-keyword.service';

describe('RiskyKeywordService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: RiskyKeywordService = TestBed.get(RiskyKeywordService);
    expect(service).toBeTruthy();
  });
});
