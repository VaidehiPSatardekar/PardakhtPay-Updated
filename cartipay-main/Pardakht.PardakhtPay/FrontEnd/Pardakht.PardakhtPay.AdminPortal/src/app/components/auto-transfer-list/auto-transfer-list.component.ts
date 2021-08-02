import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource, MatSnackBar } from '@angular/material';
import { Observable, ReplaySubject } from 'rxjs';
import { AutoTransfer, AutoTransferSearchArgs } from '../../models/autoTransfer';
import * as autoTransferactions from '../../core/actions/autoTransfer';
//import * as merchantActions from '../../core/actions/merchant';
import { fuseAnimations } from '../../core/animations';
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import { ListSearchResponse } from '../../models/types';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
//import { Merchant } from '../../models/merchant-model';
import * as tenantActions from '../../core/actions/tenant';
import { Tenant } from '../../models/tenant';
import { TimeZoneService } from '../../core/services/timeZoneService/time-zone.service';
import { TimeZone } from '../../models/timeZone';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { IGetRowsParams } from 'ag-grid-community';
import { DateFormatterComponent } from '../formatters/dateformatter';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { BooleanInverseFormatterComponent } from '../formatters/booleaninverseformatter';
import { TransferStatusDescription } from '../../models/application-settings';
import * as applicationSettingsActions from '../../core/actions/applicationSettings';

@Component({
  selector: 'app-auto-transfer-list',
  templateUrl: './auto-transfer-list.component.html',
    styleUrls: ['./auto-transfer-list.component.scss'],
    animations: fuseAnimations
})
export class AutoTransferListComponent implements OnInit, OnDestroy {
    args: AutoTransferSearchArgs;

    loading$: Observable<boolean>;
    autoTransfers$: Observable<ListSearchResponse<AutoTransfer[]>>;
    autoTransfers: ListSearchResponse<AutoTransfer[]>;
    searchError$: Observable<string>;

    tenants$: Observable<Tenant[]>;
    tenants: Tenant[] = [];

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

    transferStatuses$: Observable<TransferStatusDescription[]>;
    transferStatuses: TransferStatusDescription[];
    transferStatusError$: Observable<string>;

    gridApi;
    callBackApi;
    gridColumnApi;
    columnDefs;
    enableRtl: boolean = false;
    frameworkComponents;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    constructor(private store: Store<coreState.State>,
        private snackBar: MatSnackBar,
        private timeZoneService: TimeZoneService,
        private fuseConfigService: FuseConfigService,
        private translateService: TranslateService) {
        this.args = new AutoTransferSearchArgs();
        this.args.pageSize = 25;
        this.args.dateRange = 'dt';
        this.args.tenants = [];

        this.columnDefs = [
            {
                headerName: this.translateService.instant('GENERAL.ID'),
                field: "id",
                colId: "id",
                sortable: true,
                resizable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.LOGIN-NAME'),
                field: "friendlyName",
                colId: "friendlyName",
                resizable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.FROM-ACCOUNT-NO'),
                field: "transferFromAccount",
                colId: "transferFromAccount",
                resizable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.TO-ACCOUNT-NO'),
                field: "transferToAccount",
                colId: "transferToAccount",
                resizable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.AMOUNT'),
                field: "amount",
                colId: "amount",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200
            },
            {
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.STATUS'),
                field: "status",
                colId: "status",
                sortable: true,
                resizable: true,
                width: 200,
                valueGetter: params => this.getStatusName(params.data == undefined ? undefined : params.data.status)
            },
            {
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.DATE'),
                field: "transferRequestDateStr",
                colId: "transferRequestDateStr",
                sortable: true,
                resizable: true,
                cellRenderer: 'dateFormatterComponent',
                width: 150
            },
            {
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.TRANSFERRED_DATE'),
                field: "transferredDateStr",
                colId: "transferredDateStr",
                cellRenderer: 'dateFormatterComponent',
                sortable: true,
                resizable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('CARD-TO-CARD-ACCOUNT-GROUP.GENERAL.ACTIVE'),
                colId: "activate",
                cellRenderer: 'booleanInverseFormatterComponent',
                sortable: true,
                resizable: true,
                width: 100,
                valueGetter: params => params == undefined || params.data == undefined || (params.data.isCancelled == false && params.data.status != 1) ? undefined : (params.data.isCancelled)
            },
            {
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.CANCEL-DATE'),
                field: "cancelDateStr",
                colId: "cancelDateStr",
                cellRenderer: 'dateFormatterComponent',
                sortable: true,
                resizable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.TRANSFER-REQUEST-ID'),
                field: "requestId",
                colId: "requestId",
                sortable: true,
                resizable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('GENERAL.BANK-LOGIN-ID'),
                field: "bankLoginId",
                colId: "bankLoginId",
                sortable: false,
                resizable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('GENERAL.BANK-ACCOUNT-ID'),
                field: "bankAccountId",
                colId: "bankAccountId",
                sortable: false,
                resizable: true,
                width: 150
            }
        ];
    }

    ngOnInit() {

        this.loading$ = this.store.select(coreState.getAutoTransferLoading);
        this.autoTransfers$ = this.store.select(coreState.getAutoTransferSearchResults);
        this.searchError$ = this.store.select(coreState.getAutoTransferSearchError);

        this.tenants$ = this.store.select(coreState.getTenantSearchResults);

        this.zones = this.timeZoneService.getTimeZones();
        this.selectedZoneId = this.timeZoneService.getTimeZone().timeZoneId;

        this.fuseConfigService.config.pipe(takeUntil(this.destroyed$)).subscribe(config => {

            this.enableRtl = config.locale != null && config.locale != undefined && config.locale.direction == 'rtl';
        });

        this.frameworkComponents = {
            booleanFormatterComponent: BooleanFormatterComponent,
            booleanInverseFormatterComponent: BooleanInverseFormatterComponent,
            dateFormatterComponent: DateFormatterComponent,
            numberFormatterComponent: NumberFormatterComponent
        };

        this.searchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.autoTransfers$.pipe(takeUntil(this.destroyed$)).subscribe((data: ListSearchResponse<AutoTransfer[]>) => {
            this.autoTransfers = data;
            if (this.callBackApi) {
                if (data) {
                    this.callBackApi.successCallback(data.items, data.paging.totalItems);
                } else {
                    this.callBackApi.successCallback([]);
                }
            }
        });

        this.tenants$.pipe(takeUntil(this.destroyed$)).subscribe(items => {

            this.tenants = items;
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
                this.store.dispatch(new applicationSettingsActions.GetTransferStatus());
                this.loadAutoTransfers();
            }
        });

        this.transferStatusError$ = this.store.select(coreState.getTransferStatusesError);

        this.transferStatusError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.transferStatuses$ = this.store.select(coreState.getTransferStatuses);

        this.transferStatuses$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.transferStatuses = items;
        });
    }

    loadAutoTransfers() {

        if (this.selectedTenant == undefined || this.gridApi == undefined) {
            return;
        }
        this.args.timeZoneInfoId = this.selectedZoneId;

        this.args.tenants = [this.selectedTenant.tenantDomainPlatformMapGuid];

        this.store.dispatch(new autoTransferactions.Search(this.args));
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

        this.loadAutoTransfers();
    }

    refresh() {
        this.gridApi.paginationGoToFirstPage();
        this.loadAutoTransfers();
    }

    getStatusName(status: number): string {

        if (this.transferStatuses && status != undefined) {
            let item = this.transferStatuses.find(t => t.id == status);

            if (item != null) {
                return this.translateService.currentLang == 'fa' ? item.descriptionInFarsi : item.descriptionInEnglish;
            }
        }

        return '';
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

    ngOnDestroy(): void {
        this.store.dispatch(new autoTransferactions.ClearAll());
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

    onGridReady(readyParams) {
        this.gridApi = readyParams.api;
        this.gridColumnApi = readyParams.columnApi;

        var datasource = {
            getRows: (params: IGetRowsParams) => {
                this.callBackApi = params;
                this.loadItems(params);
            }
        };
        readyParams.api.setDatasource(datasource);
    }
}

