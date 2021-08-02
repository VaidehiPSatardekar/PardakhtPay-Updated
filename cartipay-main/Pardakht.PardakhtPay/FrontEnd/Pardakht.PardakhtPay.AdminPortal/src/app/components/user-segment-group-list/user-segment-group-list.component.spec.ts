import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserSegmentGroupListComponent } from './user-segment-group-list.component';

describe('UserSegmentGroupListComponent', () => {
  let component: UserSegmentGroupListComponent;
  let fixture: ComponentFixture<UserSegmentGroupListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserSegmentGroupListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserSegmentGroupListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
