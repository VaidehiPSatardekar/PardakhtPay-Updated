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
import * as mobileTransferCardAccountActions from '../../core/actions/mobileTransferCardAccount';
import { filter } from 'rxjs/operators/filter';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../models/tenant';
import { Owner } from '../../models/account.model';
import * as accountActions from '../../core/actions/account';
import { MobileTransferCardAccountGroup, MobileTransferCardAccountGroupItem, MobileTransferCardAccount, PaymentProviderTypes } from '../../models/mobile-transfer';
import * as mobileTransferCardAccountGroupActions from '../../core/actions/mobileTransferAccountGroup';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';
import { UserSegmentGroup } from '../../models/user-segment-group';
import * as userSegmentGroupActions from '../../core/actions/userSegmentGroup';

@Component({
    selector: 'app-mobile-transfer-account-group',
    templateUrl: './mobile-transfer-account-group.component.html',
    styleUrls: ['./mobile-transfer-account-group.component.scss'],
    animations: fuseAnimations
})
export class MobileTransferAccountGroupComponent implements OnInit, OnDestroy {

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
    mobileTransferCardAccountGroupForm: FormGroup;
    formSubmit: boolean = false;
    pageType = '';
    isCreating: boolean;
    createError$: Observable<string>;
    updateError$: Observable<string>;
    getDetailError$: Observable<string>;
    getDetailLoading$: Observable<boolean>;
    getDetailLoading: boolean;

    userSegmentGroups$: Observable<UserSegmentGroup[]>;
    userSegmentGroups: UserSegmentGroup[];
    userSegmentGroupLoadingError$: Observable<string>;
    getUserSegmentGroupsLoading$: Observable<boolean>;
    getUserSegmentGroupsLoading: boolean;

    mobileTransferCardAccountGroupCreated$: Observable<MobileTransferCardAccountGroup>;
    mobileTransferCardAccountGroupGetDetail$: Observable<MobileTransferCardAccountGroup>;
    mobileTransferCardAccountGroupUpdateSuccess: Observable<boolean>;
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

    cardToCardAccounts$: Observable<MobileTransferCardAccount[]>;
    cardToCardAccounts: MobileTransferCardAccount[];
    searchError$: Observable<string>;
    selected = [];
    loading$: Observable<boolean>;
    loading: boolean;
    paymentTypes = PaymentProviderTypes;

    id: number;

    allowAddBankAccountGroup: boolean = false;

    constructor(private store: Store<coreState.State>,
        private router: Router,
        private fb: FormBuilder,
        public snackBar: MatSnackBar,
        private route: ActivatedRoute,
        private accountService: AccountService,
        private translateService: TranslateService) {
        this.allowAddBankAccountGroup = this.accountService.isUserAuthorizedForTask(permissions.AddMobileTransferAccountGroup);
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

        this.mobileTransferCardAccountGroupCreated$ = this.store.select(coreState.getMobileTransferCardAccountGroupCreated);
        this.mobileTransferCardAccountGroupGetDetail$ = this.store.select(coreState.getMobileTransferCardAccountGroupDetails);
        this.mobileTransferCardAccountGroupUpdateSuccess = this.store.select(coreState.getMobileTransferCardAccountGroupEditSuccess);
        this.createError$ = this.store.select(coreState.getMobileTransferCardAccountGroupCreateError);
        this.updateError$ = this.store.select(coreState.getMobileTransferCardAccountGroupEditError);
        this.getDetailError$ = this.store.select(coreState.getMobileTransferCardAccountGroupDetailError);
        this.getDetailLoading$ = this.store.select(coreState.getMobileTransferCardAccountGroupDetailLoading);

        this.tenantsLoading$ = this.store.select(coreState.getTenantLoading);
        this.tenants$ = this.store.select(coreState.getTenantSearchResults);

        this.cardToCardAccounts$ = this.store.select(coreState.getAllMobileTransferCardAccounts);

        this.cardToCardAccounts$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.cardToCardAccounts = items;
            console.log(items);
        });

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
                this.loadMobileTransferCardAccountGroupDetail();
            }
            else {
                this.pageType = 'new';
                this.createForm();
            }
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid && this.mobileTransferCardAccountGroupForm && this.pageType == 'new') {
                this.mobileTransferCardAccountGroupForm.get('tenantGuid').setValue(t.tenantDomainPlatformMapGuid);
            }

            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new accountActions.GetOwners());
                this.store.dispatch(new mobileTransferCardAccountActions.GetAll());
                this.store.dispatch(new userSegmentGroupActions.GetAll());
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

            if (this.accountGuid && this.mobileTransferCardAccountGroupForm && this.pageType == 'new') {

                this.mobileTransferCardAccountGroupForm.get('ownerGuid').setValue(this.getOwnerGuid());
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

        this.userSegmentGroups$ = this.store.select(coreState.getAllUserSegmentGroups);

        this.userSegmentGroups$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.userSegmentGroups = items;
        });

        this.getUserSegmentGroupsLoading$ = this.store.select(coreState.getUserSegmentGroupLoading);

        this.getUserSegmentGroupsLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.getUserSegmentGroupsLoading = l;
        });

        this.userSegmentGroupLoadingError$ = this.store.select(coreState.getAllUserSegmentGroupError);

        this.userSegmentGroupLoadingError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });
    }

    ngOnDestroy(): void {
        this.store.dispatch(new mobileTransferCardAccountActions.ClearErrors());
        this.store.dispatch(new mobileTransferCardAccountGroupActions.ClearErrors());
        this.store.dispatch(new mobileTransferCardAccountActions.Clear());
        this.store.dispatch(new userSegmentGroupActions.Clear());
        this.destroyed$.next(true);
        this.destroyed$.complete();
        this.formChanges.emit(false);
    }

    createForm(): void {
        this.mobileTransferCardAccountGroupForm = this.fb.group({
            name: new FormControl(undefined, [Validators.required]),
            tenantGuid: new FormControl(this.selectedTenant == undefined ? undefined : this.selectedTenant.tenantDomainPlatformMapGuid, [Validators.required]),
            ownerGuid: new FormControl(this.getOwnerGuid(), [Validators.required]),
            items: this.fb.array([])
        });

        this.mobileTransferCardAccountGroupForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: MobileTransferCardAccountGroup = new MobileTransferCardAccountGroup({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.mobileTransferCardAccountGroupForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    createUpdateForm(data: MobileTransferCardAccountGroup): void {

        var array: FormGroup[] = [];

        if (data.items != null) {
            for (var i = 0; i < data.items.length; i++) {
                var item = data.items[i];

                var groups = item.userSegmentGroups;

                item.userSegmentGroups = [];

                var control = this.fb.group(item);

                control.get('userSegmentGroups').setValue(groups);

                if (!this.allowAddBankAccountGroup) {
                    control.disable();
                }

                array.push(control);
            }
        }

        this.mobileTransferCardAccountGroupForm = this.fb.group({
            id: data.id,
            name: new FormControl({ value: data.name, disabled: !this.allowAddBankAccountGroup }, [Validators.required]),
            tenantGuid: new FormControl({ value: data.tenantGuid, disabled: !this.allowAddBankAccountGroup }, [Validators.required]),
            ownerGuid: new FormControl({ value: data.ownerGuid, disabled: !this.allowAddBankAccountGroup }, [Validators.required]),
            items: this.fb.array(array)
        });

        this.mobileTransferCardAccountGroupForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: MobileTransferCardAccountGroup = new MobileTransferCardAccountGroup({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.mobileTransferCardAccountGroupForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    private loadMobileTransferCardAccountGroupDetail(): void {
        this.store.dispatch(new mobileTransferCardAccountGroupActions.GetDetails(this.id));

        this.mobileTransferCardAccountGroupGetDetail$.pipe(takeUntil(this.destroyed$)).subscribe(p => {
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

        if (this.mobileTransferCardAccountGroupForm.valid) {
            let form = this.mobileTransferCardAccountGroupForm.value;
            this.formSubmit = true;
            this.store.dispatch(new mobileTransferCardAccountGroupActions.Create(form));

            this.mobileTransferCardAccountGroupCreated$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;
                        this.router.navigate(['/mobiletransferaccountgroups']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.mobileTransferCardAccountGroupForm);
        }
    }

    onUpdateSubmit(): void {
        this.formSubmit = false;

        if (this.mobileTransferCardAccountGroupForm.valid) {
            let form = this.mobileTransferCardAccountGroupForm.value;
            this.formSubmit = true;
            this.store.dispatch(new mobileTransferCardAccountGroupActions.Edit(this.id, form));

            this.mobileTransferCardAccountGroupUpdateSuccess.pipe(filter(tnCreated => tnCreated !== false), take(1))
                .subscribe(
                    tnCreated => {
                        this.router.navigate(['/mobiletransferaccountgroups']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.mobileTransferCardAccountGroupForm);
        }
    }

    getUserSegmentGroups() {

        if (this.userSegmentGroups == undefined) {
            return [];
        }

        var ownerGuid = this.mobileTransferCardAccountGroupForm.get('ownerGuid').value;

        if (ownerGuid == undefined) {
            return [];
        }
        return this.userSegmentGroups.filter(t => t.ownerGuid == ownerGuid && t.isDefault == false);
    }

    getAccounts() {
        if (this.cardToCardAccounts == undefined || this.mobileTransferCardAccountGroupForm == undefined) {
            return [];
        }

        var items = this.mobileTransferCardAccountGroupForm.get('items').value;

        var ownerGuid = this.mobileTransferCardAccountGroupForm.get('ownerGuid').value;

        if (items == undefined || ownerGuid == undefined) {
            return [];
        }

        return this.cardToCardAccounts.filter(t => t.ownerGuid == ownerGuid && items.find(p => p.itemId == t.id) == null);
    }

    getCardNumber(accountId: number): string {
        if (this.cardToCardAccounts == undefined) {
            return '';
        }

        var account = this.cardToCardAccounts.find(t => t.id == accountId);

        if (account != null) {
            if (account.paymentProviderType == this.paymentTypes.Cartipal) {
                return account.cardNumber != null ?  account.cardNumber : '';
            } else {
                return account.merchantId != null ? account.merchantId : '';
            }
        }

        return '';
    }

    getAccountNumber(accountId: number): string {
        if (this.cardToCardAccounts == undefined) {
            return '';
        }

        var account = this.cardToCardAccounts.find(t => t.id == accountId);

        if (account != null) {
            return account.cardNumber;
        }

        return '';
    }

    getFriendlyName(accountId: number): string {
        if (this.cardToCardAccounts == undefined) {
            return '';
        }

        var account = this.cardToCardAccounts.find(t => t.id == accountId);

        if (account != null) {
            if (account.paymentProviderType == this.paymentTypes.Cartipal) {
                return account.cardHolderName;
            }
            else {
                return account.title;
            }
        }

        return '';
    }

    addAccount(account: MobileTransferCardAccount) {
        var items = this.mobileTransferCardAccountGroupForm.get('items') as FormArray;

        var item = new MobileTransferCardAccountGroupItem();
        item.itemId = account.id;
        item.groupId = this.id == undefined ? 0 : this.id;
        item.status = 1;
        item.userSegmentGroups = [];

        var group = this.fb.group(item);

        items.push(group);
    }

    deleteAccount(account) {
        var items = this.mobileTransferCardAccountGroupForm.get('items') as FormArray;
        var index = items.controls.indexOf(account);

        if (index != -1) {
            items.removeAt(index);
        }
    }

    getOwners() {
        if (this.owners) {
            var list = this.owners;

            if (list.length == 0) {
                this.mobileTransferCardAccountGroupForm.get('ownerGuid').setValue(undefined);
            }
            return list;
        }

        this.mobileTransferCardAccountGroupForm.get('ownerGuid').setValue(undefined);
        return [];
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


