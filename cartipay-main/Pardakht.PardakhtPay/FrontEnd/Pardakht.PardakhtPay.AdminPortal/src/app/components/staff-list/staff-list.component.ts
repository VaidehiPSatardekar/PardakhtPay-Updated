import { Component, Input, OnInit, ViewEncapsulation, HostListener, OnDestroy, Output, EventEmitter } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { FormControl } from '@angular/forms';
import { EXPANSION_PANEL_ANIMATION_TIMING } from '@angular/material';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { ReplaySubject } from 'rxjs';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { NotificationsService, NotificationType } from 'angular2-notifications';
import * as coreState from './../../core/index';
// import * as accountActions from './../actions/account';
import * as userActions from './../../core/actions/user';
import * as roleActions from './../../core/actions/role';
import { Role, StaffUser, UserType } from './../../core/models/user-management.model';
import { takeUntil, filter } from 'rxjs/operators';
import { PlatformConfig } from '../../core/models/platform.model';
import { UserService } from '../../core/services/user.service';
import { CellClickedEvent } from 'ag-grid-community';
import { Permissions } from '../../models/index';
import { TenantPlatformMap } from '../../core/models/tenant.model';
import { FuseConfigService } from '@fuse/services/config.service';
import { GridTranslate } from 'app/core/helpers/grid-translate';
import { FormHelper } from 'app/helpers/forms/form-helper';
import { LoginAsFormatterComponent } from '../formatter/loginasformater';

@Component({
    selector: 'tcam-staff-list',
    templateUrl: './staff-list.component.html',
    styleUrls: ['./staff-list.component.scss'],
    // encapsulation: ViewEncapsulation.Emulated,
    // animations: [
    //   trigger('indicatorRotate', [
    //     state('collapsed', style({ transform: 'rotate(0deg)' })),
    //     state('expanded', style({ transform: 'rotate(180deg)' })),
    //     transition('expanded <=> collapsed', animate(EXPANSION_PANEL_ANIMATION_TIMING))
    //   ])
    // ]
})

export class StaffListComponent implements OnInit, OnDestroy {

    @Input() tenantGuid: string;
    @Input() tenantPlatformMap: TenantPlatformMap;
    @Input() tenantGuid$: Observable<TenantPlatformMap>;
    @Input() roleFilter: string;
    @Input() autoResize = true;
    @Input() staffUser = true;
    @Input() affiliateUser = true;
    @Input() systemUser = true;
    @Input() isOperator = false;
    @Input() tenantStaffUser = false;
    @Output() isCreatingInfo: EventEmitter<boolean> = new EventEmitter();
    @Output() cancel: EventEmitter<boolean> = new EventEmitter();
    @Output() onarrowback: EventEmitter<boolean> = new EventEmitter();
    @Output() usersResponse: EventEmitter<StaffUser[]> = new EventEmitter();

    selectedBrandId = 0;
    gridApi;
    columnDefs;
    rowSelection;
    frameworkComponents;
    selected = [];
    rowClassRules;
    enableRtl: boolean = false;
    localeText;
    selectedUser: StaffUser;
    roles: Role[];
    users: StaffUser[];
    filteredUsers: StaffUser[];
    filter: string;
    // platformConfig$: Observable<PlatformConfig>;
    platformConfig: PlatformConfig;
    status: any[] = [];
    isCreating = false;
    loading$: Observable<boolean>;
    errors$: Observable<string>;
    loginAsError$: Observable<string>;
    loginAsAllowed: boolean = false;
    gridOptions: any
    isSubscribed: boolean = false;
    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    // constructor(private store: Store<coreState.State>, private guard: PermissionGuard) {}
    constructor(private store: Store<coreState.State>, private notificationService: NotificationsService,
        private fuseConfigService: FuseConfigService, private translateService: TranslateService,
        private userService: UserService,
        private gridTranslate: GridTranslate) {
        this.loginAsAllowed = this.userService.isUserAuthorizedForTask(Permissions.LoginAsStaffUser);
    }

    ngOnInit(): void {
        this.localeText = this.gridTranslate.translateAgGrid();

        if (this.roleFilter == undefined) {
            this.roleFilter = 'T';
        }

        this.loading$ = this.store.select(coreState.getUserLoading);
        this.errors$ = this.store.select(coreState.getUserGetError);
        this.loginAsError$ = this.store.select(coreState.getUserLoginAsError);

        this.fuseConfigService.config.pipe(takeUntil(this.destroyed$)).subscribe(config => {
            this.enableRtl = config.locale != null && config.locale != undefined && config.locale.direction == 'rtl';
        });

        this.store.select(coreState.getPlatformConfig).pipe(takeUntil(this.destroyed$))
            .subscribe((result: PlatformConfig) => {
                this.platformConfig = result;
            });

        this.store.select(coreState.getRolesResults).pipe(takeUntil(this.destroyed$))
            .subscribe((result: Role[]) => {
                if (result) {
                    this.roles = this.filterRoles(result);
                    //console.log(this.roles);
                }
            });

        this.store.select(coreState.getStaffUsers).pipe(takeUntil(this.destroyed$))
            .subscribe(items => {
                this.users = items;
                this.updateFilter();
            });

        if (this.tenantGuid$) {
            this.tenantGuid$.pipe(takeUntil(this.destroyed$))
                .subscribe((tenantGuid: TenantPlatformMap) => {
                    if (tenantGuid) {
                        this.tenantPlatformMap = tenantGuid;
                        this.selectedBrandId = 0;
                        this.tenantGuid = tenantGuid.tenantPlatformMapGuid;
                        this.selectedUser = undefined;
                        this.store.dispatch(new roleActions.GetAllRoles(this.tenantGuid));
                        this.store.dispatch(new userActions.GetStaffUsers(this.tenantGuid, this.selectedBrandId === 0 ? null : this.selectedBrandId));
                    }
                });
        }

        if (!this.tenantStaffUser) {
            this.store.dispatch(new roleActions.GetAllRoles(this.tenantGuid));
            this.store.dispatch(new userActions.GetStaffUsers(this.tenantGuid, this.selectedBrandId === 0 ? null : this.selectedBrandId));
        } else {
            this.filteredUsers = [];
            this.users = [];
        }


        // this.store.select(coreState.getUserCreated).pipe(takeUntil(this.destroyed$))
        //   .subscribe((result: any) => {
        //     if (result !== undefined) {
        //       this.isCreating = false;
        //     }
        //   });

        this.store.select(coreState.getUserUpdateSuccess).pipe(takeUntil(this.destroyed$))
            .subscribe((result: boolean) => {
                if (result !== undefined && result === true) {
                    this.selectedUser = undefined;
                }
            });

        this.initGrid();
    }

    ngOnDestroy(): void {
        this.tenantGuid = undefined;
        this.tenantGuid$ = undefined;
        this.filteredUsers = undefined;
        this.destroyed$.next();
        this.destroyed$.complete();
        this.store.dispatch(new userActions.ClearErrors());
    }

    getErrorMessage(control: FormControl): string {
        return FormHelper.getErrorMessage(control);
    }

    canDisplay(item: string): boolean {
        // return this.guard.isAllowed(item);
        return true;
    }
    create() {
        this.isCreating = !this.isCreating;
        this.isCreatingInfo.emit(undefined);
    }
    // onBlockUser(data: any): void {
    //  this.store.dispatch(new userActions.BlockStaffUser(data.accountId, data.value));
    // }
    onCancel() {
        this.isCreating = false;
        this.cancel.emit(undefined);
    }
    onResetPassword(event: any): void {
        // this.store.dispatch(new accountActions.SendPasswordResetToken(accountId, ''));
    }

    onCloseUser(): void {
        this.selectedUser = undefined;
    }

    getParentAccountEnabled(): boolean {
        // if this is being used in the provider context, account owners do not apply
        return this.roleFilter === 'P' ? false : this.platformConfig.parentAccountEnabled;
    }

    private filterRoles(roles: Role[]): Role[] {
        // only allow roles related to this entity type (provider or tenant)
        return roles.filter((value: Role) => value.roleHolderTypeId === this.roleFilter).map((value: Role) => {
            // remove child arrays to prevent json serialization / circular reference issues
            value.users = [];
            value.permissions = [];

            return value;
        });
    }

    private initGrid(): void {
        this.frameworkComponents = {
            loginAsFormatter: LoginAsFormatterComponent
        }

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

        this.gridOptions = {
            onColumnMoved: this.onColumnMoved
        }
    }

    onColumnMoved(params) {
        var columnState = JSON.stringify(params.columnApi.getColumnState());
        localStorage.setItem('StaffListColumnState', columnState);
    }

    setColumnState(params: any) {
        var columnState = JSON.parse(localStorage.getItem('StaffListColumnState'));
        if (columnState) {
            params.columnApi.setColumnState(columnState);
        }
    }

    private buildColDefs(): any {
        var columns = [
            { headerName: this.translateService.instant('PROVIDER.LIST-COLUMNS.USER-NAME'), field: 'username', sortable: true, filter: true, resizable: true, suppressSizeToFit: true, minWidth: 150, width: 200, maxWidth: 200, cellRenderer: undefined },
            { headerName: this.translateService.instant('PROVIDER.LIST-COLUMNS.FIRST-NAME'), field: 'firstName', sortable: true, filter: true, resizable: true, minWidth: 150, width: 250, maxWidth: 250, cellRenderer: undefined },
            { headerName: this.translateService.instant('PROVIDER.LIST-COLUMNS.LAST-NAME'), field: 'lastName', sortable: true, filter: true, resizable: true, minWidth: 150, width: 250, maxWidth: 250, suppressSizeToFit: true, cellRenderer: undefined },
            { headerName: this.translateService.instant('PROVIDER.LIST-COLUMNS.EMAIL'), field: 'email', sortable: true, filter: true, resizable: true, minWidth: 200, width: 300, maxWidth: 400, suppressSizeToFit: true, cellRenderer: undefined },
        ];

        if (this.loginAsAllowed) {
            columns = [{ headerName: this.translateService.instant('PROVIDER.LIST-COLUMNS.LOGIN-AS'), field: '', sortable: true, filter: true, resizable: true, minWidth: 90, maxWidth: 90, suppressSizeToFit: false, width: 90, cellRenderer: 'loginAsFormatter' }, ...columns];
        }

        return columns;
    }

    @HostListener('window:resize')
    onResize(): void {
        if (!this.gridApi) {
            return;
        }

        if (this.autoResize) {
            setTimeout(() => {
                this.gridApi.sizeColumnsToFit();
            });
        }
    }

    onGridReady(params): void {
        this.gridApi = params.api;
        this.setColumnState(params);
        if (this.autoResize) {
            window.setTimeout(() => {
                params.api.sizeColumnsToFit();
            });
        }
    }

    onCellClicked(e: CellClickedEvent) {
        this.onarrowback.emit(undefined);
        if (e && e.colDef && e.colDef.field != '') {
            if (e.data) {
                this.selectedUser = e.data;
            }
        }
    }

    //onRowSelected(event): void {
    //    if (event && event.colDef && event.colDef.colId != '') {
    //        // console.log(event);
    //        if (event.node.selected) {
    //            this.selectedUser = event.data;
    //            // console.log(this.selectedUser);
    //        } else {
    //            this.selectedUser = undefined;
    //        }
    //    }
    //}

    brandSelectionChanged(): void {
        this.store.dispatch(new userActions.GetStaffUsers(this.tenantGuid, this.selectedBrandId === 0 ? null : this.selectedBrandId));
    }

    updateFilter(): void {
        if (!this.users) {
            this.filteredUsers = [];
        }
        if (this.filter === undefined || this.filter == null || this.filter === '') {
            this.filteredUsers = this.users;
        } else {
            if (this.users === undefined) {
                this.filteredUsers = [];
                return;
            }
            this.filteredUsers = this.users.filter(t => (t.username != null && t.username.indexOf(this.filter) !== -1)
                || (t.firstName != null && t.firstName.indexOf(this.filter) !== -1)
                || (t.lastName != null && t.lastName.indexOf(this.filter) !== -1)
                || (t.email != null && t.email.indexOf(this.filter) !== -1)
            );
        }
        this.filterUserType();

    }

    filterUserType() {
        if (this.filteredUsers) {
            if (this.filteredUsers.length > 0) {
                if (!this.systemUser)
                    this.filteredUsers = this.filteredUsers.filter(x => x.userType != UserType.api);
                if (!this.staffUser)
                    this.filteredUsers = this.filteredUsers.filter(x => x.userType != UserType.standard);
                if (!this.affiliateUser)
                    this.filteredUsers = this.filteredUsers.filter(x => x.userType != UserType.affiliate);
                if (this.isOperator) {
                    this.usersResponse.emit(this.filteredUsers);
                }
            }
        }


    }

}
