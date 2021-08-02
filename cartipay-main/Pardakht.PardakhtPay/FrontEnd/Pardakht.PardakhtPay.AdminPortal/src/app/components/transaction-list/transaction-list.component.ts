import { Component, OnInit, ViewChild, OnDestroy, Input, Output, EventEmitter } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource, MatSnackBar, MatDialog } from '@angular/material';
import { Observable, ReplaySubject } from 'rxjs';
import { TransactionSearch, TransactionSearchArgs, PaymentTypes, TransactionStatus, PaymentType } from '../../models/transaction';
import * as transactionactions from '../../core/actions/transaction';
//import * as merchantActions from '../../core/actions/merchant';
import { fuseAnimations } from '../../core/animations';
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import { ListSearchResponse, defaultColumnDefs } from '../../models/types';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
//import { Merchant } from '../../models/merchant-model';
import * as tenantActions from '../../core/actions/tenant';
import { Tenant } from '../../models/tenant';
import { TimeZoneService } from '../../core/services/timeZoneService/time-zone.service';
import { TimeZone } from '../../models/timeZone';
import { IGetRowsParams } from 'ag-grid-community';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { DateFormatterComponent } from '../formatters/dateformatter';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { MerchantCustomer } from '../../models/merchant-customer';
import * as userSegmentGroupActions from '../../core/actions/userSegmentGroup';
import { UserSegmentGroup } from '../../models/user-segment-group';
import { IconButtonFormatterComponent } from '../formatters/iconbuttonformatter';
import { DeactivateLoginDialogComponent } from '../deactivate-login-dialog/deactivate-login-dialog.component';
import { locale } from 'moment';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';

@Component({
    selector: 'app-transaction-list',
    templateUrl: './transaction-list.component.html',
    styleUrls: ['./transaction-list.component.scss'],
    animations: fuseAnimations
})
export class TransactionListComponent implements OnInit, OnDestroy {
    args: TransactionSearchArgs;

    @Input() merchantCustomer: MerchantCustomer;
    @Input() withdrawalId: number;

    loading$: Observable<boolean>;
    transactions$: Observable<ListSearchResponse<TransactionSearch[]>>;
    transactions: ListSearchResponse<TransactionSearch[]>;
    searchError$: Observable<string>;

    tenants$: Observable<Tenant[]>;
    tenants: Tenant[] = [];

    tenantGuid$: Observable<string>;
    tenantGuid: string;

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

    userSegmentGroups$: Observable<UserSegmentGroup[]>;
    userSegmentGroups: UserSegmentGroup[];
    userSegmentGroupLoadingError$: Observable<string>;
    getUserSegmentGroupsLoading$: Observable<boolean>;
    getUserSegmentGroupsLoading: boolean;

    setAsCompleted$: Observable<boolean>;
    setAsCompletedError$: Observable<string>;

    setAsExpired$: Observable<boolean>;
    setAsExpiredError$: Observable<string>;

    callbackToMerchant$: Observable<string>;
    callbackToMerchantError$: Observable<string>;
    allowTransactionCallback: boolean = false;
    allowSetAsCompleted = false;

    gridApi;
    callBackApi;
    gridColumnApi;
    columnDefs: any[];
    enableRtl: boolean = false;
    frameworkComponents;
    rowClassRules;
    paymentTypes = PaymentTypes;
    defaultColumnDefs = defaultColumnDefs;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    constructor(private store: Store<coreState.State>,
        private snackBar: MatSnackBar,
        private timeZoneService: TimeZoneService,
        private fuseConfigService: FuseConfigService,
        private translateService: TranslateService,
        private accountService: AccountService,
        public dialog: MatDialog) {

        var cachedArgs = localStorage.getItem('transactionargs');

        if (cachedArgs != null && cachedArgs != undefined) {
            this.args = JSON.parse(cachedArgs);
            this.args.token = null;
            this.args.pageSize = 25;
        }
        else {
            this.createSearchArgs();
        }
        this.allowTransactionCallback = this.accountService.isUserAuthorizedForTask(permissions.TransactionCallBackToMerchant);
        this.allowSetAsCompleted = this.accountService.isUserAuthorizedForTask(permissions.SetDepositAsCompleted);

        this.columnDefs = [
            {
                headerName: this.translateService.instant('TRANSACTION.LIST-COLUMNS.ID'),
                field: "id",
                sortable: true,
                resizable: true,
                width: 80,
                colId: "id",
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('TRANSACTION.LIST-COLUMNS.STATUS'),
                field: "status",
                colId: "status",
                sortable: true,
                resizable: true,
                width: 150,
                valueGetter: params => this.getStatusName(params.data == undefined ? 0 : params.data.status),
                cellClassRules: {
                    "": "data == undefined",
                    "bg-color-1": "data != undefined && data.status == 1",
                    "bg-color-2": "data != undefined && data.status == 2",
                    "bg-color-3": "data != undefined && data.status == 3",
                    "bg-color-4": "data != undefined && data.status == 4",
                    "bg-color-5": "data != undefined && data.status == 5",
                    "bg-color-6": "data != undefined && data.status == 6",
                    "bg-color-7": "data != undefined && data.status == 7",
                    "bg-color-8": "data != undefined && data.status == 8"
                }
            },
            {
                headerName: this.translateService.instant('TRANSACTION.LIST-COLUMNS.TITLE'),
                colId: "merchantTitle",
                field: "merchantTitle",
                sortable: true,
                resizable: true,
                width: 200
            },
            {
                headerName: this.translateService.instant('TRANSACTION.LIST-COLUMNS.AMOUNT'),
                field: "transactionAmount",
                colId: "transactionAmount",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 100,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('TRANSACTION.LIST-COLUMNS.DATE'),
                field: "transactionDateStr",
                colId: "transactionDate",
                cellRenderer: 'dateFormatterComponent',
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agDateColumnFilter"
            },
            {
                headerName: this.translateService.instant('TRANSACTION.LIST-COLUMNS.TRANSFERRED_DATE'),
                field: "transferredDateStr",
                colId: "transferredDate",
                cellRenderer: 'dateFormatterComponent',
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agDateColumnFilter"
            },
            {
                headerName: this.translateService.instant('TRANSACTION.LIST-COLUMNS.ACCOUNT_NUMBER'),
                field: "accountNumber",
                colId: "accountNumber",
                resizable: true,
                width: 150,
                filter: "agTextColumnFilter",
                filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    filterOptions: ["equals"],
                    defaultOption: "equals"
                }
            },
            {
                headerName: this.translateService.instant('TRANSACTION.LIST-COLUMNS.MERCHANT_CARD_NUMBER'),
                field: "merchantCardNumber",
                colId: "merchantCardNumber",
                resizable: true,
                width: 170,
                filter: "agTextColumnFilter", filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    filterOptions: ["equals"],
                    defaultOption: "equals"
                }
            },
            {
                headerName: this.translateService.instant('TRANSACTION.LIST-COLUMNS.CUSTOMER_CARD_NUMBER'),
                field: "customerCardNumber",
                colId: "customerCardNumber",
                resizable: true,
                width: 170,
                filter: "agTextColumnFilter", filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    filterOptions: ["equals"],
                    defaultOption: "equals"
                }
            },
            {
                headerName: this.translateService.instant('CARD-TO-CARD.LIST-COLUMNS.CARD-HOLDER-NAME'),
                field: "cardHolderName",
                colId: "cardHolderName",
                width: 200,
                resizable: true,
                filter: "agTextColumnFilter", filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    filterOptions: ["equals"]
                }
            },
            {
                headerName: this.translateService.instant('TRANSACTION.LIST-COLUMNS.BANK_NUMBER'),
                field: "bankNumber",
                colId: "bankNumber",
                resizable: true,
                sortable: true,
                width: 100,
                filter: "agTextColumnFilter",
                filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    defaultOption: "equals"
                }
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.USER-ID'),
                field: "userId",
                colId: "userId",
                resizable: true,
                sortable: true,
                width: 100,
                cellRenderer: params => this.userHeader(params),
                filter: "agTextColumnFilter", filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    filterOptions: ["equals"],
                    defaultOption: "equals"
                }
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.WEBSITE-NAME'),
                field: "websiteName",
                colId: "websiteName",
                resizable: true,
                sortable: true,
                width: 100,
                filter: "agTextColumnFilter", filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    filterOptions: ["equals"],
                    defaultOption: "equals"
                }
            },
            //{
            //    headerName: this.translateService.instant('BANK-STATEMENT.LIST-COLUMNS.DESCRIPTION'),
            //    field: "description",
            //    colId: "description",
            //    resizable: true,
            //    sortable: true,
            //    width: 300
            //},
            {
                headerName: this.translateService.instant('WITHDRAWAL.LIST-COLUMNS.REFERENCE'),
                field: "reference",
                colId: "reference",
                resizable: true,
                sortable: true,
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
                headerName: this.translateService.instant('USER-SEGMENT.LIST-COLUMNS.GROUP'),
                field: "userSegmentGroupName",
                colId: "userSegmentGroupName", // "userSegmentGroupId",
                sortable: true,
                resizable: true,
               // valueGetter: params => params == undefined || params.data == undefined ? '' : this.getUserSegmentGroupName(params.data.userSegmentGroupId),
                width: 200,                
                filter: "agTextColumnFilter",
                filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    filterOptions: ["equals"],
                    defaultOption: "equals"
                }
            },
            {
                headerName: this.translateService.instant('TRANSACTION.PAYMENT-TYPES.PLACE-HOLDER'),
                field: "paymentType",
                colId: "paymentType",
                sortable: true,
                resizable: true,
                valueGetter: params => params == undefined || params.data == undefined ? '' : this.getPaymentTypeName(params.data.paymentType),
                width: 200
            },
            {
                headerName: this.translateService.instant('TRANSACTION.LIST-COLUMNS.EXTERNAL-ID'),
                field: "externalId",
                colId: "externalId",
                resizable: true,
                sortable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('TRANSACTION.LIST-COLUMNS.EXTERNAL-MESSAGE'),
                field: "externalMessage",
                colId: "externalMessage",
                resizable: true,
                sortable: true,
                width: 400
            },
            {
                headerName: this.translateService.instant('WITHDRAWAL.LIST-COLUMNS.ID'),
                field: "withdrawalId",
                colId: "withdrawalId",
                sortable: true,
                resizable: true,
                width: 100
            },
            {
                headerName: this.translateService.instant('TRANSACTION.LIST-COLUMNS.PROCESS-ID'),
                field: "processId",
                colId: "processId",
                sortable: true,
                resizable: true,
                width: 100
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.COMPLETED'),
                field: "id",
                colId: "setAsCompleted",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.setAsCompleted.bind(this),
                    icon: 'check_circle',
                    iconClass: 'success',
                    title: 'MANUAL-TRANSFER.LIST-COLUMNS.COMPLETED',
                    allow: true,
                    allowed: this.setAsCompletedAllowed.bind(this)
                },
                hide: false
            },
            {
                headerName: this.translateService.instant('TRANSACTION.STATUS.EXPIRED'),
                field: "id",
                colId: "setAsExpired",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.setAsExpired.bind(this),
                    icon: 'cancel',
                    iconClass: 'danger',
                    title: 'TRANSACTION.STATUS.EXPIRED',
                    allow: true,
                    allowed: this.setAsExpiredAllowed.bind(this)
                },
                hide: false
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.WITHDRAWALCALLBACKTOMERCHANT'),
                field: "id",
                colId: "id",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.callBack.bind(this),
                    icon: 'phone_callback',
                    iconClass: '',
                    title: 'MANUAL-TRANSFER.GENERAL.WITHDRAWALCALLBACKTOMERCHANT',
                    allow: true,
                    allowed: this.callbackAllowed.bind(this)
                },
                hide: false
            }
        ];

        this.rowClassRules = {
        };
    }

    ngOnInit() {

        if (this.merchantCustomer != undefined && this.merchantCustomer != null) {
            this.createSearchArgs();
            this.args.merchantCustomerId = this.merchantCustomer.id;
            //this.args.dateRange = 'all';
        }
        else if (this.withdrawalId != undefined && this.withdrawalId != null) {
            this.createSearchArgs();
            this.args.withdrawalId = this.withdrawalId;
            //this.args.dateRange = 'all';
        }
        else {
            this.args.merchantCustomerId = null;
            this.args.withdrawalId = null;
        }

        this.fuseConfigService.config.pipe(takeUntil(this.destroyed$)).subscribe(config => {

            this.enableRtl = config.locale != null && config.locale != undefined && config.locale.direction == 'rtl';
        });

        this.frameworkComponents = {
            booleanFormatterComponent: BooleanFormatterComponent,
            dateFormatterComponent: DateFormatterComponent,
            numberFormatterComponent: NumberFormatterComponent,
            iconButtonFormatterComponent: IconButtonFormatterComponent
        };

        this.loading$ = this.store.select(coreState.getTransactionLoading);
        this.transactions$ = this.store.select(coreState.getTransactionSearchResults);
        this.searchError$ = this.store.select(coreState.getTransactionSearchError);
        this.tenants$ = this.store.select(coreState.getTenantSearchResults);

        this.zones = this.timeZoneService.getTimeZones();
        this.selectedZoneId = this.timeZoneService.getTimeZone().timeZoneId;

        this.searchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.transactions$.pipe(takeUntil(this.destroyed$)).subscribe((data: ListSearchResponse<TransactionSearch[]>) => {
            this.transactions = data;

            if (this.callBackApi) {
                if (data) {
                    this.callBackApi.successCallback(data.items, data.paging.totalItems);
                } else {
                    this.callBackApi.successCallback([]);
                }
            }
        });

        this.tenants$.pipe(takeUntil(this.destroyed$)).subscribe(items => {

            this.tenants = items;
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

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new userSegmentGroupActions.GetAll());
                this.loadTransactions();
            }
        });

        this.userSegmentGroups$ = this.store.select(coreState.getAllUserSegmentGroups);

        this.userSegmentGroups$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.userSegmentGroups = items;
        });

        this.getUserSegmentGroupsLoading$ = this.store.select(coreState.getUserSegmentGroupLoading);

        this.getUserSegmentGroupsLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.getUserSegmentGroupsLoading = l;
        });

        this.userSegmentGroupLoadingError$ = this.store.select(coreState.getAllUserSegmentGroupError);

        this.userSegmentGroupLoadingError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.setAsCompletedError$ = this.store.select(coreState.getTransactionSetAsCompletedError);
        this.setAsCompletedError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.setAsExpiredError$ = this.store.select(coreState.getTransactionSetAsExpiredError);
        this.setAsExpiredError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.setAsCompleted$ = this.store.select(coreState.getTransactionSetAsCompletedSuccess);
        this.setAsCompleted$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            if (t) {
                this.refresh();
            }
        });

        this.setAsExpired$ = this.store.select(coreState.getTransactionSetAsExpiredSuccess);
        this.setAsExpired$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            if (t) {
                this.refresh();
            }
        });

        this.callbackToMerchant$ = this.store.select(coreState.transactionCallbackToMerchant);
        this.callbackToMerchant$.pipe(takeUntil(this.destroyed$)).subscribe(callbackToMerchant => {
            if (callbackToMerchant) {
                let action = this.translateService.instant('GENERAL.OK');
                let message = callbackToMerchant;
                this.snackBar.open(message, action, {
                    duration: 30000,
                    verticalPosition: 'top'
                });
            }
        });

        this.callbackToMerchantError$ = this.store.select(coreState.transactionCallbackToMerchantError);
        this.callbackToMerchantError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });
    }

    loadTransactions() {

        if (this.selectedTenant == undefined || this.callBackApi == undefined) {
            return;
        }
        this.args.timeZoneInfoId = this.selectedZoneId;

        //this.args.tenants = [this.selectedTenant.tenantDomainPlatformMapGuid];

        if (this.args.merchantCustomerId == null && this.args.withdrawalId == null) {
            localStorage.setItem('transactionargs', JSON.stringify(this.args));
        }

        this.store.dispatch(new transactionactions.Search(this.args));
    }

    refresh() {
        this.gridApi.paginationGoToFirstPage();
        this.loadTransactions();
    }

    getStatusName(status: number): string {
        switch (status) {
            case 1:
                return this.translateService.instant('TRANSACTION.STATUS.STARTED');
            case 2:
                return this.translateService.instant('TRANSACTION.STATUS.TOKEN_VALIDATED');
            case 3:
                return this.translateService.instant('TRANSACTION.STATUS.WAITING_CONFIRMATION');
            case 4:
                return this.translateService.instant('TRANSACTION.STATUS.COMPLETED');
            case 5:
                return this.translateService.instant('TRANSACTION.STATUS.EXPIRED');
            case 6:
                return this.translateService.instant('TRANSACTION.STATUS.CANCELLED');
            case 7:
                return this.translateService.instant('TRANSACTION.STATUS.FRAUD');
            case 8:
                return this.translateService.instant('TRANSACTION.STATUS.REVERSED');
            default:
                return '';
        }
    }

    getPaymentTypeName(paymentType: number) {
        var type = this.paymentTypes.find(t => t.value == paymentType);

        if (type == null) {
            return '';
        }

        return this.translateService.instant(type.key);
    }

    userHeader(params) {
        let content = '';

        if (params == undefined || params.data == undefined) {
            return '';
        }

        if (params.data.merchantCustomerId != null) {
            return '<a href="/merchantcustomer/' + params.data.merchantCustomerId + '" target="_blank">' + params.data.userId + '</>';
        }
        else {
            return params.data.userId;
        }
    }

    openCustomerDetail(merchantCustomerId: number) {
        if (merchantCustomerId != undefined && merchantCustomerId != null) {
            window.open('/merchantcustomer/' + merchantCustomerId, '_blank');
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

    getUserSegmentGroupName(id: number): string {
        if (id) {
            if (this.userSegmentGroups) {
                var segment = this.userSegmentGroups.find(t => t.id == id);

                if (segment != null) {
                    return segment.name;
                }
            }
        }

        return '';
    }

    setAsCompleted(id: number) {
        if (id > 0) {
            const dialogRef = this.dialog.open(DeactivateLoginDialogComponent, {
                width: '250px',
                data: 'MANUAL-TRANSFER.GENERAL.COMPLETE-APPROVE'
            });

            dialogRef.afterClosed().subscribe(result => {
                if (result == true) {
                    this.store.dispatch(new transactionactions.SetAsCompleted(id));
                }
            });
        }
    }

    setAsCompletedAllowed(params): boolean {
        var result = this.allowSetAsCompleted && params != undefined && params.data != undefined && (params.data.paymentType == PaymentType.Mobile && (params.data.status == TransactionStatus.Expired || params.data.status == TransactionStatus.WaitingConfirmation));
        return result;
    }

    setAsExpired(id: number) {
        if (id > 0) {
            const dialogRef = this.dialog.open(DeactivateLoginDialogComponent, {
                width: '250px',
                data: 'MANUAL-TRANSFER.GENERAL.CANCELATION-APPROVE'
            });

            dialogRef.afterClosed().subscribe(result => {
                if (result == true) {
                    this.store.dispatch(new transactionactions.SetAsExpired(id));
                }
            });
        }
    }

    setAsExpiredAllowed(params): boolean {
        var result = params != undefined && params.data != undefined && (params.data.paymentType == PaymentType.Mobile && (params.data.status == TransactionStatus.Expired || params.data.status == TransactionStatus.WaitingConfirmation));
        return result;
    }

    ngOnDestroy(): void {
        this.store.dispatch(new transactionactions.ClearAll());
        this.store.dispatch(new userSegmentGroupActions.Clear());
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

    setTimeZone() {
        let zone = this.zones.find(t => t.timeZoneId == this.selectedZoneId);
        this.timeZoneService.setTimeZone(zone);
        this.refresh();
    }

    createSearchArgs() {
        this.args = new TransactionSearchArgs();
        this.args.pageSize = 25;
        //this.args.dateRange = 'dt';
        this.args.statuses = ['1', '2', '3', '4', '5', '6', '7', '8'];
        //this.args.tenants = [];
        this.args.paymentType = 0;
    }

    onColumnMoved(params) {
        var columnState = JSON.stringify(params.columnApi.getColumnState());
        localStorage.setItem('transactionColumnState', columnState);
    }

    loadItems(params: IGetRowsParams) {
        if (params.sortModel && params.sortModel.length > 0) {
            this.args.sortOrder = params.sortModel[0].sort;
            this.args.sortColumn = params.sortModel[0].colId;
        }
        else {
            this.args.sortColumn = null;
            this.args.sortOrder = null;
        }

        this.args.filterModel = params.filterModel;

        this.args.startRow = params.startRow;
        this.args.endRow = params.endRow;

        this.loadTransactions();
    }

    onGridReady(readyParams) {
        this.gridApi = readyParams.api;
        this.gridColumnApi = readyParams.columnApi;

        var columnState: any[] = JSON.parse(localStorage.getItem('transactionColumnState'));
        if (columnState) {

            if (this.columnDefs.find(t => columnState.findIndex(r => t.colId == r.colId) == -1) != null) {
                localStorage.removeItem('transactionColumnState');
            }
            else {
                readyParams.columnApi.setColumnState(columnState);
            }
        }

        var datasource = {
            getRows: (params: IGetRowsParams) => {
                this.callBackApi = params;
                this.loadItems(params);
            }
        };
        readyParams.api.setDatasource(datasource);
    }

    callbackAllowed(params): boolean {

        return this.allowTransactionCallback;
    }

    callBack(id: number) {
        if (id > 0) {
            const dialogRef = this.dialog.open(DeactivateLoginDialogComponent, {
                width: '250px',
                data: 'MANUAL-TRANSFER.GENERAL.CALLBACK-APPROVE'
            });

            dialogRef.afterClosed().subscribe(result => {
                if (result == true) {
                    this.store.dispatch(new transactionactions.TransactionCallbackToMerchant(id));
                }
            });
        }
    }
}
