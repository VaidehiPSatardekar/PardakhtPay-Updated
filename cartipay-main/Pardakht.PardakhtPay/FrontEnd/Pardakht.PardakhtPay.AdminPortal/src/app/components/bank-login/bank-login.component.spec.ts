import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BankLoginComponent } from './bank-login.component';

describe('BankLoginComponent', () => {
  let component: BankLoginComponent;
  let fixture: ComponentFixture<BankLoginComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BankLoginComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BankLoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
