import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BlockedCardNumberListComponent } from './blocked-card-number-list.component';

describe('BlockedCardNumberListComponent', () => {
  let component: BlockedCardNumberListComponent;
  let fixture: ComponentFixture<BlockedCardNumberListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BlockedCardNumberListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BlockedCardNumberListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
