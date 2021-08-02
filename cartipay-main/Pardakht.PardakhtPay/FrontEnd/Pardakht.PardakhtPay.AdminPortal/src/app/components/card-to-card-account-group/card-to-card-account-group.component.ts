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
import { filter } from 'rxjs/operators/filter';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../models/tenant';
import { Owner } from '../../models/account.model';
import * as accountActions from '../../core/actions/account';
import { CardToCardAccountGroup, CardToCardAccountGroupItem } from '../../models/card-to-card-account-group';
import * as cardToCardAccountGroupActions from '../../core/actions/cardToCardAccountGroup';
import * as userSegmentGroupActions from '../../core/actions/userSegmentGroup';
import { UserSegmentGroup } from '../../models/user-segment-group';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';

@Component({
  selector: 'app-card-to-card-account-group',
  templateUrl: './card-to-card-account-group.component.html',
    styleUrls: ['./card-to-card-account-group.component.scss'],
    animations: fuseAnimations
})
export class CardToCardAccountGroupComponent implements OnInit, OnDestroy {

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
    cardToCardAccountGroupForm: FormGroup;
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

    cardToCardAccountGroupCreated$: Observable<CardToCardAccountGroup>;
    cardToCardAccountGroupGetDetail$: Observable<CardToCardAccountGroup>;
    cardToCardAccountGroupUpdateSuccess: Observable<boolean>;
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

    cardToCardAccounts$: Observable<CardToCardAccount[]>;
    cardToCardAccounts: CardToCardAccount[];
    searchError$: Observable<string>;
    selected = [];
    loading$: Observable<boolean>;
    loading: boolean;

    id: number;

    allowAddBankAccountGroup: boolean = false;

    statusOptions = [
        {
            translate: 'CARD-TO-CARD-ACCOUNT-GROUP.GENERAL.ACTIVE',
            value: 1
        },
        {
            translate: 'CARD-TO-CARD-ACCOUNT-GROUP.GENERAL.RESERVED',
            value: 2
        },
        {
            translate: 'CARD-TO-CARD-ACCOUNT-GROUP.GENERAL.BLOCKED',
            value: 3
        }
    ];

    constructor(private store: Store<coreState.State>,
        private router: Router,
        private fb: FormBuilder,
        public snackBar: MatSnackBar,
        private route: ActivatedRoute,
        private accountService: AccountService,
        private translateService: TranslateService) {
        this.allowAddBankAccountGroup = this.accountService.isUserAuthorizedForTask(permissions.AddBankAccountGroup);
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

        this.cardToCardAccountGroupCreated$ = this.store.select(coreState.getCardToCardAccountGroupCreated);
        this.cardToCardAccountGroupGetDetail$ = this.store.select(coreState.getCardToCardAccountGroupDetails);
        this.cardToCardAccountGroupUpdateSuccess = this.store.select(coreState.getCardToCardAccountGroupEditSuccess);
        this.createError$ = this.store.select(coreState.getCardToCardAccountGroupCreateError);
        this.updateError$ = this.store.select(coreState.getCardToCardAccountGroupEditError);
        this.getDetailError$ = this.store.select(coreState.getCardToCardAccountGroupDetailError);
        this.getDetailLoading$ = this.store.select(coreState.getCardToCardAccountGroupDetailLoading);

        this.tenantsLoading$ = this.store.select(coreState.getTenantLoading);
        this.tenants$ = this.store.select(coreState.getTenantSearchResults);

        this.cardToCardAccounts$ = this.store.select(coreState.getCardToCardAccountSearchResults);

        this.cardToCardAccounts$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.cardToCardAccounts = items;
        });

        this.loading$ = this.store.select(coreState.getCardToCardAccountLoading);

        this.loading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.loading = l;
        });

        this.searchError$ = this.store.select(coreState.getCardToCardAccountSearchError);

        this.searchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.route.params.subscribe(params => {

            this.id = params['id'];

            if (this.id != 0 && this.id != null && this.id != undefined) {
                this.pageType = 'edit';
                this.loadCardToCardAccountGroupDetail();
            }
            else {
                this.pageType = 'new';
                this.createForm();
            }
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid && this.cardToCardAccountGroupForm && this.pageType == 'new') {
                this.cardToCardAccountGroupForm.get('tenantGuid').setValue(t.tenantDomainPlatformMapGuid);
            }

            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new accountActions.GetOwners());
                this.store.dispatch(new cardToCardAccountActions.Search(''));
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

            if (this.accountGuid && this.cardToCardAccountGroupForm && this.pageType == 'new') {

                this.cardToCardAccountGroupForm.get('ownerGuid').setValue(this.getOwnerGuid());
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
        this.store.dispatch(new cardToCardAccountActions.ClearErrors());
        this.store.dispatch(new cardToCardAccountGroupActions.ClearErrors());
        this.store.dispatch(new cardToCardAccountActions.Clear());
        this.store.dispatch(new userSegmentGroupActions.Clear());
        this.destroyed$.next(true);
        this.destroyed$.complete();
        this.formChanges.emit(false);
    }

    createForm(): void {
        this.cardToCardAccountGroupForm = this.fb.group({
            name: new FormControl(undefined, [Validators.required]),
            tenantGuid: new FormControl(this.selectedTenant == undefined ? undefined : this.selectedTenant.tenantDomainPlatformMapGuid, [Validators.required]),
            ownerGuid: new FormControl(this.getOwnerGuid(), [Validators.required]),
            items: this.fb.array([])
        });

        this.cardToCardAccountGroupForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: CardToCardAccountGroup = new CardToCardAccountGroup({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.cardToCardAccountGroupForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    createUpdateForm(data: CardToCardAccountGroup): void {

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

        this.cardToCardAccountGroupForm = this.fb.group({
            id: data.id,
            name: new FormControl({ value: data.name, disabled: !this.allowAddBankAccountGroup }, [Validators.required]),
            tenantGuid: new FormControl({ value: data.tenantGuid, disabled: !this.allowAddBankAccountGroup }, [Validators.required]),
            ownerGuid: new FormControl({ value: data.ownerGuid, disabled: !this.allowAddBankAccountGroup }, [Validators.required]),
            items: this.fb.array(array)
        });

        this.cardToCardAccountGroupForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: CardToCardAccountGroup = new CardToCardAccountGroup({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.cardToCardAccountGroupForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    private loadCardToCardAccountGroupDetail(): void {
        this.store.dispatch(new cardToCardAccountGroupActions.GetDetails(this.id));

        this.cardToCardAccountGroupGetDetail$.pipe(takeUntil(this.destroyed$)).subscribe(p => {
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

        if (this.cardToCardAccountGroupForm.valid) {
            let form = this.cardToCardAccountGroupForm.value;
            this.formSubmit = true;
            this.store.dispatch(new cardToCardAccountGroupActions.Create(form));

            this.cardToCardAccountGroupCreated$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;
                        this.router.navigate(['/cardtocardaccountgroups']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.cardToCardAccountGroupForm);
        }
    }

    onUpdateSubmit(): void {
        this.formSubmit = false;

        if (this.cardToCardAccountGroupForm.valid) {
            let form = this.cardToCardAccountGroupForm.value;
            this.formSubmit = true;
            this.store.dispatch(new cardToCardAccountGroupActions.Edit(this.id, form));

            this.cardToCardAccountGroupUpdateSuccess.pipe(filter(tnCreated => tnCreated !== false), take(1))
                .subscribe(
                    tnCreated => {
                        this.router.navigate(['/cardtocardaccountgroups']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.cardToCardAccountGroupForm);
        }
    }

    getAccounts() {
        if (this.cardToCardAccounts == undefined || this.cardToCardAccountGroupForm == undefined) {
            return [];
        }

        var items = this.cardToCardAccountGroupForm.get('items').value;

        var ownerGuid = this.cardToCardAccountGroupForm.get('ownerGuid').value;

        if (items == undefined || ownerGuid == undefined) {
            return [];
        }

        return this.cardToCardAccounts.filter(t => t.ownerGuid == ownerGuid && items.find(p => p.cardToCardAccountId == t.id) == null);
    }

    getUserSegmentGroups() {

        if (this.userSegmentGroups == undefined) {
            return [];
        }

        var ownerGuid = this.cardToCardAccountGroupForm.get('ownerGuid').value;

        if (ownerGuid == undefined) {
            return [];
        }
        return this.userSegmentGroups.filter(t => t.ownerGuid == ownerGuid && t.isDefault == false);
    }


    getCardNumber(accountId: number): string {
        if (this.cardToCardAccounts == undefined) {
            return'';
        }

        var account = this.cardToCardAccounts.find(t => t.id == accountId);

        if (account != null && account.cardNumber != null) {
            return account.cardNumber;
        }

        return '';
    }

    getAccountNumber(accountId: number): string {
        if (this.cardToCardAccounts == undefined) {
            return '';
        }

        var account = this.cardToCardAccounts.find(t => t.id == accountId);

        if (account != null) {
            return account.accountNo;
        }

        return '';
    }

    getFriendlyName(accountId: number): string {
        if (this.cardToCardAccounts == undefined) {
            return '';
        }

        var account = this.cardToCardAccounts.find(t => t.id == accountId);

        if (account != null) {
            return account.friendlyName;
        }

        return '';
    }

    addAccount(account: CardToCardAccount) {
        var items = this.cardToCardAccountGroupForm.get('items') as FormArray;

        var item = new CardToCardAccountGroupItem();
        item.cardToCardAccountId = account.id;
        item.cardToCardAccountGroupId = this.id == undefined ? 0 : this.id;
        item.status = 1;
        item.loginType = account.loginType;
        item.allowCardToCard = true;
        item.allowWithdrawal = true;
        item.hideCardNumber = false;
        item.userSegmentGroups = [];

        var group = this.fb.group(item);

        items.push(group);
    }

    deleteAccount(account) {
        var items = this.cardToCardAccountGroupForm.get('items') as FormArray;
        var index = items.controls.indexOf(account);

        if (index != -1) {
            items.removeAt(index);
        }
    }

    getOwners() {
        if (this.owners) {
            var list = this.owners;

            if (list.length == 0) {
                this.cardToCardAccountGroupForm.get('ownerGuid').setValue(undefined);
            }
            return list;
        }

        this.cardToCardAccountGroupForm.get('ownerGuid').setValue(undefined);
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

