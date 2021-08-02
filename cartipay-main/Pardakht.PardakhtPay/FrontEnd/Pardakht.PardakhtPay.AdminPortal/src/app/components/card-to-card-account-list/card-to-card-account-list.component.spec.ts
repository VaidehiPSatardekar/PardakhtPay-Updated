import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardToCardAccountListComponent } from './card-to-card-account-list.component';

describe('CardToCardAccountListComponent', () => {
  let component: CardToCardAccountListComponent;
  let fixture: ComponentFixture<CardToCardAccountListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CardToCardAccountListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardToCardAccountListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
