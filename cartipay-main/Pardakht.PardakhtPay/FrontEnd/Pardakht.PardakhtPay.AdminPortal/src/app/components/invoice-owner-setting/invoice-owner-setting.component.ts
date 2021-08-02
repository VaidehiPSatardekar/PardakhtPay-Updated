import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Store } from '@ngrx/store';
import { DateAdapter, MatSnackBar } from '@angular/material';
import * as coreState from '../../core';
import { GenericHelper } from '../../helpers/generic';
import { CustomValidators, FormHelper } from '../../helpers/forms/form-helper';
import { debounceTime, distinctUntilChanged, take } from 'rxjs/operators';
import { InvoiceOwnerSetting } from '../../models/invoice';
import { fuseAnimations } from '../../core/animations';
import * as invoiceOwnerSettingActions from '../../core/actions/invoiceOwnerSetting';
import { filter } from 'rxjs/operators/filter';
import { Router, ActivatedRoute } from '@angular/router';
import { RandomService } from '../../core/services/random/random.service';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Tenant } from '../../models/tenant';import { InvoicePeriods, InvoicePeriodValues} from '../../models/invoice';
import { Owner } from 'app/models/account.model';
import * as accountActions from '../../core/actions/account';
import { AccountService } from 'app/core/services/account.service';
import * as permissions from '../../models/permissions';

@Component({
  selector: 'app-invoice-owner-setting',
  templateUrl: './invoice-owner-setting.component.html',
  styleUrls: ['./invoice-owner-setting.component.scss'],
  animations: fuseAnimations
})
export class InvoiceOwnerSettingComponent implements OnInit, OnDestroy {

  invoicePeriodValues = InvoicePeriodValues;
  
  private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
  invoiceOwnerSettingForm: FormGroup;
  formSubmit: boolean = false;
  pageType = '';
  isCreating: boolean;
  createError$: Observable<string>;
  updateError$: Observable<string>;
  getDetailError$: Observable<string>;

  invoiceOwnerSettingCreated$: Observable<InvoiceOwnerSetting>;
  invoiceOwnerSettingGetDetail$: Observable<InvoiceOwnerSetting>;
  getDetailLoading$: Observable<boolean>;
  getDetailLoading: boolean;
  invoiceOwnerSettingUpdateSuccess: Observable<boolean>;

  tenantsLoading$: Observable<boolean>;
  tenantsLoading: boolean;
  tenants$: Observable<Tenant[]>;

  selectedTenant$: Observable<Tenant>;
  selectedTenant: Tenant;

  owners$: Observable<Owner[]>;
  owners: Owner[] = undefined;
  ownersLoading$: Observable<boolean>;
  ownersLoading: boolean = false;

  allowAddInvoiceOwnerSetting: boolean = false;

  @Output() formChanges: EventEmitter<boolean> = new EventEmitter();

  id: number;

  constructor(private store: Store<coreState.State>,
      private dateAdapter: DateAdapter<any>,
      private router: Router,
      private fb: FormBuilder,
      public snackBar: MatSnackBar,
      private route: ActivatedRoute,
      private translateService: TranslateService,
      private randomService: RandomService,
      private accountService : AccountService) {
      this.dateAdapter.setLocale('gb');

      this.allowAddInvoiceOwnerSetting = this.accountService.isUserAuthorizedForTask(permissions.AddInvoiceOwnerSetting);
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
      this.invoiceOwnerSettingCreated$ = this.store.select(coreState.getInvoiceOwnerSettingCreated);
      this.invoiceOwnerSettingGetDetail$ = this.store.select(coreState.getInvoiceOwnerSettingDetails);
      this.invoiceOwnerSettingUpdateSuccess = this.store.select(coreState.getInvoiceOwnerSettingEditSuccess);
      this.createError$ = this.store.select(coreState.getInvoiceOwnerSettingCreateError);
      this.updateError$ = this.store.select(coreState.getInvoiceOwnerSettingEditError);
      this.getDetailError$ = this.store.select(coreState.getInvoiceOwnerSettingDetailError);
      this.getDetailLoading$ = this.store.select(coreState.getInvoiceOwnerSettingDetailLoading);

      this.tenantsLoading$ = this.store.select(coreState.getTenantLoading);
      this.tenants$ = this.store.select(coreState.getTenantSearchResults);

      this.route.params.subscribe(params => {

          this.id = params['id'];

          if (this.id != 0 && this.id != null && this.id != undefined) {
              this.pageType = 'edit';
              this.loadInvoiceOwnerSettingDetail();
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

          if (t && t.tenantDomainPlatformMapGuid && this.invoiceOwnerSettingForm && this.pageType == 'new') {
              this.invoiceOwnerSettingForm.get('tenantGuid').setValue(t.tenantDomainPlatformMapGuid);
          }

          if (t && t.tenantDomainPlatformMapGuid) {
            this.store.dispatch(new accountActions.GetOwners());
          }
      });

      this.owners$ = this.store.select(coreState.getOwners);
      this.ownersLoading$ = this.store.select(coreState.getAccountLoading);

      this.ownersLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
          this.ownersLoading = l;
      });

      this.owners$.pipe(takeUntil(this.destroyed$)).subscribe(items => {
          this.owners = items;
      });
  }

  ngOnDestroy(): void {
      this.store.dispatch(new invoiceOwnerSettingActions.ClearErrors());
      this.destroyed$.next(true);
      this.destroyed$.complete();
      this.formChanges.emit(false);
  }

  createForm(): void {
      this.invoiceOwnerSettingForm = this.fb.group({
          tenantGuid: new FormControl(this.selectedTenant == undefined ? undefined : this.selectedTenant.tenantDomainPlatformMapGuid, [Validators.required]),
          isActive: new FormControl(true),
          ownerGuid: new FormControl(undefined, [Validators.required]),
          startDate: new FormControl(undefined, [Validators.required]),
          invoicePeriod: new FormControl(InvoicePeriods.Monthly, [Validators.required]),
          cartipayDepositRate: new FormControl(0, [Validators.required]),
          cartipalDepositRate: new FormControl(0, [Validators.required]),
          cartipalWithdrawalRate: new FormControl(0, [Validators.required]),
          withdrawalRate: new FormControl(0, [Validators.required]),
      });

      this.invoiceOwnerSettingForm.valueChanges.pipe(
          debounceTime(300),
          distinctUntilChanged()
      )
          .subscribe(() => {
              const emptyForm: InvoiceOwnerSetting = new InvoiceOwnerSetting({});
              const changes: boolean = GenericHelper.detectNonNullableChanges(this.invoiceOwnerSettingForm.value, emptyForm);
              this.formChanges.emit(changes);
          });
  }

  createUpdateForm(data: InvoiceOwnerSetting): void {

      var array: FormGroup[] = [];

    //   data.startDate = this.convertUTCDateToLocalDate(new Date(data.startDate));

      this.invoiceOwnerSettingForm = this.fb.group({
          id: new FormControl(data.id),
          isActive: new FormControl(data.isActive),
          ownerGuid: new FormControl(data.ownerGuid, [Validators.required]),
          startDate: new FormControl(data.startDate, [Validators.required]),
          invoicePeriod: new FormControl(data.invoicePeriod, [Validators.required]),
          cartipayDepositRate: new FormControl(data.cartipayDepositRate, [Validators.required]),
          cartipalDepositRate: new FormControl(data.cartipalDepositRate, [Validators.required]),
          cartipalWithdrawalRate: new FormControl(data.cartipalWithdrawalRate, [Validators.required]),
          withdrawalRate: new FormControl(data.withdrawalRate, [Validators.required]),
          tenantGuid: new FormControl(data.tenantGuid, [Validators.required]),
      });

      this.invoiceOwnerSettingForm.valueChanges.pipe(
          debounceTime(300),
          distinctUntilChanged()
      )
          .subscribe(() => {
              const emptyForm: InvoiceOwnerSetting = new InvoiceOwnerSetting({});
              const changes: boolean = GenericHelper.detectNonNullableChanges(this.invoiceOwnerSettingForm.value, emptyForm);
              this.formChanges.emit(changes);
          });
  }

  private loadInvoiceOwnerSettingDetail(): void {
      this.store.dispatch(new invoiceOwnerSettingActions.GetDetails(this.id));

      this.invoiceOwnerSettingGetDetail$.pipe(takeUntil(this.destroyed$)).subscribe(p => {
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

      if (this.invoiceOwnerSettingForm.valid) {
          let form = this.invoiceOwnerSettingForm.value;
          this.formSubmit = true;
          this.store.dispatch(new invoiceOwnerSettingActions.Create(form));

          this.invoiceOwnerSettingCreated$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
              .subscribe(
                  tnCreated => {
                      this.isCreating = false;
                      this.router.navigate(['/invoiceownersettings']);
                  });

      }
      else {
          FormHelper.validateFormGroup(this.invoiceOwnerSettingForm);
      }
  }

  onUpdateSubmit(): void {
      this.formSubmit = false;
      this.isCreating = true;

      if (this.invoiceOwnerSettingForm.valid) {
          let form = this.invoiceOwnerSettingForm.value;
          this.formSubmit = true;
          this.store.dispatch(new invoiceOwnerSettingActions.Edit(this.id, form));

          this.invoiceOwnerSettingUpdateSuccess.pipe(filter(tnCreated => tnCreated !== false), take(1))
              .subscribe(
                  tnCreated => {
                      this.isCreating = false;

                      this.router.navigate(['/invoiceownersettings']);
                  });
      }
      else {
          FormHelper.validateFormGroup(this.invoiceOwnerSettingForm);
      }
  }

  isLoading() {
      return this.tenantsLoading || this.getDetailLoading || this.isCreating;
  }

  getOwners() {
      if (this.owners == undefined || this.owners == null) {
          return [];
      }

      return this.owners;
  }

  getOwnerName(ownerGuid: string) : string {
      if (this.owners != undefined && this.owners != null) {
          var owner = this.owners.find(t => t.accountId == ownerGuid);

          if(owner != null){
              return owner.username;
          }
      }

      return '';
  }

  convertUTCDateToLocalDate(date: Date): Date {
      var newDate = new Date(date.getTime() + date.getTimezoneOffset() * 60 * 1000);

      var offset = date.getTimezoneOffset() / 60;
      var hours = date.getHours();

      newDate.setHours(hours - offset);

      return newDate;
  }

}

