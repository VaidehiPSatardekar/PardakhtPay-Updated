import { TestBed } from '@angular/core/testing';

import { CardHolderService } from './card-holder.service';

describe('CardHolderService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CardHolderService = TestBed.get(CardHolderService);
    expect(service).toBeTruthy();
  });
});
