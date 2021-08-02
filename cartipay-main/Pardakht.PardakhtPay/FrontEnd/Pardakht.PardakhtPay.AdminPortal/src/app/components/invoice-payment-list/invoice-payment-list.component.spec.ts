import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoicePaymentListComponent } from './invoice-payment-list.component';

describe('InvoicePaymentListComponent', () => {
  let component: InvoicePaymentListComponent;
  let fixture: ComponentFixture<InvoicePaymentListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InvoicePaymentListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoicePaymentListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
