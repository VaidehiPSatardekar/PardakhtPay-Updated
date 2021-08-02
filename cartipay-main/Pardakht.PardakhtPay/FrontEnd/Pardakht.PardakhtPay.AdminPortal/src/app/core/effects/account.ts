import { HttpErrorResponse } from '@angular/common/http';
import { Inject, Injectable, InjectionToken, Optional } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';

import { Actions, Effect } from '@ngrx/effects';
import { Action } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { empty } from 'rxjs/observable/empty';
import { of } from 'rxjs/observable/of';
import { catchError } from 'rxjs/operators/catchError';
import { map } from 'rxjs/operators/map';
import { mergeMap } from 'rxjs/operators/mergeMap';
import { switchMap } from 'rxjs/operators/switchMap';
import { tap } from 'rxjs/operators/tap';

import * as account from '../../core/actions/account';
import { LoginForm, LoginResponse, Owner } from '../../models/account.model';
import { User } from '../../models/user-management.model';
import { AccountService } from '../services/account.service';
import { UserService } from '../services/user.service';
import { EffectBase } from './effect-base';
//import { FuseNavigationModel } from '../../models/navigation.model';
//import { FuseNavigationService } from '@fuse/components/navigation/navigation.service';

// import { ISuspension } from '../../models/suspension.model';

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
export class AccountEffects extends EffectBase {
    @Effect()
    login$: Observable<Action> = this.actions$.ofType<account.Login>(account.LOGIN).pipe(
        map((action: account.Login) => action.payload),
        switchMap((payload: LoginForm) => {
            if (payload === undefined) {
                return empty();
            }

            return this.accountService.logIn(payload).pipe(
                tap(() => this.navigateToDefaultPage()),
                mergeMap((response: LoginResponse) => [
                    new account.LoginComplete(response, this.accountService.tenantGuid(), this.accountService.isUserProviderAdmin(), this.accountService.isUserProviderUser(), this.accountService.isUserTenantAdmin(), this.accountService.isUserStandardUser(), this.accountService.getAccountGuid(), this.accountService.getUsername(), this.accountService.getParentAccountId())
                ]),
                catchError((err: HttpErrorResponse) => of(new account.LoginError(this.sanitiseError(err))))
            );
        })
    );


    @Effect()
    changePassword$: Observable<Action> = this.actions$.ofType<account.ChangePassword>(account.CHANGE_PASSWORD).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.accountService.changePassword(payload).pipe(
                mergeMap((response: boolean) => [
                    new account.ChangePasswordComplete(response)
                    // new notification.AddOne('Forgot password request '  + (response ? 'sent' : 'failed'))
                ]),
                catchError((err: any) => {
                    return of(new account.ChangePasswordError(this.sanitiseError(err)));
                })
            );
        })
    );


    @Effect()
    logout$: Observable<Action> = this.actions$.ofType<account.Logout>(account.LOGOUT).pipe(
        tap(() => {
            this.accountService.logOut();
            this.router.navigate(['/login']);
        }),
        switchMap(() => {
            return of(new account.LogoutComplete());
        })
    );

    @Effect()
    logoutExpired$: Observable<Action> = this.actions$.ofType<account.LoginExpired>(account.LOGIN_EXPIRED).pipe(
        tap(() => {
            this.accountService.logOut();
            this.router.navigate(['/login']);
        }),
        switchMap(() => {
            return of(new account.LogoutComplete());
        })
    );

    @Effect()
    forgotPassword$: Observable<Action> = this.actions$.ofType<account.ForgotPassword>(account.FORGOT_PASSWORD).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.accountService.forgotPassword(payload).pipe(
                mergeMap((response: boolean) => [
                    new account.ForgotPasswordComplete()
                    // new notification.AddOne('Forgot password request '  + (response ? 'sent' : 'failed'))
                ]),
                catchError((err: any) => {
                    return of(new account.ForgotPasswordError(this.sanitiseError(err)));
                })
            );
        })
    );

    @Effect()
    sendPasswordResetToken$: Observable<Action> = this.actions$.ofType<account.SendPasswordResetToken>(account.SEND_PASSWORD_RESET_TOKEN).pipe(
        switchMap((action: account.SendPasswordResetToken) => {
            if (action === undefined || action.accountId === undefined) {
                return empty();
            }

            return this.accountService.sendPasswordResetToken(action.accountId, action.tenantId).pipe(
                mergeMap((response: boolean) => [
                    new account.SendPasswordResetTokenComplete(),
                ]),
                catchError((err: any) => of(new account.SendPasswordResetTokenError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    resetPassword$: Observable<Action> = this.actions$.ofType<account.ResetPassword>(account.RESET_PASSWORD).pipe(
        switchMap((action: account.ResetPassword) => {
            if (action === undefined || action.payload === undefined) {
                return empty();
            }

            return this.accountService.resetPassword(action.payload).pipe(
                tap(() => this.router.navigate(['/login'])),
                map(() => new account.ResetPasswordComplete()),
                catchError((err: any) => of(new account.ResetPasswordError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    initialize$: Observable<Action> = this.actions$.ofType<account.Initialize>(account.INITIALIZE).pipe(
        switchMap(() => {
            if (this.accountService.isAuthenticated()) {
                return of(new account.InitializeComplete(undefined, this.accountService.tenantGuid(), this.accountService.isUserProviderAdmin(), this.accountService.isUserProviderUser(), this.accountService.isUserTenantAdmin(), this.accountService.isUserStandardUser(), this.accountService.getAccountGuid(), this.accountService.getUsername(), this.accountService.getParentAccountId()));
            } else {
                return of(new account.InitializeComplete(undefined, undefined, false, false, false, false, undefined, undefined, undefined));
            }
        })
    );

    @Effect()
    getOwners$: Observable<Action> = this.actions$.ofType<account.GetOwners>(account.GET_OWNERS).pipe(
        switchMap(() => {
            return this.accountService.getOwners().pipe(
                map((data: Owner[]) => new account.GetOwnersComplete(this.accountService.filterOwners(data))),
                catchError(err => of(new account.GetOwnersError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    createOwner$: Observable<Action> = this.actions$.ofType<account.CreateOwner>(account.CREATE_OWNER).pipe(
        switchMap((action: account.CreateOwner) => {
            return this.accountService.createUser(action.payload).pipe(
                map(() => new account.CreateOwnerComplete()),
                catchError(err => of(new account.CreateOwnerError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getOwnerDetail$: Observable<Action> = this.actions$.ofType<account.GetOwnerDetail>(account.GET_OWNER_DETAIL).pipe(
        switchMap((action: account.GetOwnerDetail) => {
            return this.accountService.getOwnerDetail(action.payload).pipe(
                map((data: Owner) => new account.GetOwnerDetailComplete(data)),
                catchError(err => of(new account.GetOwnerDetailError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    updateOwner$: Observable<Action> = this.actions$.ofType<account.EditOwner>(account.EDIT_OWNER).pipe(
        switchMap((action: account.EditOwner) => {
            return this.accountService.updateOwner(action.id, action.payload).pipe(
                map(() => new account.EditOwnerComplete()),
                catchError(err => of(new account.EditOwnerError(this.sanitiseError(err))))
            );
        })
    );

    // tslint:disable-next-line:member-ordering
    constructor(
        private actions$: Actions,
        private accountService: AccountService,
        private userService: UserService,
        //  private fuseNavigationService: FuseNavigationService,
        private router: Router,
        private route: ActivatedRoute
    ) { super(); }

    private navigateToDefaultPage(): void {
        // this.fuseNavigationService.setCurrentNavigation(new FuseNavigationModel());
        this.router.navigate(['/dashboard']);
    }
}
