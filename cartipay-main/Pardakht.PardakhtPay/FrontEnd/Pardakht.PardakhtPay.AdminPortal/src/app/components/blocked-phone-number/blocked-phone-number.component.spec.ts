import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BlockedPhoneNumberComponent } from './blocked-phone-number.component';

describe('BlockedPhoneNumberComponent', () => {
  let component: BlockedPhoneNumberComponent;
  let fixture: ComponentFixture<BlockedPhoneNumberComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BlockedPhoneNumberComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BlockedPhoneNumberComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
