import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { fuseAnimations } from '../../core/animations';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { distinctUntilChanged, debounceTime, take, filter } from 'rxjs/operators';
import { Observable, ReplaySubject } from 'rxjs';
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import { TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { takeUntil } from 'rxjs/operators';
import { ActivatedRoute, Router } from '@angular/router';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { BooleanInverseFormatterComponent } from '../formatters/booleaninverseformatter';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { DeleteButtonFormatterComponent } from '../formatters/deletebuttonformatter';
import { Withdrawal, WithdrawalTransferHistory, WithdrawalSearch } from '../../models/withdrawal';
import * as withdrawalActions from '../../core/actions/withdrawal';
import { DateFormatterComponent } from '../formatters/dateformatter';
import { TransferStatusDescription } from 'app/models/application-settings';
import * as applicationSettingsActions from '../../core/actions/applicationSettings';

@Component({
    selector: 'app-withdrawal-detail',
    templateUrl: './withdrawal-detail.component.html',
    styleUrls: ['./withdrawal-detail.component.scss'],
    animations: fuseAnimations
})
export class WithdrawalDetailComponent implements OnInit, OnDestroy {

    @Input() withdrawal: WithdrawalSearch;
    @Output() closed: EventEmitter<any> = new EventEmitter<any>();

    customerForm: FormGroup;
    isCreating: boolean;

    created$: Observable<boolean>;
    createdError$: Observable<string>;

    detailLoading$: Observable<boolean>;
    detailLoading: boolean;

    detail$: Observable<Withdrawal>;
    detailError$: Observable<string>;

    histories$: Observable<WithdrawalTransferHistory[]>;
    histories: WithdrawalTransferHistory[];
    historyError$: Observable<string>;
    

    transferStatuses$: Observable<TransferStatusDescription[]>;
    transferStatuses: TransferStatusDescription[];
    transferStatusError$: Observable<string>;

    withdrawalDetails: any[] = [];

    id: number;

    columnDefs;
    enableRtl: boolean = false;
    frameworkComponents;

    gridApi;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    constructor(
        private fb: FormBuilder,
        private store: Store<coreState.State>,
        private snackBar: MatSnackBar,
        private route: ActivatedRoute,
        private router: Router,
        private fuseConfigService: FuseConfigService,
        private translateService: TranslateService) { }

    ngOnInit() {

        this.store.dispatch(new applicationSettingsActions.ClearErrors());

        this.fuseConfigService.config.pipe(takeUntil(this.destroyed$)).subscribe(config => {

            this.enableRtl = config.locale != null && config.locale != undefined && config.locale.direction == 'rtl';
        });

        this.frameworkComponents = {
            booleanFormatterComponent: BooleanFormatterComponent,
            booleanInverseFormatterComponent: BooleanInverseFormatterComponent,
            numberFormatterComponent: NumberFormatterComponent,
            dateFormatterComponent: DateFormatterComponent,
            deleteButtonFormatterComponent: DeleteButtonFormatterComponent
        };

        this.columnDefs = [{
            headerName: this.translateService.instant('WITHDRAWAL.LIST-COLUMNS.TRANSFER_NOTES'),
            field: "transferNotes",
            sortable: true,
            resizable: true,
            width: 150
        },
        {
            headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.AMOUNT'),
            field: "amount",
            sortable: true,
            resizable: true,
            cellRenderer: 'numberFormatterComponent',
            width: 200
        },
        {
            headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.STATUS'),
            field: "transferStatus",
            sortable: true,
            resizable: true,
            width: 200,
            valueGetter: params => this.getStatusName(params.data == undefined ? '' : params.data)
        },
        {
            headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.DATE'),
            field: "requestedDate",
            sortable: true,
            resizable: true,
            cellRenderer: 'dateFormatterComponent',
            width: 200
        }];

        this.created$ = this.store.select(coreState.getMerchantCustomerUpdateSuccess);

        this.createdError$ = this.store.select(coreState.getMerchantCustomerUpdateError);

        this.createdError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.histories$ = this.store.select(coreState.getWithdrawalHistories);

        this.histories$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.histories = items;
        });

        this.historyError$ = this.store.select(coreState.getWithdrawalHistoryError);

        this.historyError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
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
         
        this.store.dispatch(new applicationSettingsActions.GetTransferStatus());

        if (this.withdrawal != null && this.withdrawal != undefined) {
            this.id = this.withdrawal.id;

            this.store.dispatch(new withdrawalActions.GetHistory(this.id));
        }
        else {
            this.route.params.subscribe(params => {

                if (params.id) {
                    this.id = params.id;

                    this.store.dispatch(new withdrawalActions.GetHistory(this.id));

                    if (this.detail$ == undefined) {
                        this.detailLoading$ = this.store.select(coreState.getMerchantCustomerDetailLoading);

                        this.detailLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
                            this.detailLoading = l;
                        });

                        this.detailError$ = this.store.select(coreState.getMerchantDetailError);

                        this.detailError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
                            if (error) {
                                this.openSnackBar(error);
                            }
                        });
                    }
                }
            });
        }
    }

    getStatusName(data): string {
        if (data.transferStatus) {

            if (this.transferStatuses) {
                let item = this.transferStatuses.find(t => t.id == data.transferStatus);

                if (item != null) {
                    return this.translateService.currentLang == 'fa' ? item.descriptionInFarsi : item.descriptionInEnglish;
                }
            }

        }

        return '';
    }

    ngOnDestroy(): void {
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

    onBack() {
        if (this.withdrawal == null || this.withdrawal == undefined) {
            this.router.navigate(['/withdrawals']);
        }
        else {
            this.closed.emit(this.withdrawal);
        }
        
    }

    onGridReady(params) {
        this.gridApi = params.api;
    }
}
