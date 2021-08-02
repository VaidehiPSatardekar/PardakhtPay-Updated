import { Component, OnInit, OnDestroy } from '@angular/core';
import { Owner } from '../../models/account.model';
import * as tenantActions from '../../core/actions/tenant';
import * as accountActions from '../../core/actions/account';
import * as loginDeviceActions from '../../core/actions/loginDeviceStatus';
import { BankLogin, BankLoginStatus, LoginDeviceStatuses } from '../../models/bank-login';
import * as coreState from '../../core';
import { Store } from '@ngrx/store';
import { DateAdapter, MatSnackBar, MatDialog } from '@angular/material';
import { fuseAnimations } from '../../core/animations';
import { filter } from 'rxjs/operators/filter';
import { takeUntil, take } from 'rxjs/operators';
import { ReplaySubject, Observable } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { Router } from '@angular/router';
import { Tenant } from '../../models/tenant';
import { DeactivateLoginDialogComponent } from '../deactivate-login-dialog/deactivate-login-dialog.component';
import { AccountService } from '../../core/services/account.service';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { DateFormatterComponent } from '../formatters/dateformatter';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { CellClickedEvent } from 'ag-grid-community';
import { BooleanInverseFormatterComponent } from '../formatters/booleaninverseformatter';
import { IconButtonFormatterComponent } from '../formatters/iconbuttonformatter';
import * as permissions from '../../models/permissions';

@Component({
    selector: 'app-login-device-status-list',
    templateUrl: './login-device-status-list.component.html',
    styleUrls: ['./login-device-status-list.component.scss'],
    animations: fuseAnimations
})
export class LoginDeviceStatusListComponent implements OnInit, OnDestroy {

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    items$: Observable<BankLogin[]>;
    items: BankLogin[];
    searchError$: Observable<string>;
    selected = [];
    loading$: Observable<boolean>;
    loading: boolean;

    tenantsLoading$: Observable<boolean>;
    tenantsLoading: boolean;
    tenants$: Observable<Tenant[]>;
    tenants: Tenant[];

    tenantGuid$: Observable<string>;
    tenantGuid: string;

    isProviderAdmin$: Observable<boolean>;
    isProviderAdmin: boolean;

    isTenantAdmin$: Observable<boolean>;
    isTenantAdmin: boolean;

    isStandardUser$: Observable<boolean>;
    isStandardUser: boolean;

    accountGuid$: Observable<string>;
    accountGuid: string;

    owners$: Observable<Owner[]>;
    owners: Owner[] = [];
    ownersLoading$: Observable<boolean>;
    ownersLoading: boolean = false;

    deactivating$: Observable<boolean>;
    deactivating: boolean;

    deactivateSuccess$: Observable<boolean>;
    deactivatingError$: Observable<string>;

    activating$: Observable<boolean>;
    activating: boolean;

    activateSuccess$: Observable<boolean>;
    activateError$: Observable<string>;

    deleting$: Observable<boolean>;
    deleting: boolean;
    deleteSuccess$: Observable<boolean>;
    deleteError$: Observable<string>;

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    loginDeviceStatus$: Observable<string>;
    loginDeviceStatusError$: Observable<string>;

    qrcoderegistering$: Observable<boolean>;
    qrcoderegistering: boolean;

    qrcoderegisterSuccess$: Observable<boolean>;
    qrCodeRegisterError$: Observable<string>;

    allowAddBankLogin: boolean = false;

    loginStatus = BankLoginStatus;

    loginDeviceStatus = LoginDeviceStatuses;

    columnDefs;
    enableRtl: boolean = false;
    frameworkComponents;
    rowClassRules;

    gridApi;

    constructor(private translateService: TranslateService,
        private store: Store<coreState.State>,
        private snackBar: MatSnackBar,
        private fuseConfigService: FuseConfigService,
        private router: Router,
        private accountService: AccountService,
        public dialog: MatDialog) {
        this.allowAddBankLogin = this.accountService.isUserAuthorizedForTask(permissions.AddBankLogin);
    }

    ngOnInit() {

        this.fuseConfigService.config.pipe(takeUntil(this.destroyed$)).subscribe(config => {

            this.enableRtl = config.locale != null && config.locale != undefined && config.locale.direction == 'rtl';
        });

        this.frameworkComponents = {
            booleanFormatterComponent: BooleanFormatterComponent,
            dateFormatterComponent: DateFormatterComponent,
            booleanInverseFormatterComponent: BooleanInverseFormatterComponent,
            numberFormatterComponent: NumberFormatterComponent,
            iconButtonFormatterComponent: IconButtonFormatterComponent
        };

        this.columnDefs = [
            {
                headerName: this.translateService.instant('BANK-LOGIN.LIST-COLUMNS.FRIENDLY-NAME'),
                field: "friendlyName",
                width: 200,
                resizable: true,
                sortable: true,
                filter: "agTextColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            }, {
                headerName: this.translateService.instant('BANK-LOGIN.LIST-COLUMNS.BANK-NAME'),
                field: "bankName",
                width: 200,
                resizable: true,
                sortable: true,
                filter: "agTextColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            }, {
                headerName: this.translateService.instant('BANK-LOGIN.LIST-COLUMNS.IS-BLOCKED'),
                field: "isBlocked",
                width: 70,
                resizable: true,
                sortable: true,
                cellRenderer: 'booleanInverseFormatterComponent'
            }, {
                headerName: this.translateService.instant('BANK-LOGIN.LIST-COLUMNS.LOGINDEVICESTATUS'),
                field: "loginDeviceStatusId",
                width: 110,
                resizable: true,
                sortable: true,
                valueGetter: params => this.getStatusName(params.data == undefined ? '' : params.data.status),
                cellClassRules: {
                    "": "data == undefined",
                    "lbg-color-1": "data != undefined && data.status == 0",
                    "lbg-color-2": "data != undefined && data.status == 1"
                }
            }, {
                headerName: this.translateService.instant('GENERAL.ID'),
                field: "id",
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agNumberColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('GENERAL.BANK-LOGIN-ID'),
                field: "bankLoginId",
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agNumberColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            }, {
                headerName: this.translateService.instant('BANK-LOGIN.LIST-COLUMNS.SHOW-LOGINDEVICESTATUS'),
                field: "id",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.showLoginDeviceStatus.bind(this),
                    icon: 'remove_red_eye',
                    iconClass: '',
                    title: 'BANK-LOGIN.LIST-COLUMNS.SHOW-LOGINDEVICESTATUS',
                    allow: this.allowAddBankLogin
                },
                hide: !this.allowAddBankLogin
            }    
        ];

        this.rowClassRules = {
            "password-changed": "data != undefined && data.lastPasswordChangeDate != null && Math.round(Math.abs((new Date().getTime() - new Date(data.lastPasswordChangeDate).getTime())/(24*60*60*1000))) <= 7"
        };

        this.store.dispatch(new loginDeviceActions.ClearAll());
        this.items$ = this.store.select(coreState.getOwnerBankLogins);

        this.items$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.items = t;
        });

        this.searchError$ = this.store.select(coreState.getOwnerBankLoginError);
        this.loading$ = this.store.select(coreState.getOwnerBankLoginsLoading);

        this.loading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.loading = l;
        });

        this.tenants$ = this.store.select(coreState.getTenantSearchResults);

        this.owners$ = this.store.select(coreState.getOwners);
        this.ownersLoading$ = this.store.select(coreState.getAccountLoading);

        this.ownersLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.ownersLoading = l;
        });

        this.owners$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            if (items != undefined) {
                this.owners = items;
            } else {
                this.owners = [];
            }
        });

        this.tenants$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.tenants = items;
        });

        this.searchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new accountActions.GetOwners());
                this.store.dispatch(new loginDeviceActions.GetOwnerLoginList());
            }
        });

        this.isProviderAdmin$ = this.store.select(coreState.getAccountIsProviderAdmin);
        this.isProviderAdmin$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.isProviderAdmin = t;
        });

        this.isTenantAdmin$ = this.store.select(coreState.getAccountIsTenantAdmin);
        this.isTenantAdmin$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.isTenantAdmin = t;
        });

        this.isStandardUser$ = this.store.select(coreState.getAccountIsStandardUser);
        this.isStandardUser$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.isStandardUser = t;
        });

        this.accountGuid$ = this.store.select(coreState.getAccountGuid);
        this.accountGuid$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.accountGuid = t;
        });

        this.tenantGuid$ = this.store.select(coreState.getAccountTenantGuid);

        this.tenantGuid$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.tenantGuid = t;
        });

        this.deactivating$ = this.store.select(coreState.deactivateOwnerLoginProcessing);

        this.deactivating$.pipe(takeUntil(this.destroyed$)).subscribe(d => {
            this.deactivating = d;
        });

        this.deactivatingError$ = this.store.select(coreState.deactivateOwnerLoginError);

        this.deactivatingError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

       
        this.deactivateSuccess$ = this.store.select(coreState.deactivateOwnerLoginSuccess);

        this.deactivateSuccess$.pipe(takeUntil(this.destroyed$)).subscribe(d => {
            if (d == true) {
                this.store.dispatch(new loginDeviceActions.GetOwnerLoginList());
            }
        });

     
        this.loginDeviceStatus$ = this.store.select(coreState.getLoginDeviceStatus);
        this.loginDeviceStatus$.pipe(takeUntil(this.destroyed$)).subscribe(loginDeviceStatusDesc => {
            if (loginDeviceStatusDesc) {
                let action = this.translateService.instant('GENERAL.OK');
                let message = this.translateService.instant('BANK-LOGIN.GENERAL.CURRENT-PASSWORD', { loginDeviceStatusDesc });
                this.snackBar.open(message, action, {
                    duration: 30000,
                    verticalPosition: 'top'
                });
            }
        });

        this.loginDeviceStatusError$ = this.store.select(coreState.getLoginDeviceStatus);
        this.loginDeviceStatusError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });
    }

    openSnackBar(message: string, action: string = undefined) {
        if (!action) {
            action = this.translateService.instant('GENERAL.OK');
        }
        this.snackBar.open(message, action, {
            duration: 10000,
        });
    }
    onSelect() {

        if (this.selected.length > 0) {
            this.router.navigate(['/banklogin/' + this.selected[0].id]);
        }
    }

    getTenantName(tenantGuid: string): string {

        if (this.tenants != null && this.tenants != undefined) {
            var tenant = this.tenants.find(t => t.tenantDomainPlatformMapGuid == tenantGuid);

            if (tenant != null) {
                return tenant.tenantName;
            }
        }

        return '';
    }

    ngOnDestroy(): void {
        this.store.dispatch(new loginDeviceActions.ClearAll());
        this.destroyed$.next(true);
        this.destroyed$.complete();
    }

    refresh() {
        this.gridApi.paginationGoToFirstPage();
        this.loadInvoices();
    }

    loadInvoices() {
        if (this.selectedTenant == undefined) {
            return;
        }
        this.store.dispatch(new accountActions.GetOwners());
        this.store.dispatch(new loginDeviceActions.GetOwnerLoginList());
    }

    getStatusName(value: number): string {
        if (value == this.loginDeviceStatus.Active) {
            return this.translateService.instant('BANK-LOGIN.STATUS.DEVICE_ACTIVE');
        }

        if (value == this.loginDeviceStatus.InActive) {
            return this.translateService.instant('BANK-LOGIN.STATUS.DEVICE_INACTIVE');
        }

        if (value == this.loginDeviceStatus.Error) {
            return this.translateService.instant('BANK-LOGIN.STATUS.DEVICE_ERROR');
        }

        if (value == this.loginDeviceStatus.MobileNotConfigured) {
            return this.translateService.instant('BANK-LOGIN.STATUS.DEVICE_NOTCONFIGURED');
        }
        return this.translateService.instant('BANK-LOGIN.STATUS.DEVICE_NOTCONFIGURED');
    }

    getOwnerName(ownerGuid: string): string {

        if (this.owners == undefined || this.owners == null) {
            return '';
        }

        var owner = this.owners.find(t => t.accountId == ownerGuid);

        if (owner == null || owner == undefined) {
            return '';
        }

        return owner.firstName + ' ' + owner.lastName;
    }


    activateAllowed(params) {

        return this.allowAddBankLogin && params != undefined && params.data != undefined && params.data.status == 2 && params.data.isActive == false;
    }

    deActivateAllowed(params): boolean {
        var result = this.allowAddBankLogin && params != undefined && params.data != undefined && params.data.status == 2 && params.data.isActive == true;
        return result;
    }

    showPasswordAllowed(params) {
        return this.allowAddBankLogin && params != undefined && params.data != undefined && params.data.lastPasswordChangeDate != null;
    }

    showQRRegisterAllowed(params) {
        return this.allowAddBankLogin && params != undefined && params.data != undefined && params.data.bankId == 2;
    }

    showLoginDeviceStatus(id) {
        this.store.dispatch(new loginDeviceActions.ShowLoginDeviceStatus(id.toString()));
    }

    showLoginListDeviceStatus() {
        this.store.dispatch(new loginDeviceActions.ShowLoginListDeviceStatus());
    }

    onCellClicked(e: CellClickedEvent) {
    }

    onGridReady(params) {
        this.gridApi = params.api;
    }

}
