import { Component, OnInit, Output, EventEmitter, OnDestroy, NgZone } from '@angular/core';
import * as coreState from '../../../core';
import { Store } from '@ngrx/store';
import { Observable, ReplaySubject } from 'rxjs';
import { Merchant } from '../../../models/merchant-model';
import * as merchantActions from '../../../core/actions/merchant';
import { fuseAnimations } from '../../../core/animations';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../../models/tenant';
import { FuseConfigService } from '../../../../@fuse/services/config.service';
import { DeleteButtonFormatterComponent } from '../../formatters/deletebuttonformatter';
import { CellClickedEvent } from 'ag-grid-community';
import { AccountService } from '../../../core/services/account.service';
import * as permissions from '../../../models/permissions';

@Component({
  selector: 'app-merchant-list',
  templateUrl: './merchant-list.component.html',
    styleUrls: ['./merchant-list.component.scss'],
    animations: fuseAnimations
})
export class MerchantListComponent implements OnInit, OnDestroy {

    merchants$: Observable<Merchant[]>;
    merchants: Merchant[];
    searchError$: Observable<string>;
    selected = [];
    loading$: Observable<boolean>;

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    deleteSuccess$: Observable<boolean>;
    deleteError$: Observable<string>;

    columnDefs;
    enableRtl: boolean = false;
    frameworkComponents;

    gridApi;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    allowAddMerchant: boolean = false;

    constructor(private store: Store<coreState.State>,
        public snackBar: MatSnackBar,
        private translateService: TranslateService,
        private accountService: AccountService,
        private fuseConfigService: FuseConfigService,
        private ngZone: NgZone, 
        private router: Router) {
        this.allowAddMerchant = this.accountService.isUserAuthorizedForTask(permissions.AddMerchant);
    }

    ngOnInit() {

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
                width: 150,
                filter: "agNumberColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('MERCHANT.LIST-COLUMNS.TITLE'),
                field: "title",
                width: 200,
                resizable: true,
                sortable: true,
                filter: "agTextColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            },
            {
                headerName: this.translateService.instant('MERCHANT.LIST-COLUMNS.PRIMARY-DOMAIN'),
                field: "domainAddress",
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
                hide: !this.allowAddMerchant
            }];

        this.merchants$ = this.store.select(coreState.getMerchantSearchResults);

        this.merchants$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.merchants = items;
        });

        this.loading$ = this.store.select(coreState.getMerchantLoading);
        this.searchError$ = this.store.select(coreState.getMerchantSearchError);

        this.searchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;
            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new merchantActions.Search(''));
            }
        });

        this.deleteSuccess$ = this.store.select(coreState.getMerchantDeleteSuccess);
        this.deleteSuccess$.pipe(takeUntil(this.destroyed$)).subscribe(t => {

            if (t == true) {
                this.store.dispatch(new merchantActions.Search(''));
            }
        });

        this.deleteError$ = this.store.select(coreState.getMerchantDeleteError);

        this.deleteError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });
    }

    delete(id: number) {
        if (id) {
            this.store.dispatch(new merchantActions.Delete(id));
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

    onCellClicked(e: CellClickedEvent) {
        if (e && e.colDef && e.colDef.field != 'id') {
            var selectedRows = this.gridApi.getSelectedRows();

            if (selectedRows.length > 0) {
                var selected = selectedRows[0];

                this.ngZone.run(() => this.router.navigate(['/merchant/' + selected.id]));
            }
        }
    }

    ngOnDestroy(): void {
        this.store.dispatch(new merchantActions.ClearErrors());
        this.destroyed$.next(true);
        this.destroyed$.complete();
    }

    onGridReady(params) {
        this.gridApi = params.api;
    }
}
