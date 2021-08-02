import { Component, OnInit, OnDestroy } from '@angular/core';
import * as coreState from '../../core';
import { Store } from '@ngrx/store';
import { Observable, ReplaySubject } from 'rxjs';
import { MobileTransferDevice, MobileTransferDeviceStatus, MobileTransferDeviceStatuses } from '../../models/mobile-transfer';
import * as mobileTransferDeviceActions from '../../core/actions/mobile-transfer-device';
import { fuseAnimations } from '../../core/animations';
import { Router } from '@angular/router';
import { MatSnackBar, MatDialog } from '@angular/material';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../models/tenant';
import * as accountActions from '../../core/actions/account';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { CellClickedEvent } from 'ag-grid-community';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { DateFormatterComponent } from '../formatters/dateformatter';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';
import { DeactivateLoginDialogComponent } from '../deactivate-login-dialog/deactivate-login-dialog.component';
import { IconButtonFormatterComponent } from '../formatters/iconbuttonformatter';

@Component({
    selector: 'app-mobile-transfer-device-list',
    templateUrl: './mobile-transfer-device-list.component.html',
    styleUrls: ['./mobile-transfer-device-list.component.scss'],
    animations: fuseAnimations
})
export class MobileTransferDeviceListComponent implements OnInit, OnDestroy {

    mobileTransferDevices$: Observable<MobileTransferDevice[]>;
    mobileTransferDevices: MobileTransferDevice[];
    searchError$: Observable<string>;
    selected = [];
    loading$: Observable<boolean>;
    loading: boolean;

    sendSmsError$: Observable<string>;

    checkStatusError$: Observable<string>;
    checkStatusCompleted$: Observable<boolean>;

    removeError$: Observable<string>;
    removeCompleted$: Observable<string>;

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    columnDefs;
    enableRtl: boolean = false;
    frameworkComponents;

    gridApi;

    allowAddBankLogin: boolean = false;

    statusEnum = MobileTransferDeviceStatus;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    constructor(private store: Store<coreState.State>,
        public snackBar: MatSnackBar,
        private translateService: TranslateService,
        private fuseConfigService: FuseConfigService,
        private accountService: AccountService,
        private router: Router,
        public dialog: MatDialog) {
        this.allowAddBankLogin = this.accountService.isUserAuthorizedForTask(permissions.AddMobileTransferDevice);
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
                headerName: this.translateService.instant('MOBILE-TRANSFER-DEVICE.LIST-COLUMNS.PHONE-NUMBER'),
                field: "phoneNumber",
                width: 200,
                resizable: true,
                sortable: true,
                filter: "agTextColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            }, {
                headerName: this.translateService.instant('MOBILE-TRANSFER-DEVICE.LIST-COLUMNS.STATUS'),
                field: "status",
                width: 200,
                resizable: true,
                sortable: true,
                valueGetter: params => this.getStatusName(params.data == undefined ? '' : params.data.status),
                filter: "agTextColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            }, {
                headerName: this.translateService.instant('MOBILE-TRANSFER-DEVICE.LIST-COLUMNS.VERIFICATION-CODE-SEND-DATE'),
                field: "verifyCodeSendDate",
                width: 200,
                resizable: true,
                sortable: true,
                cellRenderer: 'dateFormatterComponent'
            }, {
                headerName: this.translateService.instant('MOBILE-TRANSFER-DEVICE.LIST-COLUMNS.VERIFIED-DATE'),
                field: "verifiedDate",
                width: 200,
                resizable: true,
                sortable: true,
                cellRenderer: 'dateFormatterComponent'
            }, {
                headerName: this.translateService.instant('MOBILE-TRANSFER-DEVICE.LIST-COLUMNS.EXTERNAL-ID'),
                field: "externalId",
                width: 200,
                resizable: true,
                sortable: true,
                filter: "agNumberColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            }, {
                headerName: this.translateService.instant('MOBILE-TRANSFER-DEVICE.LIST-COLUMNS.EXTERNAL-STATUS'),
                field: "externalStatus",
                width: 200,
                resizable: true,
                sortable: true,
                filter: "agTextColumnFilter",
                filterParams: {
                    applyButton: true,
                    clearButton: true
                }
            }, {
                headerName: this.translateService.instant('MOBILE-TRANSFER-DEVICE.LIST-COLUMNS.IS-ACTIVE'),
                field: "isActive",
                width: 200,
                resizable: true,
                sortable: true,
                cellRenderer: 'booleanFormatterComponent'
            },
            {
                headerName: this.translateService.instant('MOBILE-TRANSFER-DEVICE.LIST-COLUMNS.SEND-SMS'),
                field: "id",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.sendSms.bind(this),
                    icon: 'textsms',
                    iconClass: 'success',
                    title: 'MOBILE-TRANSFER-DEVICE.LIST-COLUMNS.SEND-SMS',
                    allow: this.allowAddBankLogin,
                    allowed: this.sendSmsAllowed.bind(this)
                },
                hide: !this.allowAddBankLogin
            },
            {
                headerName: this.translateService.instant('MOBILE-TRANSFER-DEVICE.LIST-COLUMNS.CHECK-STATUS'),
                field: "id",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.checkStatus.bind(this),
                    icon: 'cached',
                    iconClass: 'success',
                    title: 'MOBILE-TRANSFER-DEVICE.LIST-COLUMNS.CHECK-STATUS',
                    allow: this.allowAddBankLogin,
                    allowed: this.checkStatusAllowed.bind(this)
                },
                hide: !this.allowAddBankLogin
            },
            {
                headerName: this.translateService.instant('MOBILE-TRANSFER-DEVICE.LIST-COLUMNS.REMOVE'),
                field: "id",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.remove.bind(this),
                    icon: 'remove_circle',
                    iconClass: 'danger',
                    title: 'MOBILE-TRANSFER-DEVICE.LIST-COLUMNS.REMOVE',
                    allow: this.allowAddBankLogin,
                    allowed: this.removeAllowed.bind(this)
                },
                hide: !this.allowAddBankLogin
            }
        ];

        this.store.dispatch(new mobileTransferDeviceActions.ClearErrors());
        this.mobileTransferDevices$ = this.store.select(coreState.getMobileTransferDeviceSearchResults);

        this.mobileTransferDevices$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.mobileTransferDevices = items;

            if (this.gridApi && items) {
                this.gridApi.setRowData(this.mobileTransferDevices);
            }
        });

        this.loading$ = this.store.select(coreState.getMobileTransferDeviceLoading);

        this.loading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.loading = l;
        });

        this.searchError$ = this.store.select(coreState.getMobileTransferDeviceSearchError);

        this.searchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;
            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new mobileTransferDeviceActions.GetAll());
            }
        });

        this.sendSmsError$ = this.store.select(coreState.getMobileTransferDeviceSendSmsError);
        this.sendSmsError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.checkStatusError$ = this.store.select(coreState.getMobileTransferDeviceCheckStatusError);
        this.checkStatusError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {

            if (error) {
                this.openSnackBar(error);
            }

        });

        this.removeError$ = this.store.select(coreState.getMobileTransferDeviceRemoveError);

        this.removeError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });
    }

    sendSms(id: number) {
        if (id > 0) {
            const dialogRef = this.dialog.open(DeactivateLoginDialogComponent, {
                width: '250px',
                data: 'MOBILE-TRANSFER-DEVICE.GENERAL.SEND-SMS-APPROVE'
            });

            dialogRef.afterClosed().subscribe(result => {
                if (result == true) {
                    this.store.dispatch(new mobileTransferDeviceActions.SendSms(id));
                }
            });
        }
    }

    sendSmsAllowed(params): boolean {
        var result = this.allowAddBankLogin && params != undefined && params.data != undefined && (params.data.status != this.statusEnum.PhoneNumberVerified && params.data.status != this.statusEnum.Removed);
        return result;
    }

    checkStatus(id: number) {
        if (id > 0) {
            this.store.dispatch(new mobileTransferDeviceActions.CheckStatus(id));
        }
    }

    checkStatusAllowed(params): boolean {
        var result = this.allowAddBankLogin && params != undefined && params.data != undefined;
        return result;
    }

    remove(id: number) {
        if (id > 0) {
            const dialogRef = this.dialog.open(DeactivateLoginDialogComponent, {
                width: '250px',
                data: 'MOBILE-TRANSFER-DEVICE.GENERAL.REMOVE-APPROVE'
            });

            dialogRef.afterClosed().subscribe(result => {
                if (result == true) {
                    this.store.dispatch(new mobileTransferDeviceActions.Remove(id));
                }
            });
        }
    }

    removeAllowed(params): boolean {
        var result = this.allowAddBankLogin && params != undefined && params.data != undefined && (params.data.status != this.statusEnum.Created && params.data.status != this.statusEnum.Removed);
        return result;
    }

    getStatusName(status) {
        if (status == undefined || status == null || status == '') {
            return '';
        }

        var statusValue = MobileTransferDeviceStatuses.find(t => t.value == status);

        if (statusValue != null && statusValue != undefined) {
            return this.translateService.instant(statusValue.key);
        }

        return '';
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
        this.store.dispatch(new mobileTransferDeviceActions.ClearErrors());
        this.destroyed$.next(true);
        this.destroyed$.complete();
    }

    onCellClicked(e: CellClickedEvent) {

        if (!this.allowAddBankLogin) {
            return;
        }

        if (e && e.colDef && e.colDef.field != 'id') {
            var selectedRows = this.gridApi.getSelectedRows();

            if (selectedRows.length > 0) {
                var selected = selectedRows[0];

                this.router.navigate(['/mobiletransferdevice/' + selected.id]);
            }
        }
    }

    onGridReady(params) {
        this.gridApi = params.api;
    }
}


