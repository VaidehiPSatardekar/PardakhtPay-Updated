import { Component, OnInit, OnDestroy } from '@angular/core';
import { Owner } from '../../models/account.model';
import * as tenantActions from '../../core/actions/tenant';
import * as accountActions from '../../core/actions/account';
import * as bankLoginActions from '../../core/actions/bankLogin';
import { BankLogin, BankLoginStatus } from '../../models/bank-login';
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
import { DeleteLoginDialogComponent } from '../delete-login-dialog/delete-login-dialog.component';
import { DeactivateLoginDialogComponent } from '../deactivate-login-dialog/deactivate-login-dialog.component';
import { AccountService } from '../../core/services/account.service';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { DateFormatterComponent } from '../formatters/dateformatter';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { CellClickedEvent, IGetRowsParams } from 'ag-grid-community';
import { BooleanInverseFormatterComponent } from '../formatters/booleaninverseformatter';
import { IconButtonFormatterComponent } from '../formatters/iconbuttonformatter';
import { BankConnectionProgramFormatterComponent } from '../formatters/bankConnectionProgramFormatter';

import * as permissions from '../../models/permissions';

@Component({
    selector: 'app-bank-login-list',
    templateUrl: './bank-login-list.component.html',
    styleUrls: ['./bank-login-list.component.scss'],
    animations: fuseAnimations
})
export class BankLoginListComponent implements OnInit, OnDestroy {

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

    password$: Observable<string>;
    passwordError$: Observable<string>;

    qrcoderegistering$: Observable<boolean>;
    qrcoderegistering: boolean;

    qrcoderegisterSuccess$: Observable<boolean>;
    qrCodeRegisterError$: Observable<string>;

    getOTP$: Observable<string>;
    getOTPError$: Observable<string>;

    switchBankConnectionProgram$: Observable<string>;
    switchBankConnectionProgramError$: Observable<string>;

    allowAddBankLogin: boolean = false;

    loginStatus = BankLoginStatus;

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
            iconButtonFormatterComponent: IconButtonFormatterComponent,
            bankConnectionProgramFormatterComponent: BankConnectionProgramFormatterComponent
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
                headerName: this.translateService.instant('BANK-LOGIN.LIST-COLUMNS.STATUS'),
                field: "status",
                width: 110,
                resizable: true,
                sortable: true,
                valueGetter: params => this.getStatusName(params.data == undefined ? '' : params.data.status),
                cellClassRules: {
                    "": "data == undefined",
                    "lbg-color-1": "data != undefined && data.status == 1",
                    "lbg-color-2": "data != undefined && data.status == 2",
                    "lbg-color-3": "data != undefined && data.status == 3",
                    "lbg-color-4": "data != undefined && data.status == 4",
                    "lbg-color-5": "data != undefined && data.status == 5"
                }
            }, {
                headerName: this.translateService.instant('CARD-TO-CARD.GENERAL.ADD-CARD-TO-CARD'),
                field: "id",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.addCardToCard.bind(this),
                    icon: 'credit_card',
                    iconClass: '',
                    title: 'CARD-TO-CARD.GENERAL.ADD-CARD-TO-CARD',
                    allow: this.allowAddBankLogin
                },
                hide: !this.allowAddBankLogin
            }, {
                headerName: this.translateService.instant('BANK-LOGIN.LIST-COLUMNS.EDIT'),
                field: "id",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.edit.bind(this),
                    icon: 'edit',
                    iconClass: '',
                    title: 'BANK-LOGIN.LIST-COLUMNS.EDIT',
                    allow: this.allowAddBankLogin
                },
                hide: !this.allowAddBankLogin
            }, {
                headerName: this.translateService.instant('BANK-LOGIN.LIST-COLUMNS.DEACTIVATE'),
                field: "id",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.deactivate.bind(this),
                    icon: 'remove_circle',
                    iconClass: 'danger',
                    title: 'BANK-LOGIN.LIST-COLUMNS.DEACTIVATE',
                    allow: this.allowAddBankLogin,
                    allowed: this.deActivateAllowed.bind(this)
                },
                hide: !this.allowAddBankLogin
            }, {
                headerName: this.translateService.instant('BANK-LOGIN.LIST-COLUMNS.ACTIVATE'),
                field: "id",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.activate.bind(this),
                    icon: 'check',
                    iconClass: 'success',
                    title: 'BANK-LOGIN.LIST-COLUMNS.ACTIVATE',
                    allow: this.allowAddBankLogin,
                    allowed: this.activateAllowed.bind(this)
                },
                hide: !this.allowAddBankLogin
            }, {
                headerName: this.translateService.instant('BANK-LOGIN.LIST-COLUMNS.DELETE'),
                field: "id",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.delete.bind(this),
                    icon: 'delete_forever',
                    iconClass: 'danger',
                    title: 'BANK-LOGIN.LIST-COLUMNS.DELETE',
                    allow: this.allowAddBankLogin
                },
                hide: !this.allowAddBankLogin
            },
            {
                headerName: this.translateService.instant('BANK-LOGIN.GENERAL.CHANGE-LOGIN-INFORMATION'),
                field: "id",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.changePassword.bind(this),
                    icon: 'vpn_key',
                    iconClass: '',
                    title: 'BANK-LOGIN.GENERAL.CHANGE-LOGIN-INFORMATION',
                    allow: this.allowAddBankLogin,
                    allowed: undefined
                },
                hide: !this.allowAddBankLogin
            },
            {
                headerName: this.translateService.instant('BANK-LOGIN.LIST-COLUMNS.LAST-PASSWORD-CHANGED-DATE'),
                field: "lastPasswordChangeDate",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'dateFormatterComponent'
            },
            {
                headerName: this.translateService.instant('BANK-LOGIN.LIST-COLUMNS.SHOW-PASSWORD'),
                field: "id",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.showPassword.bind(this),
                    icon: 'remove_red_eye',
                    iconClass: '',
                    title: 'BANK-LOGIN.LIST-COLUMNS.SHOW-PASSWORD',
                    allow: this.allowAddBankLogin,
                    allowed: this.showPasswordAllowed.bind(this)
                },
                hide: !this.allowAddBankLogin
            },
            {
                headerName: this.translateService.instant('BANK-LOGIN.LIST-COLUMNS.SWITCH-BANK-CONNECTION-PROGRAM'),
                field: "id",
                width: 300,
                resizable: true,
                sortable: true,
                cellRenderer: 'bankConnectionProgramFormatterComponent',//(params) => params.data.bankConnectionProgram,
                cellRendererParams: {
                    onButtonClick: this.switchBankConnectionProgram.bind(this),
                    icon: 'remove_red_eye',
                    iconClass: '',
                    title: 'BANK-LOGIN.LIST-COLUMNS.SWITCH-BANK-CONNECTION-PROGRAM',
                    allow: this.allowAddBankLogin,
                    allowed: this.showbankConnectionProgramSwitchAllowed.bind(this)                        
                },
                hide: !this.allowAddBankLogin
            },
            //{
            //    headerName: this.translateService.instant('BANK-LOGIN.LIST-COLUMNS.QRREGISTER'),
            //    field: "id",
            //    width: 110,
            //    resizable: true,
            //    sortable: true,
            //    cellRenderer: 'iconButtonFormatterComponent',
            //    cellRendererParams: {              
            //        onButtonClick: this.qrCodeRegister.bind(this),
            //        icon: 'texture',
            //        iconClass: '',
            //        title: 'BANK-LOGIN.LIST-COLUMNS.QRREGISTER',
            //        allow: this.allowAddBankLogin, 
            //        allowed: this.showQRRegisterAllowed.bind(this)
            //    },
            //    hide: !this.allowAddBankLogin
            //},
            //{
            //    headerName: this.translateService.instant('BANK-LOGIN.LIST-COLUMNS.GENERATE-OTP'),
            //    field: "bankLoginId",
            //    width: 110,
            //    resizable: true,
            //    sortable: true,
            //    cellRenderer: 'iconButtonFormatterComponent',
            //    cellRendererParams: {
            //        onButtonClick: this.getOTP.bind(this),
            //        icon: 'contact_phone',
            //        iconClass: '',
            //        title: 'BANK-LOGIN.LIST-COLUMNS.GENERATE-OTP',
            //        allow: this.allowAddBankLogin,
            //        allowed: this.showQRRegisterAllowed.bind(this)
            //    },
            //    hide: !this.allowAddBankLogin
            //},
            {
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
            },
            {
                headerName: this.translateService.instant('CARD-TO-CARD.LIST-COLUMNS.OWNER'),
                field: "ownerGuid",
                width: 200,
                resizable: true,
                sortable: true,
                valueGetter: params => this.getOwnerName(params.data == undefined ? '' : params.data.ownerGuid),
                filter: "agTextColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            }
        ];

        this.rowClassRules = {
            "password-changed": "data != undefined && data.lastPasswordChangeDate != null && Math.round(Math.abs((new Date().getTime() - new Date(data.lastPasswordChangeDate).getTime())/(24*60*60*1000))) <= 7"
        };

        this.store.dispatch(new bankLoginActions.ClearAll());
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
                this.store.dispatch(new bankLoginActions.GetOwnerLoginList());
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

        this.deleting$ = this.store.select(coreState.deleteOwnerLoginProcessing);
        this.deleting$.pipe(takeUntil(this.destroyed$)).subscribe(d => {
            this.deleting = d;
        });

        this.deleteError$ = this.store.select(coreState.deleteOwnerLoginError);
        this.deleteError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.deleteSuccess$ = this.store.select(coreState.deleteOwnerLoginSuccess);

        this.deleteSuccess$.pipe(takeUntil(this.destroyed$)).subscribe(d => {
            if (d == true) {
                this.store.dispatch(new bankLoginActions.GetOwnerLoginList());
            }
        });

        this.deactivateSuccess$ = this.store.select(coreState.deactivateOwnerLoginSuccess);

        this.deactivateSuccess$.pipe(takeUntil(this.destroyed$)).subscribe(d => {
            if (d == true) {
                this.store.dispatch(new bankLoginActions.GetOwnerLoginList());
            }
        });

        this.activateError$ = this.store.select(coreState.activateOwnerLoginError);

        this.activateError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.activateSuccess$ = this.store.select(coreState.activateOwnerLoginSuccess);

        this.activateSuccess$.pipe(takeUntil(this.destroyed$)).subscribe(d => {
            if (d == true) {
                this.store.dispatch(new bankLoginActions.GetOwnerLoginList());
            }
        });

        this.activating$ = this.store.select(coreState.activateOwnerLoginProcessing);

        this.activating$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.activating = l;
        });

        this.password$ = this.store.select(coreState.getPassword);
        this.password$.pipe(takeUntil(this.destroyed$)).subscribe(password => {
            if (password) {
                let action = this.translateService.instant('GENERAL.OK');
                let message = this.translateService.instant('BANK-LOGIN.GENERAL.CURRENT-PASSWORD', { password });
                this.snackBar.open(message, action, {
                    duration: 30000,
                    verticalPosition: 'top'
                });
            }
        });

        this.passwordError$ = this.store.select(coreState.getPasswordError);
        this.passwordError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.getOTP$ = this.store.select(coreState.getOTP);
        this.getOTP$.pipe(takeUntil(this.destroyed$)).subscribe(otp => {
              if (otp == "Device is not registered.") {
                let action = this.translateService.instant('GENERAL.OK');
                let message = this.translateService.instant('BANK-LOGIN.GENERAL.DEVICE-NOT-REGISTERED');
                this.snackBar.open(message, action, {
                    duration: 30000,
                    verticalPosition: 'top'
                });
            }
            else if (otp) {
                let action = this.translateService.instant('GENERAL.OK');
                let message = this.translateService.instant('BANK-LOGIN.GENERAL.DEVICE-OTP', { otp });
                this.snackBar.open(message, action, {
                    duration: 30000,
                    verticalPosition: 'top'
                });
            }
        });

        this.getOTPError$ = this.store.select(coreState.getOTPError);
        this.getOTPError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.switchBankConnectionProgram$ = this.store.select(coreState.switchBankConnectionProgram);
        this.switchBankConnectionProgram$.pipe(takeUntil(this.destroyed$)).subscribe(bankConnectionProgram => {
            if (bankConnectionProgram) {
                this.store.dispatch(new bankLoginActions.GetOwnerLoginList());
            }
        });

        this.switchBankConnectionProgramError$ = this.store.select(coreState.switchBankConnectionProgramError);
        this.switchBankConnectionProgramError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });
    }

    refresh() {
        this.store.select(coreState.getOwnerBankLogins);
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
        this.store.dispatch(new bankLoginActions.ClearAll());
        this.destroyed$.next(true);
        this.destroyed$.complete();
    }

    getStatusName(value: number): string {
        if (value == this.loginStatus.WaitingInformation) {
            return this.translateService.instant('BANK-LOGIN.STATUS.WAITING-INFORMATION');
        }

        if (value == this.loginStatus.Success) {
            return this.translateService.instant('BANK-LOGIN.STATUS.SUCCESS');
        }

        if (value == this.loginStatus.Error) {
            return this.translateService.instant('BANK-LOGIN.STATUS.FAIL');
        }

        if (value == this.loginStatus.WaitingApprovement) {
            return this.translateService.instant('BANK-LOGIN.STATUS.WAITING-APPROVE');
        }
        return '';
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

    deactivate(id) {
        const dialogRef = this.dialog.open(DeactivateLoginDialogComponent, {
            width: '250px',
            data: 'BANK-LOGIN.GENERAL.DEACTIVATE-DIALOG'
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result == true) {
                this.store.dispatch(new bankLoginActions.DeactivateLoginInformation(id));
            }
        });
    }

    activate(id) {
        const dialogRef = this.dialog.open(DeactivateLoginDialogComponent, {
            width: '250px',
            data: 'BANK-LOGIN.GENERAL.ACTIVATE-DIALOG'
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result == true) {
                this.store.dispatch(new bankLoginActions.ActivateLoginInformation(id));
            }
        });
    }




    activateAllowed(params) {

        return this.allowAddBankLogin && params != undefined && params.data != undefined && params.data.status == 2 && params.data.isActive == false;
    }

    deActivateAllowed(params): boolean {
        var result = this.allowAddBankLogin && params != undefined && params.data != undefined && params.data.status == 2 && params.data.isActive == true;
        return result;
    }

    showPasswordAllowed(params) {
        return (this.allowAddBankLogin && params != undefined && params.data != undefined && params.data.lastPasswordChangeDate != null) ||
            (this.allowAddBankLogin && params != undefined && params.data != undefined && params.data.bankId == 13 && params.data.bankLoginId != 0) ;
    }

    showbankConnectionProgramSwitchAllowed(params) {
        return (this.allowAddBankLogin && params != undefined && params.data != undefined
            && (params.data.bankConnectionProgram == 'MobileBanking' || params.data.bankConnectionProgram == 'InternetBanking') && params.data.bankLoginId != 0);
    }

    showQRRegisterAllowed(params) {
        return this.allowAddBankLogin && params != undefined && params.data != undefined && params.data.bankId == 2 && params.data.bankLoginId !=0;
    }

    delete(id) {
        const dialogRef = this.dialog.open(DeleteLoginDialogComponent, {
            width: '250px',
            data: id
        });

        dialogRef.afterClosed().subscribe(result => {
            if (result == true) {
                this.store.dispatch(new bankLoginActions.DeleteLoginInformation(id));
            }
        });
    }

    addCardToCard(id) {
        this.router.navigate(['/bankloginaccount/' + id]);
    }

    edit(id) {
        this.router.navigate(['/banklogin/' + id]);
    }

    changePassword(id) {
        this.router.navigate(['/changelogininformation/' + id]);
    }

    showPassword(id) {
        this.store.dispatch(new bankLoginActions.ShowPassword(id));
    }

    switchBankConnectionProgram(id) {
        this.store.dispatch(new bankLoginActions.SwitchBankConnectionProgram(id));
    }

    qrCodeRegister(id) {
        this.router.navigate(['/banklogin/getqrregistrationdetails/' + id]);
       // this.store.dispatch(new bankLoginActions.GetQRRegistrationDetails(id));
    }

    getOTP(bankLoginId) {
        this.store.dispatch(new bankLoginActions.GetOTP(bankLoginId));
    }

    onCellClicked(e: CellClickedEvent) {
    }

    onGridReady(params) {
        this.gridApi = params.api;
    }

}
