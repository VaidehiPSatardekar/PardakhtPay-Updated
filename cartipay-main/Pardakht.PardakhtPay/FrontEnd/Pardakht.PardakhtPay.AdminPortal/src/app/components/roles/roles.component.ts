import { Component, OnInit, ViewChild, OnDestroy, Output, EventEmitter } from '@angular/core';
import { MatTabGroup } from '@angular/material';
import { ReplaySubject } from 'rxjs';
import { Role } from '../../core/models/user-management.model';
import { Store } from '@ngrx/store';
import * as coreState from './../../core/index';
import * as roleActions from './../../core/actions/role';
import { takeUntil } from 'rxjs/operators';
import { PermissionGuard } from '../../guards/permission.guard';
import { TranslateService } from '@ngx-translate/core';
import { fuseAnimations } from 'app/core/animations';
import { Permissions } from './../../models/index';


@Component({
  selector: 'tcam-roles',
  templateUrl: './roles.component.html',
  styleUrls: ['./roles.component.scss'],
  animations: fuseAnimations
})
export class RolesComponent implements OnInit, OnDestroy {

  isCreating: boolean = false;

  @ViewChild('tabgroup') tabGroup: MatTabGroup;
  permissions = Permissions;

  // tenants$: Observable<TenantPlatformMap[]>;
  providerRoles: Role[];
  tenantRoles: Role[];
  selectedTabIndex = 0;

  private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

  constructor(private permissionGuard: PermissionGuard,
    private store: Store<coreState.State>,
    private translateService: TranslateService) { }

  ngOnInit(): void {
    this.store.select(coreState.getRolesResults).pipe(takeUntil(this.destroyed$))
      .subscribe((roles: Role[]) => {
        if (roles) {
          this.providerRoles = roles.filter((role: Role) => role.roleHolderTypeId === 'P');
          this.tenantRoles = roles.filter((role: Role) => role.roleHolderTypeId === 'T');
          console.log(this.tenantRoles);
          this.onTranslateProviderRoles();
          // this.onTranslateTenantRoles();
        }
      });

    this.store.dispatch(new roleActions.GetAllRoles(undefined));
    this.store.dispatch(new roleActions.GetPermissionGroups());

    this.selectedTabIndex = this.canDisplay(this.permissions.RoleEditProvider) ? 0 : 1;

    this.store.select(coreState.getRoleCreateSuccess).pipe(takeUntil(this.destroyed$)).subscribe((result) => {
      if (result !== undefined && result === true) {
        this.isCreating = false;
        this.store.dispatch(new roleActions.GetAllRoles(undefined));
        this.store.dispatch(new roleActions.GetPermissionGroups());
      }
    });
  }
  onTranslateProviderRoles() {
    for (let i = 0; i < this.providerRoles.length; i++) {
      const element = this.providerRoles[i];
      element.name = this.translateService.instant('ROLE.PROVIDER-ROLE-LIST.' + element.name);
    }
  }
  onTranslateTenantRoles() {
    for (let i = 0; i < this.tenantRoles.length; i++) {
      const element = this.tenantRoles[i];
      element.name = this.translateService.instant('ROLE.TENANT-ROLE-LIST.' + element.name);
    }
  }
  ngOnDestroy(): void {
    this.destroyed$.next(true);
    this.destroyed$.complete();
    this.store.dispatch(new roleActions.ClearErrors());
  }

  isTabActive(index: number): boolean {
    return this.selectedTabIndex === index;
    // return (this.tabGroup.selectedIndex === 0);
  }

  onSelectedTabChange(args): void {
    // hack to get grids on tabs to resize only when visible
    this.selectedTabIndex = args.index;
  }

  canDisplay(item: string): boolean {
    return this.permissionGuard.isUserAuthorized(item);
  }

  onCloseEvent(): void {
    this.isCreating = false;
    this.store.dispatch(new roleActions.ClearErrors());
  }

  onCreateRoleClick(): void {
    this.isCreating = !this.isCreating;
    if (this.isCreating) {
    }
  }

  onCloseCreateEdit(): void {
    this.isCreating = false;
  }
}
