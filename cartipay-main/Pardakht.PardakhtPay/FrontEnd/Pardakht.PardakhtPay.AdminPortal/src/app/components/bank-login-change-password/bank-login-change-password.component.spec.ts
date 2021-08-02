import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BankLoginChangePasswordComponent } from './bank-login-change-password.component';

describe('BankLoginChangePasswordComponent', () => {
  let component: BankLoginChangePasswordComponent;
  let fixture: ComponentFixture<BankLoginChangePasswordComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BankLoginChangePasswordComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BankLoginChangePasswordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
