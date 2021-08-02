import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { MatSnackBar, DateAdapter } from '@angular/material';
import { Observable, ReplaySubject } from 'rxjs';
import * as bankStatementactions from '../../core/actions/bankStatement';
import { fuseAnimations } from '../../core/animations';
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../models/tenant';
import { TimeZoneService } from '../../core/services/timeZoneService/time-zone.service';
import { TimeZone } from '../../models/timeZone';
import * as bankLoginActions from '../../core/actions/bankLogin';
import { BankLogin, BankAccount } from '../../models/bank-login';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { DateFormatterComponent } from '../formatters/dateformatter';
import { IGetRowsParams } from 'ag-grid-community';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { GenericHelper } from '../../helpers/generic';
import { NgModel } from '@angular/forms';
import { BankStatementReportSearchArgs, TransactionReportSearchArgs } from 'app/models/report';
import { DashBoardChartWidget, DashBoardWidget, DepositBreakDownReport } from 'app/models/dashboard';
import { ListSearchResponse, defaultColumnDefs } from '../../models/types';
import * as reportActions from '../../core/actions/report';

@Component({
    selector: 'app-deposit-by-accountnumber-chart',
    templateUrl: './deposit-by-accountnumber-chart.component.html',
    styleUrls: ['./deposit-by-accountnumber-chart.component.scss'],
    animations: fuseAnimations
})
export class DepositByAccountNumberChartComponent implements OnInit, OnDestroy {

    args: TransactionReportSearchArgs;

    loading$: Observable<boolean>;
    widget$: Observable<DashBoardChartWidget>;
    widget: DashBoardChartWidget
    searchError$: Observable<string>;

    depositBreakDownListloading$: Observable<boolean>;
    depositBreakDownList$: Observable<ListSearchResponse<DepositBreakDownReport[]>>;
    depositBreakDownList: ListSearchResponse<DepositBreakDownReport[]>;
    depositBreakDownListSearchError$: Observable<string>;

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

    firstLoaded: boolean = false;

    gridApi;
    callBackApi;
    gridColumnApi;
    columnDefs: any[];
    enableRtl: boolean = false;
    frameworkComponents;
    defaultColumnDefs = defaultColumnDefs;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    constructor(private store: Store<coreState.State>,
        private snackBar: MatSnackBar,
        private timeZoneService: TimeZoneService,
        private fuseConfigService: FuseConfigService,
        public translateService: TranslateService) {

        var cachedArgs = localStorage.getItem('depositByAccountNumberchartreportargs');

        if (cachedArgs != undefined && cachedArgs != null) {
            this.args = JSON.parse(cachedArgs);

            if (!this.args.endDate || !this.args.startDate) {
                this.args.endDate = new Date().toISOString()
                this.args.startDate = new Date().toISOString();
                this.args.startDate = new Date(new Date().setDate(new Date(this.args.endDate).getDate() - 7)).toISOString();
            }
        }
        else {
            this.args = new TransactionReportSearchArgs();
            this.args.endDate = new Date().toISOString();
            this.args.startDate = new Date().toISOString();
            this.args.startDate = new Date(new Date().setDate(new Date(this.args.endDate).getDate() - 7)).toISOString();
        }
    }

    ngOnInit() {
        this.fuseConfigService.config.pipe(takeUntil(this.destroyed$)).subscribe(config => {
            this.enableRtl = config.locale != null && config.locale != undefined && config.locale.direction == 'rtl';
        });

        this.frameworkComponents = {
            dateFormatterComponent: DateFormatterComponent,
            numberFormatterComponent: NumberFormatterComponent
        };

        this.loading$ = this.store.select(coreState.getReportDepositByAccountNumberLoading);
        this.widget$ = this.store.select(coreState.getReportDepositByAccountNumberWidget);
        this.searchError$ = this.store.select(coreState.getReportDepositByAccountNumberError);

        this.columnDefs = [
            {
                headerName: "Date",
                field: "breakDownDate",
                colId: "breakDownDate",
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agDateColumnFilter"
            },
            {
                headerName: "SamanBank",
                field: "samanBank",
                colId: "samanBank",
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: "MeliBank",
                field: "meliBank",
                colId: "meliBank",
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: "Zarinpal",
                field: "zarinpal",
                colId: "zarinpal",
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: "Mellat",
                field: "mellat",
                colId: "mellat",
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: "Novin",
                field: "novin",
                colId: "novin",
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agNumberColumnFilter"
            },
            
        ];


        this.zones = this.timeZoneService.getTimeZones();
        this.selectedZoneId = this.timeZoneService.getTimeZone().timeZoneId;

        this.searchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.widget$.pipe(takeUntil(this.destroyed$)).subscribe((data: DashBoardChartWidget) => {
            this.widget = data;
        });

        this.depositBreakDownListloading$ = this.store.select(coreState.getReportDepositBreakDownListByPaymentTypeLoading);
        this.depositBreakDownList$ = this.store.select(coreState.getReportDepositBreakDownListByPaymentType);
        this.depositBreakDownListSearchError$ = this.store.select(coreState.getReportDepositBreakDownListByPaymentTypeError);

        this.depositBreakDownList$.pipe(takeUntil(this.destroyed$)).subscribe((data: ListSearchResponse<DepositBreakDownReport[]>) => {
            debugger;
            this.depositBreakDownList = data;

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

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid) {
                this.refresh();
            }
        });
    }

    loadReport() {

        if (this.selectedTenant == undefined) {
            return;
        }
        this.firstLoaded = true;
        this.args.timeZoneInfoId = this.selectedZoneId;

        var newArgs = { ...this.args };

        newArgs.startDate = new Date(new Date(this.args.startDate).getTime() - new Date().getTimezoneOffset() * 60 * 1000).toISOString();
        newArgs.endDate = new Date(new Date(this.args.endDate).getTime() - new Date().getTimezoneOffset() * 60 * 1000).toISOString();
        debugger;
        localStorage.setItem('depositByAccountNumberchartreportargs', JSON.stringify(this.args));
        this.store.dispatch(new reportActions.GetDepositByAccountNumberWidget(newArgs));
       // this.loadDepositBreakDownReport();
    }

    loadDepositBreakDownReport() {
        if (this.selectedTenant == undefined) {
            return;
        }
        this.firstLoaded = true;
        this.args.timeZoneInfoId = this.selectedZoneId;

        var newArgs = { ...this.args };

        newArgs.startDate = new Date(new Date(this.args.startDate).getTime() - new Date().getTimezoneOffset() * 60 * 1000).toISOString();
        newArgs.endDate = new Date(new Date(this.args.endDate).getTime() - new Date().getTimezoneOffset() * 60 * 1000).toISOString();
        
        localStorage.setItem('depositByAccountNumberchartreportargs', JSON.stringify(this.args));
        this.store.dispatch(new reportActions.GetDepositBreakDownList(newArgs));
    } 

    refresh() {
        this.loadReport();
    }

    equals(objOne, objTwo) {
        if (typeof objOne !== 'undefined' && typeof objTwo !== 'undefined') {
            return objOne === objTwo;
        }
    }

    selectAll(select: NgModel, values) {
        select.update.emit(values.map(t => t.accountGuid));
    }

    deselectAll(select: NgModel) {
        select.update.emit([]);
    }

    ngOnDestroy(): void {
        this.store.dispatch(new reportActions.ClearAll());
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

    onColumnMoved(params) {
        var columnState = JSON.stringify(params.columnApi.getColumnState());
        localStorage.setItem('statementColumnState', columnState);
    }

    onGridReady(readyParams) {
        this.gridApi = readyParams.api;
        this.gridColumnApi = readyParams.columnApi;

        var columnState: any[] = JSON.parse(localStorage.getItem('statementColumnState'));
        if (columnState) {
            if (this.columnDefs.find(t => columnState.findIndex(r => t.colId == r.colId) == -1) != null) {
                localStorage.removeItem('statementColumnState');
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
        debugger;
       // this.args.filterModel = params.filterModel;

        this.loadDepositBreakDownReport();
    }
}

