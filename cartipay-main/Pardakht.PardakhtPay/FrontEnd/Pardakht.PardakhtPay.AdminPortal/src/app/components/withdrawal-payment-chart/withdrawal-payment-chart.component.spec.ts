import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WithdrawalPaymentChartComponent } from './withdrawal-payment-chart.component';

describe('WithdrawalPaymentChartComponent', () => {
  let component: WithdrawalPaymentChartComponent;
  let fixture: ComponentFixture<WithdrawalPaymentChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WithdrawalPaymentChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WithdrawalPaymentChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
