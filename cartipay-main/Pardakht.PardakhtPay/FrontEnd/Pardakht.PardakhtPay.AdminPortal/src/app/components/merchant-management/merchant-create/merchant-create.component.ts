import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { FormGroup, FormControl, Validators, FormBuilder, FormArray } from '@angular/forms';
import { Store } from '@ngrx/store';
import { DateAdapter, MatSnackBar } from '@angular/material';
import * as coreState from '../../../core';
import { GenericHelper } from '../../../helpers/generic';
import { CustomValidators, FormHelper } from '../../../helpers/forms/form-helper';
import { debounceTime, distinctUntilChanged, take } from 'rxjs/operators';
import { MerchantCreate, MerchantEdit } from '../../../models/merchant-model';
import { fuseAnimations } from '../../../core/animations';
import * as merchantActions from '../../../core/actions/merchant';
import * as cardToCardActions from '../../../core/actions/cardToCardAccount';
import * as tenantActions from '../../../core/actions/tenant';
import { filter } from 'rxjs/operators/filter';
import { Router, ActivatedRoute } from '@angular/router';
import { RandomService } from '../../../core/services/random/random.service';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../../models/tenant';
import * as accountActions from '../../../core/actions/account';
import { Owner } from '../../../models/account.model';
import * as cardToCardAccountGroupActions from '../../../core/actions/cardToCardAccountGroup';
import * as mobileTransferCardAccountGroupActions from '../../../core/actions/mobileTransferAccountGroup';
import { CardToCardAccountGroup } from '../../../models/card-to-card-account-group';
import { AccountService } from '../../../core/services/account.service';
import * as permissions from '../../../models/permissions';
import { MobileTransferCardAccountGroup } from '../../../models/mobile-transfer';

@Component({
    selector: 'app-merchant-create',
    templateUrl: './merchant-create.component.html',
    styleUrls: ['./merchant-create.component.scss'],
    animations: fuseAnimations
})
export class MerchantCreateComponent implements OnInit, OnDestroy {

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
    merchantForm: FormGroup;
    formSubmit: boolean = false;
    pageType = '';
    isCreating: boolean;
    createError$: Observable<string>;
    updateError$: Observable<string>;
    getDetailError$: Observable<string>;

    merchantCreated$: Observable<MerchantCreate>;
    merchantGetDetail$: Observable<MerchantEdit>;
    getDetailLoading$: Observable<boolean>;
    getDetailLoading: boolean;
    merchantUpdateSuccess: Observable<boolean>;

    mobileTransferGroups$: Observable<MobileTransferCardAccountGroup[]>;
    mobileTransferGroups: MobileTransferCardAccountGroup[];
    mobileTransferGroupsLoading$: Observable<boolean>;
    mobileTransferGroupsLoading: boolean = false;
    mobileTransferGroupsError$: Observable<string>;

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

    @Output() formChanges: EventEmitter<boolean> = new EventEmitter();

    openedAccount: FormGroup;

    id: number;

    allowAddMerchant: boolean = false;

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
        this.allowAddMerchant = this.accountService.isUserAuthorizedForTask(permissions.AddMerchant);
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

        this.merchantCreated$ = this.store.select(coreState.getMerchantCreated);
        this.merchantGetDetail$ = this.store.select(coreState.getMerchantDetails);
        this.merchantUpdateSuccess = this.store.select(coreState.getMerchantEditSuccess);
        this.createError$ = this.store.select(coreState.getMerchantCreateError);
        this.updateError$ = this.store.select(coreState.getMerchantEditError);
        this.getDetailError$ = this.store.select(coreState.getMerchantDetailError);
        this.getDetailLoading$ = this.store.select(coreState.getMerchantDetailLoading);

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
                this.loadMerchantDetail();
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

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid && this.merchantForm && this.pageType == 'new') {
                this.merchantForm.get('tenantGuid').setValue(t.tenantDomainPlatformMapGuid);
                this.merchantForm.get('cardToCardAccountGroupId').setValue(undefined);
            }

            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new accountActions.GetOwners());
                this.store.dispatch(new cardToCardActions.Search(''));
                 this.store.dispatch(new cardToCardAccountGroupActions.GetAll());
                this.store.dispatch(new mobileTransferCardAccountGroupActions.GetAll());
            }
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

            if (this.accountGuid && this.merchantForm && this.pageType == 'new') {

                this.merchantForm.get('ownerGuid').setValue(this.getOwnerGuid());
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

        this.mobileTransferGroups$ = this.store.select(coreState.getAllMobileTransferCardAccountGroups);

        this.mobileTransferGroups$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            if (items == undefined) {
                this.mobileTransferGroups = [];
            }
            else {
                this.mobileTransferGroups = items;
            }
        });

        this.mobileTransferGroupsLoading$ = this.store.select(coreState.getMobileTransferCardAccountGroupLoading);

        this.mobileTransferGroupsLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.mobileTransferGroupsLoading = l;
        });

        this.mobileTransferGroupsError$ = this.store.select(coreState.getAllMobileTransferCardAccountGroupError);

        this.mobileTransferGroupsError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });
    }

    ngOnDestroy(): void {
        this.store.dispatch(new merchantActions.ClearErrors());
        this.store.dispatch(new cardToCardActions.ClearErrors());
        this.store.dispatch(new cardToCardAccountGroupActions.ClearErrors());
        this.store.dispatch(new mobileTransferCardAccountGroupActions.ClearErrors());
        this.destroyed$.next(true);
        this.destroyed$.complete();
        this.formChanges.emit(false);
    }

    createForm(): void {
        var apiKey = this.randomService.generateRandomKey();

        this.merchantForm = this.fb.group({
            title: new FormControl(undefined, [Validators.required]),
            domainAddress: new FormControl(undefined, [Validators.required]),
            isActive: new FormControl(true),
            apiKey: new FormControl(apiKey),
            cardToCardAccountGroupId: new FormControl(undefined),
            tenantGuid: new FormControl(this.selectedTenant == undefined ? undefined : this.selectedTenant.tenantDomainPlatformMapGuid, [Validators.required]),
            ownerGuid: new FormControl(this.getOwnerGuid(), [Validators.required]),
            minimumTransactionAmount: new FormControl(),
            mobileTransferAccountGroupId: new FormControl(undefined),
            useCardtoCardPaymentForWithdrawal: new FormControl(false),
            allowPartialPaymentForWithdrawals: new FormControl(false)
        });

        this.merchantForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: MerchantCreate = new MerchantCreate({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.merchantForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    createUpdateForm(data: MerchantEdit): void {

        this.merchantForm = this.fb.group({
            id: data.id,
            title: new FormControl(data.title, [Validators.required]),
            domainAddress: new FormControl(data.domainAddress, [Validators.required]),
            isActive: new FormControl(data.isActive),
            apiKey: new FormControl(data.apiKey),
            cardToCardAccountGroupId: new FormControl(data.cardToCardAccountGroupId),
            tenantGuid: new FormControl(data.tenantGuid, [Validators.required]),
            ownerGuid: new FormControl(data.ownerGuid, [Validators.required]),
            minimumTransactionAmount: new FormControl(data.minimumTransactionAmount),
            mobileTransferAccountGroupId: new FormControl(data.mobileTransferAccountGroupId),
            useCardtoCardPaymentForWithdrawal: new FormControl(data.useCardtoCardPaymentForWithdrawal),
            allowPartialPaymentForWithdrawals: new FormControl(data.allowPartialPaymentForWithdrawals)
        });

        this.merchantForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: MerchantCreate = new MerchantCreate({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.merchantForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    private loadMerchantDetail(): void {
        this.store.dispatch(new merchantActions.GetDetails(this.id));

        this.merchantGetDetail$.pipe(takeUntil(this.destroyed$)).subscribe(p => {
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

        if (this.merchantForm.valid) {
            let form = this.merchantForm.value;
            this.formSubmit = true;
            this.store.dispatch(new merchantActions.Create(form));

            this.merchantCreated$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;
                        this.router.navigate(['/merchants']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.merchantForm);
        }
    }

    onUpdateSubmit(): void {
        this.formSubmit = false;
        this.isCreating = true;

        if (this.merchantForm.valid) {
            let form = this.merchantForm.value;
            this.formSubmit = true;
            this.store.dispatch(new merchantActions.Edit(this.id, form));

            this.merchantUpdateSuccess.pipe(filter(tnCreated => tnCreated !== false), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;

                        this.router.navigate(['/merchants']);
                    });
        }
        else {
            FormHelper.validateFormGroup(this.merchantForm);
        }
    }

    isLoading() {
        return this.tenantsLoading || this.groupsLoading || this.getDetailLoading || this.isCreating || this.ownersLoading;
    }

    generateApiKey() {
        var key = this.randomService.generateRandomKey();
        this.merchantForm.controls['apiKey'].setValue(key);
    }

    getGroups() {

        var guid = this.merchantForm.get('tenantGuid').value;
        var ownerGuid = this.merchantForm.get('ownerGuid').value;

        if (guid && this.groups) {
            return this.groups.filter(t => t.tenantGuid == guid && t.ownerGuid == ownerGuid);
        }

        return [];
    }

    getMobileTransferGroups() {

        var guid = this.merchantForm.get('tenantGuid').value;
        var ownerGuid = this.merchantForm.get('ownerGuid').value;

        if (guid && this.mobileTransferGroups) {
            return this.mobileTransferGroups.filter(t => t.tenantGuid == guid && t.ownerGuid == ownerGuid);
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
}
