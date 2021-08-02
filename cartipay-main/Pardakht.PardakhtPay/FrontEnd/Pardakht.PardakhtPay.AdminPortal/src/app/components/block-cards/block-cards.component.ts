import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { MatSnackBar, DateAdapter, MatDialog } from '@angular/material';
import { Observable, ReplaySubject } from 'rxjs';
import * as manualTransferActions from '../../core/actions/manualTransfer';
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
import { IGetRowsParams, CellClickedEvent } from 'ag-grid-community';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { Owner } from '../../models/account.model';
import * as accountActions from '../../core/actions/account';
import { AccountService } from '../../core/services/account.service';
import { Router } from '@angular/router';
import { IconButtonFormatterComponent } from '../formatters/iconbuttonformatter';
import { DecimalPipe } from '@angular/common';
import { NgModel } from '@angular/forms';
import { BlockedCardDetail } from '../../models/blocked-card-detail';

@Component({
    selector: 'app-block-cards',
    templateUrl: './block-cards.component.html',
    styleUrls: ['./block-cards.component.scss'],
    animations: fuseAnimations
})
export class BlockCardsComponent implements OnInit, OnDestroy {

    currentAccountGuid: string = 'all';

    loading$: Observable<boolean>;
    items$: Observable<BlockedCardDetail[]>;
    items: BlockedCardDetail[] = undefined;
    searchError$: Observable<string>;

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

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    filter: string = undefined;

    filteredBankAccounts: BankAccount[] = undefined;
    constructor(private store: Store<coreState.State>,
        private snackBar: MatSnackBar,
        private timeZoneService: TimeZoneService,
        private accountService: AccountService,
        private fuseConfigService: FuseConfigService,
        public translateService: TranslateService,
        private router: Router,
        private num: DecimalPipe,
        public dialog: MatDialog) {

        this.columnDefs = [
            {
                headerName: this.translateService.instant('DASHBOARD.ACCOUNT-NO'),
                field: "accountNo",
                width: 200
            },
            {
                headerName: this.translateService.instant('DASHBOARD.CARD-NUMBER'),
                field: "cardNumber",
                width: 200
            },
            {
                headerName: this.translateService.instant('ACCOUNTING.LIST-COLUMNS.DATE'),
                field: "timeStamp",
                sortable: true,
                resizable: true,
                cellRenderer: 'dateFormatterComponent',
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

        this.loading$ = this.store.select(coreState.getBlockedCardDetailsLoading);
        this.searchError$ = this.store.select(coreState.getBlockedCardDetailsError);

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

        this.items$ = this.store.select(coreState.getBlockedCardDetails);

        this.items$.pipe(takeUntil(this.destroyed$)).subscribe((data: BlockedCardDetail[]) => {
            this.items = data;
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
                this.refresh();
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

    loadBlockedCards() {

        if (this.selectedTenant == undefined || this.accountGuid == null || this.accountGuid == undefined) {
            return;
        }
        this.firstLoaded = true;

        this.store.dispatch(new bankLoginActions.GetBlockedCardDetails(this.currentAccountGuid));
    }

    refresh() {
        if (this.gridApi) {
            this.gridApi.paginationGoToFirstPage();
        }
        this.loadBlockedCards();
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

    setTimeZone() {
        let zone = this.zones.find(t => t.timeZoneId == this.selectedZoneId);
        this.timeZoneService.setTimeZone(zone);
        this.refresh();
    }

    loadItems(params: IGetRowsParams) {

        this.loadBlockedCards();
    }

    onGridReady(readyParams) {
        this.gridApi = readyParams.api;
        this.gridColumnApi = readyParams.columnApi;

        //var datasource = {
        //    getRows: (params: IGetRowsParams) => {
        //        this.callBackApi = params;
        //        this.loadItems(params);
        //    }
        //};

        //readyParams.api.setDatasource(datasource);
    }
}