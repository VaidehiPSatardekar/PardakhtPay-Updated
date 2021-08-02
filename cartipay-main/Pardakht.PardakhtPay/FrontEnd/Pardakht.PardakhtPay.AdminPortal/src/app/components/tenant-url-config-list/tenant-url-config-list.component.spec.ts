import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TenantUrlConfigListComponent } from './tenant-url-config-list.component';

describe('TenantUrlConfigListComponent', () => {
  let component: TenantUrlConfigListComponent;
  let fixture: ComponentFixture<TenantUrlConfigListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TenantUrlConfigListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TenantUrlConfigListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
