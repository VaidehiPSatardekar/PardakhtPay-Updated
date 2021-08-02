import { Component, OnInit, Output, EventEmitter, OnDestroy, NgZone } from '@angular/core';
import * as coreState from '../../core';
import { Store } from '@ngrx/store';
import { Observable, ReplaySubject } from 'rxjs';
import { InvoiceOwnerSetting } from '../../models/invoice';
import * as invoiceOwnerSettingActions from '../../core/actions/invoiceOwnerSetting';
import { fuseAnimations } from '../../core/animations';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../models/tenant';
import { Owner } from '../../models/account.model';
import * as accountActions from '../../core/actions/account';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { DeleteButtonFormatterComponent } from '../formatters/deletebuttonformatter';
import { CellClickedEvent } from 'ag-grid-community';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';

@Component({
  selector: 'app-invoice-owner-setting-list',
  templateUrl: './invoice-owner-setting-list.component.html',
    styleUrls: ['./invoice-owner-setting-list.component.scss'],
    animations: fuseAnimations
})
export class InvoiceOwnerSettingListComponent implements OnInit, OnDestroy {

    invoiceOwnerSettings$: Observable<InvoiceOwnerSetting[]>;
    invoiceOwnerSettings: InvoiceOwnerSetting[];
    searchError$: Observable<string>;
    selected = [];
    loading$: Observable<boolean>;
    loading: boolean;

    deleteSuccess$: Observable<boolean>;
    deleteError$: Observable<string>;

    tenantGuid$: Observable<string>;
    tenantGuid: string;

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    owners$: Observable<Owner[]>;
    owners: Owner[] = undefined;
    ownersLoading$: Observable<boolean>;
    ownersLoading: boolean = false;

    columnDefs;
    enableRtl: boolean = false;
    frameworkComponents;

    gridApi;
    
    allowAddInvoiceOwnerSetting: boolean = false;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    constructor(private store: Store<coreState.State>,
        public snackBar: MatSnackBar,
        private translateService: TranslateService,
        private accountService: AccountService,
        private fuseConfigService: FuseConfigService,
        private ngZone: NgZone, 
        private router: Router) {

         this.allowAddInvoiceOwnerSetting = this.accountService.isUserAuthorizedForTask(permissions.AddInvoiceOwnerSetting);
    }

    ngOnInit() {

        this.store.dispatch(new accountActions.ClearErrors());

        this.fuseConfigService.config.pipe(takeUntil(this.destroyed$)).subscribe(config => {

            this.enableRtl = config.locale != null && config.locale != undefined && config.locale.direction == 'rtl';
        });

        this.frameworkComponents = {
            deleteButtonFormatterComponent: DeleteButtonFormatterComponent
        };

        this.columnDefs = [
            {
                headerName: this.translateService.instant('GENERAL.ID'),
                field: "id",
                sortable: true,
                resizable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('TRANSFER-ACCOUNT.LIST-COLUMNS.OWNER'),
                field: "ownerGuid",
                width: 200,
                resizable: true,
                sortable: true,
                valueGetter: params => this.getOwnerName(params.data.ownerGuid)
            },
            {
                headerName: this.translateService.instant('INVOICE-OWNER-SETTINGS.LIST-COLUMNS.PARDAKHTPAY-DEPOSIT-RATE'),
                field: "cartipayDepositRate",
                sortable: true,
                resizable: true,
                width: 250,
                type: 'numericColumn'
            },
            {
                headerName: this.translateService.instant('INVOICE-OWNER-SETTINGS.LIST-COLUMNS.PARDAKHTPAL-DEPOSIT-RATE'),
                field: "cartipalDepositRate",
                sortable: true,
                resizable: true,
                width: 250,
                type: 'numericColumn'
            },
            {
                headerName: this.translateService.instant('INVOICE-OWNER-SETTINGS.LIST-COLUMNS.PARDAKHTPAL-WITHDRAWAL-RATE'),
                field: "cartipalWithdrawalRate",
                sortable: true,
                resizable: true,
                width: 250,
                type: 'numericColumn'
            },
            {
                headerName: this.translateService.instant('INVOICE-OWNER-SETTINGS.LIST-COLUMNS.WITHDRAWAL-RATE'),
                field: "withdrawalRate",
                sortable: true,
                resizable: true,
                width: 250,
                type: 'numericColumn'
            }
          ];

        this.invoiceOwnerSettings$ = this.store.select(coreState.getAllInvoiceOwnerSettings);
        this.invoiceOwnerSettings$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.invoiceOwnerSettings = items;
        });

        this.loading$ = this.store.select(coreState.getInvoiceOwnerSettingLoading);
        this.searchError$ = this.store.select(coreState.getAllInvoiceOwnerSettingError);

        this.tenantGuid$ = this.store.select(coreState.getAccountTenantGuid);

        this.tenantGuid$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.tenantGuid = t;
        });

        this.searchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.loading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.loading = l;
        });

        this.owners$ = this.store.select(coreState.getOwners);
        this.ownersLoading$ = this.store.select(coreState.getAccountLoading);

        this.ownersLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.ownersLoading = l;
        });

        this.owners$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.owners = items;
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;
            console.log(t);
            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new accountActions.GetOwners());
                this.store.dispatch(new invoiceOwnerSettingActions.GetAll());
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
    onSelect() {
        if (this.selected.length > 0) {
            this.ngZone.run(() => this.router.navigate(['/invoiceownersetting/' + this.selected[0].id]));
        }
    }

    onCellClicked(e: CellClickedEvent) {
        if (e && e.colDef && e.colDef.field != 'id') {
            var selectedRows = this.gridApi.getSelectedRows();

            if (selectedRows.length > 0) {
                var selected = selectedRows[0];

                this.ngZone.run(() => this.router.navigate(['/invoiceownersetting/' + selected.id]));
            }
        }
    }

    ngOnDestroy(): void {
        this.store.dispatch(new invoiceOwnerSettingActions.ClearErrors());
        this.store.dispatch(new accountActions.ClearErrors());
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

    onGridReady(params) {
        this.gridApi = params.api;
    }
}

