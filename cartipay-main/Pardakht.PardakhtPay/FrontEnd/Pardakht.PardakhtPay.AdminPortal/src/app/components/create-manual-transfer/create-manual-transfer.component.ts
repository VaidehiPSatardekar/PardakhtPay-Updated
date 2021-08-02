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
import { CellClickedEvent } from 'ag-grid-community';
import { AccountService } from '../../core/services/account.service';
import * as bankLoginActions from '../../core/actions/bankLogin';
import { BankLogin, BankAccount } from '../../models/bank-login';
import { Bank } from '../../models/bank';
import { FormGroup, FormControl, Validators, FormBuilder, FormArray } from '@angular/forms';
import { CustomValidators, FormHelper } from '../../helpers/forms/form-helper';
import { GenericHelper } from '../../helpers/generic';
import { ManualTransfer, ManualTransferSearchArgs, ManualTransferStatuses, TransferTypes, TransferPriorities } from '../../models/manual-transfer';
import * as manualTransferActions from '../../core/actions/manualTransfer';
import { CardToCardAccount } from '../../models/card-to-card-account';
import * as cardToCardAccountActions from '../../core/actions/cardToCardAccount'
import { greaterThenZeroValidator } from '../../validators/greater-then-zero-validator';
import { manualTransferAmountValidator } from '../../validators/manual-transfer-amount-validator';
import { DeactivateLoginDialogComponent } from '../deactivate-login-dialog/deactivate-login-dialog.component';
import { TransferTypeEnum } from '../../models/manual-transfer';
import * as permissions from '../../models/permissions';
import { CurrencyPipe, DecimalPipe } from '@angular/common';

@Component({
    selector: 'app-create-manual-transfer',
    templateUrl: './create-manual-transfer.component.html',
    styleUrls: ['./create-manual-transfer.component.scss'],
    animations: fuseAnimations
})
export class CreateManualTransferComponent implements OnInit, OnDestroy {

    MINIMUM_SATNA_LIMIT = 150000000;

    manualTransferForm: FormGroup;
    formSubmit: boolean = false;
    isCreating: boolean;
    createError$: Observable<string>;
    created$: Observable<boolean>;

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

    transferTypes = TransferTypes;

    priorities = TransferPriorities;

    parentGuid: string;
    allowAddManualTransfer: boolean = false;

    getDetailError$: Observable<string>;
    getDetailLoading$: Observable<boolean>;
    getDetailLoading: boolean;
    getDetail$: Observable<ManualTransfer>;

    filter: string = undefined;
    filteredBankAccounts: BankAccount[] = undefined;
    filteredCardToCardAccount: CardToCardAccount[] = undefined;
    cardToCardAccountMultiFilterCtrl: FormControl = new FormControl();

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    // filterText: string = undefined;

    constructor(private store: Store<coreState.State>,
        public snackBar: MatSnackBar,
        private translateService: TranslateService,
        private accountService: AccountService,
        private fb: FormBuilder,
        private route: ActivatedRoute,
        private fuseConfigService: FuseConfigService,
        private router: Router,
        public dialog: MatDialog,
        private currencyPipe: CurrencyPipe,
        private decimalPipe: DecimalPipe) {

        this.parentGuid = this.accountService.getParentAccountId();

        this.allowAddManualTransfer = this.accountService.isUserAuthorizedForTask(permissions.AddManualTransfer);
    }

    ngOnInit() {
        this.store.dispatch(new accountActions.ClearErrors());

        this.created$ = this.store.select(coreState.getManualTransferCreateSuccess);
        this.created$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            console.log(t);
        });
        this.createError$ = this.store.select(coreState.getManualTransferCreateError);

        this.createError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
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

            if (this.manualTransferForm) {
                this.manualTransferForm.get('ownerGuid').setValue(this.getOwnerGuid());
            }
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

        this.cardToCardAccountMultiFilterCtrl.valueChanges
            .pipe(takeUntil(this.destroyed$))
            .subscribe(() => {
                this.filterMulti(0, this.cardToCardAccountMultiFilterCtrl);
            });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;
            if (t && t.tenantDomainPlatformMapGuid) {
                if (this.manualTransferForm) {
                    this.manualTransferForm.get('tenantGuid').setValue(t.tenantDomainPlatformMapGuid);
                }
                this.store.dispatch(new accountActions.GetOwners());
                this.store.dispatch(new transferAccountActions.Search(''));
                this.store.dispatch(new bankLoginActions.Search(false));
                this.store.dispatch(new bankLoginActions.SearchBanks());
                this.store.dispatch(new cardToCardAccountActions.Search(''));
            }
        });

        this.route.params.subscribe(params => {

            const id = params['id'];

            if (id != 0 && id != null && id != undefined) {

                this.getDetail$ = this.store.select(coreState.getManualTransferDetails);
                this.getDetailError$ = this.store.select(coreState.getManualTransferDetailError);
                this.getDetailLoading$ = this.store.select(coreState.getManualTransferDetailLoading);

                this.getDetail$.pipe(takeUntil(this.destroyed$)).subscribe(p => {
                    if (p) {
                        this.createCopyForm(p);
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

                this.store.dispatch(new manualTransferActions.GetDetails(id));
            }
            else {
                this.createForm();
            }
        });
    }

    transformAmount(element) {

        let amount = this.manualTransferForm.get('amount').value.split(',').join('');

        let value = this.decimalPipe.transform(amount, '1.0-0');

        if (value) {
            element.target.value = value;
        }
        else {
            element.target.value = 0;
        }
    }

    createForm(): void {
        this.manualTransferForm = this.fb.group({
            cardToCardAccountIds: new FormControl(undefined, [Validators.required]),
            transferType: new FormControl(undefined, [Validators.required]),
            priority: new FormControl(1, [Validators.required]),
            amount: new FormControl(0, [manualTransferAmountValidator]),
            transferAccountId: new FormControl(undefined, [Validators.required]),
            immediateTransfer: new FormControl(true),
            expectedTransferDate: new FormControl(undefined),
            comment: new FormControl(undefined),
            transferWholeAmount: new FormControl(false),
            tenantGuid: new FormControl(this.selectedTenant == undefined ? undefined : this.selectedTenant.tenantDomainPlatformMapGuid, [Validators.required]),
            ownerGuid: new FormControl(this.getOwnerGuid(), [Validators.required]),
            text: new FormControl(undefined)
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

    createCopyForm(data: ManualTransfer): void {

        data.expectedTransferDate = this.convertUTCDateToLocalDate(new Date(data.expectedTransferDate));

        const disabled = false;

        this.manualTransferForm = this.fb.group({
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
            status: new FormControl(data.status),
            text: new FormControl(undefined)
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

    convertUTCDateToLocalDate(date: Date): Date {
        var newDate = new Date(date.getTime() + date.getTimezoneOffset() * 60 * 1000);

        var offset = date.getTimezoneOffset() / 60;
        var hours = date.getHours();

        newDate.setHours(hours - offset);

        return newDate;
    }

    onSubmit(): void {        
        if (this.manualTransferForm.valid) {
            let form = this.manualTransferForm.value;

            if (form.immediateTransfer == true) {
                const dialogRef = this.dialog.open(DeactivateLoginDialogComponent, {
                    width: '250px',
                    data: 'MANUAL-TRANSFER.GENERAL.IMMEDIATE-APPROVE'
                });

                dialogRef.afterClosed().subscribe(result => {
                    if (result == true) {
                        this.store.dispatch(new manualTransferActions.Create(form));
                        this.isCreating = true;
                        this.created$.pipe(filter(tnCreated => tnCreated == true), take(1))
                            .subscribe(
                                tnCreated => {
                                    this.isCreating = false;
                                    this.router.navigate(['/manualtransfers']);
                                });
                    }
                });
            }
            else {
                this.store.dispatch(new manualTransferActions.Create(form));
                this.isCreating = true;
                this.created$.pipe(filter(tnCreated => tnCreated == true), take(1))
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
        return this.loading || this.bankLoginLoading || this.banksLoading;
    }

    getOwners() {
        if (this.owners == undefined || this.owners == null) {
            return [];
        }

        return this.owners;
    }

    getTransferAccounts() {
        if (this.transferAccounts == undefined || this.transferAccounts == null || this.manualTransferForm == undefined) {
            return [];
        }

        let ownerGuid = this.manualTransferForm.get('ownerGuid').value;

        let text = this.manualTransferForm.get('text').value;

        var accounts = this.transferAccounts.filter(t => t.ownerGuid == ownerGuid);

        if (text) {
            text = text.toLowerCase();

            accounts = this
                .transferAccounts
                .filter(t =>
                    (t.accountHolderFirstName && t.accountHolderFirstName.toLowerCase().includes(text))
                    || (t.accountHolderLastName && t.accountHolderLastName.toLowerCase().includes(text))
                    || (t.accountNo && t.accountNo.toLowerCase().includes(text))
                    || (t.friendlyName && t.friendlyName.toLowerCase().includes(text))
                    || (t.iban && t.iban.toLowerCase().includes(text)));
        }

        return accounts;
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
 
    filterBankAccounts(text) {
        this.filter = text;

        if (this.cardToCardAccounts) {
            this.filteredBankAccounts = this.cardToCardAccounts.filter(t => this.getLoginName(t.loginGuid).toLowerCase().includes(text) || t.accountNo.toLowerCase().includes(text));
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

        //return '6432ace2-37e6-4b36-b63d-d333fba89125';
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

    // filterPayees(text){
    //     this.filterText = text;

    //     if(this.filterText){
    //         this.filterText = this.filterText.toLowerCase();
    //     }
    // }
}
