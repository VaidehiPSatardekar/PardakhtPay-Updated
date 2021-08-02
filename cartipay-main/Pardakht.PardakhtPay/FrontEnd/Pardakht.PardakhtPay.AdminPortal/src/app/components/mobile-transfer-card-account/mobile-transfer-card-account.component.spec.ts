import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MobileTransferCardAccountComponent } from './mobile-transfer-card-account.component';

describe('MobileTransferCardAccountComponent', () => {
  let component: MobileTransferCardAccountComponent;
  let fixture: ComponentFixture<MobileTransferCardAccountComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MobileTransferCardAccountComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MobileTransferCardAccountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
