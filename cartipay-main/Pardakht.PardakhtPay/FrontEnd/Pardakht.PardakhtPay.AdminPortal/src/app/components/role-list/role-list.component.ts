import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { Observable, ReplaySubject } from 'rxjs';
import { Role, PermissionGroup, CloneRoleRequest } from '../../core/models/user-management.model';
import { Store } from '@ngrx/store';
import * as coreState from './../../core/index';
import * as roleActions from './../../core/actions/role';
import { takeUntil } from 'rxjs/operators';
import { ValueFormatterParams } from 'ag-grid-community';
import { GridTranslate } from 'app/core/helpers/grid-translate';
import { FuseConfigService } from '@fuse/services/config.service';

@Component({
  selector: 'tcam-role-list',
  templateUrl: './role-list.component.html',
  styleUrls: ['./role-list.component.scss']
})
export class RoleListComponent implements OnInit, OnDestroy {

  @Input() roles: Role[];
  @Input() isActive = false;
  @Input() isTenantRole = false;
  @Input() showRestrictedPermissions = false;

  columnDefs;
  enableRtl;
  rowSelection;
  frameworkComponents;
  rowClassRules;
  localeText;
  selectedRole: Role;
  // tenants$: Observable<TenantPlatformMap[]>;
  loading$: Observable<boolean>;
  error$: Observable<string>;
  isCreating = false;
  permissionGroups$: Observable<PermissionGroup[]>;
  errors$: Observable<{ [key: string]: string }>;

  private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

  constructor(private store: Store<coreState.State>,
    private translateService: TranslateService,
    private fuseConfigService: FuseConfigService,
    private gridTranslate: GridTranslate) { }

  ngOnInit(): void {
    this.localeText = this.gridTranslate.translateAgGrid();

    this.loading$ = this.store.select(coreState.getRolesLoading);
    this.errors$ = this.store.select(coreState.getRolesError);
    this.permissionGroups$ = this.store.select(coreState.getPermissionGroupsResults);
    this.fuseConfigService.config.pipe(takeUntil(this.destroyed$)).subscribe(config => {
      this.enableRtl = config.locale != null && config.locale != undefined && config.locale.direction == 'rtl';
    });
    this.store.select(coreState.getRoleUpdateSuccess).pipe(takeUntil(this.destroyed$))
      .subscribe((result: any) => {
        if (result !== undefined) {
          this.selectedRole = undefined;
        }
      });

    this.initGrid();
  }

  ngOnDestroy(): void {
    this.destroyed$.next(true);
    this.destroyed$.complete();
  }

  onGridReady(params): void {
    // this.gridApi = params.api;
    // this.gridColumnApi = params.columnApi;

    params.api.sizeColumnsToFit();

    // window.addEventListener('resize', () => 
    //   setTimeout(() => params.api.sizeColumnsToFit())
    // );
  }

  onRowSelected(event): void {
    if (event.node.selected) {
      this.selectedRole = event.data;
      // console.log(this.selectedUser);
    } else {
      this.selectedRole = undefined;
    }
  }

  canDisplay(item: string): boolean {
    return true;
    // return this.guard.isAllowed(item);
  }

  onCreateRole(request: Role): void {
    // // console.log(request);
    // request.roleHolderTypeId = this.tabGroup.selectedIndex === 0 ? 'P' : 'T';
    // if (this.loggedOnUserIsTenant) {
    //   // tenant cannot set these values
    //   request.tenantId = this.loggedOnUser.tenantId;
    //   request.roleHolderTypeId = 'T';
    //   request.isFixed = false;
    //   console.log(request);
    // }

    // this.formSubmit = true;
    // this.store.dispatch(new roleActions.CreateRole(request));
  }

  onEditRole(request: Role): void {
    // console.log(request);
    // this.formSubmit = true;
    this.store.dispatch(new roleActions.EditRole(request));
  }

  onCloneRole(request: CloneRoleRequest): void {
    // if (this.loggedOnUserIsTenant) {
    //   request.tenantId = this.loggedOnUser.tenantId;
    // }
    // this.store.dispatch(new roleActions.CloneRole(request));
  }

  onCloseRole(): void {
    this.selectedRole = undefined;
    this.store.dispatch(new roleActions.ClearErrors());
  }

  canEditRole(role: Role): boolean {
    // if (role.isFixed) {
    //   if (!this.canDisplay('ROLE-EDIT-FIXED')) {
    //     return false;
    //   }
    // }
    // if (role.roleHolderTypeId === 'P') {
    //   if (!this.canDisplay('ROLE-EDIT-PVR')) {
    //     return false;
    //   }
    // }
    // if (role.roleHolderTypeId === 'T') {
    //   if (!this.canDisplay('ROLE-EDIT-TNT')) {
    //     return false;
    //   }
    // }

    return true;
  }

  private initGrid(): void {
    this.columnDefs = this.buildColDefs();
    // this.frameworkComponents = {
    //   dateFormatterComponent: DateFormatterComponent,
    //   scoreFormatterComponent: ScoreFormatterComponent,
    //   timeFormatterComponent: TimeFormatterComponent,
    //   distinctValueFilterComponent: DistinctValueFilterComponent
    // };
    this.rowSelection = 'single';
    // this.rowClassRules = {
    //   'light-blue-600 font-weight-600': (params: any) => params.data.sportEvent.statusDescription === 'Live'
    // };
  }

  private buildColDefs(): any {
    if (this.isTenantRole) {
      return [
        { headerName: this.translateService.instant('ROLE.LIST-COLUMNS.ROLE-NAME'), field: 'name', sortable: true, filter: true, resizable: true, suppressSizeToFit: true, minWidth: 200, width: 200, maxWidth: 300 },
        {
          headerName: this.translateService.instant('ROLE.LIST-COLUMNS.GLOBAL'), field: 'isFixed', valueFormatter: this.boolValueFormatter.bind(this),
          sortable: true, filter: true, resizable: true, suppressSizeToFit: true, minWidth: 100, width: 100, maxWidth: 100,
          cellClass: params => {
            if (params.value == null && params.value == undefined)
              return null;
            if (params.value) {
              return 'success';
            } else {
              return 'rejected';
            }
          }
        }, { headerName: this.translateService.instant('ROLE.LIST-COLUMNS.TENANCY-NAME'), field: 'tenancyName', sortable: true, filter: true, resizable: true, suppressSizeToFit: true, minWidth: 200, width: 200, maxWidth: 300 }
      ];
    }
    else {
      return [
        { headerName: this.translateService.instant('ROLE.LIST-COLUMNS.ROLE-NAME'), field: 'name', sortable: true, filter: true, resizable: true, suppressSizeToFit: true, minWidth: 200, width: 200, maxWidth: 300 },
        {
          headerName: this.translateService.instant('ROLE.LIST-COLUMNS.GLOBAL'), field: 'isFixed',
          valueFormatter: this.boolValueFormatter.bind(this),
          cellClass: params => {
            if (params.value == null && params.value == undefined)
              return null;
            if (params.value) {
              return 'success';
            } else {
              return 'rejected';
            }
          },
          sortable: true, filter: true, resizable: true, suppressSizeToFit: true, minWidth: 100, width: 100, maxWidth: 100
        }
      ];

    }
  }

  boolValueFormatter(params: ValueFormatterParams): any {
    if (params.value && typeof params.value === 'number') {
    }
    else if (params.value != undefined) {
      return this.translateService.instant(`DOMAIN.LIST-COLUMNS.FLAG.${(params.value.toString()).toUpperCase()}`);
    }
  }
}
