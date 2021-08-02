import { TestBed } from '@angular/core/testing';

import { InvoicePaymentService } from './invoice-payment.service';

describe('InvoicePaymentService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: InvoicePaymentService = TestBed.get(InvoicePaymentService);
    expect(service).toBeTruthy();
  });
});
