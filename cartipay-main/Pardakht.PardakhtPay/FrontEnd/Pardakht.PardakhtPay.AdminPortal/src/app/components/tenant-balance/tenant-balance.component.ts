import { Component, OnInit, Output, EventEmitter, OnDestroy } from '@angular/core';
import * as coreState from '../../core';
import { Store } from '@ngrx/store';
import { Observable, ReplaySubject } from 'rxjs';
import { fuseAnimations } from '../../core/animations';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../models/tenant';
import { Owner } from '../../models/account.model';
import * as accountActions from '../../core/actions/account';
import * as userSegmentGroupActions from '../../core/actions/userSegmentGroup';
import * as reportActions from '../../core/actions/report';
import { UserSegmentGroup } from '../../models/user-segment-group';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { BooleanInverseFormatterComponent } from '../formatters/booleaninverseformatter';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { DeleteButtonFormatterComponent } from '../formatters/deletebuttonformatter';
import { CellClickedEvent } from 'ag-grid-community';
import { UserSegmentReport, UserSegmentReportSearchArgs, TenantBalance, TenantBalanceSearchArgs } from 'app/models/report';
import { TimeZone } from 'app/models/timeZone';
import { TimeZoneService } from 'app/core/services/timeZoneService/time-zone.service';
import { BankLogin, BankAccount } from 'app/models/bank-login';
import * as bankLoginActions from '../../core/actions/bankLogin';
import { NgModel } from '@angular/forms';

@Component({
  selector: 'app-tenant-balance',
  templateUrl: './tenant-balance.component.html',
    styleUrls: ['./tenant-balance.component.scss'],
    animations: fuseAnimations
})
export class TenantBalanceComponent implements OnInit, OnDestroy {
    
    args: TenantBalanceSearchArgs;
    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    owners$: Observable<Owner[]>;
    owners: Owner[] = undefined;
    ownersLoading$: Observable<boolean>;
    ownersLoading: boolean = false;

    reportItems$: Observable<TenantBalance[]>;
    reportItems: TenantBalance[];
    reportLoading$: Observable<boolean>;
    reportLoading: boolean = false;
    reportError$: Observable<string>;

    bankLogins$: Observable<BankLogin[]>;
    bankLogins: BankLogin[] = [];
    bankLoginError$: Observable<string>;
    bankLoginLoading$: Observable<boolean>;
    bankLoginLoading: boolean;

    bankAccounts$: Observable<BankAccount[]>;
    bankAccountsError$: Observable<string>;
    bankAccountLoading$: Observable<boolean>;
    bankAccountsLoading: boolean;
    bankAccounts: BankAccount[];

    columnDefs;
    enableRtl: boolean = false;
    frameworkComponents;

    gridApi;
    firstLoaded: boolean = false;

    filter: string = undefined;

    filteredBankAccounts: BankAccount[] = undefined;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    allowAddUserSegmentGroup: boolean = false;

    total: number = 0;

    constructor(private store: Store<coreState.State>,
        public snackBar: MatSnackBar,
        public translateService: TranslateService,
        private fuseConfigService: FuseConfigService) {
        
            var cachedArgs = localStorage.getItem('tenantbalanceargs');

        if (cachedArgs != undefined && cachedArgs != null) {
            this.args = JSON.parse(cachedArgs);
        }
        else {
            this.args = new TenantBalanceSearchArgs();
            this.args.accountGuids = [];
        }

    }

    ngOnInit() {
        this.fuseConfigService.config.pipe(takeUntil(this.destroyed$)).subscribe(config => {

            this.enableRtl = config.locale != null && config.locale != undefined && config.locale.direction == 'rtl';
        });

        this.frameworkComponents = {
            booleanFormatterComponent: BooleanFormatterComponent,
            booleanInverseFormatterComponent: BooleanInverseFormatterComponent,
            numberFormatterComponent: NumberFormatterComponent,
            deleteButtonFormatterComponent: DeleteButtonFormatterComponent
        };

        this.columnDefs = [
            {
                headerName: this.translateService.instant('USER-SEGMENT.LIST-COLUMNS.NAME'),
                field: "bankName",
                sortable: true,
                resizable: true,
                width: 300,
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('TRANSACTION.LIST-COLUMNS.AMOUNT'),
                field: "amount",
                resizable: true,
                sortable: true,
                width: 300,
                cellRenderer: 'numberFormatterComponent',
                filter: "agNumberColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: '%',
                field: "percentage",
                width: 200,
                resizable: true,
                sortable: true,
                cellRenderer: 'numberFormatterComponent',
                valueGetter: params => this.getPercentage(params.data.amount)
            },
            {
                headerName: this.translateService.instant('USER-SEGMENT.LIST-COLUMNS.OWNER'),
                field: "ownerGuid",
                width: 200,
                resizable: true,
                sortable: true,
                valueGetter: params => this.getOwnerName(params.data.ownerGuid),
                filter: "agTextColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            }];

        this.bankLoginError$ = this.store.select(coreState.getBankLoginSearchError);
        this.bankLogins$ = this.store.select(coreState.getBankLoginSearchResults);
        this.bankLoginLoading$ = this.store.select(coreState.getBankLoginLoading);

        this.bankAccounts$ = this.store.select(coreState.getBankAccountSearchResults);
        this.bankAccountsError$ = this.store.select(coreState.getBankAccountSearchError);
        this.bankAccountLoading$ = this.store.select(coreState.getBankAccountsLoading);

        this.store.dispatch(new userSegmentGroupActions.ClearErrors());

        this.owners$ = this.store.select(coreState.getOwners);
        this.ownersLoading$ = this.store.select(coreState.getAccountLoading);

        this.ownersLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.ownersLoading = l;
        });

        this.owners$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            if (items != undefined) {
                this.owners = items;
            }
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
                        this.refresh();
                    }
                }
                else {
                    this.args.accountGuids = this.bankAccounts.map(t => t.accountGuid);
                    this.refresh();
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
                        this.refresh();
                    }
                }
                else {
                    this.args.accountGuids = this.bankAccounts.map(t => t.accountGuid);
                    this.refresh();
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
                this.store.dispatch(new bankLoginActions.Search(false));
                this.store.dispatch(new bankLoginActions.SearchAccounts(false));
                this.refresh();
            }
        });

        this.reportItems$ = this.store.select(coreState.getReportTenantBalance);
        this.reportItems$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            
            if (items) {
                this.total = this.getTotal(items);
            }
            else {
                this.total = 0;
            }

            this.reportItems = items;
        });

        this.reportLoading$ = this.store.select(coreState.getReportTenantBalanceLoading);

        this.reportLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.reportLoading = l;
        });

        this.reportError$ = this.store.select(coreState.getReportTenantBalanceError);
        this.reportError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });
    }

    getRowStyle(params) {
        if (params.node.rowPinned) {
            return { "font-weight": "bold" };
        }
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

    }

    refresh() {
        this.store.dispatch(new reportActions.GetTenantBalance(this.args));

        localStorage.setItem('tenantbalanceargs', JSON.stringify(this.args));
    }

    ngOnDestroy(): void {
        this.store.dispatch(new userSegmentGroupActions.ClearErrors());
        this.store.dispatch(new reportActions.ClearAll());
        this.destroyed$.next(true);
        this.destroyed$.complete();
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

    getPercentage(value: number): number {

        if (this.total == 0) {
            return 0;
        }

        return Number((value / this.total * 100).toFixed(2));
    }

    getTotal(items: TenantBalance[]): number {
        if (items == undefined) {
            return 0;
        }

        var sum = 0;

        for (var i = 0; i < items.length; i++) {
            sum += items[i].amount;
        }

        return sum;
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
        this.filter = text;

        if (this.bankAccounts) {
            this.filteredBankAccounts = this.bankAccounts.filter(t => this.getLoginName(t.loginGuid).toLowerCase().includes(text) || t.accountNo.toLowerCase().includes(text));
        }
    }

    getAccountNumber(accountGuid: string): string {
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

    onGridReady(params) {
        this.gridApi = params.api;

        if (this.reportItems && this.reportItems.length > 0) {

            var totalRow = new TenantBalance();
            totalRow.amount = this.total;
            totalRow.bankName = 'Total';

            this.gridApi.setPinnedBottomRowData([totalRow]);
        }
    }
}


