import { Component, OnInit, Output, EventEmitter, OnDestroy } from '@angular/core';
import { ReplaySubject, Observable } from 'rxjs';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { Store } from '@ngrx/store';
import * as coreState from '../../core';
import * as applicationSettingsActions from '../../core/actions/applicationSettings';
import { TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material';
import { takeUntil, filter, take } from 'rxjs/operators';
import { FormHelper } from '../../helpers/forms/form-helper';
import { OwnerSetting } from 'app/models/owner-setting';
import { fuseAnimations } from '../../core/animations';

@Component({
  selector: 'app-owner-setting',
  templateUrl: './owner-setting.component.html',
  styleUrls: ['./owner-setting.component.scss'],
  animations: fuseAnimations
})
export class OwnerSettingComponent implements OnInit, OnDestroy {

  private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);
  settingsForm: FormGroup;
  formSubmit: boolean = false;
  isCreating: boolean;
  getDetailError$: Observable<string>;
  getDetailLoading$: Observable<boolean>;
  getDetailLoading: boolean;

  getDetail$: Observable<OwnerSetting>;
  updateSuccess$: Observable<boolean>;
  @Output() formChanges: EventEmitter<boolean> = new EventEmitter();

  constructor(private store: Store<coreState.State>,
      private translateService: TranslateService,
      private fb: FormBuilder,
      public snackBar: MatSnackBar) { }

  ngOnInit() {

      this.getDetailError$ = this.store.select(coreState.getOwnerSettingError);
      this.getDetailLoading$ = this.store.select(coreState.getApplicationSettingsLoading);
      this.getDetail$ = this.store.select(coreState.getOwnerSetting);
      this.updateSuccess$ = this.store.select(coreState.getOwnerSettingUpdated);

      this.getDetailError$.pipe(takeUntil(this.destroyed$)).subscribe(error => {
          if (error) {
              this.openSnackBar(error);
          }
      });

      this.getDetailLoading$.pipe(takeUntil(this.destroyed$)).subscribe(l => {
          this.getDetailLoading = l;
      });

      this.getDetail$.pipe(takeUntil(this.destroyed$)).subscribe(detail => {
          if (detail) {
              this.createUpdateForm(detail);
          }
      });

      this.updateSuccess$.pipe(filter(tnCreated => tnCreated))
          .subscribe(
              () => {
                  this.isCreating = false;
                  this.openSnackBar('Operation completed');
              });

      this.store.dispatch(new applicationSettingsActions.GetOwnerSetting());
  }

  createUpdateForm(data: OwnerSetting): void {

      this.settingsForm = this.fb.group({
        id: new FormControl(data.id),
        waitAmountForCurrentWithdrawal: new FormControl(data.waitAmountForCurrentWithdrawal)
      });
  }

  onSubmit() {
      this.formSubmit = false;

      if (this.settingsForm.valid) {
          let form = this.settingsForm.value;
          this.formSubmit = true;
          this.store.dispatch(new applicationSettingsActions.SaveOwnerSetting(form));

      }
      else {
          FormHelper.validateFormGroup(this.settingsForm);
      }
  }

  ngOnDestroy(): void {
      this.store.dispatch(new applicationSettingsActions.Clear());
      this.destroyed$.next(true);
      this.destroyed$.complete();
      this.formChanges.emit(false);
  }

  openSnackBar(message: string, action: string = undefined) {
      if (!action) {
          action = this.translateService.instant('GENERAL.OK');
      }
      this.snackBar.open(message, action, {
          duration: 10000,
      });
  }

}
