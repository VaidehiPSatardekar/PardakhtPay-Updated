import { Component, OnInit, OnDestroy, Input, NgZone } from '@angular/core';
import { MatSnackBar, MatDialog } from '@angular/material';
import { Observable, ReplaySubject } from 'rxjs';
import { Invoice, InvoiceSearchArgs, InvoicePaymentSearchArgs, InvoicePayment } from '../../models/invoice';
import * as invoicePaymentActions from '../../core/actions/invoicePayment';
import * as merchantActions from '../../core/actions/merchant';
import { fuseAnimations } from '../../core/animations';
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import { ListSearchResponse, defaultColumnDefs } from '../../models/types';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Merchant } from '../../models/merchant-model';
import { Tenant } from '../../models/tenant';
import { TimeZoneService } from '../../core/services/timeZoneService/time-zone.service';
import { TimeZone } from '../../models/timeZone';
import { IGetRowsParams, CellClickedEvent } from 'ag-grid-community';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { DateFormatterComponent } from '../formatters/dateformatter';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { MerchantCustomer } from '../../models/merchant-customer';
import { IconButtonFormatterComponent } from '../formatters/iconbuttonformatter';
import { Router } from '@angular/router';
import * as accountActions from '../../core/actions/account';
import { Owner } from 'app/models/account.model';
import { AccountService } from 'app/core/services/account.service';
import * as permissions from '../../models/permissions';

@Component({
    selector: 'app-invoice-payment-list',
    templateUrl: './invoice-payment-list.component.html',
    styleUrls: ['./invoice-payment-list.component.scss'],
    animations: fuseAnimations
})
export class InvoicePaymentListComponent implements OnInit, OnDestroy {

    args: InvoicePaymentSearchArgs;

    loading$: Observable<boolean>;
    items$: Observable<ListSearchResponse<InvoicePayment[]>>;
    items: ListSearchResponse<InvoicePayment[]>;
    searchError$: Observable<string>;

    merchants$: Observable<Merchant[]>;
    merchantError$: Observable<string>;
    merchants: Merchant[];

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

    owners$: Observable<Owner[]>;
    owners: Owner[] = undefined;
    ownersLoading$: Observable<boolean>;
    ownersLoading: boolean = false;

    gridApi;
    callBackApi;
    gridColumnApi;
    columnDefs: any[];
    enableRtl: boolean = false;
    frameworkComponents;
    rowClassRules;

    defaultColumnDefs = defaultColumnDefs;

    allowAddInvoicePayment: boolean = false;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    constructor(private store: Store<coreState.State>,
        private snackBar: MatSnackBar,
        private timeZoneService: TimeZoneService,
        private fuseConfigService: FuseConfigService,
        private translateService: TranslateService,
        public dialog: MatDialog,
        private ngZone: NgZone, 
        private router: Router,
        private accountService : AccountService) {

            this.allowAddInvoicePayment = this.accountService.isUserAuthorizedForTask(permissions.AddInvoicePayment);

        var cachedArgs = localStorage.getItem('invoicePaymentargs');

        if (cachedArgs != null && cachedArgs != undefined) {
            this.args = JSON.parse(cachedArgs);
        } else {
            this.createSearchArgs();
        }

        this.zones = this.timeZoneService.getTimeZones();
        this.selectedZoneId = this.timeZoneService.getTimeZone().timeZoneId;
    }

    ngOnInit() {
        this.store.dispatch(new invoicePaymentActions.Clear());

        this.loading$ = this.store.select(coreState.getInvoicePaymentLoading);
        this.items$ = this.store.select(coreState.getAllInvoicePayments);
        this.searchError$ = this.store.select(coreState.getAllInvoicePaymentError);

        this.owners$ = this.store.select(coreState.getOwners);
        this.ownersLoading$ = this.store.select(coreState.getAccountLoading);

        this.ownersLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.ownersLoading = l;
        });

        this.owners$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.owners = items;
        });

        this.searchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.tenantGuid$ = this.store.select(coreState.getAccountTenantGuid);

        this.tenantGuid$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.tenantGuid = t;
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

        this.merchants$ = this.store.select(coreState.getAllMerchants);
        this.merchantError$ = this.store.select(coreState.getAllMerchantError);

        this.merchants$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.merchants = items;
        });

        this.items$.pipe(takeUntil(this.destroyed$)).subscribe((data: ListSearchResponse<InvoicePayment[]>) => {
            this.items = data;
            if (this.callBackApi) {
                if (data) {
                    this.callBackApi.successCallback(data.items, data.paging.totalItems);
                } else {
                    this.callBackApi.successCallback([]);
                }
            }
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new accountActions.GetOwners());
                this.loadInvoices();
            }
        });

        this.columnDefs = [
            {
                headerName: '',
                field: "id",
                colId: "detail",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    icon: 'search',
                    iconClass: 'success',
                    allow: true,
                    allowed: undefined
                },
                hide: false
            },
            {
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.AMOUNT'),
                field: "amount",
                colId: "amount",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('INVOICE-PAYMENT.LIST-COLUMNS.PAYMENT-REFERENCE'),
                field: "paymentReference",
                colId: "paymentReference",
                sortable: true,
                resizable: true,
                width: 250,
                filter: "agTextColumnFilter"
            },
            {
                headerName: this.translateService.instant('INVOICE-PAYMENT.LIST-COLUMNS.CREATE-DATE'),
                field: "createDateStr",
                colId: "createDate",
                sortable: true,
                resizable: true,
                cellRenderer: 'dateFormatterComponent',
                width: 250,
                filter: "agDateColumnFilter"
            },
            {
                headerName: this.translateService.instant('INVOICE-PAYMENT.LIST-COLUMNS.PAYMENT-DATE'),
                field: "paymentDateStr",
                colId: "paymentDate",
                cellRenderer: 'dateFormatterComponent',
                sortable: true,
                resizable: true,
                width: 250,
                filter: "agDateColumnFilter"
            },
            {
                headerName: this.translateService.instant('TRANSFER-ACCOUNT.LIST-COLUMNS.OWNER'),
                field: "userId",
                colId: "userId",
                sortable: true,
                resizable: true,
                width: 250,
                cellRenderer: params => this.getOwnerName(params),
                filter: "agTextColumnFilter",
                filterParams: {
                    suppressAndOrCondition: true,
                    applyButton: true,
                    clearButton: true,
                    caseSensitive: true,
                    defaultOption: "equals"
                }
            }
        ];

        this.rowClassRules = {
        };

        this.fuseConfigService.config.pipe(takeUntil(this.destroyed$)).subscribe(config => {

            this.enableRtl = config.locale != null && config.locale != undefined && config.locale.direction == 'rtl';
        });

        this.frameworkComponents = {
            booleanFormatterComponent: BooleanFormatterComponent,
            dateFormatterComponent: DateFormatterComponent,
            numberFormatterComponent: NumberFormatterComponent,
            iconButtonFormatterComponent: IconButtonFormatterComponent
        };

        this.store.dispatch(new merchantActions.GetAll());
    }

    loadInvoices() {
        if (this.selectedTenant == undefined || this.callBackApi == undefined) {
            return;
        }

        this.args.timeZoneInfoId = this.selectedZoneId;

        this.store.dispatch(new invoicePaymentActions.Search(this.args));
    }

    refresh() {
        this.gridApi.paginationGoToFirstPage();
        this.loadInvoices();
    }

    getMerchantTitle(id: number): string {

        if (this.merchants) {
            var merchant = this.merchants.find(t => t.id == id);

            if (merchant != null) {
                return merchant.title;
            }
        }

        return '';
    }

    ngOnDestroy(): void {
        this.store.dispatch(new invoicePaymentActions.Clear());
        this.store.dispatch(new merchantActions.ClearErrors());
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

        this.loadInvoices();
    }

    getOwnerName(params): string {
        if (params) {
            var data = params.data;
            if (data != undefined && data != null) {
                if (this.owners) {
                    var owner = this.owners.find(t => t.accountId == data.ownerGuid);

                    if (owner != null) {
                        return owner.username;
                    }
                }
            }
        }
        return '';
    }

    onCellClicked(e: CellClickedEvent) {
        if (e && e.colDef && e.colDef.colId == 'detail') {
            if (e.data) {
                this.ngZone.run(() => this.router.navigate(['/invoicepayment/' + e.data.id]));
            }
        }
    }

    createSearchArgs() {
        this.args = new InvoiceSearchArgs();
        this.args.pageSize = 25;
    }

    onColumnMoved(params) {
        var columnState = JSON.stringify(params.columnApi.getColumnState());
        localStorage.setItem('invoicePaymentColumnState', columnState);
    }

    onGridReady(readyParams) {
        this.gridApi = readyParams.api;
        this.gridColumnApi = readyParams.columnApi;

        var columnState: any[] = JSON.parse(localStorage.getItem('invoicePaymentColumnState'));
        if (columnState) {
            if (this.columnDefs.find(t => columnState.findIndex(r => t.colId == r.colId) == -1) != null) {
                localStorage.removeItem('invoicePaymentColumnState');
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


