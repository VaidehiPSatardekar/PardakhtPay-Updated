import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoiceOwnerSettingListComponent } from './invoice-owner-setting-list.component';

describe('InvoiceOwnerSettingListComponent', () => {
  let component: InvoiceOwnerSettingListComponent;
  let fixture: ComponentFixture<InvoiceOwnerSettingListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InvoiceOwnerSettingListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoiceOwnerSettingListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
