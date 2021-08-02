import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MobileTransferAccountGroupComponent } from './mobile-transfer-account-group.component';

describe('MobileTransferAccountGroupComponent', () => {
  let component: MobileTransferAccountGroupComponent;
  let fixture: ComponentFixture<MobileTransferAccountGroupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MobileTransferAccountGroupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MobileTransferAccountGroupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
