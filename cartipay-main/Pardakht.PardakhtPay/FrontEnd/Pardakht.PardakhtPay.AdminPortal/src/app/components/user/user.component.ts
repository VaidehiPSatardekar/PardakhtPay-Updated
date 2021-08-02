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
import { filter } from 'rxjs/operators/filter';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../models/tenant';
import { Owner } from '../../models/account.model';
import * as tenantActions from '../../core/actions/tenant';
import * as accountActions from '../../core/actions/account';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
    styleUrls: ['./user.component.scss'],
    animations: fuseAnimations
})
export class UserComponent implements OnInit, OnDestroy {

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
    userForm: FormGroup;
    pageType = 'new';
    isCreating: boolean;
    //createError$: Observable<string>;
    getDetailError$: Observable<string>;
    getDetailLoading$: Observable<boolean>;
    getDetailLoading: boolean;
    formSubmit: boolean = false;

    userCreated$: Observable<boolean>;
    userGetDetail$: Observable<Owner>;
    userUpdateSuccess$: Observable<boolean>;

    @Output() formChanges: EventEmitter<boolean> = new EventEmitter();

    tenantsLoading$: Observable<boolean>;
    tenantsLoading: boolean;
    tenants$: Observable<Tenant[]>;
    //tenantSearchError$: Observable<string>;

    tenantGuid$: Observable<string>;
    tenantGuid: string;

    isProviderAdmin$: Observable<boolean>;
    isProviderAdmin: boolean;

    isTenantAdmin$: Observable<boolean>;
    isTenantAdmin: boolean;

    isStandardUser$: Observable<boolean>;
    isStandardUser: boolean;

    accountGuid$: Observable<string>;
    accountGuid: string;

    id: number;
    constructor(private store: Store<coreState.State>,
        private dateAdapter: DateAdapter<any>,
        private router: Router,
        private fb: FormBuilder,
        public snackBar: MatSnackBar,
        private route: ActivatedRoute,
        private translateService: TranslateService) {
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
        //todo
        // this.userCreated$ = this.store.select(coreState.getUserCreated);
        this.userGetDetail$ = this.store.select(coreState.getUserDetail);
        //this.createError$ = this.store.select(coreState.getUserCreateError);
        this.getDetailError$ = this.store.select(coreState.getUserDetailError);
        this.getDetailLoading$ = this.store.select(coreState.getAccountLoading);
        this.userUpdateSuccess$ = this.store.select(coreState.getUserUpdateSuccess);

        this.tenantsLoading$ = this.store.select(coreState.getTenantLoading);
        this.tenants$ = this.store.select(coreState.getTenantSearchResults);
        //this.tenantSearchError$ = this.store.select(coreState.getTenantSearchError);

        this.route.params.subscribe(params => {
            this.id = params['id'];

            if (this.id != 0 && this.id != null && this.id != undefined) {
                this.pageType = 'edit';
                this.loadUserDetail();
            }
            else {
                this.pageType = 'new';
                this.createForm();
            }
        });

        this.tenantGuid$ = this.store.select(coreState.getAccountTenantGuid);

        this.tenantGuid$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.tenantGuid = t;

            if (this.tenantGuid) {
                if (this.userForm) {
                    this.userForm.get('tenantDomainPlatformMapGuid').setValue(t);
                }
            }
        });

        //this.createError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
        //    if (error != undefined) {
        //        this.openSnackBar(error);
        //    }
        //});

        this.getDetailError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.getDetailLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
            this.getDetailLoading = l;
        });

        //this.tenantSearchError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
        //    if (error) {
        //        this.openSnackBar(error);
        //    }
        //});

        this.isProviderAdmin$ = this.store.select(coreState.getAccountIsProviderAdmin);
        this.isProviderAdmin$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.isProviderAdmin = t;
            //if (t == true) {
            //    this.store.dispatch(new tenantActions.Search());
            //}
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
        
    }

    ngOnDestroy(): void {
        this.store.dispatch(new accountActions.ClearErrors());
        this.destroyed$.next(true);
        this.destroyed$.complete();
        this.formChanges.emit(false);
    }

    createForm(): void {
        this.userForm = this.fb.group({
            tenantDomainPlatformMapGuid: new FormControl(this.tenantGuid, [Validators.required]),
            firstName: new FormControl(undefined, [Validators.required]),
            lastName: new FormControl(undefined, [Validators.required]),
            username: new FormControl(undefined, [Validators.required]),
            password: new FormControl(undefined, [Validators.required]),
            email: new FormControl(undefined, [Validators.required]),
            isApiKeyUser: new FormControl(false)
        });

        this.userForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: Owner = new Owner({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.userForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    createUpdateForm(data: Owner): void {

        var array: FormGroup[] = [];

        this.userForm = this.fb.group({
            id: data.id,
            tenantDomainPlatformMapGuid: new FormControl(data.tenantDomainPlatformMapGuid),
            firstName: new FormControl(data.firstName, [Validators.required]),
            lastName: new FormControl(data.lastName, [Validators.required]),
            username: new FormControl(data.username, [Validators.required]),
            email: new FormControl(data.email, [Validators.required]),
            roleGuid: new FormControl(data.roleGuid),
            isApiKeyUser: new FormControl(false)
        });

        this.userForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: Owner = new Owner({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.userForm.value, emptyForm);
                this.formChanges.emit(changes);
            });
    }

    private loadUserDetail(): void {
        this.store.dispatch(new accountActions.GetOwnerDetail(this.id));

        this.userGetDetail$.pipe(takeUntil(this.destroyed$)).subscribe(p => {
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

        if (this.userForm.valid) {
            let form = this.userForm.value;
            //console.log(form);
            this.formSubmit = true;
            this.store.dispatch(new accountActions.CreateOwner(form));

            this.userCreated$.pipe(filter(tnCreated => tnCreated !== false), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;
                        this.router.navigate(['/users']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.userForm);
        }
    }

    onUpdateSubmit(): void {
        this.formSubmit = false;

        if (this.userForm.valid) {
            let form = this.userForm.value;
            //console.log(form);
            this.formSubmit = true;
            this.store.dispatch(new accountActions.EditOwner(this.id, form));

            this.userUpdateSuccess$.pipe(filter(tnCreated => tnCreated !== false), take(1))
                .subscribe(
                    tnCreated => {
                        this.router.navigate(['/users']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.userForm);
        }
    }

    isLoading() {
        return this.getDetailLoading;
    }

}

