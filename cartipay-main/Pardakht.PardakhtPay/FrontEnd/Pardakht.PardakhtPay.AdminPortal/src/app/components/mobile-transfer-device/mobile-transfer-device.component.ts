import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { FormGroup, FormControl, Validators, FormBuilder, FormArray } from '@angular/forms';
import { Store } from '@ngrx/store';
import { DateAdapter, MatSnackBar } from '@angular/material';
import * as coreState from '../../core';
import { GenericHelper } from '../../helpers/generic';
import { FormHelper } from '../../helpers/forms/form-helper';
import { debounceTime, distinctUntilChanged, take } from 'rxjs/operators';
import { MobileTransferDevice, MobileTransferDeviceStatus } from '../../models/mobile-transfer';
import { fuseAnimations } from '../../core/animations';
import * as mobileTransferDeviceActions from '../../core/actions/mobile-transfer-device';
import { filter } from 'rxjs/operators/filter';
import { Router, ActivatedRoute } from '@angular/router';
import { RandomService } from '../../core/services/random/random.service';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../models/tenant';
import * as accountActions from '../../core/actions/account';
import { Owner } from '../../models/account.model';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';
import { iranianPhoneNumberValidator } from '../../validators/iranian-phone-number-validator';

@Component({
  selector: 'app-mobile-transfer-device',
  templateUrl: './mobile-transfer-device.component.html',
    styleUrls: ['./mobile-transfer-device.component.scss'],
    animations: fuseAnimations
})
export class MobileTransferDeviceComponent implements OnInit, OnDestroy {

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
    mobileTransferDeviceForm: FormGroup;
    formSubmit: boolean = false;
    pageType = '';
    isCreating: boolean;
    createError$: Observable<string>;
    updateError$: Observable<string>;
    getDetailError$: Observable<string>;

    activateError$: Observable<string>;
    activateCompleted$: Observable<boolean>;

    mobileTransferDeviceCreated$: Observable<MobileTransferDevice>;
    mobileTransferDeviceGetDetail$: Observable<MobileTransferDevice>;
    getDetailLoading$: Observable<boolean>;
    getDetailLoading: boolean;
    mobileTransferDeviceUpdateSuccess: Observable<boolean>;

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

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    @Output() formChanges: EventEmitter<boolean> = new EventEmitter();

    openedAccount: FormGroup;

    id: number;

    statusEnum = MobileTransferDeviceStatus;

    status: number = undefined;

    allowAddMobileTransferDevice: boolean = false;

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
        this.allowAddMobileTransferDevice = this.accountService.isUserAuthorizedForTask(permissions.AddMobileTransferDevice);
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

        this.mobileTransferDeviceCreated$ = this.store.select(coreState.getMobileTransferDeviceCreated);
        this.mobileTransferDeviceGetDetail$ = this.store.select(coreState.getMobileTransferDeviceDetails);
        this.mobileTransferDeviceUpdateSuccess = this.store.select(coreState.getMobileTransferDeviceEditSuccess);
        this.createError$ = this.store.select(coreState.getMobileTransferDeviceCreateError);
        this.updateError$ = this.store.select(coreState.getMobileTransferDeviceEditError);
        this.getDetailError$ = this.store.select(coreState.getMobileTransferDeviceDetailError);
        this.getDetailLoading$ = this.store.select(coreState.getMobileTransferDeviceDetailLoading);

        this.tenantsLoading$ = this.store.select(coreState.getTenantLoading);
        this.tenants$ = this.store.select(coreState.getTenantSearchResults);

        this.route.params.subscribe(params => {

            this.id = params['id'];

            if (this.id != 0 && this.id != null && this.id != undefined) {
                this.pageType = 'edit';
                this.loadMobileTransferDeviceDetail();
            }
            else {
                this.pageType = 'new';
                this.createForm();
            }
        });

        this.createError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error != undefined) {
                this.isCreating = false;
                this.openSnackBar(error);
            }
        });

        this.updateError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error != undefined) {
                this.isCreating = false;
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

            if (t && t.tenantDomainPlatformMapGuid && this.mobileTransferDeviceForm && this.pageType == 'new') {
                this.mobileTransferDeviceForm.get('tenantGuid').setValue(t.tenantDomainPlatformMapGuid);
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
        });

        this.activateError$ = this.store.select(coreState.getMobileTransferDeviceActivateError);

        this.activateError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.isCreating = false;
                this.openSnackBar(error);
            }
        });

        this.activateCompleted$ = this.store.select(coreState.getMobileTransferDeviceActivateSucces);
    }

    ngOnDestroy(): void {
        this.store.dispatch(new mobileTransferDeviceActions.ClearErrors());
        this.destroyed$.next(true);
        this.destroyed$.complete();
        this.formChanges.emit(false);
    }

    createForm(): void {

        this.mobileTransferDeviceForm = this.fb.group({
            tenantGuid: new FormControl(this.selectedTenant == undefined || this.selectedTenant == null ? undefined : this.selectedTenant.tenantDomainPlatformMapGuid, [Validators.required]),
            phoneNumber: new FormControl(undefined, [Validators.required, iranianPhoneNumberValidator]),
            isActive: new FormControl(true)
        });

        this.mobileTransferDeviceForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: MobileTransferDevice = new MobileTransferDevice({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.mobileTransferDeviceForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    createUpdateForm(data: MobileTransferDevice): void {

        this.status = data.status;
        
        this.mobileTransferDeviceForm = this.fb.group({
            id: data.id,
            tenantGuid: new FormControl(data.tenantGuid, [Validators.required]),
            phoneNumber: new FormControl({ value: data.phoneNumber, disabled: true }, [Validators.required]),
            verificationCode: new FormControl(data.verificationCode),
            status: new FormControl(data.status),
            verifyCodeSendDate: new FormControl(data.verifyCodeSendDate),
            verifiedDate: new FormControl(data.verifiedDate),
            externalId: new FormControl(data.externalId),
            externalStatus: new FormControl(data.externalStatus),
            isActive: new FormControl(data.isActive)
        });

        this.mobileTransferDeviceForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: MobileTransferDevice = new MobileTransferDevice({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.mobileTransferDeviceForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    private loadMobileTransferDeviceDetail(): void {
        this.store.dispatch(new mobileTransferDeviceActions.GetDetails(this.id));

        this.mobileTransferDeviceGetDetail$.pipe(takeUntil(this.destroyed$)).subscribe(p => {
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

        if (this.mobileTransferDeviceForm.valid) {
            let form = this.mobileTransferDeviceForm.value;
            this.formSubmit = true;
            this.store.dispatch(new mobileTransferDeviceActions.Create(form));

            this.mobileTransferDeviceCreated$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;
                        this.router.navigate(['/mobiletransferdevices']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.mobileTransferDeviceForm);
        }
    }

    onUpdateSubmit(): void {
        this.formSubmit = false;
        this.isCreating = true;

        if (this.mobileTransferDeviceForm.valid) {
            let form = this.mobileTransferDeviceForm.value;
            this.formSubmit = true;
            this.store.dispatch(new mobileTransferDeviceActions.Edit(this.id, form));

            this.mobileTransferDeviceUpdateSuccess.pipe(filter(tnCreated => tnCreated !== false), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;

                        this.router.navigate(['/mobiletransferdevices']);
                    });
        }
        else {
            FormHelper.validateFormGroup(this.mobileTransferDeviceForm);
        }
    }

    onSendVerificationCode(): void {
        this.formSubmit = false;
        this.isCreating = true;

        if (this.mobileTransferDeviceForm.valid) {
            this.formSubmit = true;
            var code = this.mobileTransferDeviceForm.get('verificationCode').value;
            this.store.dispatch(new mobileTransferDeviceActions.Activate(this.id, code));

            this.activateCompleted$.pipe(filter(tnCreated => tnCreated !== false), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;

                        this.router.navigate(['/mobiletransferdevices']);
                    });
        }
        else {
            FormHelper.validateFormGroup(this.mobileTransferDeviceForm);
        }
    }

    isLoading() {
        return this.tenantsLoading || this.getDetailLoading || this.isCreating;
    }
}
