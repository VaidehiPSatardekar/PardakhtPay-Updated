import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserSegmentGroupComponent } from './user-segment-group.component';

describe('UserSegmentGroupComponent', () => {
  let component: UserSegmentGroupComponent;
  let fixture: ComponentFixture<UserSegmentGroupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserSegmentGroupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserSegmentGroupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
