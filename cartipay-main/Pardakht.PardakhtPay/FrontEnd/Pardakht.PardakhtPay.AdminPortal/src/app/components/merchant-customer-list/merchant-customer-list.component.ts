import { Component, OnInit, OnDestroy } from '@angular/core';
import { fuseAnimations } from '../../core/animations';
import { IGetRowsParams, CellClickedEvent } from 'ag-grid-community';
import { MerchantCustomerSearchArgs, MerchantCustomer, CustomerPhoneNumbers } from '../../models/merchant-customer';
import { Observable, ReplaySubject } from 'rxjs';
import { ListSearchResponse, defaultColumnDefs } from '../../models/types';
import { Tenant } from '../../models/tenant';
import { Store } from '@ngrx/store';
import * as coreState from '../../core'
import { MatSnackBar } from '@angular/material';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { DateFormatterComponent } from '../formatters/dateformatter';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import * as merchantCustomer from '../../core/actions/merchantCustomer';
import * as userSegmentGroupActions from '../../core/actions/userSegmentGroup';
import { UserSegmentGroup } from '../../models/user-segment-group';
import { IconButtonFormatterComponent } from '../formatters/iconbuttonformatter';
import * as permissions from '../../models/permissions';
import { AccountService } from '../../core/services/account.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-merchant-customer-list',
  templateUrl: './merchant-customer-list.component.html',
    styleUrls: ['./merchant-customer-list.component.scss'],
    animations: fuseAnimations
})
export class MerchantCustomerListComponent implements OnInit, OnDestroy {
    args: MerchantCustomerSearchArgs;

    loading$: Observable<boolean>;
    searchItems$: Observable<ListSearchResponse<MerchantCustomer[]>>;
    searchItems: ListSearchResponse<MerchantCustomer[]>;
    searchError$: Observable<string>;

    userSegmentGroups$: Observable<UserSegmentGroup[]>;
    userSegmentGroups: UserSegmentGroup[];
    userSegmentGroupLoadingError$: Observable<string>;
    getUserSegmentGroupsLoading$: Observable<boolean>;
    getUserSegmentGroupsLoading: boolean;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    isProviderAdmin$: Observable<boolean>;
    isProviderAdmin: boolean;

    isTenantAdmin$: Observable<boolean>;
    isTenantAdmin: boolean;

    isStandardUser$: Observable<boolean>;
    isStandardUser: boolean;

    accountGuid$: Observable<string>;
    accountGuid: string;
    rowClassRules;

    downloadPhoneNumbers$: Observable<CustomerPhoneNumbers>;
    downloadPhoneNumbersError$: Observable<string>;

    gridApi;
    callBackApi;
    gridColumnApi;
    columnDefs;
    enableRtl: boolean = false;
    frameworkComponents;

    defaultColumnDefs = defaultColumnDefs;

    selectedMerchantCustomer: MerchantCustomer = null;
    allowExportPhoneNumbers: boolean = false;
    allowRemoveRegisteredPhoneNumbers: boolean = false;

    constructor(private store: Store<coreState.State>,
        private snackBar: MatSnackBar,
        private router: Router,
        private accountService: AccountService,
        private fuseConfigService: FuseConfigService,
        private translateService: TranslateService) {
        this.args = new MerchantCustomerSearchArgs();
        this.args.pageSize = 25;
        this.allowExportPhoneNumbers = this.accountService.isUserAuthorizedForTask(permissions.ListCustomersPhoneNumbers);
        this.allowRemoveRegisteredPhoneNumbers = this.accountService.isUserAuthorizedForTask(permissions.RemoveCustomerRegisteredPhoneNumbers);
        
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
                    icon: 'details',
                    iconClass: 'success',
                    allow: true,
                    allowed: undefined
                },
                hide: false
            },
            {
                headerName: this.translateService.instant('GENERAL.ID'),
                field: "id",
                sortable: true,
                resizable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.WEBSITE-NAME'),
                field: "websiteName",
                sortable: true,
                resizable: true,
                width: 150,
                filter: "agTextColumnFilter"
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.USER-ID'),
                field: "userId",
                sortable: true,
                resizable: true,
                width: 200
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-TRANSACTION-COUNT'),
                field: "totalTransactionCount",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-COMPLETED-TRANSACTION-COUNT'),
                field: "totalCompletedTransactionCount",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-DEPOSIT-AMOUNT'),
                field: "totalDepositAmount",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-WITHDRAWAL-COUNT'),
                field: "totalWithdrawalCount",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-COMPLETED-WITHDRAWAL-COUNT'),
                field: "totalCompletedWithdrawalCount",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-WITHDRAWAL-AMOUNT'),
                field: "totalWithdrawalAmount",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('USER-SEGMENT.LIST-COLUMNS.GROUP'),
                field: "userSegmentGroupId",
                sortable: true,
                resizable: true,
                valueGetter: params => params == undefined || params.data == undefined ? '' : this.getUserSegmentGroupName(params.data.userSegmentGroupId),
                width: 200
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.REGISTER-DATE'),
                field: "registerDate",
                sortable: true,
                resizable: true,
                cellRenderer: 'dateFormatterComponent',
                width: 150
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-DEPOSIT'),
                field: "totalDeposit",
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 150,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-WITHDRAW'),
                field: "totalWithdraw",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 150,
                filter: "agNumberColumnFilter"
            },

            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.DEPOSIT-NUMBER'),
                field: "depositNumber",
                sortable: true,
                resizable: true,
                width: 200,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.WITHDRAW-NUMBER'),
                field: "withdrawNumber",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.ACTIVITY-SCORE'),
                field: "activityScore",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.GROUP-NAME'),
                field: "groupName",
                sortable: true,
                resizable: true,
                width: 200,
                filter: "agTextColumnFilter"
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.LAST-ACTIVITY-DATE'),
                field: "lastActivity",
                sortable: true,
                resizable: true,
                cellRenderer: 'dateFormatterComponent',
                width: 200
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-SPORTBOOK-AMOUNT'),
                field: "userTotalSportbook",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-SPORTBOOK-COUNT'),
                field: "userSportbookNumber",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-CASINO-AMOUNT'),
                field: "userTotalCasino",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-CASINO-COUNT'),
                field: "userCasinoNumber",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200,
                filter: "agNumberColumnFilter"
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.PHONE-NUMBER'),
                field: "phoneNumber",
                sortable: true,
                resizable: true,
                width: 200
            }
            ,
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.PHONE-NUMBER-RELATED-CUSTOMERS'),
                field: "phoneNumberRelatedCustomers",
                sortable: true,
                resizable: true,
                width: 200
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.CARD-NUMBER-RELATED-CUSTOMERS'),
                field: "cardNumberRelatedCustomers",
                sortable: true,
                resizable: true,
                width: 200
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.DIFFERENT-CARD-NUMBER-COUNT'),
                field: "differentCardNumberCount",
                sortable: true,
                resizable: true,
                width: 200
            },
            {
                headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.REMOVE-REGISTERED-PHONE'),
                field: "id",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.removeRegisteredPhone.bind(this),
                    icon: 'remove_circle_outline',
                    iconClass: '',
                    title: 'MERCHANT-CUSTOMER.LIST-COLUMNS.REMOVE-REGISTERED-PHONE'//,
                    //allow: this.allowRemoveRegisteredPhoneNumbers,
                    //allowed: this.showRemoveRegisteredPhoneAllowed.bind(this)
                },
               // hide: !this.allowRemoveRegisteredPhoneNumbers
            },
        ];

        this.rowClassRules = {

        };
    }

    ngOnInit() {

        this.store.dispatch(new merchantCustomer.ClearAll());

        this.fuseConfigService.config.pipe(takeUntil(this.destroyed$)).subscribe(config => {

            this.enableRtl = config.locale != null && config.locale != undefined && config.locale.direction == 'rtl';
        });

        this.frameworkComponents = {
            booleanFormatterComponent: BooleanFormatterComponent,
            dateFormatterComponent: DateFormatterComponent,
            numberFormatterComponent: NumberFormatterComponent,
            iconButtonFormatterComponent: IconButtonFormatterComponent
        };

        this.loading$ = this.store.select(coreState.getMerchantCustomerLoading);
        this.searchItems$ = this.store.select(coreState.getMerchantCustomerSearchResults);

        this.searchError$ = this.store.select(coreState.getMerchantCustomerSearchError);
        
        this.searchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.searchItems$.pipe(takeUntil(this.destroyed$)).subscribe((data: ListSearchResponse<MerchantCustomer[]>) => {
            this.searchItems = data;

            if (this.callBackApi) {
                if (data) {
                    this.callBackApi.successCallback(data.items, data.paging.totalItems);
                } else {
                    this.callBackApi.successCallback([]);
                }
            }
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

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new userSegmentGroupActions.GetAll());
                this.loadTransactions();
            }
        });

        this.userSegmentGroups$ = this.store.select(coreState.getAllUserSegmentGroups);

        this.userSegmentGroups$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            if (items != undefined) {
                this.userSegmentGroups = items;
            }
        });

        this.getUserSegmentGroupsLoading$ = this.store.select(coreState.getUserSegmentGroupLoading);

        this.getUserSegmentGroupsLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.getUserSegmentGroupsLoading = l;
        });

        this.userSegmentGroupLoadingError$ = this.store.select(coreState.getAllUserSegmentGroupError);

        this.userSegmentGroupLoadingError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.downloadPhoneNumbers$ = this.store.select(coreState.downloadPhoneNumbers);

        this.downloadPhoneNumbers$.pipe(takeUntil(this.destroyed$)).subscribe(item => {
            if (item != undefined) {
                console.log(item);
                var url = window.URL.createObjectURL(item.data);

                var link = document.createElement("a");
                link.setAttribute("href", url);
                link.setAttribute("download", item.fileName);
                link.style.display = "none";
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            }
        });

        this.downloadPhoneNumbersError$ = this.store.select(coreState.downloadPhoneNumbersError);

        this.downloadPhoneNumbersError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });
    }

    getUserSegmentGroupName(id: number): string {
        if (id) {
            if (this.userSegmentGroups) {
                var segment = this.userSegmentGroups.find(t => t.id == id);

                if (segment != null) {
                    return segment.name;
                }
            }
        }

        return '';
    }

    loadTransactions() {
        if (!this.selectedTenant || !this.gridApi) {
            return;
        }
        this.store.dispatch(new merchantCustomer.Search(this.args));
    }

    refresh() {
        this.gridApi.paginationGoToFirstPage();
        this.loadTransactions();
    }

    downloadPhoneNumbers() {
        this.store.dispatch(new merchantCustomer.DownloadPhoneNumbers(this.args));
    }

    ngOnDestroy(): void {
        this.store.dispatch(new merchantCustomer.ClearAll());
        this.store.dispatch(new userSegmentGroupActions.Clear());
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

    detailClosed() {
        this.selectedMerchantCustomer = undefined;
        this.gridApi.deselectAll();
    }

    onCellClicked(e: CellClickedEvent) {
        if (e && e.colDef && e.colDef.colId == 'detail') {
            if (e.data) {
                this.selectedMerchantCustomer = e.data;
            }
        }
    }

    removeRegisteredPhone(id) {
        this.router.navigate(['/merchantcustomer/getregisteredphones/' + id]);
        // this.store.dispatch(new bankLoginActions.GetQRRegistrationDetails(id));
    }

    showRemoveRegisteredPhoneAllowed(params) {
        return this.allowRemoveRegisteredPhoneNumbers;
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
        this.loadTransactions();
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
