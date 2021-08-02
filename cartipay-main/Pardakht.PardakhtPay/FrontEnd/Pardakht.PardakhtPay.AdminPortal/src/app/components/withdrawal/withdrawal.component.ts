import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { FormGroup, FormControl, Validators, FormBuilder, FormArray } from '@angular/forms';
import { Store } from '@ngrx/store';
import { DateAdapter, MatSnackBar } from '@angular/material';
import * as coreState from '../../core';
import { GenericHelper } from '../../helpers/generic';
import { CustomValidators, FormHelper } from '../../helpers/forms/form-helper';
import { debounceTime, distinctUntilChanged, take, takeUntil } from 'rxjs/operators';
import { Withdrawal, WithdrawalCreate } from '../../models/withdrawal';
import { fuseAnimations } from '../../core/animations';
import * as withdrawalActions from '../../core/actions/withdrawal';
import * as tenantActions from '../../core/actions/tenant';
import * as transferAccountActions from '../../core/actions/transferAccount';
import { filter } from 'rxjs/operators/filter';
import { Router, ActivatedRoute } from '@angular/router';
import { RandomService } from '../../core/services/random/random.service';
import { TranslateService } from '@ngx-translate/core';
import { Tenant } from '../../models/tenant';
import { TransferAccount } from '../../models/transfer-account';

@Component({
  selector: 'app-withdrawal',
  templateUrl: './withdrawal.component.html',
    styleUrls: ['./withdrawal.component.scss'],
    animations: fuseAnimations
})
export class WithdrawalComponent implements OnInit, OnDestroy {

    pageType = 'new';

    private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
    withdrawalForm: FormGroup;
    isCreating: boolean;
    createError$: Observable<string>;
    withdrawalCreated$: Observable<Withdrawal>;
    tenants$: Observable<Tenant[]>;
    tenants: Tenant[] = [];
    //getAllTenantError$: Observable<string>;
    transferAccounts$: Observable<TransferAccount[]>;
    transferAccountError$: Observable<string>;
    transferAccounts: TransferAccount[];

    //tenantGuid$: Observable<string>;
    //tenantGuid: string;

    selectedTenant$: Observable<Tenant>;
    selectedTenant: Tenant;

    //selectedTenantId: number;

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
        this.withdrawalCreated$ = this.store.select(coreState.getWithdrawalCreated);
        this.createError$ = this.store.select(coreState.getWithdrawalCreateError);
        //this.getAllTenantError$ = this.store.select(coreState.getTenantSearchError);
        this.tenants$ = this.store.select(coreState.getTenantSearchResults);
        this.transferAccountError$ = this.store.select(coreState.getTransferAccountSearchError);
        this.transferAccounts$ = this.store.select(coreState.getTransferAccountSearchResults);

        this.createError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error != undefined) {
                this.openSnackBar(error);
            }
        });

        //this.getAllTenantError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
        //    if (error != undefined) {
        //        this.openSnackBar(error);
        //    }
        //});

        this.transferAccountError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
            if (error) {
                this.openSnackBar(error);
            }
        });

        this.transferAccounts$.pipe(takeUntil(this.destroyed$)).subscribe(accounts => {
            this.transferAccounts = accounts;
        });

        //this.tenantGuid$ = this.store.select(coreState.getAccountTenantGuid);

        //this.tenantGuid$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
        //    this.tenantGuid = t;

        //    if (this.tenantGuid) {
        //        this.withdrawalForm.get('tenantGuid').setValue(t);
        //    }
        //});

        this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

        this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
            this.selectedTenant = t;

            if (t && t.tenantDomainPlatformMapGuid && this.withdrawalForm && this.pageType == 'new') {
                this.withdrawalForm.get('tenantGuid').setValue(t.tenantDomainPlatformMapGuid);
            }

            //this.store.dispatch(new tenantActions.Search());
            if (t && t.tenantDomainPlatformMapGuid) {
                this.store.dispatch(new transferAccountActions.Search(''));
            }
        });
    }

    ngOnDestroy(): void {
        this.store.dispatch(new withdrawalActions.ClearErrors());
        //this.store.dispatch(new tenantActions.Clear());
        this.store.dispatch(new transferAccountActions.Clear());
        this.destroyed$.next(true);
        this.destroyed$.complete();
    }

    createForm(): void {
        this.withdrawalForm = this.fb.group({
            tenantGuid: new FormControl(this.selectedTenant == undefined ? undefined : this.selectedTenant.tenantDomainPlatformMapGuid, [Validators.required]),
            transferAccountId: new FormControl(undefined, [Validators.required]),
            amount: new FormControl(undefined, [Validators.required])
        });

        this.withdrawalForm.valueChanges.pipe(
            debounceTime(300),
            distinctUntilChanged()
        )
            .subscribe(() => {
                const emptyForm: Withdrawal = new Withdrawal({});
                const changes: boolean = GenericHelper.detectNonNullableChanges(this.withdrawalForm.value, emptyForm);
            });
    }

    //onTenantChanged() {
    //    var tenant = this.withdrawalForm.get('tenantId');
    //    this.withdrawalForm.get('bankAccountId').setValue(undefined);

    //    if (tenant) {
    //        this.store.dispatch(new transferAccountActions.GetAccounts(tenant.value));
    //    }
    //    else {
    //        this.store.dispatch(new transferAccountActions.ClearAll());
    //    }
    //}

    getTransferAccounts() {
        var tenantGuid = this.withdrawalForm.get('tenantGuid').value;

        if (this.transferAccounts == undefined || this.transferAccounts == null) {
            return [];
        }

        return this.transferAccounts.filter(p => p.tenantGuid == tenantGuid);
    }

    getErrorMessage(control: FormControl): string {
        return FormHelper.getErrorMessage(control);
    }

    onSubmit(): void {

        if (this.withdrawalForm.valid) {
            let form = this.withdrawalForm.value;
            //console.log(form);
            this.store.dispatch(new withdrawalActions.Create(form));

            this.withdrawalCreated$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
                .subscribe(
                    tnCreated => {
                        this.isCreating = false;
                        this.router.navigate(['/withdrawals']);
                    });

        }
        else {
            FormHelper.validateFormGroup(this.withdrawalForm);
        }
    }

}
