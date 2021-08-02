import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BlockedCardNumberComponent } from './blocked-card-number.component';

describe('BlockedCardNumberComponent', () => {
  let component: BlockedCardNumberComponent;
  let fixture: ComponentFixture<BlockedCardNumberComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BlockedCardNumberComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BlockedCardNumberComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
