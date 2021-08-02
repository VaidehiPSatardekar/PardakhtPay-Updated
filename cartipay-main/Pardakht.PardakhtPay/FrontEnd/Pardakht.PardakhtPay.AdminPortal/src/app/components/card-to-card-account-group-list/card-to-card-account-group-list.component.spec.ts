import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CardToCardAccountGroupListComponent } from './card-to-card-account-group-list.component';

describe('CardToCardAccountGroupListComponent', () => {
  let component: CardToCardAccountGroupListComponent;
  let fixture: ComponentFixture<CardToCardAccountGroupListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CardToCardAccountGroupListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CardToCardAccountGroupListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
