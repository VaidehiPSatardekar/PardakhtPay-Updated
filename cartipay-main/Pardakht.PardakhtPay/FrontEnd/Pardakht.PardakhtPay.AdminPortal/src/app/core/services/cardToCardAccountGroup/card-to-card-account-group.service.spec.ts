import { TestBed } from '@angular/core/testing';

import { CardToCardAccountGroupService } from './card-to-card-account-group.service';

describe('CardToCardAccountGroupService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CardToCardAccountGroupService = TestBed.get(CardToCardAccountGroupService);
    expect(service).toBeTruthy();
  });
});
