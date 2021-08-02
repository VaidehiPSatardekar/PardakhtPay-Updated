import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AutoTransferListComponent } from './auto-transfer-list.component';

describe('AutoTransferListComponent', () => {
  let component: AutoTransferListComponent;
  let fixture: ComponentFixture<AutoTransferListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AutoTransferListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AutoTransferListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
