import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ManualTransferListComponent } from './manual-transfer-list.component';

describe('ManualTransferListComponent', () => {
  let component: ManualTransferListComponent;
  let fixture: ComponentFixture<ManualTransferListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ManualTransferListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ManualTransferListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
