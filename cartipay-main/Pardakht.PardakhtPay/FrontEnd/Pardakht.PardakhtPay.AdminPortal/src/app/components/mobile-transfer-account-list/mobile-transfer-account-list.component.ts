import { Component, OnInit, Output, EventEmitter, OnDestroy, NgZone } from '@angular/core';
import * as coreState from '../../core';
import { Store } from '@ngrx/store';
import { Observable, ReplaySubject } from 'rxjs';
import { MobileTransferCardAccount, PaymentProviders } from '../../models/mobile-transfer';
import * as mobileTransferCardAccountActions from '../../core/actions/mobileTransferCardAccount';
import { fuseAnimations } from '../../core/animations';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../models/tenant';
import { Owner } from '../../models/account.model';
import * as accountActions from '../../core/actions/account';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { CellClickedEvent } from 'ag-grid-community';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { DateFormatterComponent } from '../formatters/dateformatter';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';

@Component({
    selector: 'app-mobile-transfer-account-list',
    templateUrl: './mobile-transfer-account-list.component.html',
    styleUrls: ['./mobile-transfer-account-list.component.scss'],
    animations: fuseAnimations
})

export class MobileTransferAccountListComponent implements OnInit, OnDestroy {

    mobileTransferCardAccounts$: Observable<MobileTransferCardAccount[]>;
    mobileTransferCardAccounts: MobileTransferCardAccount[];
    searchError$: Observable<string>;
    selected = [];
    loading$: Observable<boolean>;
    loading: boolean;

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    owners$: Observable<Owner[]>;
    owners: Owner[] = [];
    ownersLoading$: Observable<boolean>;
    ownersLoading: boolean = false;

    columnDefs;
    enableRtl: boolean = false;
    frameworkComponents;

    providers = PaymentProviders;

    gridApi;

    allowAddBankLogin: boolean = false;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    constructor(private store: Store<coreState.State>,
        public snackBar: MatSnackBar,
        private translateService: TranslateService,
        private fuseConfigService: FuseConfigService,
        private accountService: AccountService,
        private ngZone: NgZone, 
        private router: Router) {
        this.allowAddBankLogin = this.accountService.isUserAuthorizedForTask(permissions.AddMobileTransferAccount);
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

        this.columnDefs = [
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
            }, {
                headerName: this.translateService.instant('TRANSACTION.PAYMENT-TYPES.PAYMENT-GATEWAY'),
                field: "paymentProviderType",
                width: 200,
                resizable: true,
                sortable: true,
                filter: "agTextColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                },
                valueGetter: (params) => this.getPaymentGatewayName(params)
            }, {
                headerName: this.translateService.instant('CARD-TO-CARD.LIST-COLUMNS.CARD-NUMBER'),
                field: "cardNumber",
                width: 200,
                resizable: true,
                sortable: true,
                filter: "agTextColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            }, {
                headerName: this.translateService.instant('CARD-TO-CARD.LIST-COLUMNS.CARD-HOLDER-NAME'),
                field: "cardHolderName",
                width: 200,
                resizable: true,
                sortable: true,
                filter: "agTextColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            }, {
                headerName: this.translateService.instant('MOBILE-TRANSFER-CARD-ACCOUNT.GENERAL.MERCHANT-ID'),
                field: "merchantId",
                width: 200,
                resizable: true,
                sortable: true,
                filter: "agTextColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            },, {
                headerName: this.translateService.instant('MOBILE-TRANSFER-CARD-ACCOUNT.GENERAL.TITLE'),
                field: "title",
                width: 200,
                resizable: true,
                sortable: true,
                filter: "agTextColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            },, {
                headerName: this.translateService.instant('CARD-TO-CARD.LIST-COLUMNS.IS-ACTIVE'),
                field: "isActive",
                width: 200,
                resizable: true,
                sortable: true,
                cellRenderer: 'booleanFormatterComponent'
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

        this.store.dispatch(new mobileTransferCardAccountActions.ClearErrors());
        this.mobileTransferCardAccounts$ = this.store.select(coreState.getAllMobileTransferCardAccounts);

        this.mobileTransferCardAccounts$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.mobileTransferCardAccounts = items;
        });

        this.loading$ = this.store.select(coreState.getMobileTransferCardAccountLoading);

        this.loading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.loading = l;
        });

        this.searchError$ = this.store.select(coreState.getAllMobileTransferCardAccountError);

        this.searchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

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

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;
            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new accountActions.GetOwners());
                this.store.dispatch(new mobileTransferCardAccountActions.GetAll());
            }
        });
    }

    openSnackBar(message: string, action: string = undefined) {
        if (!action) {
            action = this.translateService.instant('GENERAL.OK');
        }
        this.snackBar.open(message, action, {
            duration: 10000,
        });
    }

    ngOnDestroy(): void {
        this.store.dispatch(new mobileTransferCardAccountActions.ClearErrors());
        this.store.dispatch(new accountActions.ClearErrors());
        this.destroyed$.next(true);
        this.destroyed$.complete();
    }

    getOwnerName(ownerGuid: string): string {

        if (this.owners == undefined || this.owners == null || ownerGuid == '') {
            return '';
        }

        var owner = this.owners.find(t => t.accountId == ownerGuid);

        if (owner == null || owner == undefined) {
            return '';
        }

        return owner.firstName + ' ' + owner.lastName;
    }
    
    getPaymentGatewayName(params){
        if(params == undefined || params == null || params.data == null || params.data == undefined){
            return '';
        }

        var item = this.providers.find(t => t.value == params.data.paymentProviderType);

        if(item != null){
            return this.translateService.instant(item.key);
        }

        return '';
    }

    onCellClicked(e: CellClickedEvent) {

        if (!this.allowAddBankLogin) {
            return;
        }

        if (e && e.colDef && e.colDef.field != 'id') {
            var selectedRows = this.gridApi.getSelectedRows();

            if (selectedRows.length > 0) {
                var selected = selectedRows[0];

                this.ngZone.run(() => this.router.navigate(['/mobiletransfercardaccount/' + selected.id]));
            }
        }
    }

    onGridReady(params) {
        this.gridApi = params.api;
    }
}

