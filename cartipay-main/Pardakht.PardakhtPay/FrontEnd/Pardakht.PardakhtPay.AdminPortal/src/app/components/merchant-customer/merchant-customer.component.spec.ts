import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MerchantCustomerComponent } from './merchant-customer.component';

describe('MerchantCustomerComponent', () => {
  let component: MerchantCustomerComponent;
  let fixture: ComponentFixture<MerchantCustomerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MerchantCustomerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MerchantCustomerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
