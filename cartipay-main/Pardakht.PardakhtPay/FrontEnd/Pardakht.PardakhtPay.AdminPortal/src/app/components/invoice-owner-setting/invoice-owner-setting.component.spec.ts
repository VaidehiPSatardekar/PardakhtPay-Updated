import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoiceOwnerSettingComponent } from './invoice-owner-setting.component';

describe('InvoiceOwnerSettingComponent', () => {
  let component: InvoiceOwnerSettingComponent;
  let fixture: ComponentFixture<InvoiceOwnerSettingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InvoiceOwnerSettingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InvoiceOwnerSettingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
