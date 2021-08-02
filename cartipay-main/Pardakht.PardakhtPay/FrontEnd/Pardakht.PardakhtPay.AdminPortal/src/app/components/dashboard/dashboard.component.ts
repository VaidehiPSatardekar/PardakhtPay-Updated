import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { fuseAnimations } from '../../core/animations';
import { DashBoardWidget, DashBoardChartWidget, DashboardQuery, DashboardAccountStatusDTO, DashboardMerchantTransactionReportDTO } from '../../models/dashboard';
import { Observable, ReplaySubject } from 'rxjs';
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import { takeUntil } from 'rxjs/operators';
import * as dashboardActions from '../../core/actions/dashboard';
import { MatSnackBar } from '@angular/material';
import { TranslateService } from '@ngx-translate/core';
import * as tenantActions from '../../core/actions/tenant';
import { Tenant } from '../../models/tenant';
import { TimeZoneService } from '../../core/services/timeZoneService/time-zone.service';
import { TimeZone } from '../../models/timeZone';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { BooleanInverseFormatterComponent } from '../formatters/booleaninverseformatter';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';



@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss'],
    animations: fuseAnimations
})
export class DashboardComponent implements OnInit, OnDestroy, AfterViewInit {

    transactionWidget$: Observable<DashBoardWidget>;
    transactionWidget: DashBoardWidget;
    transactionWidgetError$: Observable<string>;

    merchantTransactionWidget$: Observable<DashboardMerchantTransactionReportDTO[]>;
    merchantTransactionWidget: DashboardMerchantTransactionReportDTO[];
    merchantTransactionWidgetError$: Observable<string>;

    merchantTransactionWidgetRange: string = 'dt';

    transactionGraphWidget$: Observable<DashBoardChartWidget>;
    transactionGraphWidget: DashBoardChartWidget;
    transactionGraphWidgetError$: Observable<string>;

    accountingGraphWidget$: Observable<DashBoardChartWidget>;
    accountingGraphWidget: DashBoardChartWidget;
    accountingGraphWidgetError$: Observable<string>;

    accountStatusWidget$: Observable<DashboardAccountStatusDTO[]>;
    accountStatusWidget: DashboardAccountStatusDTO[];
    accountStatusWidgetError$: Observable<string>;

    transactionDepositBreakDownGraphWidget$: Observable<DashBoardChartWidget>;
    transactionDepositBreakDownGraphWidget: DashBoardChartWidget;
    transactionDepositBreakDownGraphWidgetError$: Observable<string>;

    transactioByPaymentTypeWidget$: Observable<DashboardMerchantTransactionReportDTO[]>;
    transactioByPaymentTypeWidget: DashboardMerchantTransactionReportDTO[];
    transactioByPaymentTypeWidgetError$: Observable<string>;
    transactioByPaymentTypeWidgetRange: string = 'dt';


    ranges = [
        { key: "dt", value: "DATERANGE.TODAY" },
        { key: "dy", value:"DATERANGE.YESTERDAY" },
        { key: "dtw",value: "DATERANGE.THIS_WEEK" },
        { key: "dlw",value: "DATERANGE.LAST_WEEK" },
        { key: "dtm",value: "DATERANGE.THIS_MONTH" },
        { key: "dlm",value: "DATERANGE.LAST_MONTH" },
        { key: "all",value: "DATERANGE.ALL" }];

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;
    frameworkComponents;

    selectedZone: TimeZone;
    zones: TimeZone[];

    accountRows: any[] = [];
    merchantTransactionRows: any[] = [];
    transactioPaymentTypeRows: any[] = [];

    gridApi;
    gridColumnApi;
    columnDefs;
    enableRtl: boolean = false;

    merchantTransactionColumnDefs;
    transactionByPaymentTypeColumnDefs;
    merchantGridApi;
    transactionByPaymentTypeGridApi;

    allowAccountWidget: boolean = false;
    allowTransactionWidget: boolean = false;
    allowMerchantWidget: boolean = false;
    allowDepositGraph: boolean = false;
    allowDepositBreakDownGraph: boolean = false;
    allowAccountingGraph: boolean = false;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    constructor(private store: Store<coreState.State>,
        private translateService: TranslateService,
        private timeZoneService: TimeZoneService,
        private fuseConfigService: FuseConfigService,
        private accountService: AccountService,
        private snackBar: MatSnackBar) {
        this.zones = this.timeZoneService.getTimeZones();

        this.allowAccountWidget = this.accountService.isUserAuthorizedForTask(permissions.AccountSummary);
        this.allowTransactionWidget = this.accountService.isUserAuthorizedForTask(permissions.DepositTotal);
        this.allowMerchantWidget = this.accountService.isUserAuthorizedForTask(permissions.MerchantWidget);
        this.allowDepositGraph = this.accountService.isUserAuthorizedForTask(permissions.DepositGraph);
        this.allowAccountingGraph = this.accountService.isUserAuthorizedForTask(permissions.AccountingGraph);
        this.allowDepositBreakDownGraph = this.accountService.isUserAuthorizedForTask(permissions.DepositGraph);

        this.selectedZone = this.timeZoneService.getTimeZone();
        this.fuseConfigService.config.pipe(takeUntil(this.destroyed$)).subscribe(config => {

            this.enableRtl = config.locale != null && config.locale != undefined && config.locale.direction == 'rtl';
        });

        this.frameworkComponents = {
            booleanFormatterComponent: BooleanFormatterComponent,
            booleanInverseFormatterComponent: BooleanInverseFormatterComponent,
            numberFormatterComponent: NumberFormatterComponent
        };

        this.columnDefs = [
            {
                headerName: this.translateService.instant('DASHBOARD.LOGIN-NAME'),
                field: "friendlyName",
                width: 200,
                filter: "agTextColumnFilter",
                resizable: true,
                filterParams: {
                    //applyButton: true,
                    clearButton: true
                },
                sortable: true
            },
            {
                headerName: this.translateService.instant('DASHBOARD.BANK'),
                field: "bankName",
                width: 150,
                filter: "agSetColumnFilter",
                resizable: true,
                sortable: true,
                filterParams: {
                    //applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('DASHBOARD.ACCOUNT-NO'),
                field: "accountNo",
                width: 200,
                filter: "agTextColumnFilter",
                resizable: true,
                sortable: true,
                filterParams: {
                    //applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('DASHBOARD.ACCOUNT-STATUS'),
                field: "status",
                sortable: true,
                resizable: true,
                cellClassRules: {
                    "": "data == undefined",
                    "danger": "data != undefined && data.status != 'باز' && data.status != 'فعال' && data.status != 'OPEN'"
                },
                filter: "agSetColumnFilter",
                width: 100
            },
            {
                headerName: this.translateService.instant('DASHBOARD.BLOCKED'),
                field: "isBlocked", cellRenderer: 'booleanInverseFormatterComponent',
                sortable: true,
                resizable: true,
                filter: "agSetColumnFilter",
                width: 150
            },
            {
                headerName: this.translateService.instant('DASHBOARD.CARD-HOLDER-NAME'),
                field: "cardHolderName",
                width: 200,
                filter: "agTextColumnFilter",
                resizable: true,
                sortable: true,
                filterParams: {
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('DASHBOARD.CARD-NUMBER'),
                field: "cardNumber",
                width: 200,
                filter: "agTextColumnFilter",
                resizable: true,
                sortable: true,
                filterParams: {
                    //applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('DASHBOARD.ACCOUNT-BALANCE'),
                field: "accountBalance",
                cellRenderer: 'numberFormatterComponent',
                resizable: true,
                sortable: true,
                width: 200,
                filter: "agNumberColumnFilter",
                filterParams: {
                    //applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('DASHBOARD.BLOCKED-BALANCE'),
                field: "blockedBalance",
                cellRenderer: 'numberFormatterComponent',
                resizable: true,
                sortable: true,
                width: 200,
                cellClassRules: {
                    "": "data == undefined",
                    "danger": "data != undefined && data.blockedBalance > 0"
                }
            },
            {
                headerName: this.translateService.instant('DASHBOARD.PENDING-WITHDRAWAL-AMOUNT'),
                field: "pendingWithdrawalAmount",
                cellRenderer: 'numberFormatterComponent',
                width: 200,
                filter: "agNumberColumnFilter",
                resizable: true,
                sortable: true,
                filterParams: {
                    //applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('DASHBOARD.TOTAL-DEPOSIT-TODAY'),
                field: "totalDepositToday",
                cellRenderer: 'numberFormatterComponent',
                width: 200,
                filter: "agNumberColumnFilter",
                resizable: true,
                sortable: true,
                filterParams: {
                    //applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('DASHBOARD.TOTAL-WITHDRAWAL-TODAY'),
                field: "totalWithdrawalToday",
                cellRenderer: 'numberFormatterComponent',
                resizable: true,
                width: 200,
                filter: "agNumberColumnFilter",
                sortable: true,
                filterParams: {
                    //applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('DASHBOARD.NORMAL-WITHDRAWABLE'),
                field: "normalWithdrawable",
                cellRenderer: 'numberFormatterComponent',
                resizable: true,
                width: 200,
                filter: "agNumberColumnFilter",
                sortable: true,
                filterParams: {
                    //applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('DASHBOARD.PAYA-WITHDRAWABLE'),
                field: "payaWithdrawable",
                cellRenderer: 'numberFormatterComponent',
                resizable: true,
                width: 200,
                filter: "agNumberColumnFilter",
                sortable: true,
                filterParams: {
                    //applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('DASHBOARD.SATNA-WITHDRAWABLE'),
                field: "satnaWithdrawable",
                cellRenderer: 'numberFormatterComponent',
                resizable: true,
                width: 200,
                filter: "agNumberColumnFilter",
                sortable: true,
                filterParams: {
                    //applyButton: true,
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
                    //applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('GENERAL.BANK-ACCOUNT-ID'),
                field: "bankAccountId",
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agNumberColumnFilter",
                filterParams: {
                    //applyButton: true,
                    clearButton: true
                }
            }
        ];

        this.merchantTransactionColumnDefs = [
            {
                headerName: this.translateService.instant('GENERAL.MERCHANT'),
                field: "title",
                sortable: true,
                resizable: true,
                width: 200
            },
            {
                headerName: this.translateService.instant('DASHBOARD.TOTAL_TRANSACTION_AMOUNT'),
                field: "transactionSum",
                cellRenderer: 'numberFormatterComponent',
                sortable: true,
                resizable: true,
                width: 200
            },
            {
                headerName: this.translateService.instant('DASHBOARD.TOTAL_TRANSACTION_COUNT'),
                field: "transactionCount",
                cellRenderer: 'numberFormatterComponent',
                sortable: true,
                resizable: true,
                width: 200
            },
            {
                headerName: this.translateService.instant('DASHBOARD.TOTAL_WITHDRAWAL_AMOUNT'),
                field: "withdrawalSum",
                cellRenderer: 'numberFormatterComponent',
                sortable: true,
                resizable: true,
                width: 200
            },
            {
                headerName: this.translateService.instant('DASHBOARD.TOTAL_WITHDRAWAL_COUNT'),
                field: "withdrawalCount",
                cellRenderer: 'numberFormatterComponent',
                sortable: true,
                resizable: true,
                width: 200
            }
        ];

        this.transactionByPaymentTypeColumnDefs = [
            {
                headerName: 'Type',
                field: "title",
                sortable: true,
                resizable: true,
                width: 200
            },
            {
                headerName: this.translateService.instant('DASHBOARD.TOTAL_TRANSACTION_AMOUNT'),
                field: "transactionSum",
                cellRenderer: 'numberFormatterComponent',
                sortable: true,
                resizable: true,
                width: 200
            },
            {
                headerName: this.translateService.instant('DASHBOARD.TOTAL_TRANSACTION_COUNT'),
                field: "transactionCount",
                cellRenderer: 'numberFormatterComponent',
                sortable: true,
                resizable: true,
                width: 200
            },
            {
                headerName: this.translateService.instant('DASHBOARD.TOTAL_WITHDRAWAL_AMOUNT'),
                field: "withdrawalSum",
                cellRenderer: 'numberFormatterComponent',
                sortable: true,
                resizable: true,
                width: 200
            },
            {
                headerName: this.translateService.instant('DASHBOARD.TOTAL_WITHDRAWAL_COUNT'),
                field: "withdrawalCount",
                cellRenderer: 'numberFormatterComponent',
                sortable: true,
                resizable: true,
                width: 200
            }
        ];

    }

    ngOnInit() {

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid) {
                this.loadDashboard();
            }
        });
    }

    getWidget(item: any, widget: string): void {
        switch (widget) {
            case 'transactionWidget': {
                if (this.allowTransactionWidget) {
                    this.store.dispatch(new dashboardActions.GetTransactionWidget(new DashboardQuery(this.selectedTenant.tenantDomainPlatformMapGuid, item.value, '', this.selectedZone.timeZoneId)));
                }
                break;
            }
            case 'merchantTransactionWidget': {
                if (this.allowMerchantWidget) {
                    this.store.dispatch(new dashboardActions.GetMerchantTransactionWidget(new DashboardQuery(this.selectedTenant.tenantDomainPlatformMapGuid, item.value, '', this.selectedZone.timeZoneId)));
                }
                break;
            }
            case 'transactionGraphWidget': {
                //console.log('test');
                if (this.allowDepositGraph) {
                    this.store.dispatch(new dashboardActions.GetTransactionGraphWidget(new DashboardQuery(this.selectedTenant.tenantDomainPlatformMapGuid, item, '', this.selectedZone.timeZoneId)));
                }
                break;
            }
            case 'accountingGraphWidget': {
                if (this.allowAccountingGraph) {
                    this.store.dispatch(new dashboardActions.GetAccountingGraphWidget(new DashboardQuery(this.selectedTenant.tenantDomainPlatformMapGuid, item, '', this.selectedZone.timeZoneId)));
                }
                break;
            }
            case 'transactionDepositBreakDownGraphWidget': {
                if (this.allowDepositBreakDownGraph) {
                    this.store.dispatch(new dashboardActions.GetTransactionDepositBreakDownGraphWidget(new DashboardQuery(this.selectedTenant.tenantDomainPlatformMapGuid, item, '', this.selectedZone.timeZoneId)));
                }
                break;
            }
            case 'transactionWithdrawalWidget': {
                if (this.allowTransactionWidget) {
                    this.store.dispatch(new dashboardActions.GetTransactionWithdrawalWidget(new DashboardQuery(this.selectedTenant.tenantDomainPlatformMapGuid, item.value, '', this.selectedZone.timeZoneId)));
                }
                break;
            }
            case 'transactioByPaymentTypeWidget': {
                if (this.allowMerchantWidget) {
                    this.store.dispatch(new dashboardActions.GetTransactionByPaymentTypeWidget(new DashboardQuery(this.selectedTenant.tenantDomainPlatformMapGuid, item.value, '', this.selectedZone.timeZoneId)));
                }
                break;
            }
            default: break;
        }
    }
            

    ngAfterViewInit(): void {

        this.transactionWidgetError$ = this.store.select(coreState.getDashboardTransactionWidgetError);

        this.transactionWidgetError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.transactionWidget$ = this.store.select(coreState.getDashboardTransactionWidget);
        this.transactionWidget$.pipe(takeUntil(this.destroyed$)).subscribe((widget: DashBoardWidget) => {
             this.transactionWidget = widget;
        });

        this.merchantTransactionWidgetError$ = this.store.select(coreState.getDashboardMerchantTransactionWidgetError);

        this.merchantTransactionWidgetError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.merchantTransactionWidget$ = this.store.select(coreState.getDashboardMerchantTransactionWidget);
        this.merchantTransactionWidget$.pipe(takeUntil(this.destroyed$)).subscribe((widget: DashboardMerchantTransactionReportDTO[]) => {
            this.merchantTransactionWidget = widget;
            this.merchantTransactionRows = widget;
        });

        this.transactionGraphWidgetError$ = this.store.select(coreState.getDashboardTransactionGraphWidgetError);

        this.transactionGraphWidgetError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.transactionGraphWidget$ = this.store.select(coreState.getDashboardTransactionGraphWidget);
        this.transactionGraphWidget$.pipe(takeUntil(this.destroyed$)).subscribe((widget: DashBoardChartWidget) => {
            this.transactionGraphWidget = widget;
        });

        this.accountingGraphWidgetError$ = this.store.select(coreState.getDashboardAccountingGraphWidgetError);

        this.accountingGraphWidgetError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.accountingGraphWidget$ = this.store.select(coreState.getDashboardAccountingGraphWidget);

        this.accountingGraphWidget$.pipe(takeUntil(this.destroyed$)).subscribe((widget: DashBoardChartWidget) => {
            this.accountingGraphWidget = widget;
        });

        this.accountStatusWidget$ = this.store.select(coreState.getDashboardAccountStatusWidget);

        this.accountStatusWidget$.pipe(takeUntil(this.destroyed$)).subscribe(widget => {
            this.accountStatusWidget = widget;

            if (!widget) {
                this.accountRows = [];
            }
            else {
                this.accountRows = widget;
            }
        });

        this.accountStatusWidgetError$ = this.store.select(coreState.getDashboardAccountStatusWidgetError);

        this.accountStatusWidgetError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.transactionDepositBreakDownGraphWidgetError$ = this.store.select(coreState.getDashboardTransactionDepositBreakDownGraphWidgetError);

        this.transactionDepositBreakDownGraphWidgetError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.transactionDepositBreakDownGraphWidget$ = this.store.select(coreState.getDashboardTransactionDepositBreakDownGraphWidget);
        this.transactionDepositBreakDownGraphWidget$.pipe(takeUntil(this.destroyed$)).subscribe((widget: DashBoardChartWidget) => {
            this.transactionDepositBreakDownGraphWidget = widget;
        });

        this.transactioByPaymentTypeWidgetError$ = this.store.select(coreState.getDashboardTransactionByPaymentTypeWidgetError);

        this.transactioByPaymentTypeWidgetError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.transactioByPaymentTypeWidget$ = this.store.select(coreState.getDashboardTransactionByPaymentTypeWidget);
        this.transactioByPaymentTypeWidget$.pipe(takeUntil(this.destroyed$)).subscribe((widget: DashboardMerchantTransactionReportDTO[]) => {
            this.transactioByPaymentTypeWidget = widget;
            this.transactioPaymentTypeRows = widget;
        });



       //// this.transactionWithdrawalWidgetError$ = this.store.select(coreState.getDashboardTransactionWidgetError);

        ////this.transactionWithdrawalWidgetError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
        ////    if (error) {
        ////        this.openSnackBar(error);
        ////    }
        ////});

        //this.transactionWithdrawalWidget$ = this.store.select(coreState.getDashboardTransactionWithdrawalWidget);
        //this.transactionWithdrawalWidget$.pipe(takeUntil(this.destroyed$)).subscribe((widget: DashBoardWidget) => {
        //    this.transactionWithdrawalWidget = widget;
        //});
    }

    openSnackBar(message: string, action: string = undefined) {
        if (!action) {
            action = this.translateService.instant('GENERAL.OK');
        }
        this.snackBar.open(message, action, {
            duration: 10000,
        });
    }

    loadDashboard() {

        if (this.allowTransactionWidget) {
            this.store.dispatch(new dashboardActions.GetTransactionWidget(new DashboardQuery(this.selectedTenant.tenantDomainPlatformMapGuid, this.transactionWidget == undefined ? 'dt' : this.transactionWidget.currentRange, '', this.selectedZone.timeZoneId)));
        }

        if (this.allowMerchantWidget) {
            this.store.dispatch(new dashboardActions.GetMerchantTransactionWidget(new DashboardQuery(this.selectedTenant.tenantDomainPlatformMapGuid, this.merchantTransactionWidget == undefined ? 'dt' : this.merchantTransactionWidgetRange, '', this.selectedZone.timeZoneId)));
        }

        if (this.allowDepositGraph) {
            this.store.dispatch(new dashboardActions.GetTransactionGraphWidget(new DashboardQuery(this.selectedTenant.tenantDomainPlatformMapGuid, this.transactionGraphWidget == undefined ? 'wl' : this.transactionGraphWidget.currentRange, '', this.selectedZone.timeZoneId)));
        }

        if (this.allowAccountingGraph) {
            this.store.dispatch(new dashboardActions.GetAccountingGraphWidget(new DashboardQuery(this.selectedTenant.tenantDomainPlatformMapGuid, this.accountingGraphWidget == undefined ? 'wl' : this.accountingGraphWidget.currentRange, '', this.selectedZone.timeZoneId)));
        }

        if (this.allowAccountWidget) {
            this.store.dispatch(new dashboardActions.GetAccountStatusWidget(new DashboardQuery(this.selectedTenant.tenantDomainPlatformMapGuid, 'dl', '', this.selectedZone.timeZoneId)));
        }
        if (this.allowDepositBreakDownGraph) {
            this.store.dispatch(new dashboardActions.GetTransactionDepositBreakDownGraphWidget(new DashboardQuery(this.selectedTenant.tenantDomainPlatformMapGuid, this.transactionDepositBreakDownGraphWidget == undefined ? 'wl' : this.transactionDepositBreakDownGraphWidget.currentRange, '', this.selectedZone.timeZoneId)));
        }
        if (this.allowTransactionWidget) {
            this.store.dispatch(new dashboardActions.GetTransactionByPaymentTypeWidget(new DashboardQuery(this.selectedTenant.tenantDomainPlatformMapGuid, this.transactioByPaymentTypeWidget == undefined ? 'dt' : this.transactioByPaymentTypeWidgetRange, '', this.selectedZone.timeZoneId)));
        }


        
        ////if (this.allowTransactionWidget) {
        ////    this.store.dispatch(new dashboardActions.GetTransactionWithdrawalWidget(new DashboardQuery(this.selectedTenant.tenantDomainPlatformMapGuid, this.transactionWithdrawalWidget == undefined ? 'dt' : this.transactionWithdrawalWidget.currentRange, '', this.selectedZone.timeZoneId)));
        ////}
    }

    ngOnDestroy(): void {
        this.store.dispatch(new dashboardActions.ClearErrors());
        this.destroyed$.next(true);
        this.destroyed$.complete();
    }

    setTimeZone(zone: TimeZone) {
        this.selectedZone = zone;
        this.timeZoneService.setTimeZone(zone);
    }

    onGridReady(params) {
        this.gridApi = params.api;
        this.gridColumnApi = params.columnApi;
    }

    onMerchantTransactionGridReady(params) {
        this.merchantGridApi = params.api;
    }

    onTransactionByPaymentTypeGridReady(params) {
        this.transactionByPaymentTypeGridApi = params.api;
    }
}
