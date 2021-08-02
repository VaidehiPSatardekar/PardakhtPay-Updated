import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MobileTransferAccountGroupListComponent } from './mobile-transfer-account-group-list.component';

describe('MobileTransferAccountGroupListComponent', () => {
  let component: MobileTransferAccountGroupListComponent;
  let fixture: ComponentFixture<MobileTransferAccountGroupListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MobileTransferAccountGroupListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MobileTransferAccountGroupListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
