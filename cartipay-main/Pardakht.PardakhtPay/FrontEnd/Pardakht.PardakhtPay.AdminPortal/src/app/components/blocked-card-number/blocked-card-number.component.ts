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
import * as accountActions from '../../core/actions/account';
import { BlockedCardNumber } from '../../models/blocked-card-number';
import * as blockedCardNumberActions from '../../core/actions/blockedCardNumber';
import { AccountService } from '../../core/services/account.service';
import * as permissions from '../../models/permissions';

@Component({
  selector: 'app-blocked-card-number',
  templateUrl: './blocked-card-number.component.html',
  styleUrls: ['./blocked-card-number.component.scss'],
  animations: fuseAnimations
})
export class BlockedCardNumberComponent implements OnInit, OnDestroy {

  private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
  blockedCardNumberForm: FormGroup;
  formSubmit: boolean = false;
  pageType = '';
  isCreating: boolean;
  createError$: Observable<string>;
  updateError$: Observable<string>;
  getDetailError$: Observable<string>;
  getDetailLoading$: Observable<boolean>;
  getDetailLoading: boolean;

  blockedCardNumberCreated$: Observable<BlockedCardNumber>;
  blockedCardNumberGetDetail$: Observable<BlockedCardNumber>;
  blockedCardNumberUpdateSuccess: Observable<boolean>;
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

  searchError$: Observable<string>;
  selected = [];
  loading$: Observable<boolean>;
  loading: boolean;

  id: number;

  allowAddBlockedCardNumber: boolean = false;

  constructor(private store: Store<coreState.State>,
      private router: Router,
      private fb: FormBuilder,
      public snackBar: MatSnackBar,
      private route: ActivatedRoute,
      private accountService: AccountService,
      private translateService: TranslateService) {

      this.allowAddBlockedCardNumber = this.accountService.isUserAuthorizedForTask(permissions.AddBlockedCardNumber);
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

      this.blockedCardNumberCreated$ = this.store.select(coreState.getBlockedCardNumberCreated);
      this.blockedCardNumberGetDetail$ = this.store.select(coreState.getBlockedCardNumberDetails);
      this.blockedCardNumberUpdateSuccess = this.store.select(coreState.getBlockedCardNumberEditSuccess);
      this.createError$ = this.store.select(coreState.getBlockedCardNumberCreateError);
      this.updateError$ = this.store.select(coreState.getBlockedCardNumberEditError);
      this.getDetailError$ = this.store.select(coreState.getBlockedCardNumberDetailError);
      this.getDetailLoading$ = this.store.select(coreState.getBlockedCardNumberDetailLoading);

      this.tenantsLoading$ = this.store.select(coreState.getTenantLoading);
      this.tenants$ = this.store.select(coreState.getTenantSearchResults);

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
              this.loadBlockedCardNumberDetail();
          }
          else {
              this.pageType = 'new';
              this.createForm();
          }
      });

      this.selectedTenant$ = this.store.select(coreState.getSelectedTenant);

      this.selectedTenant$.pipe(takeUntil(this.destroyed$)).subscribe(t => {
          this.selectedTenant = t;
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
      this.store.dispatch(new blockedCardNumberActions.ClearErrors());
      this.destroyed$.next(true);
      this.destroyed$.complete();
      this.formChanges.emit(false);
  }

  createForm(): void {
      this.blockedCardNumberForm = this.fb.group({
          cardNumber: new FormControl(undefined, [Validators.required])
      });

      this.blockedCardNumberForm.valueChanges.pipe(
          debounceTime(300),
          distinctUntilChanged()
      )
          .subscribe(() => {
              const emptyForm: BlockedCardNumber = new BlockedCardNumber({});
              const changes: boolean = GenericHelper.detectNonNullableChanges(this.blockedCardNumberForm.value, emptyForm);
              this.formChanges.emit(changes);
          });
  }

  createUpdateForm(data: BlockedCardNumber): void {

      this.blockedCardNumberForm = this.fb.group({
          id: data.id,
          cardNumber: new FormControl({ value: data.cardNumber, disabled: !this.allowAddBlockedCardNumber }, [Validators.required]),
      });

      this.blockedCardNumberForm.valueChanges.pipe(
          debounceTime(300),
          distinctUntilChanged()
      )
          .subscribe(() => {
              const emptyForm: BlockedCardNumber = new BlockedCardNumber({});
              const changes: boolean = GenericHelper.detectNonNullableChanges(this.blockedCardNumberForm.value, emptyForm);
              this.formChanges.emit(changes);
          });
  }

  private loadBlockedCardNumberDetail(): void {
      this.store.dispatch(new blockedCardNumberActions.GetDetails(this.id));

      this.blockedCardNumberGetDetail$.pipe(takeUntil(this.destroyed$)).subscribe(p => {
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

      if (this.blockedCardNumberForm.valid) {
          let form = this.blockedCardNumberForm.value;
          this.formSubmit = true;
          this.store.dispatch(new blockedCardNumberActions.Create(form));

          this.blockedCardNumberCreated$.pipe(filter(tnCreated => tnCreated !== undefined), take(1))
              .subscribe(
                  tnCreated => {
                      this.isCreating = false;
                      this.router.navigate(['/blockedcardnumbers']);
                  });

      }
      else {
          FormHelper.validateFormGroup(this.blockedCardNumberForm);
      }
  }

  onUpdateSubmit(): void {
      this.formSubmit = false;

      if (this.blockedCardNumberForm.valid) {
          let form = this.blockedCardNumberForm.value;
          this.formSubmit = true;
          this.store.dispatch(new blockedCardNumberActions.Edit(this.id, form));

          this.blockedCardNumberUpdateSuccess.pipe(filter(tnCreated => tnCreated !== false), take(1))
              .subscribe(
                  tnCreated => {
                      this.router.navigate(['/blockedcardnumbers']);
                  });

      }
      else {
          FormHelper.validateFormGroup(this.blockedCardNumberForm);
      }
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