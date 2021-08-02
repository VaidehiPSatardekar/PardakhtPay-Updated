import { Component, OnInit, Output, EventEmitter, OnDestroy, NgZone } from '@angular/core';
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
import * as mobileTransferCardAccountGroupActions from '../../core/actions/mobileTransferAccountGroup';
import { MobileTransferCardAccountGroup } from '../../models/mobile-transfer';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { BooleanInverseFormatterComponent } from '../formatters/booleaninverseformatter';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { DeleteButtonFormatterComponent } from '../formatters/deletebuttonformatter';
import { CellClickedEvent } from 'ag-grid-community';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';

@Component({
  selector: 'app-mobile-transfer-account-group-list',
  templateUrl: './mobile-transfer-account-group-list.component.html',
    styleUrls: ['./mobile-transfer-account-group-list.component.scss'],
    animations: fuseAnimations
})
export class MobileTransferAccountGroupListComponent implements OnInit, OnDestroy {

    groups$: Observable<MobileTransferCardAccountGroup[]>;
    groups: MobileTransferCardAccountGroup[];
    groupsLoading$: Observable<boolean>;
    groupsLoading: boolean = false;
    groupsError$: Observable<string>;

    deleteSuccess$: Observable<boolean>;
    deleteError$: Observable<string>;

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

    allowAddBankAccountGroup: boolean = false;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    constructor(private store: Store<coreState.State>,
        public snackBar: MatSnackBar,
        private accountService: AccountService,
        private translateService: TranslateService,
        private fuseConfigService: FuseConfigService,
        private ngZone: NgZone, 
        private router: Router) {
        this.allowAddBankAccountGroup = this.accountService.isUserAuthorizedForTask(permissions.AddMobileTransferAccountGroup);
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
                headerName: this.translateService.instant('CARD-TO-CARD-ACCOUNT-GROUP.LIST-COLUMNS.OWNER'),
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
            },
            {
                headerName: this.translateService.instant('CARD-TO-CARD-ACCOUNT-GROUP.LIST-COLUMNS.NAME'),
                field: "name",
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
                hide: !this.allowAddBankAccountGroup
            }];

        this.store.dispatch(new mobileTransferCardAccountGroupActions.ClearErrors());

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

        this.groups$ = this.store.select(coreState.getAllMobileTransferCardAccountGroups);

        this.groups$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            if (items == undefined) {
                this.groups = [];
            }
            else {
                this.groups = items;
            }
        });

        this.groupsLoading$ = this.store.select(coreState.getMobileTransferCardAccountGroupLoading);

        this.groupsLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.groupsLoading = l;
        });

        this.groupsError$ = this.store.select(coreState.getAllMobileTransferCardAccountGroupError);

        this.groupsError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.deleteSuccess$ = this.store.select(coreState.getMobileTransferCardAccountGroupDeleteSuccess);
        this.deleteSuccess$.pipe(takeUntil(this.destroyed$)).subscribe(t => {

            if (t == true) {
                this.store.dispatch(new mobileTransferCardAccountGroupActions.GetAll());
            }
        });

        this.deleteError$ = this.store.select(coreState.getMobileTransferCardAccountGroupDeleteError);

        this.deleteError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;
            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new accountActions.GetOwners());
                this.store.dispatch(new mobileTransferCardAccountGroupActions.GetAll());
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

    }

    delete(id: number) {
        if (id) {
            this.store.dispatch(new mobileTransferCardAccountGroupActions.Delete(id));
        }
    }

    onCellClicked(e: CellClickedEvent) {
        if (e && e.colDef && e.colDef.field != 'id') {
            var selectedRows = this.gridApi.getSelectedRows();

            if (selectedRows.length > 0) {
                var selected = selectedRows[0];

                this.ngZone.run(() => this.router.navigate(['/mobiletransferaccountgroup/' + selected.id]));
            }
        }
    }

    ngOnDestroy(): void {
        this.store.dispatch(new mobileTransferCardAccountGroupActions.ClearErrors());
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

