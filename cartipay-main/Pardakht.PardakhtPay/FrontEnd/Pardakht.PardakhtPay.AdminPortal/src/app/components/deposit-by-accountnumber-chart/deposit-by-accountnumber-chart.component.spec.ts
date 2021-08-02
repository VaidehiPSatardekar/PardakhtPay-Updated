import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DepositByAccountNumberChartComponent } from './deposit-by-accountnumber-chart.component';

describe('DepositByAccountNumberChartComponent', () => {
    let component: DepositByAccountNumberChartComponent;
    let fixture: ComponentFixture<DepositByAccountNumberChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
        declarations: [DepositByAccountNumberChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
      fixture = TestBed.createComponent(DepositByAccountNumberChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
