import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RiskyKeywordComponent } from './risky-keyword.component';

describe('RiskyKeywordComponent', () => {
  let component: RiskyKeywordComponent;
  let fixture: ComponentFixture<RiskyKeywordComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RiskyKeywordComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RiskyKeywordComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
