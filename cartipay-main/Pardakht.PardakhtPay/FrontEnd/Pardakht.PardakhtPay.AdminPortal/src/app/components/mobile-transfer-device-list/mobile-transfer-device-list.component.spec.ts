import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MobileTransferDeviceListComponent } from './mobile-transfer-device-list.component';

describe('MobileTransferDeviceListComponent', () => {
  let component: MobileTransferDeviceListComponent;
  let fixture: ComponentFixture<MobileTransferDeviceListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MobileTransferDeviceListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MobileTransferDeviceListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
