import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Store } from '@ngrx/store';
import { DateAdapter, MatSnackBar } from '@angular/material';
import * as coreState from '../../core';
import { GenericHelper } from '../../helpers/generic';
import { CustomValidators, FormHelper } from '../../helpers/forms/form-helper';
import { debounceTime, distinctUntilChanged, take } from 'rxjs/operators';
import { TenantUrlConfig } from '../../models/tenant-url-config';
import { fuseAnimations } from '../../core/animations';
import * as tenantUrlConfigActions from '../../core/actions/tenantUrlConfig';
import * as merchantActions from '../../core/actions/merchant';
import { filter } from 'rxjs/operators/filter';
import { Router, ActivatedRoute } from '@angular/router';
import { RandomService } from '../../core/services/random/random.service';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Merchant } from '../../models/merchant-model';
import { Tenant } from '../../models/tenant';

@Component({
  selector: 'app-tenant-url-config',
  templateUrl: './tenant-url-config.component.html',
    styleUrls: ['./tenant-url-config.component.scss'],
    animations: fuseAnimations
})
export class TenantUrlConfigComponent implements OnInit, OnDestroy {

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
    tenantUrlConfigForm: FormGroup;
    formSubmit: boolean = false;
    pageType = '';
    isCreating: boolean;
    createError$: Observable<string>;
    updateError$: Observable<string>;
    getDetailError$: Observable<string>;

    tenantUrlConfigCreated$: Observable<TenantUrlConfig>;
    tenantUrlConfigGetDetail$: Observable<TenantUrlConfig>;
    getDetailLoading$: Observable<boolean>;
    getDetailLoading: boolean;
    tenantUrlConfigUpdateSuccess: Observable<boolean>;

    merchantsLoading$: Observable<boolean>;
    merchantsLoading: boolean;
    merchants$: Observable<Merchant[]>;
    merchants: Merchant[];
    merchantsSearchError$: Observable<string>;

    tenantsLoading$: Observable<boolean>;
    tenantsLoading: boolean;
    tenants$: Observable<Tenant[]>;
    //tenantSearchError$: Observable<string>;

    //tenantGuid$: Observable<string>;
    //tenantGuid: string;

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

    @Output() formChanges: EventEmitter<boolean> = new EventEmitter();

    openedAccount: FormGroup;

    id: number;

    constructor(private store: Store<coreState.State>,
        private dateAdapter: DateAdapter<any>,
        private router: Router,
        private fb: FormBuilder,
        public snackBar: MatSnackBar,
        private route: ActivatedRoute,
        private translateService: TranslateService,
        private randomService: RandomService) {
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
        this.tenantUrlConfigCreated$ = this.store.select(coreState.getTenantUrlConfigCreated);
        this.tenantUrlConfigGetDetail$ = this.store.select(coreState.getTenantUrlConfigDetails);
        this.tenantUrlConfigUpdateSuccess = this.store.select(coreState.getTenantUrlConfigEditSuccess);
        this.createError$ = this.store.select(coreState.getTenantUrlConfigCreateError);
        this.updateError$ = this.store.select(coreState.getTenantUrlConfigEditError);
        this.getDetailError$ = this.store.select(coreState.getTenantUrlConfigDetailError);
        this.getDetailLoading$ = this.store.select(coreState.getTenantUrlConfigDetailLoading);

        this.merchants$ = this.store.select(coreState.getMerchantSearchResults);
        this.merchantsLoading$ = this.store.select(coreState.getMerchantLoading);
        this.merchantsSearchError$ = this.store.select(coreState.getMerchantSearchError);

        this.tenantsLoading$ = this.store.select(coreState.getTenantLoading);
        this.tenants$ = this.store.select(coreState.getTenantSearchResults);
        //this.tenantSearchError$ = this.store.select(coreState.getTenantSearchError);

        this.route.params.subscribe(params => {

            this.id = params['id'];

            if (this.id != 0 && this.id != null && this.id != undefined) {
                this.pageType = 'edit';
                this.loadTenantUrlConfigDetail();
            }
            else {
                this.pageType = 'new';
                this.createForm();
            }
        });

        this.merchants$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
            this.merchants = items;
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

        this.merchantsSearchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        //this.tenantSearchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
        //    if (error) {
        //        this.openSnackBar(error);
        //    }
        //});

        this.merchantsLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.merchantsLoading = l;
        });

        this.tenantsLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.tenantsLoading = l;
        });

        this.getDetailLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.getDetailLoading = l;
        });

        //this.tenantGuid$ = this.store.select(coreState.getAccountTenantGuid);

        //this.tenantGuid$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
        //    this.tenantGuid = t;

        //    if (this.tenantGuid && this.tenantUrlConfigForm && this.pageType == 'new') {
        //        this.tenantUrlConfigForm.get('tenantGuid').setValue(t);
        //    }
        //});

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid && this.tenantUrlConfigForm && this.pageType == 'new') {
                this.tenantUrlConfigForm.get('tenantGuid').setValue(t.tenantDomainPlatformMapGuid);
            }

            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new merchantActions.Search(''));
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
    }

    ngOnDestroy(): void {
        this.store.dispatch(new tenantUrlConfigActions.ClearErrors());
        this.store.dispatch(new merchantActions.ClearErrors());
        this.destroyed$.next(true);
        this.destroyed$.complete();
        this.formChanges.emit(false);
    }

    createForm(): void {
        var apiKey = this.randomService.generateRandomKey();

        this.tenantUrlConfigForm = this.fb.group({
            merchantId: new FormControl(undefined, [Validators.required]),
            apiUrl: new FormControl(undefined, [Validators.required]),
            isPaymentUrl: new FormControl(false),
            isServiceUrl: new FormControl(false),
            tenantGuid: new FormControl(this.selectedTenant == undefined ? undefined : this.selectedTenant.tenantDomainPlatformMapGuid, [Validators.required])
        });

        this.tenantUrlConfigForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: TenantUrlConfig = new TenantUrlConfig({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.tenantUrlConfigForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    createUpdateForm(data: TenantUrlConfig): void {

        var array: FormGroup[] = [];

        this.tenantUrlConfigForm = this.fb.group({
            id: data.id,
            merchantId: new FormControl(data.merchantId, [Validators.required]),
            apiUrl: new FormControl(data.apiUrl, [Validators.required]),
            isPaymentUrl: new FormControl(data.isPaymentUrl),
            isServiceUrl: new FormControl(data.isServiceUrl),
            tenantGuid: new FormControl(data.tenantGuid, [Validators.required])
        });

        this.tenantUrlConfigForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: TenantUrlConfig = new TenantUrlConfig({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.tenantUrlConfigForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    private loadTenantUrlConfigDetail(): void {
        this.store.dispatch(new tenantUrlConfigActions.GetDetails(this.id));

        this.tenantUrlConfigGetDetail$.pipe(takeUntil(this.destroyed$)).subscribe(p => {
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

        if (this.tenantUrlConfigForm.valid) {
            let form = this.tenantUrlConfigForm.value;
            this.formSubmit = true;
            this.store.dispatch(new tenantUrlConfigActions.Create(form));

            this.tenantUrlConfigCreated$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;
                        this.router.navigate(['/tenanturlconfiglist']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.tenantUrlConfigForm);
        }
    }

    onUpdateSubmit(): void {
        this.formSubmit = false;
        this.isCreating = true;

        if (this.tenantUrlConfigForm.valid) {
            let form = this.tenantUrlConfigForm.value;
            this.formSubmit = true;
            this.store.dispatch(new tenantUrlConfigActions.Edit(this.id, form));

            this.tenantUrlConfigUpdateSuccess.pipe(filter(tnCreated => tnCreated !== false), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;

                        this.router.navigate(['/tenanturlconfiglist']);
                    });
        }
        else {
            FormHelper.validateFormGroup(this.tenantUrlConfigForm);
        }
    }

    isLoading() {
        return this.tenantsLoading || this.merchantsLoading || this.getDetailLoading || this.isCreating;
    }

}
