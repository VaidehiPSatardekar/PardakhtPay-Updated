import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BankLoginListComponent } from './bank-login-list.component';

describe('BankLoginListComponent', () => {
  let component: BankLoginListComponent;
  let fixture: ComponentFixture<BankLoginListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BankLoginListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BankLoginListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
