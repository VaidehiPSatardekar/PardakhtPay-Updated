import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardToCardAccountComponent } from './card-to-card-account.component';

describe('CardToCardAccountComponent', () => {
  let component: CardToCardAccountComponent;
  let fixture: ComponentFixture<CardToCardAccountComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CardToCardAccountComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardToCardAccountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
