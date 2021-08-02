import { Component, OnInit, ViewChild, OnDestroy, Input } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource, MatSnackBar, MatDialog } from '@angular/material';
import { Observable, ReplaySubject } from 'rxjs';
import { WithdrawalSearch, WithdrawalSearchArgs, WithdrawalReceipt, WithdrawalProcessType, WithdrawalProcessTypes, Withdrawal, WithdrawalStatuses, WithdrawalStatusEnum } from '../../models/withdrawal';
import * as withdrawalActions from '../../core/actions/withdrawal';
import * as merchantActions from '../../core/actions/merchant';
import { fuseAnimations } from '../../core/animations';
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import { ListSearchResponse, defaultColumnDefs } from '../../models/types';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Merchant } from '../../models/merchant-model';
import * as applicationSettingsActions from '../../core/actions/applicationSettings';
import { Tenant } from '../../models/tenant';
import { TimeZoneService } from '../../core/services/timeZoneService/time-zone.service';
import { TimeZone } from '../../models/timeZone';
import { IGetRowsParams, CellClickedEvent } from 'ag-grid-community';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { DateFormatterComponent } from '../formatters/dateformatter';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { MerchantCustomer } from '../../models/merchant-customer';
import { DownloadWithdrawalReceiptFormatterComponent } from '../formatters/downloadWithdrawalReceiptFormatter';
import { IconButtonFormatterComponent } from '../formatters/iconbuttonformatter';
import { DeactivateLoginDialogComponent } from '../deactivate-login-dialog/deactivate-login-dialog.component';
import { TransferStatusEnum } from '../../models/types';
import { Router } from '@angular/router';
import { TransferStatusDescription } from '../../models/application-settings';
import * as permissions from '../../models/permissions';
import { AccountService } from '../../core/services/account.service';


@Component({
    selector: 'app-withdrawal-list',
    templateUrl: './withdrawal-list.component.html',
    styleUrls: ['./withdrawal-list.component.scss'],
    animations: fuseAnimations
})
export class WithdrawalListComponent implements OnInit, OnDestroy {

    args: WithdrawalSearchArgs;

    selectedWithdrawal: Withdrawal = undefined;

    @Input() merchantCustomer: MerchantCustomer;

    loading$: Observable<boolean>;
    items$: Observable<ListSearchResponse<WithdrawalSearch[]>>;
    items: ListSearchResponse<WithdrawalSearch[]>;
    searchError$: Observable<string>;

    merchants$: Observable<Merchant[]>;
    merchantError$: Observable<string>;
    merchants: Merchant[];

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

    receipt$: Observable<WithdrawalReceipt>;

    receiptError$: Observable<string>;

    cancelSuccess$: Observable<boolean>;
    cancelError$: Observable<string>;

    retrySuccess$: Observable<boolean>;
    retryError$: Observable<string>;

    setAsCompletedSuccess$: Observable<boolean>;
    setAsCompletedError$: Observable<string>;

    changeProcessTypeSuccess$: Observable<boolean>;
    changeProcessTypeError$: Observable<string>;

    changeAllProcessTypeSuccess$: Observable<boolean>;

    transferStatuses$: Observable<TransferStatusDescription[]>;
    transferStatuses: TransferStatusDescription[];
    transferStatusError$: Observable<string>;

    callbackToMerchant$: Observable<string>;
    callbackToMerchantError$: Observable<string>;


    allowWithdrawCallback: boolean = false;


    gridApi;
    callBackApi;
    gridColumnApi;
    columnDefs: any[];
    enableRtl: boolean = false;
    frameworkComponents;
    rowClassRules;

    defaultColumnDefs = defaultColumnDefs;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    statuses = [
        {
            status: 0,
            translate: 'WITHDRAWAL.STATUS.ALL'
        },
        {
            status: 1,
            translate: 'WITHDRAWAL.STATUS.PENDING'
        },
        {
            status: 2,
            translate: 'WITHDRAWAL.STATUS.TRANSFERRED'
        },
        {
            status: 3,
            translate: 'WITHDRAWAL.STATUS.CANCELLED'
        },
        {
            status: 4,
            translate: 'WITHDRAWAL.STATUS.SENT'
        },
        {
            status: 5,
            translate: 'WITHDRAWAL.STATUS.CONFIRMED'
        },
        {
            status: 6,
            translate: 'WITHDRAWAL.STATUS.REFUNDED'
        },
        {
            status: 7,
            translate: 'WITHDRAWAL.STATUS.PENDING-APPROVAL'
        },
        {
            status: 8,
            translate: 'WITHDRAWAL.STATUS.PARTIAL-PAID'
        }]

    constructor(private store: Store<coreState.State>,
        private snackBar: MatSnackBar,
        private timeZoneService: TimeZoneService,
        private fuseConfigService: FuseConfigService,
        private translateService: TranslateService,
        private accountService: AccountService,
        public dialog: MatDialog,
        private router: Router) {

        var cachedArgs = localStorage.getItem('withdrawalargs');

        if (cachedArgs != null && cachedArgs != undefined) {
            this.args = JSON.parse(cachedArgs);
        } else {
            this.createSearchArgs();
        }

        this.zones = this.timeZoneService.getTimeZones();
        this.selectedZoneId = this.timeZoneService.getTimeZone().timeZoneId;
        this.allowWithdrawCallback = this.accountService.isUserAuthorizedForTask(permissions.WithdrawalsCallBackToMerchant);

    }

    ngOnInit() {
        this.store.dispatch(new withdrawalActions.Clear());
        this.store.dispatch(new applicationSettingsActions.ClearErrors());

        if (this.merchantCustomer != undefined && this.merchantCustomer != null) {
            this.createSearchArgs();
            this.args.merchantCustomerId = this.merchantCustomer.id;
        }
        else {
            this.args.merchantCustomerId = null;
        }

        this.loading$ = this.store.select(coreState.getWithdrawalLoading);
        this.items$ = this.store.select(coreState.getWithdrawalSearchResults);
        this.searchError$ = this.store.select(coreState.getWithdrawalSearchError);

        this.searchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.tenantGuid$ = this.store.select(coreState.getAccountTenantGuid);

        this.tenantGuid$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.tenantGuid = t;
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

        this.receipt$ = this.store.select(coreState.getWithdrawalReceipt);

        this.receipt$.pipe(takeUntil(this.destroyed$)).subscribe(item => {
            if (item != undefined) {
                console.log(item);
                var url = window.URL.createObjectURL(item.data);

                var link = document.createElement("a");
                link.setAttribute("href", url);
                link.setAttribute("download", item.fileName);
                link.style.display = "none";
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            }
        });

        this.receiptError$ = this.store.select(coreState.getWithdrawalReceiptError);

        this.receiptError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.merchants$ = this.store.select(coreState.getAllMerchants);
        this.merchantError$ = this.store.select(coreState.getAllMerchantError);

        this.merchants$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.merchants = items;
        });

        this.items$.pipe(takeUntil(this.destroyed$)).subscribe((data: ListSearchResponse<WithdrawalSearch[]>) => {
            this.items = data;
            if (this.callBackApi) {
                if (data) {
                    this.callBackApi.successCallback(data.items, data.paging.totalItems);
                } else {
                    this.callBackApi.successCallback([]);
                }
            }
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new applicationSettingsActions.GetTransferStatus());
                this.loadWithdrawals();
            }
        });

        this.cancelError$ = this.store.select(coreState.getWithdrawalCancelError);
        this.cancelError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.cancelSuccess$ = this.store.select(coreState.getWithdrawalCancelSuccess);
        this.cancelSuccess$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            if (t) {
                this.refresh();
            }
        });

        this.retryError$ = this.store.select(coreState.getWithdrawalRetryError);
        this.retryError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.retrySuccess$ = this.store.select(coreState.getWithdrawalRetrySuccess);
        this.retrySuccess$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            if (t) {
                this.refresh();
            }
        });

        this.setAsCompletedError$ = this.store.select(coreState.getWithdrawalSetAsCompletedError);
        this.setAsCompletedError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.setAsCompletedSuccess$ = this.store.select(coreState.getWithdrawalSetAsCompletedSuccess);
        this.setAsCompletedSuccess$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            if (t) {
                this.refresh();
            }
        });

        this.changeProcessTypeError$ = this.store.select(coreState.getWithdrawalChangeProcessTypeError);

        this.changeProcessTypeError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.changeProcessTypeSuccess$ = this.store.select(coreState.getWithdrawalChangeProcessTypeSuccess);

        this.changeProcessTypeSuccess$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            if (t) {
                this.refresh();
            }
        });

        this.changeAllProcessTypeSuccess$ = this.store.select(coreState.getWithdrawalChangeAllProcessTypeSuccess);

        this.changeAllProcessTypeSuccess$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            if (t) {
                this.refresh();
            }
        });

        this.transferStatusError$ = this.store.select(coreState.getTransferStatusesError);

        this.transferStatusError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.transferStatuses$ = this.store.select(coreState.getTransferStatuses);

        this.transferStatuses$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.transferStatuses = items;
        });

        this.columnDefs = [
            {
                headerName: '',
                field: "id",
                colId: "detail",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    icon: 'details',
                    iconClass: 'success',
                    allow: true,
                    allowed: undefined
                },
                hide: false
            },
            {
                headerName: this.translateService.instant('GENERAL.ID'),
                field: "id",
                colId: "id",
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.LOGIN-NAME'),
                field: "friendlyName",
                colId: "friendlyName",
                resizable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('TRANSACTION.LIST-COLUMNS.TITLE'),
                field: "merchantId",
                colId: "merchantId",
                resizable: true,
                width: 150,
                valueGetter: params => params == undefined || params.data == undefined ? '' : this.getMerchantTitle(params.data.merchantId)
            },
            {
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.FROM-ACCOUNT-NO'),
                field: "fromAccountNumber",
                colId: "fromAccountNumber",
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
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.TO-ACCOUNT-NO'),
                field: "toAccountNumber",
                colId: "toAccountNumber",
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
                headerName: this.translateService.instant('WITHDRAWAL.LIST-COLUMNS.TO-IBAN-NUMBER'),
                field: "toIbanNumber",
                colId: "toIbanNumber",
                resizable: true,
                width: 150,
                filter: "agTextColumnFilter",
                filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    caseSensitive: true,
                    filterOptions: ["equals"],
                    defaultOption: "equals"
                }
            },
            {
                headerName: this.translateService.instant('CARD-TO-CARD.LIST-COLUMNS.CARD-NUMBER'),
                field: "cardNumber",
                colId: "cardNumber",
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
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.AMOUNT'),
                field: "amount",
                colId: "amount",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('WITHDRAWAL.LIST-COLUMNS.REMAINING-AMOUNT'),
                field: "remainingAmount",
                colId: "remainingAmount",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('WITHDRAWAL.LIST-COLUMNS.PENDING-APPROVAL-AMOUNT'),
                field: "pendingApprovalAmount",
                colId: "pendingApprovalAmount",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.STATUS'),
                field: "withdrawalStatus",
                colId: "withdrawalStatus",
                sortable: true,
                resizable: true,
                width: 200,
                valueGetter: params => this.getStatusName(params.data == undefined ? '' : params.data)
            },
            {
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.DATE'),
                field: "transferRequestDateStr",
                colId: "transferRequestDate",
                sortable: true,
                resizable: true,
                cellRenderer: 'dateFormatterComponent',
                width: 150,
                filter: "agDateColumnFilter"
            },
            {
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.TRANSFERRED_DATE'),
                field: "transferDateStr",
                colId: "transferDate",
                cellRenderer: 'dateFormatterComponent',
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agDateColumnFilter"
            },
            {
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.CANCEL-DATE'),
                field: "cancelDateStr",
                colId: "cancelDate",
                cellRenderer: 'dateFormatterComponent',
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agDateColumnFilter"
            },
            {
                headerName: this.translateService.instant('WITHDRAWAL.LIST-COLUMNS.EXPECTED-TRANSFER-DATE'),
                field: "expectedTransferDateStr",
                colId: "expectedTransferDate",
                cellRenderer: 'dateFormatterComponent',
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agDateColumnFilter"
            },
            {
                headerName: this.translateService.instant('WITHDRAWAL.LIST-COLUMNS.TRANSFER_NOTES'),
                field: "transferNotes",
                colId: "transferNotes",
                resizable: true,
                width: 150,
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
                sortable: true,
                resizable: true,
                width: 150,
                cellRenderer: params => this.userHeader(params),
                filter: "agTextColumnFilter",
                filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    caseSensitive: true,
                    defaultOption: "equals"
                }
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.WEBSITE-NAME'),
                field: "websiteName",
                colId: "websiteName",
                resizable: true,
                sortable: true,
                width: 150,
                filter: "agTextColumnFilter",
                filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    caseSensitive: true,
                    clearButton: true,
                    defaultOption: "equals"
                }
            },
            {
                headerName: this.translateService.instant('WITHDRAWAL.LIST-COLUMNS.FIRST-NAME'),
                field: "firstName",
                colId: "firstName",
                resizable: true,
                sortable: true,
                width: 150,
                filter: "agTextColumnFilter",
                filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    filterOptions: ["equals"],
                    caseSensitive: true,
                    defaultOption: "equals"
                }
            },
            {
                headerName: this.translateService.instant('WITHDRAWAL.LIST-COLUMNS.LAST-NAME'),
                field: "lastName",
                colId: "lastName",
                resizable: true,
                sortable: true,
                width: 150,
                filter: "agTextColumnFilter",
                filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    filterOptions: ["equals"],
                    caseSensitive: true,
                    defaultOption: "equals"
                }
            },
            {
                headerName: this.translateService.instant('CARD-TO-CARD.LIST-COLUMNS.CARD-HOLDER-NAME'),
                field: "cardHolderName",
                colId: "cardHolderName",
                width: 200,
                resizable: true,
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
                headerName: this.translateService.instant('BANK-STATEMENT.LIST-COLUMNS.DESCRIPTION'),
                field: "description",
                colId: "description",
                resizable: true,
                sortable: true,
                width: 150,
                filter: "agTextColumnFilter",
                filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    caseSensitive: true,
                    defaultOption: "equals"
                }
            },
            {
                headerName: this.translateService.instant('WITHDRAWAL.LIST-COLUMNS.REFERENCE'),
                field: "reference",
                colId: "reference",
                resizable: true,
                sortable: true,
                width: 150,
                filter: "agTextColumnFilter",
                filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    defaultOption: "equals"
                }
            },
            {
                headerName: this.translateService.instant('WITHDRAWAL.LIST-COLUMNS.TRACKING-NUMBER'),
                field: "trackingNumber",
                colId: "trackingNumber",
                resizable: true,
                sortable: true,
                width: 150,
                filter: "agTextColumnFilter",
                filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    defaultOption: "equals"
                }
            },
            {
                headerName: this.translateService.instant('WITHDRAWAL.LIST-COLUMNS.DOWNLOAD_RECEIPT'),
                field: "trackingNumber",
                colId: "trackingNumber",
                cellRenderer: 'downloadReceiptCompoent',
                resizable: true,
                sortable: true,
                width: 150
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
            },
            {
                headerName: this.translateService.instant('WITHDRAWAL.LIST-COLUMNS.PROCESS-TYPE'),
                field: "merchantId",
                colId: "merchantId",
                resizable: true,
                width: 150,
                valueGetter: params => params == undefined || params.data == undefined ? '' : this.getProcessType(params.data.withdrawalProcessType)
            },
            {
                headerName: this.translateService.instant('WITHDRAWAL.LIST-COLUMNS.CARD-TO-CARD-TRY-COUNT'),
                field: "cardToCardTryCount",
                colId: "cardToCardTryCount",
                sortable: true,
                resizable: true,
                width: 100
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
                    allow: true,
                    allowed: this.cancelationAllowed.bind(this)
                },
                hide: false
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.RETRY'),
                field: "id",
                colId: "retry",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.retry.bind(this),
                    icon: 'refresh',
                    iconClass: '',
                    title: 'MANUAL-TRANSFER.GENERAL.RETRY',
                    allow: true,
                    allowed: this.retryAllowed.bind(this)
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
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.COMPLETED'),
                field: "id",
                colId: "completed",
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
                headerName: this.translateService.instant('WITHDRAWAL.LIST-COLUMNS.TRANSFER-FROM-ACCOUNT'),
                field: "id",
                colId: "changeprocesstype",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.changeProcessTypeToTransfer.bind(this),
                    icon: 'fast_forward',
                    iconClass: 'success',
                    title: 'WITHDRAWAL.LIST-COLUMNS.TRANSFER-FROM-ACCOUNT',
                    allow: true,
                    allowed: this.changeProcessTypeToTransferAllowed.bind(this)
                },
                hide: false
            }
        ];

        this.rowClassRules = {
            "confirmed-row": "data != undefined && data.withdrawalStatus == 4"
        };

        this.fuseConfigService.config.pipe(takeUntil(this.destroyed$)).subscribe(config => {

            this.enableRtl = config.locale != null && config.locale != undefined && config.locale.direction == 'rtl';
        });

        this.frameworkComponents = {
            booleanFormatterComponent: BooleanFormatterComponent,
            dateFormatterComponent: DateFormatterComponent,
            numberFormatterComponent: NumberFormatterComponent,
            downloadReceiptCompoent: DownloadWithdrawalReceiptFormatterComponent,
            iconButtonFormatterComponent: IconButtonFormatterComponent
        };

        this.callbackToMerchant$ = this.store.select(coreState.withdrawalCallbackToMerchant);
        this.callbackToMerchant$.pipe(takeUntil(this.destroyed$)).subscribe(callbackToMerchant => {
            if (callbackToMerchant) {
                 let action = this.translateService.instant('GENERAL.OK');
                let message =  callbackToMerchant;
                this.snackBar.open(message, action, {
                    duration: 30000,
                    verticalPosition: 'top'
                });
            }
        });

        this.callbackToMerchantError$ = this.store.select(coreState.withdrawalCallbackToMerchantError);
        this.callbackToMerchantError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.store.dispatch(new merchantActions.GetAll());
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

    cancel(id: number) {
        if (id > 0) {
            const dialogRef = this.dialog.open(DeactivateLoginDialogComponent, {
                width: '250px',
                data: 'MANUAL-TRANSFER.GENERAL.CANCELATION-APPROVE'
            });

            dialogRef.afterClosed().subscribe(result => {
                if (result == true) {
                    this.store.dispatch(new withdrawalActions.Cancel(id));
                }
            });
        }
    }

    cancelationAllowed(params): boolean {
        var result = params != undefined && params.data != undefined && (params.data.withdrawalStatus == 0 || params.data.withdrawalStatus == 1 || params.data.withdrawalStatus == 6 || params.data.withdrawalStatus == 7)
            && (params.data.transferStatus != TransferStatusEnum.Complete && params.data.transferStatus != TransferStatusEnum.CompletedWithNoReceipt && params.data.transferStatus != TransferStatusEnum.AwaitingConfirmation);
        return result;
    }

    retry(id: number) {
        if (id > 0) {
            const dialogRef = this.dialog.open(DeactivateLoginDialogComponent, {
                width: '250px',
                data: 'MANUAL-TRANSFER.GENERAL.RETRY-APPROVE'
            });

            dialogRef.afterClosed().subscribe(result => {
                if (result == true) {
                    this.store.dispatch(new withdrawalActions.Retry(id));
                }
            });
        }
    }

    retryAllowed(params): boolean {
        var now = new Date();
        //now = moment(now).toDate();
        var miliseconds = now.getTime() + (now.getTimezoneOffset() * 60 * 1000);

        var result = params != undefined
            && params.data != undefined
            && (params.data.withdrawalStatus == 6 || params.data.withdrawalStatus == 7 || ((params.data.withdrawalStatus == 8 || params.data.withdrawalStatus == 9)
                && params.data.updateDate != null && (((miliseconds - new Date(params.data.updateDate).getTime()) / (1000 * 60 * 60)) > 24)))
            && (params.data.transferStatus != TransferStatusEnum.Complete
                && (params.data.transferStatus != TransferStatusEnum.AwaitingConfirmation || params.data.isLoginBlocked == true)
                && params.data.transferStatus != TransferStatusEnum.CompletedWithNoReceipt
                && params.data.transferStatus != TransferStatusEnum.Cancelled);
        return result;
    }

    loadWithdrawals() {
        if (this.selectedTenant == undefined || this.callBackApi == undefined) {
            return;
        }

        if (this.args.merchantCustomerId == null) {
            localStorage.setItem('withdrawalargs', JSON.stringify(this.args));
        }

        this.args.timeZoneInfoId = this.selectedZoneId;

        this.store.dispatch(new withdrawalActions.Search(this.args));
    }

    setAsCompleted(id: number) {
        if (id > 0) {
            const dialogRef = this.dialog.open(DeactivateLoginDialogComponent, {
                width: '250px',
                data: 'MANUAL-TRANSFER.GENERAL.COMPLETE-APPROVE'
            });

            dialogRef.afterClosed().subscribe(result => {
                if (result == true) {
                    this.store.dispatch(new withdrawalActions.SetAsCompleted(id));
                }
            });
        }
    }

    setAsCompletedAllowed(params): boolean {
        var result = params != undefined && params.data != undefined && (
            params.data.transferStatus == TransferStatusEnum.BankSubmitted
            || params.data.transferStatus == TransferStatusEnum.AwaitingConfirmation
            || params.data.transferStatus == TransferStatusEnum.FailedFromBank
            || params.data.transferStatus == TransferStatusEnum.CompletedWithNoReceipt
            || params.data.withdrawalStatus == WithdrawalStatusEnum.PendingCardToCardConfirmation
            || params.data.withdrawalStatus == WithdrawalStatusEnum.PartialPaid
            || params.data.withdrawalStatus == WithdrawalStatusEnum.Sent
            || params.data.withdrawalStatus == WithdrawalStatusEnum.Pending);
        return result;
    }

    changeProcessTypeToTransfer(id: number) {
        if (id > 0) {
            const dialogRef = this.dialog.open(DeactivateLoginDialogComponent, {
                width: '250px',
                data: 'WITHDRAWAL.GENERAL.PROCESS-TYPE-TO-APPROVE'
            });

            dialogRef.afterClosed().subscribe(result => {
                if (result == true) {
                    this.store.dispatch(new withdrawalActions.ChangeProcessType(id, WithdrawalProcessType.Transfer));
                }
            });
        }
    }

    changeProcessTypeToTransferAllowed(params): boolean {
        var result = params != undefined && params.data != undefined && (params.data.withdrawalProcessType == WithdrawalProcessType.Both && params.data.withdrawalStatus == 1);
        return result;
    }

    changeAllProcessTypeToTransfer() {
        if (this.args) {
            const dialogRef = this.dialog.open(DeactivateLoginDialogComponent, {
                width: '250px',
                data: 'WITHDRAWAL.GENERAL.PROCESS-TYPE-TO-APPROVE'
            });

            dialogRef.afterClosed().subscribe(result => {
                if (result == true) {
                    this.store.dispatch(new withdrawalActions.ChangeAllProcessType(this.args, WithdrawalProcessType.Transfer));
                }
            });
        }
    }

    refresh() {
        this.gridApi.paginationGoToFirstPage();
        this.loadWithdrawals();
    }

    getStatusName(data): string {
        if (data.transferId) {

            if (this.transferStatuses) {
                let item = this.transferStatuses.find(t => t.id == data.transferStatus);

                if (item != null) {
                    return this.translateService.currentLang == 'fa' ? item.descriptionInFarsi : item.descriptionInEnglish;
                }
            }

        } else {
            let item = WithdrawalStatuses.find(t => t.status == data.withdrawalStatus);

            if (item != null) {
                return this.translateService.instant(item.translate);
            }
        }

        return '';
    }

    getMerchantTitle(id: number): string {

        if (this.merchants) {
            var merchant = this.merchants.find(t => t.id == id);

            if (merchant != null) {
                return merchant.title;
            }
        }

        return '';
    }

    getProcessType(processType: number): string {
        var item = WithdrawalProcessTypes.find(t => t.value == processType);

        if (item != null) {
            return this.translateService.instant(item.key);
        }

        return '';
    }

    ngOnDestroy(): void {
        this.store.dispatch(new withdrawalActions.Clear());
        this.store.dispatch(new merchantActions.ClearErrors());
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

        this.loadWithdrawals();
    }

    onCellClicked(e: CellClickedEvent) {
        if (e && e.colDef && e.colDef.colId == 'detail') {
            if (e.data) {
                this.selectedWithdrawal = e.data;
            }
        }
    }

    detailClosed() {
        this.selectedWithdrawal = undefined;
        this.gridApi.deselectAll();
    }

    createSearchArgs() {
        this.args = new WithdrawalSearchArgs();
        this.args.pageSize = 25;
        this.args.status = 0;
    }

    callbackAllowed(params): boolean {
       
        return this.allowWithdrawCallback;
    }

    callBack(id: number) {
        if (id > 0) {
            const dialogRef = this.dialog.open(DeactivateLoginDialogComponent, {
                width: '250px',
                data: 'MANUAL-TRANSFER.GENERAL.CALLBACK-APPROVE'
            });

            dialogRef.afterClosed().subscribe(result => {
                if (result == true) {
                    this.store.dispatch(new withdrawalActions.CallbackToMerchant(id));
                }
            });
        }
    }

    onColumnMoved(params) {
        var columnState = JSON.stringify(params.columnApi.getColumnState());
        localStorage.setItem('withdrawalColumnState', columnState);
    }

    onGridReady(readyParams) {
        this.gridApi = readyParams.api;
        this.gridColumnApi = readyParams.columnApi;

        var columnState: any[] = JSON.parse(localStorage.getItem('withdrawalColumnState'));
        if (columnState) {
            if (this.columnDefs.find(t => columnState.findIndex(r => t.colId == r.colId) == -1) != null) {
                localStorage.removeItem('withdrawalColumnState');
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
}

