import { TestBed } from '@angular/core/testing';

import { AccountingService } from './accounting.service';
import { TranslateModule } from '@ngx-translate/core';

describe('AccountingService', () => {
    beforeEach(() => TestBed.configureTestingModule({
        }));

  it('should be created', () => {
    const service: AccountingService = TestBed.get(AccountingService);
    expect(service).toBeTruthy();
  });
});
