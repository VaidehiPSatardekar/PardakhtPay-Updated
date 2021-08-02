import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BlockCardsComponent } from './block-cards.component';

describe('BlockCardsComponent', () => {
  let component: BlockCardsComponent;
  let fixture: ComponentFixture<BlockCardsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BlockCardsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BlockCardsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
