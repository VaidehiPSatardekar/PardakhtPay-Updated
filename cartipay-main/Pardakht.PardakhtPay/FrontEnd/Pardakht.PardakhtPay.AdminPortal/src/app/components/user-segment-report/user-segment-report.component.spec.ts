import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserSegmentReportComponent } from './user-segment-report.component';

describe('UserSegmentReportComponent', () => {
  let component: UserSegmentReportComponent;
  let fixture: ComponentFixture<UserSegmentReportComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserSegmentReportComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserSegmentReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
