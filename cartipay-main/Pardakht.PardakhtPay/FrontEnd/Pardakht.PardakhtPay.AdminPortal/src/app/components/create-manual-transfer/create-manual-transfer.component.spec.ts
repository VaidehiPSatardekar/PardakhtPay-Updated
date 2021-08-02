import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateManualTransferComponent } from './create-manual-transfer.component';

describe('CreateManualTransferComponent', () => {
  let component: CreateManualTransferComponent;
  let fixture: ComponentFixture<CreateManualTransferComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateManualTransferComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateManualTransferComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
