import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DepositWithdrawalChartComponent } from './deposit-withdrawal-chart.component';

describe('DepositWithdrawalChartComponent', () => {
  let component: DepositWithdrawalChartComponent;
  let fixture: ComponentFixture<DepositWithdrawalChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DepositWithdrawalChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DepositWithdrawalChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
