import { Component, OnInit } from '@angular/core';
import { PermissionGuard } from './../../guards/permission.guard';
import { StaffUser } from '../../core/models/user-management.model';
import { TranslateService } from '@ngx-translate/core';
import { fuseAnimations } from 'app/core/animations';


@Component({
  selector: 'tcam-provider-staff-list',
  templateUrl: './provider-staff-list.component.html',
  styleUrls: ['./provider-staff-list.component.scss'],
  animations: fuseAnimations
})
export class ProviderStaffListComponent implements OnInit {

  isTenantUser: any;
  title: any;
  isCreate: boolean = true;
  isarrowBack: boolean = false;

  constructor(private permissionGuard: PermissionGuard, private translateService: TranslateService) {

    this.isCreating(false);
    this.permissionGuard.getCurrentUser().subscribe((user: StaffUser) => {
      this.isTenantUser = user.tenantGuid != "";
      if (this.isTenantUser) {
        this.permissionGuard.logout();
      }
    });
  }

  ngOnInit(): void {
  }

  isCreating(params: any) {
    if (params) {
      this.title = this.translateService.instant('PROVIDER.CREATE-PROVIDER-USER');
      this.isarrowBack = true;

    } else {
      this.title = this.translateService.instant('PROVIDER.PROVIDER-MANAGEMENT');
    }
  }
  onCancel() {
    this.title = this.translateService.instant('PROVIDER.PROVIDER-MANAGEMENT');
    this.isarrowBack = false;
    this.isCreate = !this.isCreate;
  }
  onArrowBack() {
    this.title = this.translateService.instant('PROVIDER.CREATE-PROVIDER-USER');
    this.isarrowBack = !this.isarrowBack;
  }

}
