import { Component, OnInit, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import { Observable, ReplaySubject } from 'rxjs';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { Store } from '@ngrx/store';
import { DateAdapter, MatSnackBar } from '@angular/material';
import * as coreState from '../../core';
import { GenericHelper } from '../../helpers/generic';
import { CustomValidators, FormHelper } from '../../helpers/forms/form-helper';
import { debounceTime, distinctUntilChanged, take } from 'rxjs/operators';
import { InvoicePayment } from '../../models/invoice';
import { fuseAnimations } from '../../core/animations';
import * as invoicePaymentActions from '../../core/actions/invoicePayment';
import { filter } from 'rxjs/operators/filter';
import { Router, ActivatedRoute } from '@angular/router';
import { RandomService } from '../../core/services/random/random.service';
import { TranslateService } from '@ngx-translate/core';
import { takeUntil } from 'rxjs/operators';
import { Owner } from 'app/models/account.model';
import * as accountActions from '../../core/actions/account';
import { Tenant } from 'app/models/tenant';
import * as permissions from '../../models/permissions';
import { AccountService } from 'app/core/services/account.service';

@Component({
  selector: 'app-invoice-payment',
  templateUrl: './invoice-payment.component.html',
  styleUrls: ['./invoice-payment.component.scss'],
  animations: fuseAnimations
})
export class InvoicePaymentComponent implements OnInit, OnDestroy {
  
  private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
  invoicePaymentForm: FormGroup;
  formSubmit: boolean = false;
  pageType = '';
  isCreating: boolean;
  createError$: Observable<string>;
  updateError$: Observable<string>;
  getDetailError$: Observable<string>;

  invoicePaymentCreated$: Observable<InvoicePayment>;
  invoicePaymentGetDetail$: Observable<InvoicePayment>;
  getDetailLoading$: Observable<boolean>;
  getDetailLoading: boolean;
  invoicePaymentUpdateSuccess: Observable<boolean>;

  tenantsLoading$: Observable<boolean>;
  tenantsLoading: boolean;
  tenants$: Observable<Tenant[]>;

  selectedTenant$: Observable<Tenant>;
  selectedTenant: Tenant;

  owners$: Observable<Owner[]>;
  owners: Owner[] = undefined;
  ownersLoading$: Observable<boolean>;
  ownersLoading: boolean = false;

  allowAddInvoicePayment: boolean = false;

  @Output() formChanges: EventEmitter<boolean> = new EventEmitter();

  id: number;

  constructor(private store: Store<coreState.State>,
      private dateAdapter: DateAdapter<any>,
      private router: Router,
      private fb: FormBuilder,
      public snackBar: MatSnackBar,
      private route: ActivatedRoute,
      private translateService: TranslateService,
      private accountService : AccountService) {
      this.dateAdapter.setLocale('gb');
      this.allowAddInvoicePayment = this.accountService.isUserAuthorizedForTask(permissions.AddInvoicePayment);
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
      this.invoicePaymentCreated$ = this.store.select(coreState.getInvoicePaymentCreated);
      this.invoicePaymentGetDetail$ = this.store.select(coreState.getInvoicePaymentDetails);
      this.invoicePaymentUpdateSuccess = this.store.select(coreState.getInvoicePaymentEditSuccess);
      this.createError$ = this.store.select(coreState.getInvoicePaymentCreateError);
      this.updateError$ = this.store.select(coreState.getInvoicePaymentEditError);
      this.getDetailError$ = this.store.select(coreState.getInvoicePaymentDetailError);
      this.getDetailLoading$ = this.store.select(coreState.getInvoicePaymentDetailLoading);

      this.tenantsLoading$ = this.store.select(coreState.getTenantLoading);
      this.tenants$ = this.store.select(coreState.getTenantSearchResults);

      this.route.params.subscribe(params => {

          this.id = params['id'];

          if (this.id != 0 && this.id != null && this.id != undefined) {
              this.pageType = 'edit';
              this.loadInvoicePaymentDetail();
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

          if (t && t.tenantDomainPlatformMapGuid && this.invoicePaymentForm && this.pageType == 'new') {
              this.invoicePaymentForm.get('tenantGuid').setValue(t.tenantDomainPlatformMapGuid);
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
      this.store.dispatch(new invoicePaymentActions.ClearErrors());
      this.destroyed$.next(true);
      this.destroyed$.complete();
      this.formChanges.emit(false);
  }

  createForm(): void {
      this.invoicePaymentForm = this.fb.group({
          tenantGuid: new FormControl(this.selectedTenant == undefined ? undefined : this.selectedTenant.tenantDomainPlatformMapGuid, [Validators.required]),
          ownerGuid: new FormControl(undefined, [Validators.required]),
          paymentDate: new FormControl(undefined, [Validators.required]),
          paymentReference: new FormControl(undefined),
          amount: new FormControl(undefined, [Validators.required])
      });

      this.invoicePaymentForm.valueChanges.pipe(
          debounceTime(300),
          distinctUntilChanged()
      )
          .subscribe(() => {
              const emptyForm: InvoicePayment = new InvoicePayment({});
              const changes: boolean = GenericHelper.detectNonNullableChanges(this.invoicePaymentForm.value, emptyForm);
              this.formChanges.emit(changes);
          });
  }

  createUpdateForm(data: InvoicePayment): void {

      var array: FormGroup[] = [];

    //   data.paymentDate = this.convertUTCDateToLocalDate(new Date(data.paymentDate));

      this.invoicePaymentForm = this.fb.group({
          id: new FormControl(data.id),
          ownerGuid: new FormControl(data.ownerGuid, [Validators.required]),
          paymentDate: new FormControl(data.paymentDate, [Validators.required]),
          paymentReference: new FormControl(data.paymentReference),
          amount: new FormControl(data.amount, [Validators.required]),
          tenantGuid: new FormControl(data.tenantGuid, [Validators.required])
      });

      this.invoicePaymentForm.valueChanges.pipe(
          debounceTime(300),
          distinctUntilChanged()
      )
          .subscribe(() => {
              const emptyForm: InvoicePayment = new InvoicePayment({});
              const changes: boolean = GenericHelper.detectNonNullableChanges(this.invoicePaymentForm.value, emptyForm);
              this.formChanges.emit(changes);
          });
  }

  private loadInvoicePaymentDetail(): void {
      this.store.dispatch(new invoicePaymentActions.GetDetails(this.id));

      this.invoicePaymentGetDetail$.pipe(takeUntil(this.destroyed$)).subscribe(p => {
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

      if (this.invoicePaymentForm.valid) {
          let form = this.invoicePaymentForm.value;
          this.formSubmit = true;
          this.store.dispatch(new invoicePaymentActions.Create(form));

          this.invoicePaymentCreated$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
              .subscribe(
                  tnCreated => {
                      this.isCreating = false;
                      this.router.navigate(['/invoicepayments']);
                  });

      }
      else {
          FormHelper.validateFormGroup(this.invoicePaymentForm);
      }
  }

  onUpdateSubmit(): void {
      this.formSubmit = false;
      this.isCreating = true;

      if (this.invoicePaymentForm.valid) {
          let form = this.invoicePaymentForm.value;
          this.formSubmit = true;
          this.store.dispatch(new invoicePaymentActions.Edit(this.id, form));

          this.invoicePaymentUpdateSuccess.pipe(filter(tnCreated => tnCreated !== false), take(1))
              .subscribe(
                  tnCreated => {
                      this.isCreating = false;

                      this.router.navigate(['/invoicepayments']);
                  });
      }
      else {
          FormHelper.validateFormGroup(this.invoicePaymentForm);
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

