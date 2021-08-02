import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { FormGroup, FormControl, Validators, FormBuilder, FormArray } from '@angular/forms';
import { Store } from '@ngrx/store';
import { DateAdapter, MatSnackBar } from '@angular/material';
import * as coreState from '../../core';
import { GenericHelper } from '../../helpers/generic';
import { CustomValidators, FormHelper } from '../../helpers/forms/form-helper';
import { debounceTime, distinctUntilChanged, take } from 'rxjs/operators';
import { CardToCardAccount } from '../../models/card-to-card-account';
import { fuseAnimations } from '../../core/animations';
import * as cardToCardAccountActions from '../../core/actions/cardToCardAccount';
import * as bankLoginActions from '../../core/actions/bankLogin';
import { filter } from 'rxjs/operators/filter';
import { Router, ActivatedRoute } from '@angular/router';
import { RandomService } from '../../core/services/random/random.service';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { BankLogin, BankAccount } from '../../models/bank-login';
import { Tenant } from '../../models/tenant';
import { Owner } from '../../models/account.model';
import * as tenantActions from '../../core/actions/tenant';
import * as accountActions from '../../core/actions/account';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';

@Component({
  selector: 'app-card-to-card-account',
  templateUrl: './card-to-card-account.component.html',
    styleUrls: ['./card-to-card-account.component.scss'],
    animations: fuseAnimations
})
export class CardToCardAccountComponent implements OnInit, OnDestroy {

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
    cardToCardAccountForm: FormGroup;
    formSubmit: boolean = false;
    pageType = '';
    isCreating: boolean;
    createError$: Observable<string>;
    updateError$: Observable<string>;
    getDetailError$: Observable<string>;
    getDetailLoading$: Observable<boolean>;
    getDetailLoading: boolean;

    bankLogins$: Observable<BankLogin[]>;
    bankLogins: BankLogin[] = [];
    bankLoginError$: Observable<string>;
    bankLoginLoading$: Observable<boolean>;
    bankLoginLoading: boolean;

    bankAccounts$: Observable<BankAccount[]>;
    bankAccountsError$: Observable<string>;
    bankAccountLoading$: Observable<boolean>;
    bankAccountsLoading: boolean;
    bankAccounts: BankAccount[];

    cardToCardAccountCreated$: Observable<CardToCardAccount>;
    cardToCardAccountGetDetail$: Observable<CardToCardAccount>;
    cardToCardAccountUpdateSuccess: Observable<boolean>;
    @Output() formChanges: EventEmitter<boolean> = new EventEmitter();

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

    openedAccount: FormGroup;

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    id: number;

    allowAddBankLogin: boolean = false;

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
        this.allowAddBankLogin = this.accountService.isUserAuthorizedForTask(permissions.AddBankLogin);
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

        this.cardToCardAccountCreated$ = this.store.select(coreState.getCardToCardAccountCreated);
        this.cardToCardAccountGetDetail$ = this.store.select(coreState.getCardToCardAccountDetails);
        this.cardToCardAccountUpdateSuccess = this.store.select(coreState.getCardToCardAccountEditSuccess);
        this.createError$ = this.store.select(coreState.getCardToCardAccountCreateError);
        this.updateError$ = this.store.select(coreState.getCardToCardAccountEditError);
        this.getDetailError$ = this.store.select(coreState.getCardToCardAccountDetailError);
        this.getDetailLoading$ = this.store.select(coreState.getCardToCardAccountDetailLoading);

        this.bankLoginError$ = this.store.select(coreState.getBankLoginSearchError);
        this.bankLogins$ = this.store.select(coreState.getBankLoginSearchResults);
        this.bankLoginLoading$ = this.store.select(coreState.getBankLoginLoading);

        this.bankAccounts$ = this.store.select(coreState.getBankAccountSearchResults);
        this.bankAccountsError$ = this.store.select(coreState.getBankAccountSearchError);
        this.bankAccountLoading$ = this.store.select(coreState.getBankAccountsLoading);

        this.tenantsLoading$ = this.store.select(coreState.getTenantLoading);
        this.tenants$ = this.store.select(coreState.getTenantSearchResults);

        this.bankLogins$.pipe(takeUntil(this.destroyed$)).subscribe(logins => {
            this.bankLogins = logins;
        });

        this.route.params.subscribe(params => {

            this.id = params['id'];

            if (this.id != 0 && this.id != null && this.id != undefined) {
                this.pageType = 'edit';
                this.loadCardToCardAccountDetail();
            }
            else {
                this.pageType = 'new';
                this.createForm();
            }
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid && this.cardToCardAccountForm && this.pageType == 'new') {
                this.cardToCardAccountForm.get('tenantGuid').setValue(t.tenantDomainPlatformMapGuid);
            }

            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new accountActions.GetOwners());
                this.store.dispatch(new bankLoginActions.Search(false));
                this.store.dispatch(new bankLoginActions.SearchAccounts(false));
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

        this.bankLoginError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.bankAccountsError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.bankAccounts$.pipe(takeUntil(this.destroyed$)).subscribe(accounts => {
            if (accounts) {
                this.bankAccounts = accounts;
            }
            else {
                this.bankAccounts = [];
            }
        });

        this.bankLoginLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.bankLoginLoading = l;
        });

        this.bankAccountLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.bankAccountsLoading = l;
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
            if (this.accountGuid && this.cardToCardAccountForm && this.pageType == 'new') {
                this.cardToCardAccountForm.get('ownerGuid').setValue(this.getOwnerGuid());
            }
        });

        this.tenantsLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.tenantsLoading = l;
        });

        this.owners$ = this.store.select(coreState.getOwners);

        this.owners$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            if (items != undefined) {
                this.owners = items;
            } else {
                this.owners = [];
            }
        });
    }

    ngOnDestroy(): void {
        this.store.dispatch(new cardToCardAccountActions.ClearErrors());
        this.store.dispatch(new bankLoginActions.ClearAll());
        this.destroyed$.next(true);
        this.destroyed$.complete();
        this.formChanges.emit(false);
    }

    createForm(): void {
        this.cardToCardAccountForm = this.fb.group({
            tenantGuid: new FormControl(this.selectedTenant == undefined ? undefined : this.selectedTenant.tenantDomainPlatformMapGuid, [Validators.required]),
            ownerGuid: new FormControl(this.getOwnerGuid(), [Validators.required]),
            loginGuid: new FormControl(undefined, [Validators.required]),
            accountGuid: new FormControl(undefined, [Validators.required]),
            isActive: new FormControl(true),
            cardNumber: new FormControl(undefined, [Validators.required]),
            cardHolderName: new FormControl(undefined, [Validators.required]),
            safeAccountNumber: new FormControl(undefined),
            transferThreshold: new FormControl(undefined),
            isTransferThresholdActive: new FormControl(false),
            transferThresholdLimit: new FormControl(undefined),
            switchLimitAmount: new FormControl(undefined),
            switchCreditDailyLimit: new FormControl(undefined),
            switchOnLimit: new FormControl(false),
            switchIfHasReserveAccount: new FormControl(false)
        });

        this.cardToCardAccountForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: CardToCardAccount = new CardToCardAccount({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.cardToCardAccountForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    createUpdateForm(data: CardToCardAccount): void {

        var array: FormGroup[] = [];

            this.cardToCardAccountForm = this.fb.group({
                id: data.id,
                tenantGuid: new FormControl(data.tenantGuid, [Validators.required]),
                ownerGuid: new FormControl(data.ownerGuid, [Validators.required]),
                loginGuid: new FormControl(data.loginGuid, [Validators.required]),
                accountGuid: new FormControl(data.accountGuid, [Validators.required]),
                isActive: new FormControl(data.isActive),
                cardNumber: new FormControl(data.cardNumber, [Validators.required]),
                cardHolderName: new FormControl(data.cardHolderName, [Validators.required]),
                safeAccountNumber: new FormControl(data.safeAccountNumber),
                transferThreshold: new FormControl(data.transferThreshold),
                isTransferThresholdActive: new FormControl(data.isTransferThresholdActive),
                transferThresholdLimit: new FormControl(data.transferThresholdLimit),
                switchLimitAmount: new FormControl(data.switchLimitAmount),
                switchCreditDailyLimit: new FormControl(data.switchCreditDailyLimit),
                switchOnLimit: new FormControl(data.switchOnLimit),
                switchIfHasReserveAccount: new FormControl(data.switchIfHasReserveAccount)
            });

        this.cardToCardAccountForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: CardToCardAccount = new CardToCardAccount({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.cardToCardAccountForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    private loadCardToCardAccountDetail(): void {
        this.store.dispatch(new cardToCardAccountActions.GetDetails(this.id));

        this.cardToCardAccountGetDetail$.pipe(takeUntil(this.destroyed$)).subscribe(p => {
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

        if (this.cardToCardAccountForm.valid) {
            let form = this.cardToCardAccountForm.value;
            //console.log(form);
            this.formSubmit = true;
            this.store.dispatch(new cardToCardAccountActions.Create(form));

            this.cardToCardAccountCreated$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;
                        this.router.navigate(['/cardtocardaccounts']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.cardToCardAccountForm);
        }
    }

    onUpdateSubmit(): void {
        this.formSubmit = false;

        if (this.cardToCardAccountForm.valid) {
            let form = this.cardToCardAccountForm.value;
            this.formSubmit = true;
            this.store.dispatch(new cardToCardAccountActions.Edit(this.id, form));

            this.cardToCardAccountUpdateSuccess.pipe(filter(tnCreated => tnCreated !== false), take(1))
                .subscribe(
                    tnCreated => {
                        this.router.navigate(['/cardtocardaccounts']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.cardToCardAccountForm);
        }
    }

    onLoginChanged() {

    }

    getAccounts() {
        var loginGuid = this.cardToCardAccountForm.get('loginGuid').value;

        if (loginGuid && this.bankAccounts) {
            return this.bankAccounts.filter(t => t.loginGuid == loginGuid);
        }

        return [];
    }

    getOwners() {

        if (this.owners) {
            var list = this.owners;

            if (list.length == 0) {
                this.cardToCardAccountForm.get('ownerGuid').setValue(undefined);
            }

            return list;
        }

        this.cardToCardAccountForm.get('ownerGuid').setValue(undefined);
        return [];
    }

    getLogins() {
        var ownerGuid = this.cardToCardAccountForm.get('ownerGuid').value;

        if (ownerGuid && this.bankLogins) {
            return this.bankLogins.filter(t => t.ownerGuid == ownerGuid);
        }

        return [];
    }

    onToggleAccountDisplay(account: FormGroup): void {
        if (this.openedAccount === undefined) {
            this.toggleExpandedAccount(account);
        }
        else if (this.openedAccount === account) {
            this.toggleExpandedAccount(undefined);
        }
    }

    private toggleExpandedAccount(account: FormGroup | undefined): void {
        this.openedAccount = account;
    }

    isLoading() {
        return this.getDetailLoading || this.bankLoginLoading || this.bankAccountsLoading;
    }

    getOwnerGuid() {
        if (this.parentGuid == undefined || this.parentGuid == '' || this.parentGuid == null) {
            return this.accountGuid;
        }

        return this.parentGuid;
    }

}

