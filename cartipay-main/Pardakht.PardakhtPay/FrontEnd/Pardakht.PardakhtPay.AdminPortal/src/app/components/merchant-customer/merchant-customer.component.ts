import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { MerchantCustomer, MerchantRelation, MerchantCustomerCardNumbers } from '../../models/merchant-customer';
import { fuseAnimations } from '../../core/animations';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { distinctUntilChanged, debounceTime, take, filter } from 'rxjs/operators';
import { UserSegmentGroup } from '../../models/user-segment-group';
import * as merchantCustomer from '../../core/actions/merchantCustomer';
import { Observable, ReplaySubject } from 'rxjs';
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import { CustomValidators, FormHelper } from '../../helpers/forms/form-helper';
import { TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { takeUntil } from 'rxjs/operators';
import { ActivatedRoute, Router } from '@angular/router';
import { Merchant } from '../../models/merchant-model';
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { BooleanInverseFormatterComponent } from '../formatters/booleaninverseformatter';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { DeleteButtonFormatterComponent } from '../formatters/deletebuttonformatter';

@Component({
    selector: 'app-merchant-customer',
    templateUrl: './merchant-customer.component.html',
    styleUrls: ['./merchant-customer.component.scss'],
    animations: fuseAnimations
})
export class MerchantCustomerComponent implements OnInit, OnDestroy {

    @Input() merchantCustomer: MerchantCustomer;
    @Input() userSegmentGroups: UserSegmentGroup[];

    @Output() closed: EventEmitter<any> = new EventEmitter<any>();

    customerForm: FormGroup;
    isCreating: boolean;

    created$: Observable<boolean>;
    createdError$: Observable<string>;

    detailLoading$: Observable<boolean>;
    detailLoading: boolean;

    detail$: Observable<MerchantCustomer>;
    detailError$: Observable<string>;

    userSegmentGroups$: Observable<UserSegmentGroup[]>;

    phoneNumberRelateds$: Observable<MerchantRelation[]>;
    phoneNumberRelateds: MerchantRelation[];
    phoneNumberRelatedError$: Observable<string>;

    cardNumbers$: Observable<MerchantCustomerCardNumbers[]>;
    cardNumbers: MerchantCustomerCardNumbers[];
    cardsError$: Observable<string>;

    customerDetails: any[] = [];

    id: number;

    columnDefs;
    enableRtl: boolean = false;
    frameworkComponents;

    gridApi;

    phoneNumberColumnDefs;
    phoneNumberGridApi;

    cardColumnDefs;
    cardsGridApi;

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

        this.fuseConfigService.config.pipe(takeUntil(this.destroyed$)).subscribe(config => {

            this.enableRtl = config.locale != null && config.locale != undefined && config.locale.direction == 'rtl';
        });

        this.frameworkComponents = {
            booleanFormatterComponent: BooleanFormatterComponent,
            booleanInverseFormatterComponent: BooleanInverseFormatterComponent,
            numberFormatterComponent: NumberFormatterComponent,
            deleteButtonFormatterComponent: DeleteButtonFormatterComponent
        };

        this.columnDefs = [{
            headerName: '',
            field: "key",
            sortable: true,
            resizable: true,
            width: 150
        },
        {
            headerName: '',
            field: "value",
            sortable: true,
            resizable: true,
            width: 200
        }];

        this.phoneNumberColumnDefs = [{
            headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.WEBSITE-NAME'),
            field: "websiteName",
            sortable: true,
            resizable: true,
            width: 150
        },
        {
            headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.USER-ID'),
            field: "userId",
            sortable: true,
            resizable: true,
            width: 200,
            cellRenderer: params => this.userHeader(params)
        },
        {
            headerName: this.translateService.instant('TRANSACTION.LIST-COLUMNS.TITLE'),
            field: "merchantTitle",
            sortable: true,
            resizable: true,
            width: 200
        },
        {
            headerName: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.RELATION-TYPE'),
            field: "relationKey",
            sortable: true,
            resizable: true,
            width: 200,
            cellRenderer: params => this.getRelationDescription(params)
        },
        {
            headerName: this.translateService.instant('GENERAL.VALUE'),
            field: "value",
            sortable: true,
            resizable: true,
            width: 200
        }];

        this.cardColumnDefs = [{
            headerName: this.translateService.instant('TRANSACTION.LIST-COLUMNS.CUSTOMER_CARD_NUMBER'),
            field: "cardNumber",
            sortable: true,
            resizable: true,
            width: 250
        },
        {
            headerName: this.translateService.instant('DASHBOARD.CARD-HOLDER-NAME'),
            field: "cardHolderName",
            sortable: true,
            resizable: true,
            width: 250
        },
        {
            headerName: this.translateService.instant('GENERAL.COUNT'),
            field: "count",
            sortable: true,
            resizable: true,
            width: 200
        }];


        this.phoneNumberRelatedError$ = this.store.select(coreState.getPhoneNumberRelatedCustomerError);

        this.phoneNumberRelatedError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.phoneNumberRelateds$ = this.store.select(coreState.getPhoneNumberRelatedCustomers);

        this.phoneNumberRelateds$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            setTimeout(() => {
                this.phoneNumberRelateds = items;
            });
        });

        this.cardNumbers$ = this.store.select(coreState.getMerchantCustomerCards);

        this.cardNumbers$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.cardNumbers = items;
        });

        this.cardsError$ = this.store.select(coreState.getMerchantCustomerCardsError);

        this.cardsError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.created$ = this.store.select(coreState.getMerchantCustomerUpdateSuccess);

        this.createdError$ = this.store.select(coreState.getMerchantCustomerUpdateError);

        this.createdError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.route.params.subscribe(params => {

            if (params.id) {
                this.id = params.id;

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

                    this.detail$ = this.store.select(coreState.getMerchantCustomerDetail);

                    this.detail$.pipe(takeUntil(this.destroyed$)).subscribe(detail => {
                        this.merchantCustomer = detail;

                        if (this.merchantCustomer) {
                            this.createForm();
                        }
                    });

                    this.userSegmentGroups$ = this.store.select(coreState.getAllUserSegmentGroups);

                    this.userSegmentGroups$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
                        if (items != undefined) {
                            this.userSegmentGroups = items;
                        }
                    });
                }

                this.loadMerchantCustomer();
            }
        });

        if (this.merchantCustomer) {
            this.createForm();
        }
    }

    createForm(): void {
        if (this.merchantCustomer) {
            this.customerForm = this.fb.group({
                id: new FormControl(this.merchantCustomer.id),
                websiteName: new FormControl({ value: this.merchantCustomer.websiteName, disabled: true }),
                userId: new FormControl({ value: this.merchantCustomer.userId, disabled: true }),
                totalTransactionCount: new FormControl({ value: this.merchantCustomer.totalTransactionCount, disabled: true }),
                totalCompletedTransactionCount: new FormControl({ value: this.merchantCustomer.totalCompletedTransactionCount, disabled: true }),
                totalDepositAmount: new FormControl({ value: this.merchantCustomer.totalDepositAmount, disabled: true }),
                totalWithdrawalCount: new FormControl({ value: this.merchantCustomer.totalWithdrawalCount, disabled: true }),
                totalCompletedWithdrawalCount: new FormControl({ value: this.merchantCustomer.totalCompletedWithdrawalCount, disabled: true }),
                totalWithdrawalAmount: new FormControl({ value: this.merchantCustomer.totalWithdrawalAmount, disabled: true }),
                registerDate: new FormControl({ value: this.merchantCustomer.registerDate, disabled: true }),
                userSegmentGroupId: new FormControl(this.merchantCustomer.userSegmentGroupId)
            });

            this.customerDetails = [];

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.WEBSITE-NAME'),
                value: this.merchantCustomer.websiteName
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.USER-ID'),
                value: this.merchantCustomer.userId
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-TRANSACTION-COUNT'),
                value: this.merchantCustomer.totalTransactionCount
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-COMPLETED-TRANSACTION-COUNT'),
                value: this.merchantCustomer.totalCompletedTransactionCount
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-DEPOSIT-AMOUNT'),
                value: this.merchantCustomer.totalDepositAmount
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-WITHDRAWAL-COUNT'),
                value: this.merchantCustomer.totalWithdrawalCount
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-COMPLETED-WITHDRAWAL-COUNT'),
                value: this.merchantCustomer.totalCompletedWithdrawalCount
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-WITHDRAWAL-AMOUNT'),
                value: this.merchantCustomer.totalWithdrawalAmount
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.REGISTER-DATE'),
                value: this.merchantCustomer.registerDate
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-DEPOSIT'),
                value: this.merchantCustomer.totalDeposit
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-WITHDRAW'),
                value: this.merchantCustomer.totalWithdraw
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.DEPOSIT-NUMBER'),
                value: this.merchantCustomer.depositNumber
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.WITHDRAW-NUMBER'),
                value: this.merchantCustomer.withdrawNumber
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.ACTIVITY-SCORE'),
                value: this.merchantCustomer.activityScore
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.GROUP-NAME'),
                value: this.merchantCustomer.groupName
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.LAST-ACTIVITY-DATE'),
                value: this.merchantCustomer.lastActivity
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-SPORTBOOK-AMOUNT'),
                value: this.merchantCustomer.userTotalSportbook
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-SPORTBOOK-COUNT'),
                value: this.merchantCustomer.userSportbookNumber
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-CASINO-AMOUNT'),
                value: this.merchantCustomer.userTotalCasino
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.TOTAL-CASINO-COUNT'),
                value: this.merchantCustomer.userCasinoNumber
            });

            this.customerDetails.push({
                key: this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.PHONE-NUMBER'),
                value: this.merchantCustomer.phoneNumberIsBlocked ? this.merchantCustomer.phoneNumber + ' Blocked' : this.merchantCustomer.phoneNumber
            });

            this.store.dispatch(new merchantCustomer.GetPhoneNumberRelatedCustomers(this.merchantCustomer.id));
            this.store.dispatch(new merchantCustomer.GetCustomerCardNumbers(this.merchantCustomer.id));
        }
    }

    loadMerchantCustomer() {
        if (!this.id) {
            return;
        }

        this.store.dispatch(new merchantCustomer.GetDetails(this.id));
    }

    userHeader(params) {
        let content = '';

        if (params == undefined || params.data == undefined) {
            return '';
        }

        if (params.data.id != null) {
            return '<a href="/merchantcustomer/' + params.data.id + '" target="_blank">' + params.data.userId + '</>';
        }
        else {
            return params.data.userId;
        }
    }

    getRelationDescription(params): string {
        if (params == undefined || params.data == undefined) {
            return '';
        }

        return this.translateService.instant('MERCHANT-CUSTOMER.LIST-COLUMNS.' + params.data.relationKey);
    }

    onSubmit(): void {

        this.isCreating = true;

        if (this.customerForm.valid) {
            let form = this.customerForm.value;
            this.store.dispatch(new merchantCustomer.Edit(this.merchantCustomer.id, form));

            this.created$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;
                        this.merchantCustomer.userSegmentGroupId = this.customerForm.get('userSegmentGroupId').value;
                        this.onBack();
                    });

        }
        else {
            FormHelper.validateFormGroup(this.customerForm);
        }
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

    getUserSegmentGroups() {
        if (this.userSegmentGroups == undefined) {
            return [];
        }

        return this.userSegmentGroups.filter(t => t.ownerGuid == this.merchantCustomer.ownerGuid && t.isDefault == false);
    }

    onBack() {
        if (this.id) {
            this.router.navigate(['/merchantcustomers']);
        }
        else {
            this.closed.emit(this.merchantCustomer);
        }
    }

    onGridReady(params) {
        this.gridApi = params.api;
    }

    onPhoneNumberGridReady(params) {
        this.phoneNumberGridApi = params.api;
    }

    onCardGridReady(params) {
        this.cardsGridApi = params.api;
    }
}
