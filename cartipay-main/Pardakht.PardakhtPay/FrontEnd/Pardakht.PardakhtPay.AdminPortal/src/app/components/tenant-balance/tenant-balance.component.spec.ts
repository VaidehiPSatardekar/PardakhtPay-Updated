import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TenantBalanceComponent } from './tenant-balance.component';

describe('TenantBalanceComponent', () => {
  let component: TenantBalanceComponent;
  let fixture: ComponentFixture<TenantBalanceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TenantBalanceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TenantBalanceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
