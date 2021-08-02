import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { FormGroup, FormControl, Validators, FormBuilder, FormArray } from '@angular/forms';
import { Store } from '@ngrx/store';
import { DateAdapter, MatSnackBar } from '@angular/material';
import * as coreState from '../../core';
import { FormHelper } from '../../helpers/forms/form-helper';
import { debounceTime, distinctUntilChanged, take } from 'rxjs/operators';
import { CardToCardAccount } from '../../models/card-to-card-account';
import { fuseAnimations } from '../../core/animations';
import * as cardToCardAccountActions from '../../core/actions/cardToCardAccount';
import * as bankLoginActions from '../../core/actions/bankLogin';
import { filter } from 'rxjs/operators/filter';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { BankLogin, BankAccount, BankLoginStatus, CreateLoginFromLoginRequestDTO, LoginTypeEnum, LoginTypes, RegisterLogin} from '../../models/bank-login';
import 'rxjs/add/observable/interval';
import { Tenant } from '../../models/tenant';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';
import { Bank } from 'app/models/bank';


@Component({
    selector: 'app-bank-login-account',
    templateUrl: './bank-login-account.component.html',
    styleUrls: ['./bank-login-account.component.scss'],
    animations: fuseAnimations
})
export class BankLoginAccountComponent implements OnInit, OnDestroy {

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    accountForm: FormGroup;
    createLoginForm: FormGroup;
    loginRegistrationForm: FormGroup;

    formSubmit: boolean = false;

    banks$: Observable<Bank[]>;
    banks: Bank[];
    banksLoading$: Observable<boolean>;
    banksLoading: boolean;
    banksError$: Observable<string>;


    bankAccounts$: Observable<BankAccount[]>;
    bankAccountsError$: Observable<string>;
    bankAccountLoading$: Observable<boolean>;
    bankAccountsLoading: boolean;

    createError$: Observable<string>;
    cardToCardAccountCreated$: Observable<CardToCardAccount>;

    getDetailError$: Observable<string>;
    getDetailLoading$: Observable<boolean>;
    getDetailLoading: boolean;

    bankLoginCreated$: Observable<boolean>;
    bankLoginGetDetail$: Observable<BankLogin>;

    bankLoginCreatedFromRequest$: Observable<boolean>;
    bankLoginCreatedFromRequestError$: Observable<string>;

    bankLoginRequestRegistered$: Observable<boolean>;
    bankLoginRequestRegisteredError$: Observable<string>;

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    interval: any = undefined;

    intervalMiliseconds = 10000;

    formCreated: boolean = false;

    id: number;

    loginStatus = BankLoginStatus;
    accounts: string[];

    loginTypes = LoginTypes;

    allowAddBankLogin: boolean = false;
    allowSelectLoginType: boolean = false;
    showSecondPassword: boolean = false;

    isEmailAddressNeeded: boolean = false;
    bankId: number = undefined;

    constructor(private store: Store<coreState.State>,
        private dateAdapter: DateAdapter<any>,
        private router: Router,
        private fb: FormBuilder,
        public snackBar: MatSnackBar,
        private route: ActivatedRoute,
        private accountService: AccountService,
        private translateService: TranslateService) {
        this.dateAdapter.setLocale('gb');
        this.allowAddBankLogin = this.accountService.isUserAuthorizedForTask(permissions.AddBankLogin);
        this.allowSelectLoginType = this.accountService.isUserAuthorizedForTask(permissions.SelectLoginType);
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
        this.bankLoginGetDetail$ = this.store.select(coreState.getBankLoginDetail);
        this.getDetailError$ = this.store.select(coreState.getBankLoginDetailError);
        this.getDetailLoading$ = this.store.select(coreState.getBankLoginDetailLoading);

        this.bankAccountLoading$ = this.store.select(coreState.getBankAccountsLoading);
        this.bankAccounts$ = this.store.select(coreState.getBankAccountSearchResults);
        this.bankAccountsError$ = this.store.select(coreState.getBankAccountSearchError);
        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.cardToCardAccountCreated$ = this.store.select(coreState.getCardToCardAccountCreated);
        this.createError$ = this.store.select(coreState.getCardToCardAccountCreateError);

        this.bankLoginCreatedFromRequestError$ = this.store.select(coreState.getCreateBankLoginFromRequestError);
        this.bankLoginCreatedFromRequest$ = this.store.select(coreState.getCreateBankLoginFromRequestSuccess);

        this.bankLoginRequestRegisteredError$ = this.store.select(coreState.getCreateBankLoginFromRequestError);
        this.bankLoginRequestRegistered$ = this.store.select(coreState.getBankLoginRequestRegisteredSuccess);

        this.banks$ = this.store.select(coreState.getBanksSearchResult);
        this.banksLoading$ = this.store.select(coreState.getBanksLoading);
        this.banksError$ = this.store.select(coreState.getBanksSearchError);

        this.bankLoginCreatedFromRequestError$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            if (t) {
                this.openSnackBar(t);
            }
        });

        this.createError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error != undefined) {
                this.openSnackBar(error);
            }
        });

        this.getDetailError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.bankAccountsError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.getDetailLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.getDetailLoading = l;
        });

        this.bankAccountLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.bankAccountsLoading = l;
        });

        this.banksLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.banksLoading = l;
        });

        this.banksError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.banks$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.banks = items;

            this.getIsEmailAddressNeeded();
        });

        this.route.params.subscribe(params => {
            this.id = params['id'];

            if (this.id != 0 && this.id != null && this.id != undefined) {
                this.loadDetail();
                this.store.dispatch(new bankLoginActions.SearchBanks());
            }
        });

        this.bankLoginGetDetail$.pipe(takeUntil(this.destroyed$)).subscribe(p => {
            if (p) {
                if (!this.bankId) {
                    this.bankId = p.bankId;
                    this.getIsEmailAddressNeeded();
                }
                this.createUpdateForm(p);

                if (p.status == this.loginStatus.Success) {
                    this.loadAccounts();
                    this.unsubscribeInterval();
                }
                else if (p.status == this.loginStatus.Error) {
                    this.unsubscribeInterval();
                }
                else if (p.status == this.loginStatus.WaitingApprovement) {
                    this.accounts = p.accounts;
                    this.unsubscribeInterval();
                }
                else if (p.status == this.loginStatus.AwaitingRegistration) {
                    this.accounts = p.accounts;
                    this.unsubscribeInterval();
                }
                else {
                    if (this.interval == undefined) {
                        this.interval = Observable.interval(this.intervalMiliseconds).subscribe(() => {
                            this.loadDetail();
                        });
                    }
                }
            }
        });
    }

    private getIsEmailAddressNeeded() {
        if (this.banks && this.bankId && this.bankId > 0) {
            var bank = this.banks.find(t => t.id == this.bankId);

            if (bank != null) {
                this.isEmailAddressNeeded = bank.isEmailAddressNeeded;
            }
        }
    }

    loadDetail() {
        this.store.dispatch(new bankLoginActions.GetDetails(this.id));
    }

    loadAccounts() {
        if (this.accountForm) {
            var loginGuid = this.accountForm.get('loginGuid').value;

            if (loginGuid) {
                this.store.dispatch(new bankLoginActions.SearchAccountsByLoginGuid(loginGuid));
            }
        }
    }

    createUpdateForm(data: BankLogin): void {
        this.showSecondPassword = data.isSecondPasswordNeeded;

        if (data.status == this.loginStatus.AwaitingRegistration) {
            this.createLoginForm = undefined;
            this.accountForm = undefined;

            this.loginRegistrationForm = this.fb.group({
                bankId: data.bankId,
                bankName: new FormControl({ value: data.bankName, disabled: true }),
                friendlyName: new FormControl({ value: data.friendlyName, disabled: true }, [Validators.required]),
                otp: new FormControl(undefined, [Validators.required]),
                loginRequestId: data.loginRequestId,
                accountNumber: new FormControl(undefined),
                loadPreviousStatements: new FormControl(false),
                isBlockCard: new FormControl(false),
                secondPassword: new FormControl(undefined),
                mobileNumber: new FormControl(undefined),
                status: new FormControl({ value: data.status, disabled: true }),
                loginType: new FormControl(data.loginType),
                emailAddress: new FormControl(undefined),
                emailPassword: new FormControl(undefined)
            });
        }
        else if (data.status != this.loginStatus.WaitingApprovement) {
            this.createLoginForm = undefined;
            this.loginRegistrationForm = undefined;

            this.accountForm = this.fb.group({
                id: data.id,
                loginGuid: data.loginGuid,
                tenantGuid: data.tenantGuid,
                ownerGuid: data.ownerGuid,
                status: new FormControl({ value: data.status, disabled: true }),
                bankName: new FormControl({ value: data.bankName, disabled: true }),
                friendlyName: new FormControl({ value: data.friendlyName, disabled: true }, [Validators.required]),
                accountGuid: new FormControl(undefined, [Validators.required]),
                isActive: new FormControl(true),
                cardNumber: new FormControl(undefined, [Validators.required]),
                cardHolderName: new FormControl(undefined, [Validators.required]),
                safeAccountNumber: new FormControl(undefined),
                transferThreshold: new FormControl(0),
                isTransferThresholdActive: new FormControl(false),
                transferThresholdLimit: new FormControl(undefined)
            });
        }
        
        else {
            this.accountForm = undefined;
            this.createLoginForm = undefined;
            this.loginRegistrationForm = undefined;

            this.createLoginForm = this.fb.group({
                bankName: new FormControl({ value: data.bankName, disabled: true }),
                friendlyName: new FormControl({ value: data.friendlyName, disabled: true }, [Validators.required]),
                loginRequestId: data.loginRequestId,
                accountNumber: new FormControl(undefined, [Validators.required]),
                loadPreviousStatements: new FormControl(false),
                isBlockCard: new FormControl(false),
                secondPassword: new FormControl(undefined),
                mobileNumber: new FormControl(undefined),
                status: new FormControl({ value: data.status, disabled: true }),
                loginType: new FormControl(data.loginType),
                emailAddress: new FormControl(undefined),
                emailPassword: new FormControl(undefined),
                processCountIn24Hrs: new FormControl(undefined)
            });
        }
    }

    setFormData(data: BankLogin) {
        if (data == undefined) {
            return;
        }
        this.accountForm.get('loginGuid').setValue(data.loginGuid);
        this.accountForm.get('status').setValue(data.status);
        this.accountForm.get('bankName').setValue(data.bankName);
        this.accountForm.get('friendlyName').setValue(data.friendlyName);
    }

    onSubmit(): void {
        this.formSubmit = false;

        if (this.accountForm.valid) {

            let form = this.accountForm.value;
            var account = new CardToCardAccount(form);
            account.id = 0;
            this.formSubmit = true;
            this.store.dispatch(new cardToCardAccountActions.Create(account));

            this.cardToCardAccountCreated$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
                .subscribe(
                    tnCreated => {
                        this.router.navigate(['/cardtocardaccounts']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.accountForm);
        }
    }

    onCreateLoginSubmit(): void {
        this.formSubmit = false;

        if (this.createLoginForm.valid) {

            var emailAddress : string = this.createLoginForm.get('emailAddress').value;

            if(emailAddress){
                if(emailAddress.indexOf('@gmail') == -1 && emailAddress.indexOf('@yahoo') == -1){
                    this.openSnackBar(this.translateService.instant('BANK-LOGIN.GENERAL.WRONG-EMAIL-ADDRESS-PROVIDER'));
                    return;
                }
            }
            let form = this.createLoginForm.value;
            var account = new CreateLoginFromLoginRequestDTO(form);

            if (this.allowSelectLoginType == false || account.loginType == undefined || account.loginType == null || account.loginType == 0) {
                account.loginType = LoginTypeEnum.CardToCard;
            }
            this.formSubmit = true;
            this.store.dispatch(new bankLoginActions.CreateLoginFromLoginRequest(account));
            
            this.bankLoginCreatedFromRequest$.pipe(filter(tnCreated => tnCreated == true), take(1))
                .subscribe(
                    tnCreated => {
                        this.loadDetail();
                    });

        }
        else {
            FormHelper.validateFormGroup(this.accountForm);
        }
    }

    onRegisterLoginSubmit(): void {
        
        this.formSubmit = false;

        if (this.loginRegistrationForm.valid) {           
            let form = this.loginRegistrationForm.value;
            var account = new RegisterLogin(form);

            this.formSubmit = true;
            this.store.dispatch(new bankLoginActions.RegisterLoginRequest(account));
            
            this.bankLoginRequestRegistered$.pipe(filter(tnCreated => tnCreated == true), take(1))
                .subscribe(                    
                    tnCreated => {
                        this.router.navigate(['/banklogins']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.accountForm);
        }
    }

    waitingInformations(): boolean {
        
        if (this.accountForm == undefined) {
            return false;
        }

        var value = this.accountForm.get('status').value;

        var numValue = Number(value);

        if (numValue == this.loginStatus.WaitingInformation) {
            return true;
        }

        return false;
    }

    loginSuccess(): boolean {
        if (this.accountForm == undefined) {
            return false;
        }

        var value = this.accountForm.get('status').value;

        var numValue = Number(value);

        if (numValue == this.loginStatus.Success) {
            return true;
        }

        return false;
    }

    waitingApprove(): boolean {
        if (this.createLoginForm == undefined) {
            return false;
        }

        var value = this.createLoginForm.get('status').value;

        var numValue = Number(value);

        if (numValue == this.loginStatus.WaitingApprovement) {
            return true;
        }

        return false;
    }

    awaitingRegistration(): boolean {
        if (this.loginRegistrationForm == undefined) {
            return false;
        }

        var value = this.loginRegistrationForm.get('status').value;

        var numValue = Number(value);

        if (numValue == this.loginStatus.AwaitingRegistration) {
            return true;
        }

        return false;
    } 

    loginError(): boolean {

        if (this.accountForm == undefined) {
            return false;
        }

        var value = this.accountForm.get('status').value;

        var numValue = Number(value);

        if (numValue == this.loginStatus.Error) {
            return true;
        }

        return false;
    }

    unsubscribeInterval() {
        if (this.interval) {
            this.interval.unsubscribe();
        }
    }

    isLoading(): boolean {
        return this.getDetailLoading;
    }

    ngOnDestroy(): void {
        this.store.dispatch(new bankLoginActions.ClearAll());
        this.unsubscribeInterval();
        this.destroyed$.next(true);
        this.destroyed$.complete();
    }
}
