import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DeleteLoginDialogComponent } from './delete-login-dialog.component';

describe('DeleteLoginDialogComponent', () => {
  let component: DeleteLoginDialogComponent;
  let fixture: ComponentFixture<DeleteLoginDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DeleteLoginDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeleteLoginDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
