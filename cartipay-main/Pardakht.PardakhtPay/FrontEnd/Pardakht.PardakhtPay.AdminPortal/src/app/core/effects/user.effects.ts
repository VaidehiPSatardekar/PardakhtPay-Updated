import { Inject, Injectable, InjectionToken, Optional } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { Actions, Effect } from '@ngrx/effects';
import { Action } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { empty } from 'rxjs/observable/empty';
import { of } from 'rxjs/observable/of';
import { catchError } from 'rxjs/operators/catchError';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { map } from 'rxjs/operators/map';
import { mergeMap } from 'rxjs/operators/mergeMap';
import { skip } from 'rxjs/operators/skip';
import { switchMap } from 'rxjs/operators/switchMap';
import { takeUntil } from 'rxjs/operators/takeUntil';
import { Scheduler } from 'rxjs/Scheduler';
import { tap } from 'rxjs/operators/tap';
import { async } from 'rxjs/scheduler/async';
import { AddOne } from '../actions/notification';
import { NotificationMessage } from '../actions/notification'
import { NotificationType } from 'angular2-notifications';

import * as user from '../actions/user';
import { EffectBase } from '../../core/effects/effect-base';
import { ISuspension, StaffUser, PasswordResetResponse, CreateStaffUserResponse, LoginAsStaffUserRequest, StaffUserPerformanceTime } from '../models/user-management.model';
import { LoginForm, LoginResponse } from '../models/user-management.model';
import { UserService } from './../services/user.service';

export const SEARCH_DEBOUNCE: InjectionToken<number> = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER: InjectionToken<Scheduler> = new InjectionToken<Scheduler>('Search Scheduler');

/**
 * Effects offer a way to isolate and easily test side-effects within your
 * application.
 *
 * If you are unfamiliar with the operators being used in these examples, please
 * check out the sources below:
 *
 * Official Docs: http://reactivex.io/rxjs/manual/overview.html#categories-of-operators
 * RxJS 5 Operators By Example: https://gist.github.com/btroncone/d6cf141d6f2c00dc6b35
 */

@Injectable()
export class UserEffects extends EffectBase {

  @Effect()
  initialize$: Observable<Action> = this.actions$.ofType<user.Initialize>(user.INITIALIZE).pipe(
    switchMap(() => {
      if (this.userService.isAuthenticated()) {
        return this.userService.getCurrentUser().pipe(
          map((staffUser: StaffUser) => new user.InitializeComplete(staffUser)),
          catchError((err: any) => {
            //this.userService.logOut();
            //this.router.navigate(['/login']);
            // return of(new account.InitializeError(this.sanitiseError(err)));
            return of(new user.Logout(), new user.InitializeError(''));
          })
        );
      } else {
        return of(new user.InitializeComplete(undefined));
      }
    })
  );

  @Effect()
  login$: Observable<Action> = this.actions$.ofType<user.Login>(user.LOGIN).pipe(
    map((action: user.Login) => action.payload),
    switchMap((payload: LoginForm) => {
      return this.userService.logIn(payload).pipe(
        tap(() => this.navigateToDefaultPage()),
        mergeMap((response: LoginResponse) => [
          new user.LoginComplete(response),
          // new user.Initialize()
        ]),
        catchError((err: HttpErrorResponse) => of(new user.LoginError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  loginAs$: Observable<Action> = this.actions$.ofType<user.LoginAsStaffUser>(user.LOGIN_AS_STAFF_USER).pipe(
    map((action: user.LoginAsStaffUser) => action.payload),
    switchMap((payload: LoginAsStaffUserRequest) => {
      return this.userService.logInAsStaffUser(payload).pipe(
        tap(() => {
          //this.navigateToDefaultPage();
          window.location.href = '/';
        }),
        mergeMap((response: LoginResponse) => [
          new user.LoginAsStaffUserComplete(response),
        ]),
        catchError((err: HttpErrorResponse) => of(new user.LoginAsStaffUserError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  logout$: Observable<Action> = this.actions$.ofType<user.Logout>(user.LOGOUT).pipe(
    switchMap(() => {
      return this.userService.logOut().pipe(
        tap(() => {
          this.router.navigate(['/login']);
        }),
        mergeMap(() => [
          new user.LogoutComplete()
        ]),
        catchError((err: any) => of(new user.LogoutError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  logoutExpired$: Observable<Action> = this.actions$.ofType<user.LoginExpired>(user.LOGIN_EXPIRED).pipe(
    tap(() => {
      localStorage.clear();
      this.router.navigate(['/login']);
    }),
    switchMap(() => {
      return of(new user.LogoutComplete());
    })
  );

  @Effect()
  addIdleTime: Observable<Action> = this.actions$.ofType<user.AddIdleTime>(user.ADD_IDLE_TIME).pipe(

    switchMap((action: user.AddIdleTime) => {
      return this.userService.addIdleTime(action.addIdleMinutesRequest).pipe(

        map((response: StaffUserPerformanceTime) => new user.AddIdleTimeComplete(response)),
        catchError((err: any) => of(new user.AddIdleTimeError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  getStaffUsers$: Observable<Action> = this.actions$.ofType<user.GetStaffUsers>(user.GET_STAFF_USERS).pipe(
    switchMap((action: user.GetStaffUsers) => {
      if (action === undefined) {
        return empty();
      }

      return this.userService.getStaffUsers(action.tenantGuid, action.brandId).pipe(
        map((response: StaffUser[]) => new user.GetStaffUsersComplete(response)),
        catchError((err: any) => of(new user.GetStaffUsersError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  getAffiliateUsers$: Observable<Action> = this.actions$.ofType<user.GetAffiliateUsers>(user.GET_AFFILIATE_USERS).pipe(
    switchMap((action: user.GetAffiliateUsers) => {
      if (action === undefined) {
        return empty();
      }
      return this.userService.getAffiliateUsers(action.tenantGuid).pipe(
        map((response: StaffUser[]) => new user.GetAffiliateUsersComplete(response)),
        catchError((err: any) => of(new user.GetAffiliateUsersError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  getSystemUsers$: Observable<Action> = this.actions$.ofType<user.GetSystemUsers>(user.GET_SYSTEM_USERS).pipe(
    switchMap((action: user.GetSystemUsers) => {
      if (action === undefined) {
        return empty();
      }
      return this.userService.getSystemUsers(action.tenantGuid).pipe(
        map((response: StaffUser[]) => new user.GetSystemUsersComplete(response)),
        catchError((err: any) => of(new user.GetSystemUsersError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  updateTrackingTime: Observable<Action> = this.actions$.ofType<user.UpdateTrackingTime>(user.UPDATE_TRACKING_TIME).pipe(

    switchMap((action: user.UpdateTrackingTime) => {
      return this.userService.updateTrackingTime().pipe(

        map((response: StaffUserPerformanceTime) => new user.UpdateTrackingTimeComplete(response)),
        catchError((err: any) => of(new user.UpdateTrackingTimeError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  createStaffUser$: Observable<Action> = this.actions$.ofType<user.CreateStaffUser>(user.CREATE_STAFF_USER).pipe(
    map((action: user.CreateStaffUser) => action.payload),
    switchMap((payload: StaffUser) => {
      if (payload === undefined) {
        return empty();
      }

      return this.userService.createStaffUser(payload).pipe(
        mergeMap((response: CreateStaffUserResponse) => [
          new user.CreateStaffUserComplete(response),
          // response.tenantId && response.tenantId > 0 ? new tenant.CreateStaffUserComplete(response) : new provider.CreateStaffUserComplete(response),
          new AddOne(new NotificationMessage('User ' + response.staffUser.username + ' created'))
        ]),
        catchError((err: any) => of(new user.CreateStaffUserError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  editStaffUser$: Observable<Action> = this.actions$.ofType<user.EditStaffUser>(user.EDIT_STAFF_USER).pipe(
    switchMap((action: user.EditStaffUser) => {
      if (action === undefined || action.payload === undefined) {
        return empty();
      }

      return this.userService.editStaffUser(action.payload).pipe(
        mergeMap((response: StaffUser) => [
          new user.EditStaffUserComplete(response),
          // response.tenantId && response.tenantId > 0 ? new tenant.EditStaffUserComplete(response) : new provider.EditStaffUserComplete(response),
          new AddOne(new NotificationMessage('User ' + response.username + ' updated'))
        ]),
        catchError((err: any) => of(new user.EditStaffUserError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  changePassword$: Observable<Action> = this.actions$.ofType<user.ChangePassword>(user.CHANGE_PASSWORD).pipe(
    map(action => action.payload),
    switchMap(payload => {
      if (payload === undefined) {
        return empty();
      }

      return this.userService.changePassword(payload).pipe(
        mergeMap((response: boolean) => [
          new user.ChangePasswordComplete(response)
          // new notification.AddOne('Forgot password request '  + (response ? 'sent' : 'failed'))
        ]),
        catchError((err: any) => {
          return of(new user.ChangePasswordError(this.sanitiseError(err)));
        })
      );
    })
  );

  @Effect()
  forgotPassword$: Observable<Action> = this.actions$.ofType<user.ForgotPassword>(user.FORGOT_PASSWORD).pipe(
    switchMap((action: user.ForgotPassword) => {
      if (action === undefined || action.email === undefined) {
        return empty();
      }

      return this.userService.forgotPassword(action.email).pipe(
        mergeMap((response: any) => [
          new user.ForgotPasswordComplete(),
          new AddOne(new NotificationMessage('Password reset - an email will be sent with your new password'))
        ]),
        catchError((err: any) => {
          console.log(err);
          return of(new user.ForgotPasswordError(this.sanitiseError(err)));
        })
      );
    })
  );

  @Effect()
  forgotPasswordByUsername$: Observable<Action> = this.actions$.ofType<user.ForgotPasswordByUsername>(user.FORGOT_PASSWORD_BY_USERNAME).pipe(
    switchMap((action: user.ForgotPasswordByUsername) => {
      if (action === undefined || action.username === undefined) {
        return empty();
      }

      return this.userService.forgotPasswordByUsername(action.username).pipe(
        mergeMap((response: any) => [
          new user.ForgotPasswordByUsernameComplete(),
          new AddOne(new NotificationMessage('Password reset - an email will be sent with your new password'))
        ]),
        catchError((err: any) => {
          console.log(err);
          return of(new user.ForgotPasswordByUsernameError(this.sanitiseError(err)));
        })
      );
    })
  );

  @Effect()
  resetPassword$: Observable<Action> = this.actions$.ofType<user.ResetPassword>(user.RESET_PASSWORD).pipe(
    switchMap((action: user.ResetPassword) => {
      if (action === undefined || action.payload === undefined) {
        return empty();
      }

      return this.userService.resetPassword(action.payload).pipe(
        mergeMap((response: PasswordResetResponse) => [
          new user.ResetPasswordComplete(response),
          new AddOne(new NotificationMessage('Password reset'))
        ]),
        catchError((err: any) => of(new user.ResetPasswordError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  blockStaffUser$: Observable<Action> = this.actions$.ofType<user.BlockStaffUser>(user.BLOCK_STAFF_USER).pipe(
    switchMap((action: user.BlockStaffUser) => {
      if (action === undefined || action.payload === undefined) {
        return empty();
      }

      return this.userService.blockStaffUser(action.payload).pipe(
        mergeMap((response: StaffUser) => [
          new user.BlockStaffUserComplete(response),
          new AddOne(new NotificationMessage('User ' + response.username + ' has been ' + (response.isBlocked ? 'blocked' : 'unblocked')))
        ]),
        catchError((err: any) => of(new user.BlockStaffUserError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  deleteStaffUser$: Observable<Action> = this.actions$.ofType<user.DeleteStaffUser>(user.DELETE_STAFF_USER).pipe(
    switchMap((action: user.DeleteStaffUser) => {
      if (action === undefined || action.payload === undefined) {
        return empty();
      }

      return this.userService.deleteStaffUser(action.payload).pipe(
        mergeMap((response: StaffUser) => [
          new user.DeleteStaffUserComplete(response),
          new AddOne(new NotificationMessage('User ' + response.username + ' has been deleted'))
        ]),
        catchError((err: any) => of(new user.DeleteStaffUserError(this.sanitiseError(err))))
      );
    })
  );

  constructor(
    private actions$: Actions,
    private router: Router,
    private userService: UserService,
    @Optional()
    @Inject(SEARCH_DEBOUNCE)
    private debounce: number = 300,
    /**
       * You inject an optional Scheduler that will be undefined
       * in normal application usage, but its injected here so that you can mock out
       * during testing using the RxJS TestScheduler for simulating passages of time.
       */
    @Optional()
    @Inject(SEARCH_SCHEDULER)
    private scheduler: Scheduler
  ) { super(); }

  private navigateToDefaultPage(): void {
    // this.fuseNavigationService.setCurrentNavigation(new FuseNavigationModel());
    this.router.navigate(['']);
  }
}
