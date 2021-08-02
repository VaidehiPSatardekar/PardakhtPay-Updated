import { TranslateService } from '@ngx-translate/core';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Store } from '@ngrx/store';
import * as coreState from './../../core/index';
import * as tenantActions from './../../core/actions/tenant.actions';
import { TenantPlatformMap } from '../../core/models/tenant.model';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { PermissionGuard } from '../../guards/permission.guard';
import { StaffUser } from '../../core/models/user-management.model';
import { fuseAnimations } from 'app/core/animations';
import { FuseConfigService } from '@fuse/services/config.service';

@Component({
  selector: 'tenant-staff-users',
  templateUrl: './tenant-staff-users.component.html',
  styleUrls: ['./tenant-staff-users.component.scss'],
  animations: fuseAnimations
})
export class TenantStaffUsersComponent implements OnInit, OnDestroy {

  tenants$: Observable<TenantPlatformMap[]>;
  loadingTenants$: Observable<boolean>;
  tenantErrors$: Observable<string>;
  selectedTenant$: Subject<TenantPlatformMap>;
  selectedTenant: TenantPlatformMap;
  selectedAllTenant: TenantPlatformMap;
  isCreate: boolean = true;
  title: any;
  tenants: TenantPlatformMap[];
  isTenantUser: boolean = false;
  isarrowBack: boolean = false;
  private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

  constructor(private store: Store<coreState.State>, private permissionGuard: PermissionGuard,
    private fuseConfigService: FuseConfigService, private translateService: TranslateService) {

    this.isCreating(false);
    this.loadingTenants$ = this.store.select(coreState.getTenantLoading);
    this.tenantErrors$ = this.store.select(coreState.getTenantMappingListError);
    this.tenants$ = this.store.select(coreState.getTenantSelectList);



    this.tenants$.pipe(takeUntil(this.destroyed$)).subscribe(tenants => {
      this.tenants = tenants ? tenants : [];

      this.permissionGuard.getCurrentUser().subscribe((user: StaffUser) => {
      debugger;

        this.isTenantUser = user.tenantGuid != "";
        const tenantPlatformMap = new TenantPlatformMap();
        tenantPlatformMap.id = 1;
        tenantPlatformMap.tenantPlatformMapGuid ="PardakhtPay";
        this.tenantSelectionChanged(tenantPlatformMap);
        // if (this.isTenantUser) {
        //   const tenantPlatformMap = tenants.find(q => q.tenantPlatformMapGuid === user.tenantGuid);
        //   this.tenantSelectionChanged(tenantPlatformMap);
        // } else {
        //   if (this.selectedAllTenant == undefined) {
        //     this.selectedAllTenant = new TenantPlatformMap();
        //     this.selectedAllTenant.tenantPlatformMapGuid = user.tenantGuid;
        //   }
        // }
      });
    });

    this.store.dispatch(new tenantActions.GetTenantSelectList());

  }

  ngOnInit(): void {
    this.store.dispatch(new tenantActions.GetTenantSelectList());
  }
  
  backArrowClicked() {

  }
  tenantSelectionChanged(tenant: TenantPlatformMap) {
    this.selectedTenant = tenant;
    if (!this.selectedTenant$) {
      this.selectedTenant$ = new Subject<TenantPlatformMap>();
    }
    this.selectedTenant$.next(tenant);
  }

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.selectedAllTenant = undefined;
    this.destroyed$.complete();
  }
  isCreating(params: any) {
    if (params) {
      this.title = this.translateService.instant('TENANT-STAFF-USERS.OPERATOR-CREATE-STAFF-USERS');
      this.isarrowBack = true;
    } else {
      this.title = this.translateService.instant('NAV-MENU.OPERATOR-TENANT-STAFF-USERS');
    }
  }
  onCancel() {
    this.title = this.translateService.instant('NAV-MENU.OPERATOR-TENANT-STAFF-USERS');
    this.isarrowBack = false;
    this.isCreate = !this.isCreate;
  }
  onArrowBack() {
    this.title = this.translateService.instant('TENANT-STAFF-USERS.OPERATOR-CREATE-STAFF-USERS');
    this.isarrowBack = !this.isarrowBack;
  }

}
