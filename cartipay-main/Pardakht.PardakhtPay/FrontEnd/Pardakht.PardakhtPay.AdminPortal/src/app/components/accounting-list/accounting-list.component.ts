import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource, MatSnackBar } from '@angular/material';
import { Observable, ReplaySubject } from 'rxjs';
import { TransactionSearch, TransactionSearchArgs } from '../../models/transaction';
import * as accountings from '../../core/actions/accounting';
//import * as merchantActions from '../../core/actions/merchant';
import { fuseAnimations } from '../../core/animations';
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import { ListSearchResponse } from '../../models/types';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { DailyAccountingDTO, AccountingGroupingTypes, AccountingSearchArgs, AccountingGroupingType } from '../../models/accounting';
//import { Merchant } from '../../models/merchant-model';
import { Tenant } from '../../models/tenant';
import * as tenantActions from '../../core/actions/tenant';
import { TimeZoneService } from '../../core/services/timeZoneService/time-zone.service';
import { TimeZone } from '../../models/timeZone';
import { IGetRowsParams } from 'ag-grid-community';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { DateFormatterComponent } from '../formatters/dateformatter';
import { NumberFormatterComponent } from '../formatters/numberformatter';

@Component({
    selector: 'app-accounting-list',
    templateUrl: './accounting-list.component.html',
    styleUrls: ['./accounting-list.component.scss'],
    animations: fuseAnimations
})
export class AccountingListComponent implements OnInit, OnDestroy {
    args: AccountingSearchArgs;

    loading$: Observable<boolean>;
    searchItems$: Observable<DailyAccountingDTO[]>;
    searchItems: DailyAccountingDTO[];
    searchError$: Observable<string>;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    tenants$: Observable<Tenant[]>;
    tenants: Tenant[] = [];

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

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
    rowClassRules;

    groupTypes = AccountingGroupingTypes;

    gridApi;
    callBackApi;
    gridColumnApi;
    columnDefs;
    accountColumnDefs;
    enableRtl: boolean = false;
    frameworkComponents;

    constructor(private store: Store<coreState.State>,
        private snackBar: MatSnackBar,
        private timeZoneService: TimeZoneService,
        private fuseConfigService: FuseConfigService,
        private translateService: TranslateService) {

        var cachedArgs = localStorage.getItem('accountingreportargs');

        if (cachedArgs != undefined && cachedArgs != null) {
            this.args = JSON.parse(cachedArgs);

            if (!this.args.endDate || !this.args.startDate) {
                this.args.endDate = new Date().toISOString()
                this.args.startDate = new Date().toISOString();
                this.args.startDate = new Date(new Date().setDate(new Date(this.args.endDate).getDate() - 7)).toISOString();

                if (this.args.groupType == undefined || this.args.groupType == null || this.args.groupType == 0) {
                    this.args.groupType = AccountingGroupingType.Merchant;
                }
            }
        }
        else {
            this.args = new AccountingSearchArgs();
            this.args.endDate = new Date().toISOString();
            this.args.startDate = new Date().toISOString();
            this.args.startDate = new Date(new Date().setDate(new Date(this.args.endDate).getDate() - 7)).toISOString();
            this.args.groupType = AccountingGroupingType.Merchant;
        }

        this.zones = this.timeZoneService.getTimeZones();
        this.selectedZoneId = this.timeZoneService.getTimeZone().timeZoneId;

        this.columnDefs = [
            {
                headerName: this.translateService.instant('ACCOUNTING.LIST-COLUMNS.TITLE'),
                field: "merchantTitle",
                colId: "merchant",
                sortable: true,
                resizable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('ACCOUNTING.LIST-COLUMNS.COUNT'),
                field: "count",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200
            },
            {
                headerName: this.translateService.instant('ACCOUNTING.LIST-COLUMNS.AMOUNT'),
                field: "amount",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200
            },
            {
                headerName: this.translateService.instant('DASHBOARD.TOTAL_WITHDRAWAL_COUNT'),
                field: "withdrawalCount",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200
            },
            {
                headerName: this.translateService.instant('DASHBOARD.TOTAL_WITHDRAWAL_AMOUNT'),
                field: "withdrawalAmount",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200
            },
            {
                headerName: this.translateService.instant('ACCOUNTING.LIST-COLUMNS.TOTAL-DEPOSIT-AMOUNT-PERCENTAGE'),
                field: "depositPercentage",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200
            },
            {
                headerName: this.translateService.instant('ACCOUNTING.LIST-COLUMNS.TOTAL-WITHDRAWAL-AMOUNT-PERCENTAGE'),
                field: "withdrawalPercentage",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200
            }
        ];


        this.accountColumnDefs = [
            {
                headerName: this.translateService.instant('ACCOUNTING.LIST-COLUMNS.ACCOUNT_NUMBER'),
                field: "accountNumber",
                colId: "account",
                resizable: true,
                width: 200
            },
            {
                headerName: this.translateService.instant('ACCOUNTING.LIST-COLUMNS.CARD_NUMBER'),
                field: "cardNumber",
                colId: "card",
                resizable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('ACCOUNTING.LIST-COLUMNS.CARD_HOLDER_NAME'),
                field: "cardHolderName",
                colId: "cardHolder",
                resizable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('ACCOUNTING.LIST-COLUMNS.COUNT'),
                field: "count",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200
            },
            {
                headerName: this.translateService.instant('ACCOUNTING.LIST-COLUMNS.AMOUNT'),
                field: "amount",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200
            },
            {
                headerName: this.translateService.instant('DASHBOARD.TOTAL_WITHDRAWAL_COUNT'),
                field: "withdrawalCount",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200
            },
            {
                headerName: this.translateService.instant('DASHBOARD.TOTAL_WITHDRAWAL_AMOUNT'),
                field: "withdrawalAmount",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200
            },
            {
                headerName: this.translateService.instant('ACCOUNTING.LIST-COLUMNS.TOTAL-DEPOSIT-AMOUNT-PERCENTAGE'),
                field: "depositPercentage",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200
            },
            {
                headerName: this.translateService.instant('ACCOUNTING.LIST-COLUMNS.TOTAL-WITHDRAWAL-AMOUNT-PERCENTAGE'),
                field: "withdrawalPercentage",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200
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
            numberFormatterComponent: NumberFormatterComponent
        };

        this.loading$ = this.store.select(coreState.getAccountingLoading);
        this.searchItems$ = this.store.select(coreState.getAccountingSearchResults);

        this.searchError$ = this.store.select(coreState.getAccountingSearchError);
        
        this.tenants$ = this.store.select(coreState.getTenantSearchResults);

        this.searchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.searchItems$.pipe(takeUntil(this.destroyed$)).subscribe((data: DailyAccountingDTO[]) => {
            this.searchItems = data;
        });


        this.tenants$.pipe(takeUntil(this.destroyed$)).subscribe(items => {

            if (items && items.length > 0) {
                this.tenants = items;
            }
            else {
                this.tenants = [];
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

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid && this.gridApi) {
                this.loadTransactions();
            }
        });
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

    loadTransactions() {
        if (!this.selectedTenant) {
            return;
        }
        this.args.timeZoneInfoId = this.selectedZoneId;

        if (this.args == undefined || this.args == null) {
            return;
        }

        this.args.timeZoneInfoId = this.selectedZoneId;

        var newArgs = { ...this.args };

        newArgs.startDate = new Date(new Date(this.args.startDate).getTime() - new Date().getTimezoneOffset() * 60 * 1000).toISOString();
        newArgs.endDate = new Date(new Date(this.args.endDate).getTime() - new Date().getTimezoneOffset() * 60 * 1000).toISOString();

        if (this.args.groupType == AccountingGroupingType.Merchant) {
            this.gridApi.setColumnDefs(this.columnDefs);
        } else {
            this.gridApi.setColumnDefs(this.accountColumnDefs);
        }

        localStorage.setItem('accountingreportargs', JSON.stringify(this.args));

        this.store.dispatch(new accountings.Search(newArgs));
    }

    refresh() {
        this.loadTransactions();
    }

    ngOnDestroy(): void {
        this.store.dispatch(new accountings.ClearAll());
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

        this.loadTransactions();
    }

    onGridReady(readyParams) {
        this.gridApi = readyParams.api;
        this.gridColumnApi = readyParams.columnApi;

        if (this.selectedTenant && this.selectedTenant.tenantDomainPlatformMapGuid && this.gridApi) {
            this.loadTransactions();
        }
    }
}
