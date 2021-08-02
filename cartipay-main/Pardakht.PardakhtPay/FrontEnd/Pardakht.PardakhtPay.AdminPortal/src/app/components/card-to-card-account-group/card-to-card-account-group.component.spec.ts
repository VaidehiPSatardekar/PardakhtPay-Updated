import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardToCardAccountGroupComponent } from './card-to-card-account-group.component';

describe('CardToCardAccountGroupComponent', () => {
  let component: CardToCardAccountGroupComponent;
  let fixture: ComponentFixture<CardToCardAccountGroupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CardToCardAccountGroupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardToCardAccountGroupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
