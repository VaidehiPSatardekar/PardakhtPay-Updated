import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MobileTransferAccountListComponent } from './mobile-transfer-account-list.component';

describe('MobileTransferAccountListComponent', () => {
  let component: MobileTransferAccountListComponent;
  let fixture: ComponentFixture<MobileTransferAccountListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MobileTransferAccountListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MobileTransferAccountListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
