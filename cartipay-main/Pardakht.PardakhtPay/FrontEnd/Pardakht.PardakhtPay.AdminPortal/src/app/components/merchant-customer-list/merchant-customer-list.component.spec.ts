import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MerchantCustomerListComponent } from './merchant-customer-list.component';

describe('MerchantCustomerComponent', () => {
    let component: MerchantCustomerListComponent;
    let fixture: ComponentFixture<MerchantCustomerListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
        declarations: [MerchantCustomerListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
      fixture = TestBed.createComponent(MerchantCustomerListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
