import { Component, OnInit, OnDestroy, Input, NgZone } from '@angular/core';
import { MatSnackBar, MatDialog } from '@angular/material';
import { Observable, ReplaySubject } from 'rxjs';
import { Invoice, InvoiceSearchArgs } from '../../models/invoice';
import * as invoiceActions from '../../core/actions/invoice';
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

@Component({
  selector: 'app-invoice-list',
  templateUrl: './invoice-list.component.html',
  styleUrls: ['./invoice-list.component.scss'],
  animations: fuseAnimations
})
export class InvoiceListComponent implements OnInit, OnDestroy {

  args: InvoiceSearchArgs;

  selectedInvoice: Invoice = undefined;

  @Input() merchantCustomer: MerchantCustomer;

  loading$: Observable<boolean>;
  items$: Observable<ListSearchResponse<Invoice[]>>;
  items: ListSearchResponse<Invoice[]>;
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
  summary: boolean = false;

  defaultColumnDefs = defaultColumnDefs;

  private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

  constructor(private store: Store<coreState.State>,
    private snackBar: MatSnackBar,
    private timeZoneService: TimeZoneService,
    private fuseConfigService: FuseConfigService,
    private translateService: TranslateService,
    public dialog: MatDialog,
    private ngZone: NgZone, 
    private router: Router) {

    var cachedArgs = localStorage.getItem('invoiceargs');

    if (cachedArgs != null && cachedArgs != undefined) {
      this.args = JSON.parse(cachedArgs);
    } else {
      this.createSearchArgs();
    }

    this.zones = this.timeZoneService.getTimeZones();
    this.selectedZoneId = this.timeZoneService.getTimeZone().timeZoneId;
  }

  ngOnInit() {
    this.store.dispatch(new invoiceActions.Clear());

    this.loading$ = this.store.select(coreState.getInvoiceLoading);
    this.items$ = this.store.select(coreState.getInvoiceSearchResults);
    this.searchError$ = this.store.select(coreState.getInvoiceSearchResultError);

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

    this.items$.pipe(takeUntil(this.destroyed$)).subscribe((data: ListSearchResponse<Invoice[]>) => {
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
        headerName: '',
        field: "id",
        colId: "summary",
        width: 110,
        resizable: true,
        sortable: true,
        cellRenderer: 'iconButtonFormatterComponent',
        cellRendererParams: {
          icon: 'youtube_searched_for',
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
        headerName: this.translateService.instant('INVOICE.LIST-COLUMNS.START-DATE'),
        field: "startDateStr",
        colId: "startDate",
        sortable: true,
        resizable: true,
        cellRenderer: 'dateFormatterComponent',
        width: 150,
        filter: "agDateColumnFilter"
      },
      {
        headerName: this.translateService.instant('INVOICE.LIST-COLUMNS.END-DATE'),
        field: "endDateStr",
        colId: "endDate",
        cellRenderer: 'dateFormatterComponent',
        sortable: true,
        resizable: true,
        width: 150,
        filter: "agDateColumnFilter"
      },
      {
        headerName: this.translateService.instant('TRANSFER-ACCOUNT.LIST-COLUMNS.OWNER'),
        field: "userId",
        colId: "userId",
        sortable: true,
        resizable: true,
        width: 150,
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
      "confirmed-row": "data != undefined && data.invoiceStatus == 4"
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

  userHeader(params) {
    let content = '';

    if (params == undefined || params.data == undefined) {
      return '';
    }

    if (params.data.merchantCustomerId != null) {
      return '<a href="/merchantcustomer/' + params.data.merchantCustomerId + '" target="_blank">' + params.data.userId + '</>';
    }
    else {
      return params.data.userId;
    }
  }

  loadInvoices() {
    if (this.selectedTenant == undefined || this.callBackApi == undefined) {
      return;
    }

    this.args.timeZoneInfoId = this.selectedZoneId;

    this.store.dispatch(new invoiceActions.Search(this.args));
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
    this.store.dispatch(new invoiceActions.Clear());
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
        this.summary = false;
        this.selectedInvoice = e.data;
      }
    }
    else if (e && e.colDef && e.colDef.colId == 'summary') {
      if (e.data) {
        this.summary = true;
        this.selectedInvoice = e.data;
      }
    }
  }

  detailClosed() {
    this.selectedInvoice = undefined;
    this.gridApi.deselectAll();
  }

  createSearchArgs() {
    this.args = new InvoiceSearchArgs();
    this.args.pageSize = 25;
  }

  onColumnMoved(params) {
    var columnState = JSON.stringify(params.columnApi.getColumnState());
    localStorage.setItem('invoiceColumnState', columnState);
  }

  onGridReady(readyParams) {
    this.gridApi = readyParams.api;
    this.gridColumnApi = readyParams.columnApi;

    var columnState: any[] = JSON.parse(localStorage.getItem('invoiceColumnState'));
    if (columnState) {
      if (this.columnDefs.find(t => columnState.findIndex(r => t.colId == r.colId) == -1) != null) {
        localStorage.removeItem('invoiceColumnState');
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


