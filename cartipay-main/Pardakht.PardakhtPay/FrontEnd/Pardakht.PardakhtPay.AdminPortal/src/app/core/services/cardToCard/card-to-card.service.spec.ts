import { TestBed } from '@angular/core/testing';

import { CardToCardService } from './card-to-card.service';

describe('CardToCardService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CardToCardService = TestBed.get(CardToCardService);
    expect(service).toBeTruthy();
  });
});
