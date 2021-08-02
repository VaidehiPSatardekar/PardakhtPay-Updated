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
import * as merchantCustomerActions from '../../core/actions/merchantCustomer';
import { filter } from 'rxjs/operators/filter';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { BankLogin, BankAccount, BankLoginStatus, CreateLoginFromLoginRequestDTO, LoginTypeEnum, LoginTypes, QRRegisterRequestStatus, QrCodeRegistrationRequest } from '../../models/bank-login';
import 'rxjs/add/observable/interval';
import { Tenant } from '../../models/tenant';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';
import { RegisteredPhoneNumbers } from '../../models/user-segment-group';
import { debug } from 'util';


@Component({
    selector: 'app-remove-registered-phone',
    templateUrl: './remove-registered-phone.component.html',
    styleUrls: ['./remove-registered-phone.component.scss'],
    animations: fuseAnimations
})
export class RemoveRegisteredPhoneNumbersComponent implements OnInit, OnDestroy {

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

    registeredPhoneNumbers: RegisteredPhoneNumbers[];
    registeredPhoneNumbers$: Observable<RegisteredPhoneNumbers[]>;
    getRegisteredPhoneNumbersLoading$: Observable<boolean>;
    getRegisteredPhoneNumbersLoading: boolean;

    removeRegisteredPhone$: Observable<boolean>;
    removeRegisteredPhoneError$: Observable<string>;

    removeRegisteredPhoneForm: FormGroup;
    formSubmit: boolean = false;  

    allowRemoveRegisterLogin: boolean = false;

    interval: any = undefined;

    intervalMiliseconds = 10000;

    formCreated: boolean = false;

    id: number;

    constructor(private store: Store<coreState.State>,
        private dateAdapter: DateAdapter<any>,
        private router: Router,
        private fb: FormBuilder,
        public snackBar: MatSnackBar,
        private route: ActivatedRoute,
        private accountService: AccountService,
        private translateService: TranslateService) {
        this.dateAdapter.setLocale('gb');
        this.allowRemoveRegisterLogin = this.accountService.isUserAuthorizedForTask(permissions.AddBankLogin);
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
        this.store.select(coreState.qrCodeRegisterError).pipe(takeUntil(this.destroyed$)).subscribe(error =>{
            if(error){
                this.snackBar.open(error, this.translateService.instant('OK'));
            }
        });

        this.registeredPhoneNumbers$ = this.store.select(coreState.getRegisteredPhoneNumbers);
        this.getRegisteredPhoneNumbersLoading$ = this.store.select(coreState.getRegisteredPhoneNumbersLoading);
        this.getRegisteredPhoneNumbersLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.getRegisteredPhoneNumbersLoading = l;
        });

        this.registeredPhoneNumbers$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            if (items != undefined) {
                if (items.length > 0) {
                    this.registeredPhoneNumbers = items;
                }
                else {
                    this.openSnackBar("No phone number registered for this merchant.");
                    this.router.navigate(['/merchantcustomers']);
                }
            }            
        });

        this.removeRegisteredPhone$ = this.store.select(coreState.removeRegisteredPhoneNumbersSuccess);

        this.removeRegisteredPhoneError$ = this.store.select(coreState.removeRegisteredPhoneNumbersError);

        this.removeRegisteredPhoneError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.createForm();
        this.route.params.subscribe(params => {
            this.id = params['id'];
            if (this.id != 0 && this.id != null && this.id != undefined) {
                this.loadDetail();
            }
        });
        
    }

    loadDetail() {        
        this.store.dispatch(new merchantCustomerActions.GetRegisteredPhones(this.id));
    }

    createForm(): void {
        this.removeRegisteredPhoneForm = undefined;
        this.removeRegisteredPhoneForm = this.fb.group({            
            phoneNumber: new FormControl(undefined, [Validators.required])
        });
    }


    onSubmit(): void {
        if (this.removeRegisteredPhoneForm.valid) {
            let form = this.removeRegisteredPhoneForm.value;

            this.store.dispatch(new merchantCustomerActions.RemoveRegisteredPhones(this.id,form));
            this.removeRegisteredPhone$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
                .subscribe(
                    tnCreated => {
                        this.router.navigate(['/merchantcustomers']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.removeRegisteredPhoneForm);
        }
    }

    getRegisteredPhoneNumbers() {
          if (this.registeredPhoneNumbers == undefined) {
            return [];
        }

        return this.registeredPhoneNumbers;//.filter(t => t.ownerGuid == this.merchantCustomer.ownerGuid && t.isDefault == false);
    }

    unsubscribeInterval() {
        if (this.interval) {
            this.interval.unsubscribe();
        }
    }

    isLoading(): boolean {
        return this.getRegisteredPhoneNumbersLoading;
    }

    ngOnDestroy(): void {
        this.store.dispatch(new bankLoginActions.ClearAll());
        this.unsubscribeInterval();
        this.destroyed$.next(true);
        this.destroyed$.complete();
    }
}
