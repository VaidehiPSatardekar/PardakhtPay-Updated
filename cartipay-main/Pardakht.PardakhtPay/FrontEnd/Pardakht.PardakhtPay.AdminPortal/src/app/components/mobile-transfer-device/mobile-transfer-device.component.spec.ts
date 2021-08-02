import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MobileTransferDeviceComponent } from './mobile-transfer-device.component';

describe('MobileTransferDeviceComponent', () => {
  let component: MobileTransferDeviceComponent;
  let fixture: ComponentFixture<MobileTransferDeviceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MobileTransferDeviceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MobileTransferDeviceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
