import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BlockedPhoneNumberListComponent } from './blocked-phone-number-list.component';

describe('BlockedPhoneNumberListComponent', () => {
  let component: BlockedPhoneNumberListComponent;
  let fixture: ComponentFixture<BlockedPhoneNumberListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BlockedPhoneNumberListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BlockedPhoneNumberListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
