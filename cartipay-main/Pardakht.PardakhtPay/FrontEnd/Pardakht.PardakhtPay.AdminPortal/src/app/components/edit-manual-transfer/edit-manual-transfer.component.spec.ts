import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditManualTransferComponent } from './edit-manual-transfer.component';

describe('EditManualTransferComponent', () => {
  let component: EditManualTransferComponent;
  let fixture: ComponentFixture<EditManualTransferComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditManualTransferComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditManualTransferComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
