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
import { Tenant } from '../../models/tenant';
import { Owner } from '../../models/account.model';
import * as tenantActions from '../../core/actions/tenant';
import * as accountActions from '../../core/actions/account';
import { Bank } from '../../models/bank';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';

@Component({
  selector: 'app-bank-login',
  templateUrl: './bank-login.component.html',
    styleUrls: ['./bank-login.component.scss'],
    animations: fuseAnimations
})
export class BankLoginComponent implements OnInit, OnDestroy {

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
    bankLoginForm: FormGroup;
    formSubmit: boolean = false;
    pageType = 'new';
    isCreating: boolean;
    createError$: Observable<string>;
    getDetailError$: Observable<string>;
    getDetailLoading$: Observable<boolean>;
    getDetailLoading: boolean;

    bankLoginCreated$: Observable<BankLogin>;
    bankLoginGetDetail$: Observable<BankLogin>;
    bankLoginUpdateSuccess$: Observable<boolean>;

    banks$: Observable<Bank[]>;
    banksLoading$: Observable<boolean>;
    banksLoading: boolean;
    banksError$: Observable<string>;

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

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    owners$: Observable<Owner[]>;
    owners: Owner[] = [];
    ownersLoading$: Observable<boolean>;
    ownersLoading: boolean = false;

    id: number;

    allowAddBankLogin: boolean = false;

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
        this.bankLoginCreated$ = this.store.select(coreState.getBankLoginCreated);
        this.bankLoginGetDetail$ = this.store.select(coreState.getBankLoginDetail);
        this.createError$ = this.store.select(coreState.getBankLoginCreateError);
        this.getDetailError$ = this.store.select(coreState.getBankLoginDetailError);
        this.getDetailLoading$ = this.store.select(coreState.getBankLoginDetailLoading);
        

        this.tenantsLoading$ = this.store.select(coreState.getTenantLoading);
        this.tenants$ = this.store.select(coreState.getTenantSearchResults);

        this.banks$ = this.store.select(coreState.getBanksSearchResult);
        this.banksLoading$ = this.store.select(coreState.getBanksLoading);
        this.banksError$ = this.store.select(coreState.getBanksSearchError);
        this.bankLoginUpdateSuccess$ = this.store.select(coreState.getBankLoginUpdateSuccess);

        this.route.params.subscribe(params => {
            this.id = params['id'];

            if (this.id != 0 && this.id != null && this.id != undefined) {
                this.pageType = 'edit';
                this.loadBankLoginDetail();
            }
            else {
                this.pageType = 'new';
                this.createForm();
            }
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid && this.bankLoginForm && this.pageType == 'new') {
                this.bankLoginForm.get('tenantGuid').setValue(t.tenantDomainPlatformMapGuid);
            }

            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new accountActions.GetOwners());
                this.store.dispatch(new bankLoginActions.Search(false));
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
            if (this.bankLoginForm) {
                this.bankLoginForm.get('ownerGuid').setValue(t);
            }
        });

        this.tenantsLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.tenantsLoading = l;
        });

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

        this.banksLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.banksLoading = l;
        });

        this.banksError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });
    }

    ngOnDestroy(): void {
        this.store.dispatch(new bankLoginActions.ClearAll());
        this.destroyed$.next(true);
        this.destroyed$.complete();
        this.formChanges.emit(false);
    }

    createForm(): void {
        this.bankLoginForm = this.fb.group({
            tenantGuid: new FormControl(this.selectedTenant == undefined ? undefined : this.selectedTenant.tenantDomainPlatformMapGuid, [Validators.required]),
            ownerGuid: new FormControl(this.accountGuid, [Validators.required]),
            bankId: new FormControl(undefined, [Validators.required]),
            friendlyName: new FormControl(undefined, [Validators.required]),
            username: new FormControl(undefined),
            password: new FormControl(undefined),
            mobileusername: new FormControl(undefined),
            mobilepassword: new FormControl(undefined),
            mobilenumber: new FormControl(undefined)
            //isMobileLogin: new FormControl(false)
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

    createUpdateForm(data: BankLogin): void {

        var array: FormGroup[] = [];

        this.bankLoginForm = this.fb.group({
            id: data.id,
            tenantGuid: new FormControl({ value: data.tenantGuid, disabled: true }),
            ownerGuid: new FormControl({ value: data.ownerGuid, disabled: true }),
            bankId: new FormControl({value: data.bankId, disabled: true}),
            friendlyName: new FormControl(data.friendlyName, [Validators.required]),
            isBlockCard: new FormControl(data.isBlockCard)
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
            }
        });
    }

    getErrorMessage(control: FormControl): string {
        return FormHelper.getErrorMessage(control);
    }

    onSubmit(): void {
        this.formSubmit = false;
        if (this.bankLoginForm.valid) {
            var userName: string = this.bankLoginForm.get('username').value;
            var password: string = this.bankLoginForm.get('password').value;
            var mobileUsername: string = this.bankLoginForm.get('mobileusername').value;
            var mobilePassword: string = this.bankLoginForm.get('mobilepassword').value;

            if (userName) {
                if (!password) {
                    this.openSnackBar(this.translateService.instant('BANK-LOGIN.GENERAL.PASSWORD-REQUIRED'));
                    return;
                }
            }
            if (mobileUsername) {
                if (!mobilePassword) {
                    this.openSnackBar(this.translateService.instant('BANK-LOGIN.GENERAL.PASSWORD-REQUIRED'));
                    return;
                }
            }
            if (password) {
                if (!userName) {
                    this.openSnackBar(this.translateService.instant('BANK-LOGIN.GENERAL.USERNAME-REQUIRED'));
                    return;
                }
            }
            if (mobilePassword) {
                if (!mobileUsername) {
                    this.openSnackBar(this.translateService.instant('BANK-LOGIN.GENERAL.USERNAME-REQUIRED'));
                    return;
                }
            }
            if (!userName && !mobileUsername) {
                this.openSnackBar(this.translateService.instant('BANK-LOGIN.GENERAL.LOGIN-CREDENTIAL-REQUIRED'));
                return;
            }


            let form = this.bankLoginForm.value;
            this.formSubmit = true;
            this.store.dispatch(new bankLoginActions.Create(form));

            this.bankLoginCreated$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;
                        this.router.navigate(['/bankloginaccount/' + tnCreated.id]);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.bankLoginForm);
        }
    }

    onUpdateSubmit(): void {
        this.formSubmit = false;

        if (this.bankLoginForm.valid) {
            let form = this.bankLoginForm.value;
            
            this.formSubmit = true;
            this.store.dispatch(new bankLoginActions.Edit(this.id, form));

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

    getOwners() {
        if (this.owners == undefined || this.owners == null) {
            return [];
        }

        return this.owners;
    }

    isLoading() {
        return this.getDetailLoading || this.ownersLoading || this.banksLoading;
    }

}

