import { TestBed } from '@angular/core/testing';

import { ApplicationSettingsService } from './application-settings.service';
import { TranslateModule } from '@ngx-translate/core';

describe('ApplicationSettingsService', () => {
    beforeEach(() => TestBed.configureTestingModule({
        }));

  it('should be created', () => {
    const service: ApplicationSettingsService = TestBed.get(ApplicationSettingsService);
    expect(service).toBeTruthy();
  });
});
