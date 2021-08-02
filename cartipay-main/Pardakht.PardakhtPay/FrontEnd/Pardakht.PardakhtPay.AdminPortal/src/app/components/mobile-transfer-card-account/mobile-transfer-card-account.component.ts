import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { FormGroup, FormControl, Validators, FormBuilder, FormArray } from '@angular/forms';
import { Store } from '@ngrx/store';
import { DateAdapter, MatSnackBar } from '@angular/material';
import * as coreState from '../../core';
import { GenericHelper } from '../../helpers/generic';
import { CustomValidators, FormHelper } from '../../helpers/forms/form-helper';
import { debounceTime, distinctUntilChanged, take } from 'rxjs/operators';
import { MobileTransferCardAccount, PaymentProviders, PaymentProviderTypes } from '../../models/mobile-transfer';
import { fuseAnimations } from '../../core/animations';
import * as mobileTransferCardAccountActions from '../../core/actions/mobileTransferCardAccount';
import * as cardToCardActions from '../../core/actions/cardToCardAccount';
import * as tenantActions from '../../core/actions/tenant';
import { filter } from 'rxjs/operators/filter';
import { Router, ActivatedRoute } from '@angular/router';
import { RandomService } from '../../core/services/random/random.service';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../models/tenant';
import * as accountActions from '../../core/actions/account';
import { Owner } from '../../models/account.model';
import * as cardToCardAccountGroupActions from '../../core/actions/cardToCardAccountGroup';
import { CardToCardAccountGroup } from '../../models/card-to-card-account-group';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';
import { cardNumberValidator } from '../../validators/card-number-validator';
import { BankLogin } from 'app/models/bank-login';
import { CardToCardAccount } from 'app/models/card-to-card-account';
import * as bankLoginActions from '../../core/actions/bankLogin';

@Component({
    selector: 'app-mobile-transfer-card-account',
    templateUrl: './mobile-transfer-card-account.component.html',
    styleUrls: ['./mobile-transfer-card-account.component.scss'],
    animations: fuseAnimations
})
export class MobileTransferCardAccountComponent implements OnInit, OnDestroy {

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
    mobileTransferCardAccountForm: FormGroup;
    formSubmit: boolean = false;
    pageType = '';
    isCreating: boolean;
    createError$: Observable<string>;
    updateError$: Observable<string>;
    getDetailError$: Observable<string>;

    mobileTransferCardAccountCreated$: Observable<MobileTransferCardAccount>;
    mobileTransferCardAccountGetDetail$: Observable<MobileTransferCardAccount>;
    getDetailLoading$: Observable<boolean>;
    getDetailLoading: boolean;
    mobileTransferCardAccountUpdateSuccess: Observable<boolean>;

    tenantsLoading$: Observable<boolean>;
    tenantsLoading: boolean;
    tenants$: Observable<Tenant[]>;

    isProviderAdmin$: Observable<boolean>;
    isProviderAdmin: boolean;

    isTenantAdmin$: Observable<boolean>;
    isTenantAdmin: boolean;

    isStandardUser$: Observable<boolean>;
    isStandardUser: boolean;

    accountGuid$: Observable<string>;
    accountGuid: string;

    parentGuid: string;

    owners$: Observable<Owner[]>;
    owners: Owner[] = [];
    ownersLoading$: Observable<boolean>;
    ownersLoading: boolean = false;

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    groups$: Observable<CardToCardAccountGroup[]>;
    groups: CardToCardAccountGroup[];
    groupsLoading$: Observable<boolean>;
    groupsLoading: boolean = false;
    groupsError$: Observable<string>;

    providers = PaymentProviders;
    providerTypes = PaymentProviderTypes;

    bankLogins$: Observable<BankLogin[]>;
    bankLogins: BankLogin[] = [];
    bankLoginError$: Observable<string>;
    bankLoginLoading$: Observable<boolean>;
    bankLoginLoading: boolean;

    cardToCardAccounts$: Observable<CardToCardAccount[]>;
    cardToCardAccounts: CardToCardAccount[];
    cardToCardSearchError$: Observable<string>;

    @Output() formChanges: EventEmitter<boolean> = new EventEmitter();

    openedAccount: FormGroup;

    id: number;

    allowAddMobileTransferCardAccount: boolean = false;

    emptyCardAccount: CardToCardAccount = {
        id: 0,
        accountGuid: '',
        accountNo: '',
        accountType: null,
        bankAccountId: null,
        bankLoginId: null,
        cardHolderName: null,
        cardNumber: '',
        friendlyName: null,
        isActive: false,
        isTransferThresholdActive: false,
        loginGuid: null,
        loginType: null,
        ownerGuid: null,
        safeAccountNumber: null,
        switchIfHasReserveAccount: false,
        switchLimitAmount: null,
        switchOnLimit: false,
        tenantGuid: null,
        transferThreshold: null,
        transferThresholdLimit: null,
        switchCreditDailyLimit: 0
    };

    constructor(private store: Store<coreState.State>,
        private dateAdapter: DateAdapter<any>,
        private router: Router,
        private fb: FormBuilder,
        public snackBar: MatSnackBar,
        private route: ActivatedRoute,
        private accountService: AccountService,
        private translateService: TranslateService,
        private randomService: RandomService) {
        this.dateAdapter.setLocale('gb');
        this.allowAddMobileTransferCardAccount = this.accountService.isUserAuthorizedForTask(permissions.AddMobileTransferAccount);
    }

    openSnackBar(message: string, action: string = undefined) {
        if (!action) {
            action = this.translateService.instant('GENERAL.OK');
        }
        this.snackBar.open(message, action, {
            duration: 10000,
        });
    }

    ngOnInit() {

        this.parentGuid = this.accountService.getParentAccountId();

        this.mobileTransferCardAccountCreated$ = this.store.select(coreState.getMobileTransferCardAccountCreated);
        this.mobileTransferCardAccountGetDetail$ = this.store.select(coreState.getMobileTransferCardAccountDetails);
        this.mobileTransferCardAccountUpdateSuccess = this.store.select(coreState.getMobileTransferCardAccountEditSuccess);
        this.createError$ = this.store.select(coreState.getMobileTransferCardAccountCreateError);
        this.updateError$ = this.store.select(coreState.getMobileTransferCardAccountEditError);
        this.getDetailError$ = this.store.select(coreState.getMobileTransferCardAccountDetailError);
        this.getDetailLoading$ = this.store.select(coreState.getMobileTransferCardAccountDetailLoading);

        this.tenantsLoading$ = this.store.select(coreState.getTenantLoading);
        this.tenants$ = this.store.select(coreState.getTenantSearchResults);

        this.owners$ = this.store.select(coreState.getOwners);
        this.ownersLoading$ = this.store.select(coreState.getAccountLoading);

        this.ownersLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.ownersLoading = l;
        });

        this.owners$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            if (items != undefined) {
                this.owners = items;
            } else {
                this.owners = [];
            }
        });

        this.route.params.subscribe(params => {

            this.id = params['id'];

            if (this.id != 0 && this.id != null && this.id != undefined) {
                this.pageType = 'edit';
                this.loadMobileTransferCardAccountDetail();
            }
            else {
                this.pageType = 'new';
                this.createForm();
            }
        });

        this.createError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error != undefined) {
                this.openSnackBar(error);
            }
        });

        this.updateError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error != undefined) {
                this.openSnackBar(error);
            }
        });

        this.getDetailError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.tenantsLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.tenantsLoading = l;
        });

        this.getDetailLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.getDetailLoading = l;
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

            if (this.accountGuid && this.mobileTransferCardAccountForm && this.pageType == 'new') {

                this.mobileTransferCardAccountForm.get('ownerGuid').setValue(this.getOwnerGuid());
            }
        });

        this.groups$ = this.store.select(coreState.getAllCardToCardAccountGroups);

        this.groups$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.groups = items;
        });

        this.groupsLoading$ = this.store.select(coreState.getCardToCardAccountGroupLoading);

        this.groupsLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.groupsLoading = l;
        });

        this.groupsError$ = this.store.select(coreState.getAllCardToCardAccountGroupError);

        this.groupsError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
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

        this.store.dispatch(new cardToCardActions.ClearErrors());
        this.cardToCardAccounts$ = this.store.select(coreState.getCardToCardAccountSearchResults);

        this.cardToCardAccounts$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.cardToCardAccounts = items;
        });

        this.cardToCardSearchError$ = this.store.select(coreState.getCardToCardAccountSearchError);

        this.cardToCardSearchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid && this.mobileTransferCardAccountForm && this.pageType == 'new') {
                this.mobileTransferCardAccountForm.get('tenantGuid').setValue(t.tenantDomainPlatformMapGuid);
            }

            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new accountActions.GetOwners());
                this.store.dispatch(new cardToCardActions.Search(''));
                this.store.dispatch(new cardToCardAccountGroupActions.GetAll());
                this.store.dispatch(new bankLoginActions.Search(false));
            }
        });
    }

    ngOnDestroy(): void {
        this.store.dispatch(new mobileTransferCardAccountActions.ClearErrors());
        this.store.dispatch(new cardToCardActions.ClearErrors());
        this.store.dispatch(new cardToCardAccountGroupActions.ClearErrors());
        this.destroyed$.next(true);
        this.destroyed$.complete();
        this.formChanges.emit(false);
    }

    createForm(): void {

        this.mobileTransferCardAccountForm = this.fb.group({
            paymentProviderType: new FormControl(undefined, [Validators.required]),
            cardNumber: new FormControl(undefined),
            cardHolderName: new FormControl(undefined),
            merchantId: new FormControl(undefined),
            merchantPassword: new FormControl(undefined),
            terminalId: new FormControl(undefined),
            title: new FormControl(undefined),
            isActive: new FormControl(true),
            tenantGuid: new FormControl(this.selectedTenant == undefined ? undefined : this.selectedTenant.tenantDomainPlatformMapGuid, [Validators.required]),
            ownerGuid: new FormControl(this.getOwnerGuid(), [Validators.required]),
            thresholdAmount: new FormControl(0),
            cardToCardAccountGuid: new FormControl(undefined)
        });

        this.mobileTransferCardAccountForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: MobileTransferCardAccount = new MobileTransferCardAccount({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.mobileTransferCardAccountForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    createUpdateForm(data: MobileTransferCardAccount): void {

        this.mobileTransferCardAccountForm = this.fb.group({
            id: data.id,
            paymentProviderType: new FormControl(data.paymentProviderType, [Validators.required]),
            cardNumber: new FormControl(data.cardNumber),
            cardHolderName: new FormControl(data.cardHolderName),
            merchantId: new FormControl(data.merchantId),
            merchantPassword: new FormControl(data.merchantPassword),
            terminalId: new FormControl(data.terminalId),
            title: new FormControl(data.title),
            isActive: new FormControl(data.isActive),
            tenantGuid: new FormControl(data.tenantGuid, [Validators.required]),
            ownerGuid: new FormControl(data.ownerGuid, [Validators.required]),
            thresholdAmount: new FormControl(data.thresholdAmount),
            cardToCardAccountGuid: new FormControl(data.cardToCardAccountGuid)
        });

        this.mobileTransferCardAccountForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: MobileTransferCardAccount = new MobileTransferCardAccount({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.mobileTransferCardAccountForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    private loadMobileTransferCardAccountDetail(): void {
        this.store.dispatch(new mobileTransferCardAccountActions.GetDetails(this.id));

        this.mobileTransferCardAccountGetDetail$.pipe(takeUntil(this.destroyed$)).subscribe(p => {
            if (p) {
                this.createUpdateForm(p);
            }
        });
    }

    getErrorMessage(control: FormControl): string {
        return FormHelper.getErrorMessage(control);
    }

    onSubmit(): void {

        this.formSubmit = false;
        this.isCreating = true;

        if (this.mobileTransferCardAccountForm.valid) {
            let form = this.mobileTransferCardAccountForm.value;

            this.formSubmit = true;
            this.store.dispatch(new mobileTransferCardAccountActions.Create(form));

            this.mobileTransferCardAccountCreated$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;
                        this.router.navigate(['/mobiletransfercardaccounts']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.mobileTransferCardAccountForm);
        }
    }

    onUpdateSubmit(): void {
        this.formSubmit = false;
        this.isCreating = true;

        if (this.mobileTransferCardAccountForm.valid) {
            let form = this.mobileTransferCardAccountForm.value;
            this.formSubmit = true;
            this.store.dispatch(new mobileTransferCardAccountActions.Edit(this.id, form));

            this.mobileTransferCardAccountUpdateSuccess.pipe(filter(tnCreated => tnCreated !== false), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;

                        this.router.navigate(['/mobiletransfercardaccounts']);
                    });
        }
        else {
            FormHelper.validateFormGroup(this.mobileTransferCardAccountForm);
        }
    }

    isLoading() {
        return this.tenantsLoading || this.groupsLoading || this.getDetailLoading || this.isCreating || this.ownersLoading;
    }

    getGroups() {

        var guid = this.mobileTransferCardAccountForm.get('tenantGuid').value;
        var ownerGuid = this.mobileTransferCardAccountForm.get('ownerGuid').value;

        if (guid && this.groups) {
            return this.groups.filter(t => t.tenantGuid == guid && t.ownerGuid == ownerGuid);
        }

        return [];
    }

    getOwners() {

        if (this.owners == undefined || this.owners == null) {
            return [];
        }

        return this.owners;
    }

    getOwnerGuid() {
        if (this.parentGuid == undefined || this.parentGuid == '' || this.parentGuid == null) {
            return this.accountGuid;
        }

        return this.parentGuid;
    }

    checkCardNumberIsAllowed() {
        var provider = this.getProviderType();

        if (!provider) {
            return false;
        }

        if (provider == this.providerTypes.Cartipal) {
            return true;
        }

        return false;
    }

    checkCardHolderNameIsAllowed() {
        return this.checkCardNumberIsAllowed();
    }

    checkMerchantIdIsAllowed() {
        var provider = this.getProviderType();

        if (!provider) {
            return false;
        }

        return provider == this.providerTypes.Saman 
            || provider == this.providerTypes.Meli 
            || provider == this.providerTypes.Zarinpal  
            || provider == this.providerTypes.Mellat
            || provider == this.providerTypes.Novin;
    }

    checkTitleIsAllowed() {
        return this.checkMerchantIdIsAllowed();
    }

    checkTerminalIdIsAllowed() {
        var provider = this.getProviderType();

        if (!provider) {
            return false;
        }

        return provider == this.providerTypes.Meli || provider == this.providerTypes.Mellat;
    }

    checkMerchantPasswordIsAllowed() {
        var provider = this.getProviderType();

        if (!provider) {
            return false;
        }

        return provider == this.providerTypes.Meli || provider == this.providerTypes.Mellat || provider == this.providerTypes.Novin;
    }

    getProviderType() {
        if (!this.mobileTransferCardAccountForm) {
            return undefined;
        }

        var providerType = this.mobileTransferCardAccountForm.get('paymentProviderType').value;

        var cardNumber = this.mobileTransferCardAccountForm.get('cardNumber');
        var cardHolderName = this.mobileTransferCardAccountForm.get('cardHolderName');
        var merchantId = this.mobileTransferCardAccountForm.get('merchantId');
        var merchantPassword = this.mobileTransferCardAccountForm.get('merchantPassword');
        var terminalId = this.mobileTransferCardAccountForm.get('terminalId');
        var title = this.mobileTransferCardAccountForm.get('title');

        if (providerType == this.providerTypes.Cartipal) {
            cardNumber.setValidators([Validators.required]);
            cardHolderName.setValidators([Validators.required]);
            merchantId.setValidators([]);
            title.setValidators([]);
            merchantPassword.setValidators([]);
            terminalId.setValidators([]);
        }
        else {
            cardNumber.setValidators([]);
            cardHolderName.setValidators([]);
            merchantId.setValidators([Validators.required]);
            title.setValidators([Validators.required]);

            if (providerType == this.providerTypes.Meli) {
                merchantPassword.setValidators([Validators.required]);
                terminalId.setValidators([Validators.required]);
            }
            else{
                merchantPassword.setValidators([]);
                terminalId.setValidators([]);
            }
        }

        return providerType;
    }

    getCardToCardAccounts() {
        if (this.cardToCardAccounts == undefined || this.cardToCardAccounts == null || this.mobileTransferCardAccountForm == undefined) {
            return [];
        }

        var ownerGuid = this.mobileTransferCardAccountForm.get('ownerGuid').value;

        var accounts = this.cardToCardAccounts.filter(t => t.ownerGuid == ownerGuid);

        accounts.unshift(this.emptyCardAccount);

        return accounts;
    }

    setCardInformationNumber(event) {
        if (this.mobileTransferCardAccountForm && event && this.cardToCardAccounts) {
            var account: CardToCardAccount = this.cardToCardAccounts.find(t => t.accountGuid == event.value);
            if (account) {
                this.mobileTransferCardAccountForm.get('cardNumber').setValue(account.cardNumber);
                this.mobileTransferCardAccountForm.get('cardHolderName').setValue(account.cardHolderName);
            }
            else {
                this.mobileTransferCardAccountForm.get('cardNumber').setValue('');
                this.mobileTransferCardAccountForm.get('cardHolderName').setValue('');
            }
        }
    }
}

