import { Component, Input, Inject, OnInit, Output, EventEmitter, OnChanges, SimpleChanges, SimpleChange } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { TenantPlatformMap } from 'app/core/models/tenant.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'tenant-select',
  templateUrl: './tenant-select.component.html',
})
export class TenantSelectComponent implements OnInit {
  tenants: TenantPlatformMap[];
  filteredTenants: TenantPlatformMap[];

  @Output() selectionChanged = new EventEmitter();
  @Input() tenants$: Observable<TenantPlatformMap[]>;
  @Input() selectedTenant: TenantPlatformMap;
  @Input() disabled: boolean;

  constructor(public translateService: TranslateService) {
  }

  ngOnInit() {
    this.tenants$.subscribe(tenants => {
      this.tenants = tenants ? tenants : [];

      if (this.selectedTenant) {
        this.selectedTenant = this.tenants.find(t => t.id == this.selectedTenant.id);
      }
    });
  }

  changed(tenant: TenantPlatformMap): void {
    this.selectionChanged.next(tenant);
  }

  getTenants = function () {
    return this.filteredTenants || this.tenants;
  }

  filterTenant = function (searchValue) {
    this.filteredTenants = this.tenants.filter(t => t.tenant.tenancyName.toLowerCase().includes(searchValue.toLowerCase()));
  }

  ngOnChanges(changes: SimpleChanges) {
    const currentItem: SimpleChange = changes.selectedTenant;

    if (currentItem && currentItem.currentValue && this.tenants) {

      this.selectedTenant = this.tenants.find(t => t.id == currentItem.currentValue.id);
    }
    else if (currentItem && currentItem.currentValue == null) {
      this.selectedTenant = null;
    }
  }
}