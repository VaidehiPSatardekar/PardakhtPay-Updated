import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Store } from '@ngrx/store';
import { DateAdapter, MatSnackBar } from '@angular/material';
import * as coreState from '../../core';
import { GenericHelper } from '../../helpers/generic';
import { FormHelper } from '../../helpers/forms/form-helper';
import { debounceTime, distinctUntilChanged, take } from 'rxjs/operators';
import { fuseAnimations } from '../../core/animations';
import * as bankLoginActions from '../../core/actions/bankLogin';
import { filter } from 'rxjs/operators/filter';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { BankLogin } from '../../models/bank-login';
import { Bank } from 'app/models/bank';

@Component({
    selector: 'app-bank-login-change-password',
    templateUrl: './bank-login-change-password.component.html',
    styleUrls: ['./bank-login-change-password.component.scss'],
    animations: fuseAnimations
})
export class BankLoginChangePasswordComponent implements OnInit, OnDestroy {

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
    bankLoginForm: FormGroup;
    formSubmit: boolean = false;
    isCreating: boolean;
    createError$: Observable<string>;
    getDetailError$: Observable<string>;
    getDetailLoading$: Observable<boolean>;
    getDetailLoading: boolean;

    bankLoginCreated$: Observable<BankLogin>;
    bankLoginGetDetail$: Observable<BankLogin>;
    bankLoginUpdateSuccess$: Observable<boolean>;

    banks$: Observable<Bank[]>;
    banks: Bank[];
    banksLoading$: Observable<boolean>;
    banksLoading: boolean;
    banksError$: Observable<string>;

    isSecondPasswordNeeded: boolean = false;

    isEmailAddressNeeded: boolean = false;

    bankId: number = undefined;

    @Output() formChanges: EventEmitter<boolean> = new EventEmitter();

    id: number;

    constructor(private store: Store<coreState.State>,
        private dateAdapter: DateAdapter<any>,
        private router: Router,
        private fb: FormBuilder,
        public snackBar: MatSnackBar,
        private route: ActivatedRoute,
        private translateService: TranslateService) {
        this.dateAdapter.setLocale('gb');
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
        this.createError$ = this.store.select(coreState.getBankLoginUpdateInformationError);
        this.getDetailError$ = this.store.select(coreState.getBankLoginDetailError);
        this.getDetailLoading$ = this.store.select(coreState.getBankLoginDetailLoading);

        this.bankLoginUpdateSuccess$ = this.store.select(coreState.getBankLoginUpdateInformationSuccess);

        this.banks$ = this.store.select(coreState.getBanksSearchResult);
        this.banksLoading$ = this.store.select(coreState.getBanksLoading);
        this.banksError$ = this.store.select(coreState.getBanksSearchError);

        this.route.params.subscribe(params => {
            this.id = params['id'];

            if (this.id != 0 && this.id != null && this.id != undefined) {
                this.loadBankLoginDetail();
                this.store.dispatch(new bankLoginActions.SearchBanks());
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

        this.getDetailLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.getDetailLoading = l;
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
    }

    private getIsEmailAddressNeeded() {
        if (this.banks && this.bankId && this.bankId > 0) {
            var bank = this.banks.find(t => t.id == this.bankId);

            if (bank != null) {
                this.isEmailAddressNeeded = bank.isEmailAddressNeeded;
            }
        }
    }

    ngOnDestroy(): void {
        this.store.dispatch(new bankLoginActions.ClearAll());
        this.destroyed$.next(true);
        this.destroyed$.complete();
        this.formChanges.emit(false);
    }

    createUpdateForm(data: BankLogin): void {
        this.isSecondPasswordNeeded = data.isSecondPasswordNeeded;
        this.bankId = data.bankId;

        this.bankLoginForm = this.fb.group({
            id: data.id,
            username: new FormControl(undefined),
            password: new FormControl(undefined),
            mobileusername: new FormControl(undefined),
            mobilepassword: new FormControl(undefined),
            friendlyName: new FormControl(data.friendlyName),
            secondPassword: new FormControl(undefined),
            mobileNumber: new FormControl(undefined),
            emailAddress: new FormControl(undefined),
            emailPassword: new FormControl(undefined),
            processCountIn24Hrs: new FormControl(undefined)
        });

        this.bankLoginForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: BankLogin = new BankLogin({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.bankLoginForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    private loadBankLoginDetail(): void {
        this.store.dispatch(new bankLoginActions.GetDetails(this.id));

        this.bankLoginGetDetail$.pipe(takeUntil(this.destroyed$)).subscribe(p => {
            if (p) {
                this.createUpdateForm(p);

                this.getIsEmailAddressNeeded();
            }
        });
    }

    getErrorMessage(control: FormControl): string {
        return FormHelper.getErrorMessage(control);
    }

    onUpdateSubmit(): void {
        this.formSubmit = false;

        if (this.bankLoginForm.valid) {

            var emailAddress : string = this.bankLoginForm.get('emailAddress').value;

            if(emailAddress){
                if(emailAddress.indexOf('@gmail') == -1 && emailAddress.indexOf('@yahoo') == -1){
                    this.openSnackBar(this.translateService.instant('BANK-LOGIN.GENERAL.WRONG-EMAIL-ADDRESS-PROVIDER'));
                    return;
                }
            }
            let form = this.bankLoginForm.value;
            this.formSubmit = true;
            this.store.dispatch(new bankLoginActions.UpdateLoginInformation(this.id, form));

            this.bankLoginUpdateSuccess$.pipe(filter(tnCreated => tnCreated !== false), take(1))
                .subscribe(
                    tnCreated => {
                        this.router.navigate(['/banklogins']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.bankLoginForm);
        }
    }

    isLoading() {
        return this.getDetailLoading;
    }

}


