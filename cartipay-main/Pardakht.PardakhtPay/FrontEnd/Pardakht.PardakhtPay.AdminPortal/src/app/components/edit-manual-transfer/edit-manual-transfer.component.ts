import { Component, OnInit, Output, EventEmitter, OnDestroy } from '@angular/core';
import * as coreState from '../../core';
import { Store } from '@ngrx/store';
import { Observable, ReplaySubject } from 'rxjs';
import { TransferAccount } from '../../models/transfer-account';
import * as transferAccountActions from '../../core/actions/transferAccount';
import { fuseAnimations } from '../../core/animations';
import { Router, ActivatedRoute } from '@angular/router';
import { MatSnackBar, MatDialog } from '@angular/material';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil, debounceTime, distinctUntilChanged, filter, take } from 'rxjs/operators';
import { Tenant } from '../../models/tenant';
import { Owner } from '../../models/account.model';
import * as accountActions from '../../core/actions/account';
import { FuseConfigService } from '../../../@fuse/services/config.service';
import { DeleteButtonFormatterComponent } from '../formatters/deletebuttonformatter';
import { AccountService } from '../../core/services/account.service';
import * as bankLoginActions from '../../core/actions/bankLogin';
import { BankLogin } from '../../models/bank-login';
import { Bank } from '../../models/bank';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { FormHelper } from '../../helpers/forms/form-helper';
import { GenericHelper } from '../../helpers/generic';
import { ManualTransfer, ManualTransferSearchArgs, TransferTypes, TransferPriorities, ManualTransferDetail } from '../../models/manual-transfer';
import * as manualTransferActions from '../../core/actions/manualTransfer';
import { CardToCardAccount } from '../../models/card-to-card-account';
import * as cardToCardAccountActions from '../../core/actions/cardToCardAccount'
import { BooleanFormatterComponent } from '../formatters/booleanformatter';
import { BooleanInverseFormatterComponent } from '../formatters/booleaninverseformatter';
import { NumberFormatterComponent } from '../formatters/numberformatter';
import { DateFormatterComponent } from '../formatters/dateformatter';
import { manualTransferAmountValidator } from '../../validators/manual-transfer-amount-validator';
import { DownloadManualTransferReceiptFormatterComponent } from '../formatters/downloadManualTransferReceiptFormatter';
import { WithdrawalReceipt } from '../../models/withdrawal';
import { DeactivateLoginDialogComponent } from '../deactivate-login-dialog/deactivate-login-dialog.component';
import { IconButtonFormatterComponent } from '../formatters/iconbuttonformatter';
import { TransferStatusEnum } from '../../models/types';
import { TransferTypeEnum } from '../../models/manual-transfer';
import * as permissions from '../../models/permissions';
import { TransferStatusDescription } from '../../models/application-settings';
import * as applicationSettingsActions from '../../core/actions/applicationSettings';

@Component({
  selector: 'app-edit-manual-transfer',
  templateUrl: './edit-manual-transfer.component.html',
    styleUrls: ['./edit-manual-transfer.component.scss'],
    animations: fuseAnimations
})
export class EditManualTransferComponent implements OnInit, OnDestroy {

    MINIMUM_SATNA_LIMIT = 150000000;

    manualTransferForm: FormGroup;
    formSubmit: boolean = false;
    isCreating: boolean;
    updateError$: Observable<string>;
    updated$: Observable<boolean>;
    details: ManualTransferDetail[] = undefined;
    filteredCardToCardAccount: CardToCardAccount[] = undefined;
    cardToCardAccountMultiFilterCtrl: FormControl = new FormControl();

    cancelDetailError$: Observable<string>;
    retryDetailError$: Observable<string>;
    completedDetailError$: Observable<string>;

    cancelDetailSuccess$: Observable<boolean>;
    retryDetailSuccess$: Observable<boolean>;
    completedDetailSuccess$: Observable<boolean>;

    getDetailError$: Observable<string>;
    getDetailLoading$: Observable<boolean>;
    getDetailLoading: boolean;
    getDetail$: Observable<ManualTransfer>;

    transferAccounts$: Observable<TransferAccount[]>;
    transferAccounts: TransferAccount[];
    searchError$: Observable<string>;
    selected = [];
    loading$: Observable<boolean>;
    loading: boolean;

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

    bankLogins$: Observable<BankLogin[]>;
    bankLogins: BankLogin[] = [];
    bankLoginError$: Observable<string>;
    bankLoginLoading$: Observable<boolean>;
    bankLoginLoading: boolean;

    cardToCardAccounts$: Observable<CardToCardAccount[]>;
    cardToCardAccounts: CardToCardAccount[];
    cardToCardSearchError$: Observable<string>;

    banks$: Observable<Bank[]>;
    banks: Bank[];
    banksLoading$: Observable<boolean>;
    banksLoading: boolean;
    banksError$: Observable<string>;

    receipt$: Observable<WithdrawalReceipt>;

    receiptError$: Observable<string>;

    transferTypes = TransferTypes;

    transferStatuses$: Observable<TransferStatusDescription[]>;
    transferStatuses: TransferStatusDescription[];
    transferStatusError$: Observable<string>;

    priorities = TransferPriorities;

    parentGuid: string;

    columnDefs;
    enableRtl: boolean = false;
    frameworkComponents;

    gridApi;

    id: number;

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    allowAddManualTransfer: boolean = false;

    constructor(private store: Store<coreState.State>,
        public snackBar: MatSnackBar,
        private translateService: TranslateService,
        private accountService: AccountService,
        private fb: FormBuilder,
        private route: ActivatedRoute,
        private fuseConfigService: FuseConfigService,
        private router: Router,
        public dialog: MatDialog) {

        this.parentGuid = this.accountService.getParentAccountId();

        this.allowAddManualTransfer = this.accountService.isUserAuthorizedForTask(permissions.AddManualTransfer);
    }

    ngOnInit() {

        this.fuseConfigService.config.pipe(takeUntil(this.destroyed$)).subscribe(config => {

            this.enableRtl = config.locale != null && config.locale != undefined && config.locale.direction == 'rtl';
        });

        this.frameworkComponents = {
            booleanFormatterComponent: BooleanFormatterComponent,
            booleanInverseFormatterComponent: BooleanInverseFormatterComponent,
            numberFormatterComponent: NumberFormatterComponent,
            deleteButtonFormatterComponent: DeleteButtonFormatterComponent,
            dateFormatterComponent: DateFormatterComponent,
            downloadReceiptCompoent: DownloadManualTransferReceiptFormatterComponent,
            iconButtonFormatterComponent: IconButtonFormatterComponent
        };

        this.columnDefs = [
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.AMOUNT'),
                field: "amount",
                sortable: true,
                resizable: true,
                cellRenderer: 'numberFormatterComponent',
                width: 200
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.STATUS'),
                field: "transferStatus",
                sortable: true,
                resizable: true,
                width: 200,
                valueGetter: params => this.getStatusName(params.data == undefined ? undefined : params.data.transferStatus)
            },
            {
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.DATE'),
                field: "transferRequestDate",
                sortable: true,
                resizable: true,
                cellRenderer: 'dateFormatterComponent',
                width: 150
            },
            {
                headerName: this.translateService.instant('AUTO-TRANSFER.LIST-COLUMNS.TRANSFERRED_DATE'),
                field: "transferDate",
                cellRenderer: 'dateFormatterComponent',
                sortable: true,
                resizable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('WITHDRAWAL.LIST-COLUMNS.TRANSFER_NOTES'),
                field: "transferNotes",
                resizable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('WITHDRAWAL.LIST-COLUMNS.TRACKING-NUMBER'),
                field: "trackingNumber",
                resizable: true,
                sortable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('WITHDRAWAL.LIST-COLUMNS.DOWNLOAD_RECEIPT'),
                field: "trackingNumber",
                cellRenderer: 'downloadReceiptCompoent',
                resizable: true,
                sortable: true,
                width: 150
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.CANCEL'),
                field: "id",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.cancelDetail.bind(this),
                    icon: 'cancel',
                    iconClass: 'danger',
                    title: 'MANUAL-TRANSFER.LIST-COLUMNS.CANCEL',
                    allow: this.allowAddManualTransfer,
                    allowed: this.cancelationAllowed.bind(this)
                },
                hide: !this.allowAddManualTransfer
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.RETRY'),
                field: "id",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.retry.bind(this),
                    icon: 'refresh',
                    iconClass: '',
                    title: 'MANUAL-LIST-COLUMNS.GENERAL.RETRY',
                    allow: this.allowAddManualTransfer,
                    allowed: this.retryAllowed.bind(this)
                },
                hide: !this.allowAddManualTransfer
            },
            {
                headerName: this.translateService.instant('MANUAL-TRANSFER.LIST-COLUMNS.COMPLETED'),
                field: "id",
                width: 110,
                resizable: true,
                sortable: true,
                cellRenderer: 'iconButtonFormatterComponent',
                cellRendererParams: {
                    onButtonClick: this.setAsCompleted.bind(this),
                    icon: 'check_circle',
                    iconClass: 'success',
                    title: 'MANUAL-TRANSFER.LIST-COLUMNS.COMPLETED',
                    allow: this.allowAddManualTransfer,
                    allowed: this.setAsCompletedAllowed.bind(this)
                },
                hide: !this.allowAddManualTransfer
            }];

        this.store.dispatch(new accountActions.ClearErrors());

        this.getDetail$ = this.store.select(coreState.getManualTransferDetails);
        this.getDetailError$ = this.store.select(coreState.getManualTransferDetailError);
        this.getDetailLoading$ = this.store.select(coreState.getManualTransferDetailLoading);

        this.route.params.subscribe(params => {

            this.id = params['id'];

            if (this.id != 0 && this.id != null && this.id != undefined) {
                this.store.dispatch(new manualTransferActions.GetDetails(this.id));

                this.getDetail$.pipe(takeUntil(this.destroyed$)).subscribe(p => {
                    if (p) {
                        this.createForm(p);
                    }
                });
            }
        });

        this.getDetailError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.getDetailLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.getDetailLoading = l;
        });


        this.updated$ = this.store.select(coreState.getManualTransferEditSuccess);
        this.updated$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
        });
        this.updateError$ = this.store.select(coreState.getManualTransferEditError);

        this.updateError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.isCreating = false;
                this.openSnackBar(error);
            }
        });

        this.cancelDetailSuccess$ = this.store.select(coreState.getManualTransferCancelDetailSuccess);

        this.cancelDetailSuccess$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            if (t) {
                this.isCreating = false;
            }
        });

        this.retryDetailSuccess$ = this.store.select(coreState.getManualTransferRetryDetailSuccess);

        this.retryDetailSuccess$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            if (t) {
                this.isCreating = false;
            }
        });

        this.completedDetailSuccess$ = this.store.select(coreState.getManualTransferSetAsCompletedDetailSuccess);

        this.completedDetailSuccess$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            if (t) {
                this.isCreating = false;
            }
        });

        this.cancelDetailError$ = this.store.select(coreState.getManualTransferCancelDetailError);

        this.cancelDetailError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.isCreating = false;
                this.openSnackBar(error);
            }
        });

        this.retryDetailError$ = this.store.select(coreState.getManualTransferRetryDetailError);

        this.retryDetailError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.isCreating = false;
                this.openSnackBar(error);
            }
        });

        this.completedDetailError$ = this.store.select(coreState.getManualTransferSetAsCompletedDetailError);

        this.completedDetailError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.isCreating = false;
                this.openSnackBar(error);
            }
        });

        this.transferAccounts$ = this.store.select(coreState.getTransferAccountSearchResults);
        this.transferAccounts$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.transferAccounts = items;
        });

        this.loading$ = this.store.select(coreState.getTransferAccountLoading);
        this.searchError$ = this.store.select(coreState.getTransferAccountSearchError); this.tenantGuid$ = this.store.select(coreState.getAccountTenantGuid);

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

        this.bankLoginError$ = this.store.select(coreState.getBankLoginSearchError);
        this.bankLogins$ = this.store.select(coreState.getBankLoginSearchResults);
        this.bankLoginLoading$ = this.store.select(coreState.getBankLoginLoading);
        this.bankLogins$.pipe(takeUntil(this.destroyed$)).subscribe(logins => {
            this.bankLogins = logins;
        });
        this.bankLoginError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });
        this.bankLoginLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.bankLoginLoading = l;
        });

        this.store.dispatch(new cardToCardAccountActions.ClearErrors());
        this.cardToCardAccounts$ = this.store.select(coreState.getCardToCardAccountSearchResults);

        this.cardToCardAccounts$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.cardToCardAccounts = items;
            this.filteredCardToCardAccount = this.cardToCardAccounts;
        });

        this.cardToCardSearchError$ = this.store.select(coreState.getCardToCardAccountSearchError);

        this.cardToCardSearchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.banks$ = this.store.select(coreState.getBanksSearchResult);
        this.banksLoading$ = this.store.select(coreState.getBanksLoading);
        this.banksError$ = this.store.select(coreState.getBanksSearchError);

        this.banks$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.banks = items;
        });

        this.banksLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.banksLoading = l;
        });

        this.banksError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
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

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;
            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new applicationSettingsActions.GetTransferStatus());
                this.store.dispatch(new accountActions.GetOwners());
                this.store.dispatch(new transferAccountActions.Search(''));
                this.store.dispatch(new bankLoginActions.Search(false));
                this.store.dispatch(new bankLoginActions.SearchBanks());
                this.store.dispatch(new cardToCardAccountActions.Search(''));
            }
        });

        this.receipt$ = this.store.select(coreState.getManualTransferReceipt);

        this.receipt$.pipe(takeUntil(this.destroyed$)).subscribe(item => {
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

        this.receiptError$ = this.store.select(coreState.getManualTransferReceiptError);

        this.receiptError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.cardToCardAccountMultiFilterCtrl.valueChanges
            .pipe(takeUntil(this.destroyed$))
            .subscribe(() => {
                this.filterMulti(0, this.cardToCardAccountMultiFilterCtrl);
            });
    }

    protected filterMulti(type: number, ctrl: FormControl) {
        // get the search keyword
        let search = ctrl.value;
        switch (type) {
            case 0:
                if (!this.cardToCardAccounts || !this.filteredCardToCardAccount) {
                    return;
                }
                if (!search) {
                    this.filteredCardToCardAccount = this.cardToCardAccounts;
                    return;
                } else {
                    this.filteredCardToCardAccount = this.cardToCardAccounts.filter(item => item.accountNo.toLowerCase().includes(search.toLowerCase()));
                }
                break;


        }
    }

    createForm(data: ManualTransfer): void {
        var disabled = data.status != 1;

        if (this.details) {
            this.details = data.details;
            this.gridApi.setRowData(this.details);
        }
        else {
            this.details = data.details;
        }

        data.expectedTransferDate = this.convertUTCDateToLocalDate(new Date(data.expectedTransferDate));

        this.manualTransferForm = this.fb.group({
            id: data.id,
            cardToCardAccountIds: new FormControl({ value: data.cardToCardAccountIds, disabled: disabled }, [Validators.required]),
            transferType: new FormControl({ value: data.transferType, disabled: disabled }, [Validators.required]),
            priority: new FormControl({ value: data.priority, disabled: disabled }, [Validators.required]),
            amount: new FormControl({ value: data.amount, disabled: disabled }, [manualTransferAmountValidator]),
            transferAccountId: new FormControl({ value: data.transferAccountId, disabled: disabled }, [Validators.required]),
            immediateTransfer: new FormControl({ value: data.immediateTransfer, disabled: disabled }),
            expectedTransferDate: new FormControl({ value: data.expectedTransferDate, disabled: disabled }),
            comment: new FormControl({ value: data.comment, disabled: disabled }),
            transferWholeAmount: new FormControl({ value: data.transferWholeAmount, disabled: disabled }),
            tenantGuid: new FormControl(data.tenantGuid, [Validators.required]),
            ownerGuid: new FormControl(data.ownerGuid),
            status: new FormControl(data.status)
        });

        this.manualTransferForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: ManualTransfer = new ManualTransfer({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.manualTransferForm.value, emptyForm);
            });
    }

    onSubmit(): void {

        this.formSubmit = false;

        if (this.manualTransferForm.valid) {
            let form = this.manualTransferForm.value;

            if (form.immediateTransfer == true) {
                const dialogRef = this.dialog.open(DeactivateLoginDialogComponent, {
                    width: '250px',
                    data: 'MANUAL-TRANSFER.GENERAL.IMMEDIATE-APPROVE'
                });

                dialogRef.afterClosed().subscribe(result => {
                    if (result == true) {
                        this.formSubmit = true;
                        this.store.dispatch(new manualTransferActions.Edit(this.id, form));
                        this.isCreating = true;
                        this.updated$.pipe(filter(tnCreated => tnCreated == true), take(1))
                            .subscribe(
                                tnCreated => {
                                    this.isCreating = false;
                                    this.router.navigate(['/manualtransfers']);
                                });
                    }
                });
            }
            else {
                this.formSubmit = true;
                this.store.dispatch(new manualTransferActions.Edit(this.id, form));
                this.isCreating = true;
                this.updated$.pipe(filter(tnCreated => tnCreated == true), take(1))
                    .subscribe(
                        tnCreated => {
                            this.isCreating = false;
                            this.router.navigate(['/manualtransfers']);
                        });
            }

        }
        else {
            FormHelper.validateFormGroup(this.manualTransferForm);
        }
    }

    isLoading(): boolean {
        return this.loading || this.bankLoginLoading || this.banksLoading || this.isCreating;
    }

    getOwners() {
        if (this.owners == undefined || this.owners == null) {
            return [];
        }

        return this.owners;
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

    getTransferAccounts() {
        if (this.transferAccounts == undefined || this.transferAccounts == null || this.manualTransferForm == undefined) {
            return [];
        }

        var ownerGuid = this.manualTransferForm.get('ownerGuid').value;

        return this.transferAccounts.filter(t => t.ownerGuid == ownerGuid);
    }

    getCardToCardAccounts() {
        if (this.cardToCardAccounts == undefined || this.cardToCardAccounts == null || this.manualTransferForm == undefined) {
            return [];
        }

        var ownerGuid = this.manualTransferForm.get('ownerGuid').value;

        return this.cardToCardAccounts.filter(t => t.ownerGuid == ownerGuid);
    }

    getLoginName(loginGuid: string): string {
        if (this.bankLogins) {
            var login = this.bankLogins.find(t => t.loginGuid == loginGuid);

            if (login != null) {
                return login.friendlyName;
            }
        }
        return '';
    }

    checkIfTransferTypeAvailable(type: number): boolean {

        if (this.manualTransferForm == undefined || this.cardToCardAccounts == undefined || this.bankLogins == undefined || this.banks == undefined || this.transferAccounts == undefined) {
            return false;
        }

        var transferAccountId = this.manualTransferForm.get('transferAccountId').value;

        var transferAccount = this.transferAccounts.find(t => t.id == transferAccountId);

        if (transferAccount == null) {
            return false;
        }

        var iban = transferAccount.iban;

        if (GenericHelper.isNullOrUndefinedOrEmpty(iban)) {
            return false;
        }

        var accountId = this.manualTransferForm.get('cardToCardAccountIds').value;

        var account = this.cardToCardAccounts.find(t => t.id == accountId);

        if (account == null) {
            return false;
        }

        var login = this.bankLogins.find(t => t.loginGuid == account.loginGuid);

        if (login == null) {
            return false;
        }

        var bank = this.banks.find(t => t.id == login.bankId);

        if (bank == null) {
            return false;
        }

        var bankCode = GenericHelper.extractBankCode(iban);

        var same = bankCode == bank.bankCode;

        if (same) {
            return type == TransferTypeEnum.Normal;
        }
        else if (type == TransferTypeEnum.Paya) {
            return true;
        }
        else if (type == TransferTypeEnum.Satna) {
            return this.manualTransferForm.get('amount').value >= this.MINIMUM_SATNA_LIMIT || this.manualTransferForm.get('transferWholeAmount').value == true;
        }

        return false;
    }

    checkTransferType() {
        if (this.manualTransferForm == undefined || this.cardToCardAccounts == undefined || this.bankLogins == undefined || this.banks == undefined || this.transferAccounts == undefined) {
            return;
        }

        var type = this.manualTransferForm.get('transferType').value;

        var available = this.checkIfTransferTypeAvailable(type);

        if (!available) {
            if (type == TransferTypeEnum.Normal) {
                this.manualTransferForm.get('transferType').setValue(2);
            }
            else if (type == TransferTypeEnum.Satna) {
                this.manualTransferForm.get('transferType').setValue(2);
            }
            else {
                this.manualTransferForm.get('transferType').setValue(1);
            }
        }
    }

    checkValidation() {
        this.manualTransferForm.controls['amount'].updateValueAndValidity();
        this.checkTransferType();
    }

    cancelDetail(id: number) {
        if (id > 0) {
            const dialogRef = this.dialog.open(DeactivateLoginDialogComponent, {
                width: '250px',
                data: 'MANUAL-TRANSFER.GENERAL.CANCELATION-APPROVE'
            });

            dialogRef.afterClosed().subscribe(result => {
                if (result == true) {
                    this.isCreating = true;
                    this.store.dispatch(new manualTransferActions.CancelDetail(id));
                }
            });
        }
    }

    cancelationAllowed(params): boolean {
        var result = this.allowAddManualTransfer && params != undefined && params.data != undefined && (params.data.transferStatus != TransferStatusEnum.Complete && params.data.transferStatus != TransferStatusEnum.AwaitingConfirmation && params.data.transferStatus != TransferStatusEnum.CompletedWithNoReceipt && params.data.transferStatus != TransferStatusEnum.Cancelled);
        return result;
    }

    retry(id: number) {
        if (id > 0) {
            const dialogRef = this.dialog.open(DeactivateLoginDialogComponent, {
                width: '250px',
                data: 'MANUAL-TRANSFER.GENERAL.RETRY-APPROVE'
            });

            dialogRef.afterClosed().subscribe(result => {
                if (result == true) {
                    this.isCreating = true;
                    this.store.dispatch(new manualTransferActions.RetryDetail(id));
                }
            });
        }
    }

    retryAllowed(params): boolean {
        var result = this.allowAddManualTransfer && params != undefined && params.data != undefined && (params.data.transferStatus == TransferStatusEnum.BankSubmitted);
        return result;
    }

    setAsCompleted(id: number) {
        if (id > 0) {
            const dialogRef = this.dialog.open(DeactivateLoginDialogComponent, {
                width: '250px',
                data: 'MANUAL-TRANSFER.GENERAL.COMPLETE-APPROVE'
            });

            dialogRef.afterClosed().subscribe(result => {
                if (result == true) {
                    this.isCreating = true;
                    this.store.dispatch(new manualTransferActions.SetAsCompletedDetail(id));
                }
            });
        }
    }

    setAsCompletedAllowed(params): boolean {
        var result = this.allowAddManualTransfer && params != undefined && params.data != undefined && (params.data.transferStatus == TransferStatusEnum.BankSubmitted);
        return result;
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
        this.store.dispatch(new transferAccountActions.ClearErrors());
        this.store.dispatch(new accountActions.ClearErrors());
        this.store.dispatch(new bankLoginActions.ClearAll());
        this.store.dispatch(new cardToCardAccountActions.ClearErrors());
        this.destroyed$.next(true);
        this.destroyed$.complete();
    }

    getOwnerGuid() {
        if (this.parentGuid == undefined || this.parentGuid == '' || this.parentGuid == null) {
            return this.accountGuid;
        }

        return this.parentGuid;
    }

    convertUTCDateToLocalDate(date: Date): Date {
        var newDate = new Date(date.getTime() + date.getTimezoneOffset() * 60 * 1000);

        var offset = date.getTimezoneOffset() / 60;
        var hours = date.getHours();

        newDate.setHours(hours - offset);

        return newDate;
    }

    selectCardToCardAccounts(type: string) {
        if (type === 'all') {
            if (this.manualTransferForm.controls.cardToCardAccountIds.value != null) {
                if (this.manualTransferForm.controls.cardToCardAccountIds.value.length !== this.filteredCardToCardAccount.length) {
                    this.manualTransferForm.controls.cardToCardAccountIds.patchValue([...this.filteredCardToCardAccount.map(item => item.id)]);
                }
            }
        } else {
            this.manualTransferForm.controls.cardToCardAccountIds.patchValue([]);
        }
    }

    onGridReady(params) {
        this.gridApi = params.api;
    }
}
