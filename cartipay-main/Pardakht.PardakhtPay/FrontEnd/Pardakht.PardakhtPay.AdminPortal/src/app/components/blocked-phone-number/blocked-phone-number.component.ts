import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { FormGroup, FormControl, Validators, FormBuilder, FormArray } from '@angular/forms';
import { Store } from '@ngrx/store';
import { DateAdapter, MatSnackBar } from '@angular/material';
import * as coreState from '../../core';
import { GenericHelper } from '../../helpers/generic';
import { CustomValidators, FormHelper } from '../../helpers/forms/form-helper';
import { debounceTime, distinctUntilChanged, take } from 'rxjs/operators';
import { fuseAnimations } from '../../core/animations';
import { filter } from 'rxjs/operators/filter';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../models/tenant';
import { Owner } from '../../models/account.model';
import * as accountActions from '../../core/actions/account';
import { BlockedPhoneNumber } from '../../models/blocked-phone-number';
import * as blockedPhoneNumberActions from '../../core/actions/blockedPhoneNumber';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';

@Component({
  selector: 'app-blocked-phone-number',
  templateUrl: './blocked-phone-number.component.html',
    styleUrls: ['./blocked-phone-number.component.scss'],
    animations: fuseAnimations
})
export class BlockedPhoneNumberComponent implements OnInit, OnDestroy {

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
    blockedPhoneNumberForm: FormGroup;
    formSubmit: boolean = false;
    pageType = '';
    isCreating: boolean;
    createError$: Observable<string>;
    updateError$: Observable<string>;
    getDetailError$: Observable<string>;
    getDetailLoading$: Observable<boolean>;
    getDetailLoading: boolean;

    blockedPhoneNumberCreated$: Observable<BlockedPhoneNumber>;
    blockedPhoneNumberGetDetail$: Observable<BlockedPhoneNumber>;
    blockedPhoneNumberUpdateSuccess: Observable<boolean>;
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

    searchError$: Observable<string>;
    selected = [];
    loading$: Observable<boolean>;
    loading: boolean;

    id: number;

    allowAddBlockedPhoneNumber: boolean = false;

    constructor(private store: Store<coreState.State>,
        private router: Router,
        private fb: FormBuilder,
        public snackBar: MatSnackBar,
        private route: ActivatedRoute,
        private accountService: AccountService,
        private translateService: TranslateService) {

        this.allowAddBlockedPhoneNumber = this.accountService.isUserAuthorizedForTask(permissions.AddBlockedPhoneNumber);
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

        this.blockedPhoneNumberCreated$ = this.store.select(coreState.getBlockedPhoneNumberCreated);
        this.blockedPhoneNumberGetDetail$ = this.store.select(coreState.getBlockedPhoneNumberDetails);
        this.blockedPhoneNumberUpdateSuccess = this.store.select(coreState.getBlockedPhoneNumberEditSuccess);
        this.createError$ = this.store.select(coreState.getBlockedPhoneNumberCreateError);
        this.updateError$ = this.store.select(coreState.getBlockedPhoneNumberEditError);
        this.getDetailError$ = this.store.select(coreState.getBlockedPhoneNumberDetailError);
        this.getDetailLoading$ = this.store.select(coreState.getBlockedPhoneNumberDetailLoading);

        this.tenantsLoading$ = this.store.select(coreState.getTenantLoading);
        this.tenants$ = this.store.select(coreState.getTenantSearchResults);

        this.loading$ = this.store.select(coreState.getMobileTransferCardAccountLoading);

        this.loading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.loading = l;
        });

        this.searchError$ = this.store.select(coreState.getAllMobileTransferCardAccountError);

        this.searchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.route.params.subscribe(params => {

            this.id = params['id'];

            if (this.id != 0 && this.id != null && this.id != undefined) {
                this.pageType = 'edit';
                this.loadBlockedPhoneNumberDetail();
            }
            else {
                this.pageType = 'new';
                this.createForm();
            }
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;
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
        this.store.dispatch(new blockedPhoneNumberActions.ClearErrors());
        this.destroyed$.next(true);
        this.destroyed$.complete();
        this.formChanges.emit(false);
    }

    createForm(): void {
        this.blockedPhoneNumberForm = this.fb.group({
            phoneNumber: new FormControl(undefined, [Validators.required])
        });

        this.blockedPhoneNumberForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: BlockedPhoneNumber = new BlockedPhoneNumber({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.blockedPhoneNumberForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    createUpdateForm(data: BlockedPhoneNumber): void {

        this.blockedPhoneNumberForm = this.fb.group({
            id: data.id,
            phoneNumber: new FormControl({ value: data.phoneNumber, disabled: !this.allowAddBlockedPhoneNumber }, [Validators.required]),
        });

        this.blockedPhoneNumberForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: BlockedPhoneNumber = new BlockedPhoneNumber({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.blockedPhoneNumberForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    private loadBlockedPhoneNumberDetail(): void {
        this.store.dispatch(new blockedPhoneNumberActions.GetDetails(this.id));

        this.blockedPhoneNumberGetDetail$.pipe(takeUntil(this.destroyed$)).subscribe(p => {
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

        if (this.blockedPhoneNumberForm.valid) {
            let form = this.blockedPhoneNumberForm.value;
            this.formSubmit = true;
            this.store.dispatch(new blockedPhoneNumberActions.Create(form));

            this.blockedPhoneNumberCreated$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;
                        this.router.navigate(['/blockedphonenumbers']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.blockedPhoneNumberForm);
        }
    }

    onUpdateSubmit(): void {
        this.formSubmit = false;

        if (this.blockedPhoneNumberForm.valid) {
            let form = this.blockedPhoneNumberForm.value;
            this.formSubmit = true;
            this.store.dispatch(new blockedPhoneNumberActions.Edit(this.id, form));

            this.blockedPhoneNumberUpdateSuccess.pipe(filter(tnCreated => tnCreated !== false), take(1))
                .subscribe(
                    tnCreated => {
                        this.router.navigate(['/blockedphonenumbers']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.blockedPhoneNumberForm);
        }
    }

    isLoading() {
        return this.getDetailLoading;
    }

    getOwnerGuid() {
        if (this.parentGuid == undefined || this.parentGuid == '' || this.parentGuid == null) {
            return this.accountGuid;
        }

        return this.parentGuid;
    }

}


