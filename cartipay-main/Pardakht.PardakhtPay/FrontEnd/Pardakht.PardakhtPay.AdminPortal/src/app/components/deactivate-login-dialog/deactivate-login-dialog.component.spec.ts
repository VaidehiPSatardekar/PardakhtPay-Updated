import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DeactivateLoginDialogComponent } from './deactivate-login-dialog.component';

describe('DeactivateLoginDialogComponent', () => {
  let component: DeactivateLoginDialogComponent;
  let fixture: ComponentFixture<DeactivateLoginDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DeactivateLoginDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeactivateLoginDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
