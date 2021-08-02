import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { FormGroup, FormControl, Validators, FormBuilder, FormArray } from '@angular/forms';
import { Store } from '@ngrx/store';
import { DateAdapter, MatSnackBar } from '@angular/material';
import * as coreState from '../../core';
import { GenericHelper } from '../../helpers/generic';
import { CustomValidators, FormHelper } from '../../helpers/forms/form-helper';
import { debounceTime, distinctUntilChanged, take } from 'rxjs/operators';
import { TransferAccount } from '../../models/transfer-account';
import { fuseAnimations } from '../../core/animations';
import * as transferAccountActions from '../../core/actions/transferAccount';
import * as accountActions from '../../core/actions/account';
import { filter } from 'rxjs/operators/filter';
import { Router, ActivatedRoute } from '@angular/router';
import { RandomService } from '../../core/services/random/random.service';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../models/tenant';
import * as tenantActions from '../../core/actions/tenant';
import { Owner } from '../../models/account.model';
import { ibanValidator } from '../../validators/iban-validator';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';

@Component({
  selector: 'app-transfer-account',
  templateUrl: './transfer-account.component.html',
    styleUrls: ['./transfer-account.component.scss'],
    animations: fuseAnimations
})
export class TransferAccountComponent implements OnInit, OnDestroy {

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
    transferAccountForm: FormGroup;
    formSubmit: boolean = false;
    pageType = 'new';
    isCreating: boolean;
    createError$: Observable<string>;
    updateError$: Observable<string>;
    getDetailError$: Observable<string>;
    getDetailLoading$: Observable<boolean>;
    getDetailLoading: boolean;

    transferAccountCreated$: Observable<TransferAccount>;
    transferAccountGetDetail$: Observable<TransferAccount>;
    transferAccountUpdateSuccess: Observable<boolean>;

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

    owners$: Observable<Owner[]>;
    owners: Owner[] = [];
    ownersLoading$: Observable<boolean>;
    ownersLoading: boolean = false;

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    @Output() formChanges: EventEmitter<boolean> = new EventEmitter();

    id: number;

    parentGuid: string;

    allowAddTransferAccount: boolean = false;

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

        this.parentGuid = this.accountService.getParentAccountId();
        this.allowAddTransferAccount = this.accountService.isUserAuthorizedForTask(permissions.AddTransferAccount);
    }

    openSnackBar(message: string, action: string = undefined) {
        console.log(message);
        if (!action) {
            action = this.translateService.instant('GENERAL.OK');
        }
        this.snackBar.open(message, action, {
            duration: 10000,
        });
    }

    ngOnInit() {
        this.transferAccountCreated$ = this.store.select(coreState.getTransferAccountCreated);
        this.transferAccountGetDetail$ = this.store.select(coreState.getTransferAccountDetails);
        this.transferAccountUpdateSuccess = this.store.select(coreState.getTransferAccountEditSuccess);
        this.createError$ = this.store.select(coreState.getTransferAccountCreateError);
        this.updateError$ = this.store.select(coreState.getTransferAccountEditError);
        this.getDetailError$ = this.store.select(coreState.getTransferAccountDetailError);
        this.getDetailLoading$ = this.store.select(coreState.getTransferAccountDetailLoading);

        this.tenantsLoading$ = this.store.select(coreState.getTenantLoading);
        this.tenants$ = this.store.select(coreState.getTenantSearchResults);

        this.route.params.subscribe(params => {

            this.id = params['id'];

            if (this.id != 0 && this.id != null && this.id != undefined) {
                this.pageType = 'edit';
                this.loadTransferAccountDetail();
            }
            else {
                this.pageType = 'new';
                this.createForm();
            }
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid && this.transferAccountForm && this.pageType == 'new') {
                this.transferAccountForm.get('tenantGuid').setValue(t.tenantDomainPlatformMapGuid);
            }

            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new accountActions.GetOwners());
            }
        });

        this.createError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            console.log(error);
            if (error != undefined) {
                this.isCreating = false;
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

            if (this.accountGuid && this.transferAccountForm && this.pageType == 'new') {
                this.transferAccountForm.get('ownerGuid').setValue(this.getOwnerGuid());
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
    }

    ngOnDestroy(): void {
        this.store.dispatch(new transferAccountActions.ClearErrors());
        this.destroyed$.next(true);
        this.destroyed$.complete();
        this.formChanges.emit(false);
    }

    createForm(): void {
        this.transferAccountForm = this.fb.group({
            accountNo: new FormControl(undefined, [Validators.required]),
            accountHolderFirstName: new FormControl(undefined, [Validators.required]),
            accountHolderLastName: new FormControl(undefined, [Validators.required]),
            isActive: new FormControl(true),
            iban: new FormControl(undefined, [Validators.required, ibanValidator]),
            friendlyName: new FormControl(undefined, [Validators.required]),
            tenantGuid: new FormControl(this.selectedTenant == undefined ? undefined : this.selectedTenant.tenantDomainPlatformMapGuid, [Validators.required]),
            ownerGuid: new FormControl(this.getOwnerGuid(), [Validators.required])
        });

        this.transferAccountForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: TransferAccount = new TransferAccount({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.transferAccountForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    createUpdateForm(data: TransferAccount): void {

        var array: FormGroup[] = [];

        this.transferAccountForm = this.fb.group({
            id: data.id,
            accountNo: new FormControl(data.accountNo, [Validators.required]),
            accountHolderFirstName: new FormControl(data.accountHolderFirstName, [Validators.required]),
            accountHolderLastName: new FormControl(data.accountHolderLastName, [Validators.required]),
            isActive: new FormControl(data.isActive),
            iban: new FormControl(data.iban, [Validators.required]),
            friendlyName: new FormControl(data.friendlyName, [Validators.required]),
            tenantGuid: new FormControl(data.tenantGuid, [Validators.required]),
            ownerGuid: new FormControl(data.ownerGuid, [Validators.required])
        });

        this.transferAccountForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: TransferAccount = new TransferAccount({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.transferAccountForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    private loadTransferAccountDetail(): void {
        this.store.dispatch(new transferAccountActions.GetDetails(this.id));

        this.transferAccountGetDetail$.pipe(takeUntil(this.destroyed$)).subscribe(p => {
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

        if (this.transferAccountForm.valid) {
            let form = this.transferAccountForm.value;
            this.formSubmit = true;
            this.store.dispatch(new transferAccountActions.Create(form));
            this.isCreating = true;
            this.transferAccountCreated$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;
                        this.router.navigate(['/transferaccounts']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.transferAccountForm);
        }
    }

    onUpdateSubmit(): void {
        this.formSubmit = false;

        if (this.transferAccountForm.valid) {
            let form = this.transferAccountForm.value;
            this.formSubmit = true;
            this.store.dispatch(new transferAccountActions.Edit(this.id, form));
            this.isCreating = true;
            this.transferAccountUpdateSuccess.pipe(filter(tnCreated => tnCreated !== false), take(1))
                .subscribe(
                    tnCreated => {
                        this.router.navigate(['/transferaccounts']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.transferAccountForm);
        }
    }

    onLoginChanged() {

    }

    isLoading(): boolean {
        return this.getDetailLoading || this.tenantsLoading || this.isCreating;
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
}

