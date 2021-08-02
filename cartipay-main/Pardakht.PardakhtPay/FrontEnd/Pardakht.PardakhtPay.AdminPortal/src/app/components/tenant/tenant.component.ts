import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { FormGroup, FormControl, Validators, FormBuilder, FormArray } from '@angular/forms';
import { Store } from '@ngrx/store';
import { DateAdapter, MatSnackBar } from '@angular/material';
import * as coreState from '../../core';
import { GenericHelper } from '../../helpers/generic';
import { CustomValidators, FormHelper } from '../../helpers/forms/form-helper';
import { debounceTime, distinctUntilChanged, take, takeUntil } from 'rxjs/operators';
import { Tenant, TenantCreate } from '../../models/tenant';
import { fuseAnimations } from '../../core/animations';
import * as tenantActions from '../../core/actions/tenant';
import { filter } from 'rxjs/operators/filter';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-tenant',
  templateUrl: './tenant.component.html',
    styleUrls: ['./tenant.component.scss'],
    animations: fuseAnimations
})
export class TenantComponent implements OnInit, OnDestroy {

    pageType = 'new';

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
    tenantForm: FormGroup;
    isCreating: boolean;
    createError$: Observable<string>;
    tenantCreated$: Observable<Tenant>;

    //selectedMerchantId: number;

    constructor(private store: Store<coreState.State>,
        private dateAdapter: DateAdapter<any>,
        private router: Router,
        private fb: FormBuilder,
        public snackBar: MatSnackBar,
        private translateService: TranslateService) {
        this.dateAdapter.setLocale('gb');
        this.createForm();
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
        this.tenantCreated$ = this.store.select(coreState.getTenantCreated);
        this.createError$ = this.store.select(coreState.getTenantCreateError);

        this.createError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error != undefined) {
                this.openSnackBar(error);
            }
        });
    }

    ngOnDestroy(): void {
        this.store.dispatch(new tenantActions.ClearErrors());
        this.destroyed$.next(true);
        this.destroyed$.complete();
    }

    createForm(): void {
        this.tenantForm = this.fb.group({
            name: new FormControl(undefined, [Validators.required]),
            tenantAdminUsername: new FormControl(undefined, [Validators.required]),
            tenantAdminEmail: new FormControl(undefined, [Validators.required]),
            tenantAdminPassword: new FormControl(undefined, [Validators.required]),
            tenantAdminFirstName: new FormControl(undefined, [Validators.required]),
            tenantAdminLastName: new FormControl(undefined, [Validators.required])
        });

        this.tenantForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: Tenant = new Tenant({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.tenantForm.value, emptyForm);
            });
    }

    getErrorMessage(control: FormControl): string {
        return FormHelper.getErrorMessage(control);
    }

    onSubmit(): void {

        if (this.tenantForm.valid) {
            let form = this.tenantForm.value;
            //console.log(form);
            this.store.dispatch(new tenantActions.Create(form));

            this.tenantCreated$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;
                        this.router.navigate(['/tenants']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.tenantForm);
        }
    }

}

