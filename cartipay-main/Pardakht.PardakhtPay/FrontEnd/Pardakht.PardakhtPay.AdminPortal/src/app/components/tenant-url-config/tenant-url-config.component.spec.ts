import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TenantUrlConfigComponent } from './tenant-url-config.component';

describe('TenantUrlConfigComponent', () => {
  let component: TenantUrlConfigComponent;
  let fixture: ComponentFixture<TenantUrlConfigComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TenantUrlConfigComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TenantUrlConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
