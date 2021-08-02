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
import { BankLogin, BankAccount, BankLoginStatus, CreateLoginFromLoginRequestDTO, LoginTypeEnum, LoginTypes, QRRegisterRequestStatus, QrCodeRegistrationRequest } from '../../models/bank-login';
import 'rxjs/add/observable/interval';
import { Tenant } from '../../models/tenant';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';


@Component({
    selector: 'app-qr-code-registration',
    templateUrl: './qr-code-registration.component.html',
    styleUrls: ['./qr-code-registration.component.scss'],
    animations: fuseAnimations
})
export class QRCodeRegistrationComponent implements OnInit, OnDestroy {

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    qrCodeRegisterForm: FormGroup;
    formSubmit: boolean = false;

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

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    interval: any = undefined;

    intervalMiliseconds = 10000;

    formCreated: boolean = false;

    id: number;
    bankLoginId: number;
    loginStatus = BankLoginStatus;
    qrStatus = QRRegisterRequestStatus;
    accounts: string[];

    qrRegisterStatus = QRRegisterRequestStatus;

    loginTypes = LoginTypes;

    allowAddBankLogin: boolean = false;
    allowSelectLoginType: boolean = false;
    showSecondPassword: boolean = false;
    qrCodeRegistered$: Observable<boolean>;
    qrCodeRegisterSuccess$: Observable<boolean>;

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

        this.store.select(coreState.qrCodeRegisterError).pipe(takeUntil(this.destroyed$)).subscribe(error =>{
            if(error){
                this.snackBar.open(error, this.translateService.instant('OK'));
            }
        });

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

        this.route.params.subscribe(params => {
            this.id = params['id'];

            if (this.id != 0 && this.id != null && this.id != undefined) {
                this.loadDetail();
            }
        });

        this.bankLoginGetDetail$.pipe(takeUntil(this.destroyed$)).subscribe(p => {
            if (p) {
                this.bankLoginId = p.bankLoginId;
                this.createUpdateForm(p);

                if (p.qrRegistrationStatusId == this.qrRegisterStatus.Complete
                    || p.qrRegistrationStatusId == this.qrRegisterStatus.Failed
                    || p.qrRegistrationStatusId == this.qrRegisterStatus.SessionOut
                    || p.qrRegistrationStatusId == this.qrRegisterStatus.QrImageCaptured
                    || p.qrRegistrationStatusId == null || p.qrRegistrationStatusId == undefined) {
                    this.unsubscribeInterval();
                }
                else {
                    console.log(p.qrRegistrationStatusId);
                    if (this.interval == undefined) {
                        this.interval = Observable.interval(this.intervalMiliseconds).subscribe(() => {
                            this.loadDetail();
                        });
                    }
                }
            }
        });

        this.qrCodeRegistered$ = this.store.select(coreState.getQrCodeRegistrationCompleted);

        this.qrCodeRegisterSuccess$ = this.store.select(coreState.qrCodeRegisterSuccess);
    }

    loadDetail() {
        this.store.dispatch(new bankLoginActions.GetDetails(this.id));
    }

    createUpdateForm(data: BankLogin): void {
        this.qrCodeRegisterForm = undefined;
        this.qrCodeRegisterForm = this.fb.group({
            id: data.id,
            loginGuid: data.loginGuid,
            bankLoginId: data.bankLoginId,
            friendlyName: new FormControl({ value: data.friendlyName, disabled: true }, [Validators.required]),
            qrRegistrationStatus: data.qrRegistrationStatus,
            qrRegistrationStatusId: data.qrRegistrationStatusId,
            qrCodeQTP: new FormControl(undefined)
        });
    }

    setFormData(data: BankLogin) {
        if (data == undefined) {
            return;
        }

        
        this.qrCodeRegisterForm.get('loginGuid').setValue(data.loginGuid);
        this.qrCodeRegisterForm.get('status').setValue(data.status);
        this.qrCodeRegisterForm.get('bankName').setValue(data.bankName);
        this.qrCodeRegisterForm.get('friendlyName').setValue(data.friendlyName);
        this.qrCodeRegisterForm.get('bankLoginId').setValue(data.bankLoginId);
    }

    onQRCodeRegisterFormSubmit(): void {
        this.formSubmit = false;
        this.store.dispatch(new bankLoginActions.QrCodeRegister(this.bankLoginId));

        this.qrCodeRegisterSuccess$.pipe(filter(tnCreated => tnCreated == true), take(1))
                    .subscribe(
                        tnCreated => {
                            this.loadDetail();
                        });
    }
    
    onSubmit(): void {
        this.formSubmit = false;

        if (this.qrCodeRegisterForm.valid) {
            if (this.qrCodeRegisterForm.valid) {
                let form = this.qrCodeRegisterForm.value;
                var request: QrCodeRegistrationRequest = {
                    bankLoginId: this.bankLoginId,
                    otp: form.qrCodeQTP
                };

                this.formSubmit = true;
                this.store.dispatch(new bankLoginActions.RegisterQrCode(request));

                this.qrCodeRegistered$.pipe(filter(tnCreated => tnCreated == true), take(1))
                    .subscribe(
                        tnCreated => {
                            this.loadDetail();
                        });

            }
            else {
                FormHelper.validateFormGroup(this.qrCodeRegisterForm);
            }
        }
    }

    resolvingQr(): boolean {
        if (this.qrCodeRegisterForm == undefined) {
            return false;
        }

        var value = this.qrCodeRegisterForm.get('qrRegistrationStatusId').value;

        if(!value){
            return false;
        }

        var numValue = Number(value);

        if (numValue == this.qrRegisterStatus.InProgress
            || numValue == this.qrRegisterStatus.Incomplete
            || numValue == this.qrRegisterStatus.Pending) {
            return true;
        }

        return false;
    }

    success(): boolean {
        if (this.qrCodeRegisterForm == undefined) {
            return false;
        }

        var value = this.qrCodeRegisterForm.get('qrRegistrationStatusId').value;

        var numValue = Number(value);

        if (numValue == this.qrRegisterStatus.Complete) {
            return true;
        }

        return false;
    }

    waitingSms(): boolean {
        if (this.qrCodeRegisterForm == undefined) {
            return false;
        }

        var value = this.qrCodeRegisterForm.get('qrRegistrationStatusId').value;

        var numValue = Number(value);

        if (numValue == this.qrRegisterStatus.QrImageCaptured) {
            return true;
        }

        return false;
    }

    hasError(): boolean {
        if (this.qrCodeRegisterForm == undefined) {
            return false;
        }

        var value = this.qrCodeRegisterForm.get('qrRegistrationStatusId').value;

        var numValue = Number(value);

        if (numValue == this.qrRegisterStatus.Failed) {
            return true;
        }

        return false;
    }

    isSesssionOut(): boolean {
        if (this.qrCodeRegisterForm == undefined) {
            return false;
        }

        var value = this.qrCodeRegisterForm.get('qrRegistrationStatusId').value;

        var numValue = Number(value);

        if (numValue == this.qrRegisterStatus.SessionOut) {
            return true;
        }

        return false;
    }

    isNotRequested(): boolean {
        if (this.qrCodeRegisterForm == undefined) {
            return false;
        }

        var value = this.qrCodeRegisterForm.get('qrRegistrationStatusId').value;

        if (!value) {
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
