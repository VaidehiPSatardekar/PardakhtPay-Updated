import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BankLoginAccountComponent } from './bank-login-account.component';

describe('BankLoginAccountComponent', () => {
  let component: BankLoginAccountComponent;
  let fixture: ComponentFixture<BankLoginAccountComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BankLoginAccountComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BankLoginAccountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
