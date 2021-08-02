import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BankStatementListComponent } from './bank-statement-list.component';

describe('BankStatementListComponent', () => {
  let component: BankStatementListComponent;
  let fixture: ComponentFixture<BankStatementListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BankStatementListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BankStatementListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
