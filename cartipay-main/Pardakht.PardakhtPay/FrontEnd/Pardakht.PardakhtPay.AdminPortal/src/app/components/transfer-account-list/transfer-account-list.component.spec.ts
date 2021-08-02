import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TransferAccountListComponent } from './transfer-account-list.component';

describe('TransferAccountListComponent', () => {
  let component: TransferAccountListComponent;
  let fixture: ComponentFixture<TransferAccountListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TransferAccountListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TransferAccountListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
