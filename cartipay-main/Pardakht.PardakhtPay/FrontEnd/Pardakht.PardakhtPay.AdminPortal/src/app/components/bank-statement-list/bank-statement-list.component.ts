import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { MatSnackBar, DateAdapter } from '@angular/material';
import { Observable, ReplaySubject } from 'rxjs';
import { BankStatementItem, BankStatementItemSearchArgs } from '../../models/bank-statement-item';
import * as bankStatementactions from '../../core/actions/bankStatement';
import { fuseAnimations } from '../../core/animations';
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import { ListSearchResponse, defaultColumnDefs } from '../../models/types';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import * as tenantActions from '../../core/actions/tenant';
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

@Component({
    selector: 'app-bank-statement-list',
    templateUrl: './bank-statement-list.component.html',
    styleUrls: ['./bank-statement-list.component.scss'],
    animations: fuseAnimations
})
export class BankStatementListComponent implements OnInit, OnDestroy {
    args: BankStatementItemSearchArgs;

    loading$: Observable<boolean>;
    bankStatements$: Observable<ListSearchResponse<BankStatementItem[]>>;
    bankStatements: ListSearchResponse<BankStatementItem[]>;
    searchError$: Observable<string>;

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
    columnDefs: any[];
    enableRtl: boolean = false;
    frameworkComponents;

    filter: string = undefined;

    filteredBankAccounts: BankAccount[] = undefined;

    defaultColumnDefs = defaultColumnDefs;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    constructor(private store: Store<coreState.State>,
        private snackBar: MatSnackBar,
        private timeZoneService: TimeZoneService,
        private fuseConfigService: FuseConfigService,
        public translateService: TranslateService) {

        var cachedArgs = localStorage.getItem('statementargs');

        if (cachedArgs != undefined && cachedArgs != null) {
            this.args = JSON.parse(cachedArgs);
            //this.args.description = null;
            //this.args.fromDate = null;
            //this.args.toDate = null;
            //this.args.transactionId = null;
            this.args.pageSize = 25;

            if (this.args.statementItemType == 0 || this.args.statementItemType == null) {
                this.args.statementItemType = 4;
            }
        }
        else {
            this.args = new BankStatementItemSearchArgs();
            this.args.pageSize = 25;
            this.args.statementItemType = 4;
        }

        this.columnDefs = [
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
                headerName: this.translateService.instant('BANK-STATEMENT.LIST-COLUMNS.TRANSACTION-NO'),
                field: "transactionNo",
                colId: "transactionNo",
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agTextColumnFilter"
            },
            {
                headerName: this.translateService.instant('BANK-STATEMENT.LIST-COLUMNS.ACCOUNT-NO'),
                field: "accountGuid",
                colId: "accountGuid",
                sortable: true,
                resizable: true,
                width: 150,
                valueGetter: params => params == undefined || params.data == undefined ? '' : this.getAccountNumber(params.data.accountGuid)
            },
            {
                headerName: this.translateService.instant('BANK-STATEMENT.LIST-COLUMNS.DATE'),
                field: "transactionDateTimeStr",
                colId: "transactionDateTime",
                sortable: true,
                resizable: true,
                cellRenderer: 'dateFormatterComponent',
                width: 150,
                filter: "agDateColumnFilter"
            },
            {
                headerName: this.translateService.instant('BANK-STATEMENT.LIST-COLUMNS.DEBIT'),
                field: "debit",
                colId: "debit",
                sortable: true,
                resizable: true,
                width: 100,
                cellRenderer: 'numberFormatterComponent',
                cellClassRules: {
                    "debit": "x > 0"
                },
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('BANK-STATEMENT.LIST-COLUMNS.CREDIT'),
                field: "credit",
                colId: "credit",
                sortable: true,
                resizable: true,
                width: 100,
                cellRenderer: 'numberFormatterComponent',
                cellClassRules: {
                    "credit": "x > 0"
                },
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('BANK-STATEMENT.LIST-COLUMNS.BALANCE'),
                field: "balance",
                colId: "balance",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 100,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('BANK-STATEMENT.LIST-COLUMNS.DESCRIPTION'),
                field: "description",
                colId: "description",
                sortable: true,
                resizable: true,
                width: 500,
                filter: "agTextColumnFilter"
            },
            {
                headerName: this.translateService.instant('TRANSACTION.LIST-COLUMNS.ID'),
                field: "transactionId",
                colId: "transactionId",
                sortable: true,
                resizable: true,
                width: 100,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('BANK-STATEMENT.LIST-COLUMNS.USED-DATE'),
                field: "usedDateStr",
                colId: "usedDate",
                sortable: true,
                resizable: true,
                cellRenderer: 'dateFormatterComponent',
                width: 150,
                filter: "agDateColumnFilter"
            },
            {
                headerName: this.translateService.instant('BANK-STATEMENT.LIST-COLUMNS.NOTES'),
                field: "notes",
                colId: "notes",
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agTextColumnFilter"
            },
            {
                headerName: this.translateService.instant('BANK-STATEMENT.LIST-COLUMNS.WITHDRAWAL-ID'),
                field: "withdrawalId",
                colId: "withdrawalId",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 150,
                filter: "agNumberColumnFilter"
            }
        ];

        this.rowClassRules = {
            "used-row": "data != undefined && data.usedDate != null",
            "risky-row": "data != undefined && data.isRisky == true",
            "withdrawal-row": "data != undefined && (data.withdrawalId != null && data.withdrawalId != 0)"
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

        this.store.dispatch(new bankStatementactions.ClearAll());
        this.store.dispatch(new bankLoginActions.ClearAll());

        this.loading$ = this.store.select(coreState.getBankStatementLoading);
        this.bankStatements$ = this.store.select(coreState.getBankStatementSearchResults);
        this.searchError$ = this.store.select(coreState.getBankStatementSearchError);

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

        this.bankStatements$.pipe(takeUntil(this.destroyed$)).subscribe((data: ListSearchResponse<BankStatementItem[]>) => {
            this.bankStatements = data;

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
                        this.loadBankStatements();
                    }
                }
                else {
                    this.args.accountGuids = this.bankAccounts.map(t => t.accountGuid);
                    this.loadBankStatements();
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
                        this.loadBankStatements();
                    }
                }
                else {
                    this.args.accountGuids = this.bankAccounts.map(t => t.accountGuid);
                    this.loadBankStatements();
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
                this.store.dispatch(new bankLoginActions.Search(this.includeDeletedLogins));
                this.store.dispatch(new bankLoginActions.SearchAccounts(this.includeDeletedLogins));
            }
        });
    }

    deleteLoginCheckedChanged() {
        this.store.dispatch(new bankLoginActions.Search(this.includeDeletedLogins));
        this.store.dispatch(new bankLoginActions.SearchAccounts(this.includeDeletedLogins));
    }

    loadBankStatements() {

        if (this.selectedTenant == undefined || this.gridApi == undefined || this.bankLogins == undefined || this.bankLogins.length == 0 || this.bankAccounts == undefined || this.bankAccounts.length == 0 || this.args.accountGuids == null || this.args.accountGuids.length == 0) {
            return;
        }
        this.firstLoaded = true;
        this.args.timeZoneInfoId = this.selectedZoneId;

        localStorage.setItem('statementargs', JSON.stringify(this.args));

        this.store.dispatch(new bankStatementactions.Search(this.args));
    }

    refresh() {
        this.gridApi.paginationGoToFirstPage();
        this.loadBankStatements();
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

    getAccounts() {
        if (this.filteredBankAccounts) {
            return this.filteredBankAccounts;
        }

        if (this.bankAccounts) {
            return this.bankAccounts;
        }

        return [];
    }

    filterBankAccounts(text) {
        debugger;
        this.filter = text;

        if (this.bankAccounts) {
            this.filteredBankAccounts = this.bankAccounts.filter(t => this.getLoginName(t.loginGuid).toLowerCase().includes(text) || t.accountNo.toLowerCase().includes(text));
        }
    }

    getAccountNumber(accountGuid: string): string {
        debugger;
        if (this.bankAccounts != null || this.bankAccounts != undefined) {
            var account = this.bankAccounts.find(t => t.accountGuid == accountGuid);

            if (account != null) {
                return account.accountNo;
            }
        }

        return '';
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

    ngOnDestroy(): void {
        this.store.dispatch(new bankStatementactions.ClearAll());
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

        this.loadBankStatements();
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
}

