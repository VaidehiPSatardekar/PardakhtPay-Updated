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
import * as tenantActions from '../../core/actions/tenant';
import * as accountActions from '../../core/actions/account';
import { UserSegmentGroup, UserSegment } from '../../models/user-segment-group';
import * as userSegmentGroupActions from '../../core/actions/userSegmentGroup';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';

@Component({
    selector: 'app-user-segment-group',
    templateUrl: './user-segment-group.component.html',
    styleUrls: ['./user-segment-group.component.scss'],
    animations: fuseAnimations
})
export class UserSegmentGroupComponent implements OnInit, OnDestroy {

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
    userSegmentGroupForm: FormGroup;
    formSubmit: boolean = false;
    pageType = '';
    isCreating: boolean;
    createError$: Observable<string>;
    updateError$: Observable<string>;
    getDetailError$: Observable<string>;
    getDetailLoading$: Observable<boolean>;
    getDetailLoading: boolean;

    userSegmentGroupCreated$: Observable<UserSegmentGroup>;
    userSegmentGroupGetDetail$: Observable<UserSegmentGroup>;
    userSegmentGroupUpdateSuccess: Observable<boolean>;
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

    selected = [];

    types = [{
        id: 1,
        translate: 'USER-SEGMENT.TYPES.TOTAL-SUCCESSFUL-TRANSACTION-COUNT',
        type: 'number'
    }, {
        id: 2,
        translate: 'USER-SEGMENT.TYPES.TOTAL-SUCCESSFUL-TRANSACTION-AMOUNT',
        type: 'number'
        },
    //    {
    //    id: 3,
    //    translate: 'USER-SEGMENT.TYPES.TOTAL-UNPAID-TRANSACTION-COUNT',
    //    type: 'number'
    //}, {
    //    id: 4,
    //    translate: 'USER-SEGMENT.TYPES.TOTAL-EXPIRED-TRANSACTION-COUNT',
    //    type: 'number'
    //    },
        {
        id: 5,
        translate: 'USER-SEGMENT.TYPES.TOTAL-WITHDRAWAL-COUNT-PARDAKHTPAY',
        type: 'number'
    }, {
        id: 6,
        translate: 'USER-SEGMENT.TYPES.TOTAL-WITHDRAWAL-COUNT-MERCHANT',
        type: 'number'
    }, {
        id: 7,
        translate: 'USER-SEGMENT.TYPES.TOTAL-WITHDRAWAL-AMOUNT-PARDAKHTPAY',
        type: 'number'
    }, {
        id: 8,
        translate: 'USER-SEGMENT.TYPES.TOTAL-WITHDRAWAL-AMOUNT-MERCHANT',
        type: 'number'
    }, {
        id: 9,
        translate: 'USER-SEGMENT.TYPES.TOTAL-DEPOSIT-COUNT-MERCHANT',
        type: 'number'
    }, {
        id: 10,
        translate: 'USER-SEGMENT.TYPES.TOTAL-DEPOSIT-AMOUNT-MERCHANT',
        type: 'number'
    }, {
        id: 11,
        translate: 'USER-SEGMENT.TYPES.REGISTRATION-DATE',
        type: 'date'
    }, {
        id: 12,
        translate: 'USER-SEGMENT.TYPES.GROUP-NAME',
        type: 'text'
    }, {
        id: 13,
        translate: 'USER-SEGMENT.TYPES.LAST-ACITIVITY-DATE',
        type: 'date'
    }, {
        id: 14,
        translate: 'USER-SEGMENT.TYPES.ACTIVITY-SCORE',
        type: 'text'
    }, {
        id: 15,
        translate: 'USER-SEGMENT.TYPES.WEBSITE-NAME',
        type: 'text'
    }, {
        id: 16,
        translate: 'USER-SEGMENT.TYPES.TOTAL-SPORTBOOK-AMOUNT',
        type: 'number'
    }, {
        id: 17,
        translate: 'USER-SEGMENT.TYPES.TOTAL-SPORTBOOK-COUNT',
        type: 'number'
    }, {
        id: 18,
        translate: 'USER-SEGMENT.TYPES.TOTAL-CASINO-AMOUNT',
        type: 'number'
    }, {
        id: 19,
        translate: 'USER-SEGMENT.TYPES.TOTAL-CASINO-COUNT',
        type: 'number'
    }];

    compareTypes = [
        {
            id: 1,
            translate: 'USER-SEGMENT.COMPARE-TYPES.LESS-THAN'
        }, {
            id: 2,
            translate: 'USER-SEGMENT.COMPARE-TYPES.LESS-THAN-AND-EQUAL'
        }, {
            id: 3,
            translate: 'USER-SEGMENT.COMPARE-TYPES.EQUALS'
        }, {
            id: 4,
            translate: 'USER-SEGMENT.COMPARE-TYPES.NOT-EQUAL'
        }, {
            id: 5,
            translate: 'USER-SEGMENT.COMPARE-TYPES.MORE-THAN-AND-EQUAL'
        }, {
            id: 6,
            translate: 'USER-SEGMENT.COMPARE-TYPES.MORE-THAN'
        }
    ];

    id: number;

    allowAddUserSegmentGroup: boolean = false;

    constructor(private store: Store<coreState.State>,
        private router: Router,
        private fb: FormBuilder,
        public snackBar: MatSnackBar,
        private route: ActivatedRoute,
        private accountService: AccountService,
        private translateService: TranslateService) {
        this.allowAddUserSegmentGroup = this.accountService.isUserAuthorizedForTask(permissions.AddUserSegmentGroup);
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

        this.userSegmentGroupCreated$ = this.store.select(coreState.getUserSegmentGroupCreated);
        this.userSegmentGroupGetDetail$ = this.store.select(coreState.getUserSegmentGroupDetails);
        this.userSegmentGroupUpdateSuccess = this.store.select(coreState.getUserSegmentGroupEditSuccess);
        this.createError$ = this.store.select(coreState.getUserSegmentGroupCreateError);
        this.updateError$ = this.store.select(coreState.getUserSegmentGroupEditError);
        this.getDetailError$ = this.store.select(coreState.getUserSegmentGroupDetailError);
        this.getDetailLoading$ = this.store.select(coreState.getUserSegmentGroupDetailLoading);

        this.tenantsLoading$ = this.store.select(coreState.getTenantLoading);
        this.tenants$ = this.store.select(coreState.getTenantSearchResults);

        this.route.params.subscribe(params => {

            this.id = params['id'];

            if (this.id != 0 && this.id != null && this.id != undefined) {
                this.pageType = 'edit';
                this.loadUserSegmentGroupDetail();
            }
            else {
                this.pageType = 'new';
                this.createForm();
            }
        });

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid && this.userSegmentGroupForm && this.pageType == 'new') {
                this.userSegmentGroupForm.get('tenantGuid').setValue(t.tenantDomainPlatformMapGuid);
            }

            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new accountActions.GetOwners());
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

            if (this.accountGuid && this.userSegmentGroupForm && this.pageType == 'new') {
                this.userSegmentGroupForm.get('ownerGuid').setValue(this.getOwnerGuid());
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
    }

    ngOnDestroy(): void {
        this.store.dispatch(new userSegmentGroupActions.ClearErrors());
        this.destroyed$.next(true);
        this.destroyed$.complete();
        this.formChanges.emit(false);
    }

    createForm(): void {
        this.userSegmentGroupForm = this.fb.group({
            name: new FormControl(undefined, [Validators.required]),
            tenantGuid: new FormControl(this.selectedTenant == undefined ? undefined : this.selectedTenant.tenantDomainPlatformMapGuid, [Validators.required]),
            ownerGuid: new FormControl(this.getOwnerGuid(), [Validators.required]),
            order: new FormControl(0, [Validators.required]),
            isActive: new FormControl(true, [Validators.required]),
            isDefault: new FormControl(false, [Validators.required]),
            isMalicious: new FormControl(false),
            items: this.fb.array([])
        });

        this.userSegmentGroupForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: UserSegmentGroup = new UserSegmentGroup({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.userSegmentGroupForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    createUpdateForm(data: UserSegmentGroup): void {

        var array: FormGroup[] = [];

        if (data.items != null) {
            for (var i = 0; i < data.items.length; i++) {
                var item = data.items[i];

                array.push(this.fb.group(item));
            }
        }

        this.userSegmentGroupForm = this.fb.group({
            id: data.id,
            name: new FormControl(data.name, [Validators.required]),
            tenantGuid: new FormControl(data.tenantGuid, [Validators.required]),
            ownerGuid: new FormControl(data.ownerGuid, [Validators.required]),
            order: new FormControl(data.order, [Validators.required]),
            isActive: new FormControl(data.isActive, [Validators.required]),
            isDefault: new FormControl(data.isDefault, [Validators.required]),
            isMalicious: new FormControl(data.isMalicious),
            items: this.fb.array(array)
        });

        this.userSegmentGroupForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: UserSegmentGroup = new UserSegmentGroup({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.userSegmentGroupForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    private loadUserSegmentGroupDetail(): void {
        this.store.dispatch(new userSegmentGroupActions.GetDetails(this.id));

        this.userSegmentGroupGetDetail$.pipe(takeUntil(this.destroyed$)).subscribe(p => {
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

        if (this.userSegmentGroupForm.valid) {
            let form = this.userSegmentGroupForm.value;
            this.formSubmit = true;
            this.store.dispatch(new userSegmentGroupActions.Create(form));

            this.userSegmentGroupCreated$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;
                        this.router.navigate(['/usersegmentgroups']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.userSegmentGroupForm);
        }
    }

    onUpdateSubmit(): void {
        this.formSubmit = false;

        if (this.userSegmentGroupForm.valid) {
            let form = this.userSegmentGroupForm.value;
            this.formSubmit = true;
            this.store.dispatch(new userSegmentGroupActions.Edit(this.id, form));

            this.userSegmentGroupUpdateSuccess.pipe(filter(tnCreated => tnCreated !== false), take(1))
                .subscribe(
                    tnCreated => {
                        this.router.navigate(['/usersegmentgroups']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.userSegmentGroupForm);
        }
    }

    getTypes() {
        return this.types;
    }

    getTypeName(typeId: number): string {
        var type = this.types.find(t => t.id == typeId);

        if (type == null) {
            return '';
        }

        return this.translateService.instant(type.translate);
    }

    getTypeVariableType(typeId: number): string {
        var type = this.types.find(t => t.id == typeId);

        if (type == null) {
            return '';
        }

        return type.type;
    }

    addSegment(type) {

        var items = this.userSegmentGroupForm.get('items') as FormArray;

        var item = new UserSegment();
        item.userSegmentTypeId = type.id;
        item.userSegmentGroupId = this.id == undefined ? 0 : this.id;

        if (type.type) {
            if (type.type == 'number') {
                item.value = '0';
            }
            else if (type.type == 'text') {
                item.value = '';
            }
            else if (type.type == 'date') {
                var date = new Date();
                item.value = date.getFullYear() + '-' + (date.getMonth() + 1).toString().padStart(2, '0') + '-' + date.getDay().toString().padStart(2, '0');
            }
        }

        item.userSegmentCompareTypeId = 1;

        var group = this.fb.group(item);

        items.push(group);
    }

    deleteSegment(segment) {
        var items = this.userSegmentGroupForm.get('items') as FormArray;
        var index = items.controls.indexOf(segment);

        if (index != -1) {
            items.removeAt(index);
        }
    }

    getOwners() {

        if (this.owners) {
            var list = this.owners;

            if (list.length == 0) {
                this.userSegmentGroupForm.get('ownerGuid').setValue(undefined);
            }
            return list;
        }

        this.userSegmentGroupForm.get('ownerGuid').setValue(undefined);
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

