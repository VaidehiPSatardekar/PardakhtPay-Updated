import { Component, OnInit, Output, EventEmitter, OnDestroy, NgZone } from '@angular/core';
import * as coreState from '../../core';
import { Store } from '@ngrx/store';
import { Observable, ReplaySubject } from 'rxjs';
import { TransferAccount } from '../../models/transfer-account';
import * as transferAccountActions from '../../core/actions/transferAccount';
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
  selector: 'app-transfer-account-list',
  templateUrl: './transfer-account-list.component.html',
    styleUrls: ['./transfer-account-list.component.scss'],
    animations: fuseAnimations
})
export class TransferAccountListComponent implements OnInit, OnDestroy {

    transferAccounts$: Observable<TransferAccount[]>;
    transferAccounts: TransferAccount[];
    searchError$: Observable<string>;
    selected = [];
    loading$: Observable<boolean>;
    loading: boolean;

    deleteSuccess$: Observable<boolean>;
    deleteError$: Observable<string>;

    tenantGuid$: Observable<string>;
    tenantGuid: string;

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

    owners$: Observable<Owner[]>;
    owners: Owner[] = undefined;
    ownersLoading$: Observable<boolean>;
    ownersLoading: boolean = false;

    columnDefs;
    enableRtl: boolean = false;
    frameworkComponents;

    gridApi;
    
    allowAddTransferAccount: boolean = false;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    constructor(private store: Store<coreState.State>,
        public snackBar: MatSnackBar,
        private translateService: TranslateService,
        private accountService: AccountService,
        private fuseConfigService: FuseConfigService,
        private ngZone: NgZone, 
        private router: Router) {

        this.allowAddTransferAccount = this.accountService.isUserAuthorizedForTask(permissions.AddTransferAccount);
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
                headerName: this.translateService.instant('TRANSFER-ACCOUNT.LIST-COLUMNS.FRIENDLY-NAME'),
                field: "friendlyName",
                resizable: true,
                sortable: true,
                width: 300,
                filter: "agTextColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('TRANSFER-ACCOUNT.LIST-COLUMNS.IBAN'),
                field: "iban",
                resizable: true,
                sortable: true,
                width: 300,
                filter: "agTextColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('TRANSFER-ACCOUNT.LIST-COLUMNS.ACCOUNT-HOLDER-FIRST-NAME'),
                field: "accountHolderFirstName",
                resizable: true,
                sortable: true,
                width: 300,
                filter: "agTextColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('TRANSFER-ACCOUNT.LIST-COLUMNS.ACCOUNT-HOLDER-LAST-NAME'),
                field: "accountHolderLastName",
                resizable: true,
                sortable: true,
                width: 300,
                filter: "agTextColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('BANK-LOGIN.LIST-COLUMNS.DELETE'),
                field: "id",
                resizable: true,
                sortable: false,
                width: 100,
                cellRenderer: 'deleteButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.delete.bind(this)
                },
                hide: !this.allowAddTransferAccount
            }];

        this.transferAccounts$ = this.store.select(coreState.getTransferAccountSearchResults);
        this.transferAccounts$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.transferAccounts = items;
        });

        this.loading$ = this.store.select(coreState.getTransferAccountLoading);
        this.searchError$ = this.store.select(coreState.getTransferAccountSearchError);

        this.deleteSuccess$ = this.store.select(coreState.getTransferAccountDeleteSuccess);
        this.deleteSuccess$.pipe(takeUntil(this.destroyed$)).subscribe(t => {

            if (t == true) {
                this.store.dispatch(new transferAccountActions.Search(''));
            }
        });

        this.deleteError$ = this.store.select(coreState.getTransferAccountDeleteError);

        this.deleteError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

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
            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new accountActions.GetOwners());
                this.store.dispatch(new transferAccountActions.Search(''));
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
            this.ngZone.run(() => this.router.navigate(['/transferaccount/' + this.selected[0].id]));
        }
    }

    delete(id: number) {
        if (id) {
            this.store.dispatch(new transferAccountActions.Delete(id));
        }
    }

    onCellClicked(e: CellClickedEvent) {
        if (e && e.colDef && e.colDef.field != 'id') {
            var selectedRows = this.gridApi.getSelectedRows();

            if (selectedRows.length > 0) {
                var selected = selectedRows[0];

                this.ngZone.run(() => this.router.navigate(['/transferaccount/' + selected.id]));
            }
        }
    }

    ngOnDestroy(): void {
        this.store.dispatch(new transferAccountActions.ClearErrors());
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

