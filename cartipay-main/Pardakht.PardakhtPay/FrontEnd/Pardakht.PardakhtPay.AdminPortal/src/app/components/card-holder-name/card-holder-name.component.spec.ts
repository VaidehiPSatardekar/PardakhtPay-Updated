import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardHolderNameComponent } from './card-holder-name.component';

describe('CardHolderNameComponent', () => {
  let component: CardHolderNameComponent;
  let fixture: ComponentFixture<CardHolderNameComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CardHolderNameComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardHolderNameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
