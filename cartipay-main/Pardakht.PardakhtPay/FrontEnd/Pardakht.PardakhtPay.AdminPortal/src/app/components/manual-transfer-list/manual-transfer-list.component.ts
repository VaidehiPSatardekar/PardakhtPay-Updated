import { Component, OnInit, ViewChild, OnDestroy, NgZone } from '@angular/core';
import { MatSnackBar, DateAdapter, MatDialog } from '@angular/material';
import { Observable, ReplaySubject } from 'rxjs';
import { ManualTransfer, ManualTransferSearchArgs, ManualTransferStatuses, TransferTypes, TransferPriorities } from '../../models/manual-transfer';
import * as manualTransferActions from '../../core/actions/manualTransfer';
import { fuseAnimations } from '../../core/animations';
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import { ListSearchResponse } from '../../models/types';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../models/tenant';
import { TimeZoneService } from '../../core/services/timeZoneService/time-zone.service';
import { TimeZone } from '../../models/timeZone';
import * as bankLoginActions from '../../core/actions/bankLogin';
import { BankLogin, BankAccount } from '../../models/bank-login';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { DateFormatterComponent } from '../formatters/dateformatter';
import { IGetRowsParams, CellClickedEvent } from 'ag-grid-community';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { Owner } from '../../models/account.model';
import * as accountActions from '../../core/actions/account';
import { AccountService } from '../../core/services/account.service';
import { Router } from '@angular/router';
import { IconButtonFormatterComponent } from '../formatters/iconbuttonformatter';
import { DeactivateLoginDialogComponent } from '../deactivate-login-dialog/deactivate-login-dialog.component';
import { DownloadManualTransferReceiptFormatterComponent } from '../formatters/downloadManualTransferReceiptFormatter';
import { DecimalPipe } from '@angular/common';
import * as permissions from '../../models/permissions';
import { NgModel } from '@angular/forms';

@Component({
    selector: 'app-manual-transfer-list',
    templateUrl: './manual-transfer-list.component.html',
    styleUrls: ['./manual-transfer-list.component.scss'],
    animations: fuseAnimations
})
export class ManualTransferListComponent implements OnInit, OnDestroy {
    args: ManualTransferSearchArgs;

    loading$: Observable<boolean>;
    manualTransfers$: Observable<ListSearchResponse<ManualTransfer[]>>;
    manualTransfers: ListSearchResponse<ManualTransfer[]>;
    searchError$: Observable<string>;

    manualTransferCancelSuccess$: Observable<boolean>;
    manualTransferCancelError$: Observable<string>;

    tenants$: Observable<Tenant[]>;
    tenants: Tenant[] = [];

    bankLogins$: Observable<BankLogin[]>;
    bankLogins: BankLogin[] = [];
    bankLoginError$: Observable<string>;
    bankLoginLoading$: Observable<boolean>;
    bankLoginLoading: boolean;

    rowClassRules;

    bankAccounts$: Observable<BankAccount[]>;
    bankAccountsError$: Observable<string>;
    bankAccountLoading$: Observable<boolean>;
    bankAccountsLoading: boolean;
    bankAccounts: BankAccount[];

    tenantGuid$: Observable<string>;
    tenantGuid: string;

    owners$: Observable<Owner[]>;
    owners: Owner[] = undefined;
    ownersLoading$: Observable<boolean>;
    ownersLoading: boolean = false;

    selectedZoneId: string;
    zones: TimeZone[] = [];

    isProviderAdmin$: Observable<boolean>;
    isProviderAdmin: boolean;

    isTenantAdmin$: Observable<boolean>;
    isTenantAdmin: boolean;

    isStandardUser$: Observable<boolean>;
    isStandardUser: boolean;

    accountGuid$: Observable<string>;
    accountGuid: string;

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    firstLoaded: boolean = false;
    includeDeletedLogins: boolean = false;

    gridApi;
    callBackApi;
    gridColumnApi;
    columnDefs;
    enableRtl: boolean = false;
    frameworkComponents;


    allowAddManualTransfer: boolean = false;

    priorities = TransferPriorities;
    manualTransferStatuses = ManualTransferStatuses;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    filter: string = undefined;

    filteredBankAccounts: BankAccount[] = undefined;

    constructor(private store: Store<coreState.State>,
        private snackBar: MatSnackBar,
        private timeZoneService: TimeZoneService,
        private accountService: AccountService,
        private fuseConfigService: FuseConfigService,
        public translateService: TranslateService,
        private ngZone: NgZone, 
        private router: Router,
        private num: DecimalPipe,
        public dialog: MatDialog) {

        this.allowAddManualTransfer = this.accountService.isUserAuthorizedForTask(permissions.AddManualTransfer);

        var cachedArgs = localStorage.getItem('manualtransferargs');

        if (cachedArgs != undefined && cachedArgs != null) {
            this.args = JSON.parse(cachedArgs);
            this.args.fromDate = null;
            this.args.toDate = null;
            this.args.pageSize = 25;

            if (this.args.transferType == null) {
                this.args.transferType = 0;
            }

            if (this.args.status == null) {
                this.args.status = 0;
            }
        }
        else {
            this.args = new ManualTransferSearchArgs();
            this.args.pageSize = 25;
            this.args.transferType = 0;
            this.args.status = 0;
        }

        this.columnDefs = [
            {
                headerName: this.translateService.instant('GENERAL.ID'),
                field: "id",
                colId: "id",
                sortable: true,
                resizable: true,
                width: 75
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.CREATE-DATE'),
                field: "creationDateStr",
                colId: "creationDateStr",
                sortable: true,
                resizable: true,
                cellRenderer: 'dateFormatterComponent',
                width: 150
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.TRANSFER-TYPE'),
                field: "transferType",
                colId: "transferType",
                sortable: true,
                resizable: true,
                width: 150,
                valueGetter: params => this.getTransferTypeName(params.data == undefined ? '' : params.data.transferType)
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.STATUS'),
                field: "status",
                colId: "status",
                sortable: true,
                resizable: true,
                width: 150,
                valueGetter: params => this.getStatusName(params.data == undefined ? '' : params.data.status)
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.AMOUNT'),
                field: "amount",
                colId: "amount",
                sortable: true,
                resizable: true,
                width: 100,
                valueGetter: params => this.getAmount(params.data == undefined ? '' : params.data)
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.PRIORITY'),
                field: "priority",
                colId: "priority",
                sortable: true,
                resizable: true,
                width: 150,
                valueGetter: params => this.getPriorityName(params.data == undefined ? '' : params.data.priority)
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.FROM-ACCOUNT-NO'),
                field: "accountGuid",
                colId: "accountGuid",
                sortable: true,
                resizable: true,
                width: 200,
                valueGetter: params => this.getAccountName(params.data == undefined ? '' : params.data.accountGuid)
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.TO-ACCOUNT-NO'),
                field: "toAccountNo",
                colId: "toAccountNo",
                sortable: true,
                resizable: true,
                width: 200,
                filter: "agTextColumnFilter", filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    filterOptions: ["equals"],
                    defaultOption: "equals"
                }
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.IBAN'),
                field: "iban",
                colId: "iban",
                sortable: true,
                resizable: true,
                width: 200,
                filter: "agTextColumnFilter", filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    filterOptions: ["equals"],
                    defaultOption: "equals"
                }
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.FIRST-NAME'),
                field: "firstName",
                colId: "firstName",
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agTextColumnFilter", filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    filterOptions: ["equals"],
                    defaultOption: "equals"
                }
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.LAST-NAME'),
                field: "lastName",
                colId: "lastName",
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agTextColumnFilter", filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    filterOptions: ["equals"],
                    defaultOption: "equals"
                }
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.PROCESSED-DATE'),
                field: "processedDateStr",
                colId: "processedDateStr",
                sortable: true,
                resizable: true,
                cellRenderer: 'dateFormatterComponent',
                width: 150
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.IMMEDIATE-TRANSFER'),
                field: "immediateTransfer",
                colId: "immediateTransfer",
                sortable: true,
                resizable: true,
                cellRenderer: 'booleanFormatterComponent',
                width: 150
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.SCHEDULED-DATE'),
                field: "expectedTransferDateStr",
                colId: "expectedTransferDateStr",
                sortable: true,
                resizable: true,
                cellRenderer: 'dateFormatterComponent',
                width: 150
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.CANCELLED-DATE'),
                field: "cancelledDateStr",
                colId: "cancelledDateStr",
                sortable: true,
                resizable: true,
                cellRenderer: 'dateFormatterComponent',
                width: 150
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.CANCEL'),
                field: "id",
                colId: "cancel",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.cancel.bind(this),
                    icon: 'cancel',
                    iconClass: 'danger',
                    title: 'MANUAL-TRANSFER.GENERAL.CANCEL',
                    allow: this.allowAddManualTransfer,
                    allowed: this.cancelationAllowed.bind(this)
                },
                hide: !this.allowAddManualTransfer
            },
            {
                headerName: this.translateService.instant('GENERAL.COPY'),
                field: "id",
                colId: "copy",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.copy.bind(this),
                    icon: 'file_copy',
                    iconClass: '',
                    title: 'GENERAL.COPY',
                    allow: this.allowAddManualTransfer
                },
                hide: !this.allowAddManualTransfer
            },
            {
                headerName: this.translateService.instant('GENERAL.CANCELLER-ID'),
                field: "cancellerId",
                colId: "cancellerId",
                width: 200,
                resizable: true,
                sortable: true,
                valueGetter: params => this.getOwnerName(params.data == undefined ? '' : params.data.cancellerId)
            },
            {
                headerName: this.translateService.instant('GENERAL.CREATOR-ID'),
                field: "creatorId",
                colId: "creatorId",
                width: 200,
                resizable: true,
                sortable: true,
                valueGetter: params => this.getOwnerName(params.data == undefined ? '' : params.data.creatorId)
            },
            {
                headerName: this.translateService.instant('GENERAL.UPDATER-ID'),
                field: "updaterId",
                colId: "updaterId",
                width: 200,
                resizable: true,
                sortable: true,
                valueGetter: params => this.getOwnerName(params.data == undefined ? '' : params.data.updaterId)
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.OWNER'),
                field: "ownerGuid",
                colId: "ownerGuid",
                width: 200,
                resizable: true,
                sortable: true,
                valueGetter: params => this.getOwnerName(params.data == undefined ? '' : params.data.ownerGuid)
            },
            {
                headerName: this.translateService.instant('GENERAL.BANK-LOGIN-ID'),
                field: "bankLoginId",
                colId: "bankLoginId",
                sortable: true,
                resizable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('GENERAL.BANK-ACCOUNT-ID'),
                field: "bankAccountId",
                colId: "bankAccountId",
                sortable: true,
                resizable: true,
                width: 150
            }
        ];

        this.rowClassRules = {

        };
    }

    ngOnInit() {

        this.fuseConfigService.config.pipe(takeUntil(this.destroyed$)).subscribe(config => {

            this.enableRtl = config.locale != null && config.locale != undefined && config.locale.direction == 'rtl';
        });

        this.frameworkComponents = {
            booleanFormatterComponent: BooleanFormatterComponent,
            dateFormatterComponent: DateFormatterComponent,
            numberFormatterComponent: NumberFormatterComponent,
            iconButtonFormatterComponent: IconButtonFormatterComponent
        };

        this.store.dispatch(new manualTransferActions.Clear());
        this.store.dispatch(new bankLoginActions.ClearAll());

        this.loading$ = this.store.select(coreState.getManualTransferLoading);
        this.manualTransfers$ = this.store.select(coreState.getManualTransferSearchResults);
        this.searchError$ = this.store.select(coreState.getManualTransferSearchError);

        this.manualTransferCancelError$ = this.store.select(coreState.getManualTransferCancelError);

        this.manualTransferCancelError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.manualTransferCancelSuccess$ = this.store.select(coreState.getManualTransferCancelSuccess);

        this.manualTransferCancelSuccess$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            if (t) {
                this.refresh();
            }
        });

        this.tenants$ = this.store.select(coreState.getTenantSearchResults);

        this.bankLoginError$ = this.store.select(coreState.getBankLoginSearchError);
        this.bankLogins$ = this.store.select(coreState.getBankLoginSearchResults);
        this.bankLoginLoading$ = this.store.select(coreState.getBankLoginLoading);

        this.bankAccounts$ = this.store.select(coreState.getBankAccountSearchResults);
        this.bankAccountsError$ = this.store.select(coreState.getBankAccountSearchError);
        this.bankAccountLoading$ = this.store.select(coreState.getBankAccountsLoading);

        this.zones = this.timeZoneService.getTimeZones();
        this.selectedZoneId = this.timeZoneService.getTimeZone().timeZoneId;

        this.searchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.manualTransfers$.pipe(takeUntil(this.destroyed$)).subscribe((data: ListSearchResponse<ManualTransfer[]>) => {
            this.manualTransfers = data;

            if (this.callBackApi) {
                if (data) {
                    this.callBackApi.successCallback(data.items, data.paging.totalItems);
                } else {
                    this.callBackApi.successCallback([]);
                }
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

        this.owners$ = this.store.select(coreState.getOwners);
        this.ownersLoading$ = this.store.select(coreState.getAccountLoading);

        this.ownersLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.ownersLoading = l;
        });

        this.owners$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.owners = items;
        });

        this.bankLogins$.pipe(takeUntil(this.destroyed$)).subscribe(logins => {
            this.bankLogins = logins;

            if (this.bankAccounts != undefined && this.bankAccounts != null && this.bankAccounts.length > 0) {
                if (this.args.accountGuids != undefined && this.args.accountGuids != null && this.args.accountGuids.length > 0) {

                    var guids = [];

                    for (var i = 0; i < this.args.accountGuids.length; i++) {
                        var account = this.bankAccounts.find(t => t.accountGuid == this.args.accountGuids[i]);

                        if (account != null && account != undefined) {
                            guids.push(account.accountGuid);
                        }
                    }

                    if (guids.length > 0) {
                        this.args.accountGuids = guids;
                    } else {
                        this.args.accountGuids = this.bankAccounts.map(t => t.accountGuid);
                    }

                    if (this.args.accountGuids != undefined && this.args.accountGuids != null && this.args.accountGuids.length > 0 && this.firstLoaded == false) {
                        this.loadManualTransfers();
                    }
                }
                else {
                    this.args.accountGuids = this.bankAccounts.map(t => t.accountGuid);
                }
            }
        });

        this.bankLoginError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.bankAccountsError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.bankAccounts$.pipe(takeUntil(this.destroyed$)).subscribe(accounts => {
            if (accounts) {
                this.bankAccounts = accounts;

                if (this.args.accountGuids != undefined && this.args.accountGuids != null && this.args.accountGuids.length > 0) {

                    var guids = [];

                    for (var i = 0; i < this.args.accountGuids.length; i++) {
                        var account = accounts.find(t => t.accountGuid == this.args.accountGuids[i]);

                        if (account != null && account != undefined) {
                            guids.push(account.accountGuid);
                        }
                    }

                    if (guids.length > 0) {
                        this.args.accountGuids = guids;
                    } else {
                        this.args.accountGuids = accounts.map(t => t.accountGuid);
                    }

                    if (this.args.accountGuids != undefined && this.args.accountGuids != null && this.args.accountGuids.length > 0 && this.firstLoaded == false) {
                        this.loadManualTransfers();
                    }
                }
                else {
                    this.args.accountGuids = this.bankAccounts.map(t => t.accountGuid);
                }
            }
            else {
                this.bankAccounts = [];
            }
        });

        this.bankLoginLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.bankLoginLoading = l;
        });

        this.bankAccountLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.bankAccountsLoading = l;
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new accountActions.GetOwners());
                this.store.dispatch(new bankLoginActions.Search(this.includeDeletedLogins));
                this.store.dispatch(new bankLoginActions.SearchAccounts(this.includeDeletedLogins));
            }
        });
    }

    deleteLoginCheckedChanged() {
        this.store.dispatch(new bankLoginActions.Search(this.includeDeletedLogins));
        this.store.dispatch(new bankLoginActions.SearchAccounts(this.includeDeletedLogins));
    }

    equals(objOne, objTwo) {
        if (typeof objOne !== 'undefined' && typeof objTwo !== 'undefined') {
            return objOne === objTwo;
        }
    }

    filterBankAccounts(text) {
        this.filter = text;

        if (this.bankAccounts) {
            this.filteredBankAccounts = this.bankAccounts.filter(t => this.getLoginName(t.loginGuid).toLowerCase().includes(text) || t.accountNo.toLowerCase().includes(text));
        }
    }

    selectAll(select: NgModel, values) {
        select.update.emit(values.map(t => t.accountGuid));
    }

    deselectAll(select: NgModel) {
        select.update.emit([]);
    }

    copy(id) {
        this.ngZone.run(() => this.router.navigate(['/newmanualtransfer/' + id]));
    }

    loadManualTransfers() {

        if (this.selectedTenant == undefined || this.gridApi == undefined || this.bankLogins == undefined || this.bankLogins.length == 0 || this.bankAccounts == undefined || this.bankAccounts.length == 0 || this.args.accountGuids == null || this.args.accountGuids.length == 0) {
            return;
        }
        this.firstLoaded = true;
        this.args.timeZoneInfoId = this.selectedZoneId;


        localStorage.setItem('manualtransferargs', JSON.stringify(this.args));

        this.store.dispatch(new manualTransferActions.Search(this.args));
    }

    refresh() {
        this.gridApi.paginationGoToFirstPage();
        this.loadManualTransfers();
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

    getAccounts() {
        if (this.filteredBankAccounts) {
            return this.filteredBankAccounts;
        }

        if (this.bankAccounts) {
            return this.bankAccounts;
        }

        return [];
    }

    getLoginName(loginGuid: string): string {
        if (this.bankLogins) {
            var login = this.bankLogins.find(t => t.loginGuid == loginGuid);

            if (login != null) {
                return login.friendlyName;
            }
        }
        return '';
    }

    getAccountName(accountGuid: string): string {
        if (this.bankAccounts) {
            var account = this.bankAccounts.find(t => t.accountGuid == accountGuid);

            if (account != null) {
                return account.accountNo;
            }
        }

        return '';
    }

    getStatusName(status: number): string {

        let item = ManualTransferStatuses.find(t => t.value == status);

        if (item != null) {
            return this.translateService.instant(item.key);
        }

        return '';
    }

    getPriorityName(priority: number): string {
        let item = this.priorities.find(t => t.value == priority);

        if (item != null) {
            return this.translateService.instant(item.key);
        }

        return '';
    }

    getTransferTypeName(transferType: number): string {

        let item = TransferTypes.find(t => t.value == transferType);

        if (item != null) {
            return this.translateService.instant(item.key);
        }

        return '';
    }

    getAmount(data) {
        if (data == undefined || data == null) {
            return '';
        }
        if (data.transferWholeAmount == true) {
            return this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.BALANCE');
        }

        return this.num.transform(data.amount);
    }

    cancel(id: number) {
        if (id > 0) {
            const dialogRef = this.dialog.open(DeactivateLoginDialogComponent, {
                width: '250px',
                data: 'MANUAL-TRANSFER.GENERAL.CANCELATION-APPROVE'
            });

            dialogRef.afterClosed().subscribe(result => {
                if (result == true) {
                    this.store.dispatch(new manualTransferActions.Cancel(id));
                }
            });
        }
    }

    cancelationAllowed(params): boolean {
        var result = this.allowAddManualTransfer && params != undefined && params.data != undefined && params.data.status == 1;
        return result;
    }

    ngOnDestroy(): void {
        this.store.dispatch(new accountActions.ClearErrors());
        this.store.dispatch(new manualTransferActions.Clear());
        this.store.dispatch(new bankLoginActions.ClearAll());
        this.destroyed$.next(true);
        this.destroyed$.complete();
    }

    openSnackBar(message: string, action: string = undefined) {
        if (!action) {
            action = this.translateService.instant('GENERAL.OK');
        }
        this.snackBar.open(message, action, {
            duration: 10000,
        });
    }

    onCellClicked(e: CellClickedEvent) {
        if (e && e.colDef && e.colDef.field != 'id') {
            var selectedRows = this.gridApi.getSelectedRows();

            if (selectedRows.length > 0) {
                var selected = selectedRows[0];

                this.ngZone.run(() => this.router.navigate(['/editmanualtransfer/' + selected.id]));
            }
        }
    }

    setTimeZone() {
        let zone = this.zones.find(t => t.timeZoneId == this.selectedZoneId);
        this.timeZoneService.setTimeZone(zone);
        this.refresh();
    }

    loadItems(params: IGetRowsParams) {
        this.args.startRow = params.startRow;
        this.args.endRow = params.endRow;
        if (params.sortModel && params.sortModel.length > 0) {
            this.args.sortOrder = params.sortModel[0].sort;
            this.args.sortColumn = params.sortModel[0].colId;
        }
        else {
            this.args.sortColumn = null;
            this.args.sortOrder = null;
        }

        this.args.filterModel = params.filterModel;

        this.loadManualTransfers();
    }

    onGridReady(readyParams) {
        this.gridApi = readyParams.api;
        this.gridColumnApi = readyParams.columnApi;

        var datasource = {
            getRows: (params: IGetRowsParams) => {
                this.callBackApi = params;
                this.loadItems(params);
            }
        };

        readyParams.api.setDatasource(datasource);
    }
}
