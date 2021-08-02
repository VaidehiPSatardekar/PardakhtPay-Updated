import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginDeviceStatusListComponent } from './login-device-status-list.component';

describe('LoginDeviceStatusListComponent', () => {
    let component: LoginDeviceStatusListComponent;
    let fixture: ComponentFixture<LoginDeviceStatusListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
        declarations: [LoginDeviceStatusListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
      fixture = TestBed.createComponent(LoginDeviceStatusListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
