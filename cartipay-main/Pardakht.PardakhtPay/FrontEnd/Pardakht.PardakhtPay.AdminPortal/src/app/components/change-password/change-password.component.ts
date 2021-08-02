import { Component, OnInit, Inject, OnDestroy } from '@angular/core';
import { Observable } from 'rxjs';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { ChangePasswordForm } from '../../core/models/user-management.model';
import { ReplaySubject } from 'rxjs/ReplaySubject';
import { Store } from '@ngrx/store';
import * as coreState from './../../core/index';
import * as userActions from './../../core/actions/user';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'lib-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent implements OnInit, OnDestroy {
  passwordChanged$: Observable<boolean>;
  loading$: Observable<boolean>;
  errorMessage$: Observable<string>;
  errorMessage: string;
  changed = false;
  hide = true;
  private destroyed$: ReplaySubject<boolean> = new ReplaySubject(1);

  ngOnDestroy(): void {
    this.destroyed$.next(true);
    this.destroyed$.complete();
    // this.store.dispatch(new account.ClearErrors());
  }

  constructor(
    public dialogRef: MatDialogRef<ChangePasswordComponent>,
    private store: Store<coreState.State>,
    @Inject(MAT_DIALOG_DATA) public data: ChangePasswordForm) { }

  onNoClick(): void {
    this.dialogRef.close();
  }

  ngOnInit(): void {
    this.passwordChanged$ = this.store.select(coreState.getPasswordChanged);
    this.passwordChanged$.pipe(takeUntil(this.destroyed$))
      .subscribe((result: boolean) => {
        if (result === true) {
          this.changed = true;
        } else {
          this.changed = false;
        }
      });

    this.loading$ = this.store.select(coreState.getUserLoading);

    this.errorMessage$ = this.store.select(coreState.getPasswordChangeError);
    this.errorMessage$.pipe(takeUntil(this.destroyed$))
      .subscribe((errorMessage: string) => {
        if (errorMessage) {
          this.errorMessage = errorMessage;
        }
      });

    this.store.dispatch(new userActions.ChangePasswordInit());
  }

  passwordInputChanged(): void{
    this.errorMessage = undefined;
  }

  onChangeClick(result): void {
    // TODO: utilise code from the generic platform using reactive forms + validators
    if (result.newPassword !== result.newPasswordConfirm)
    {
      this.errorMessage = 'New password confirmation does not match';
      return;
    }
    this.store.dispatch(new userActions.ChangePassword(this.data));
  }

}
