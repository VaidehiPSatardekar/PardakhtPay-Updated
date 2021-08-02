import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as bankLogin from '../actions/bankLogin';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { BankLogin, BankAccount } from "../../models/bank-login";
import { HttpErrorResponse } from "@angular/common/http";
import { BankLoginService } from "../services/bankLogin/bank-login.service";
import { Bank } from "../../models/bank";
import { BlockedCardDetail } from "../../models/blocked-card-detail";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class BankLoginEffects extends EffectBase {
    @Effect()
    search$: Observable<Action> = this.actions$.ofType<bankLogin.Search>(bankLogin.SEARCH).pipe(
        map(action => action.payload),
        switchMap(payload => {
            const nextSearch$ = this.actions$.ofType(bankLogin.SEARCH).pipe(skip(1));

            return this.bankLoginService.search(payload).pipe(
                takeUntil(nextSearch$),
                map((items: BankLogin[]) => new bankLogin.SearchComplete(items)),
                catchError(err => of(new bankLogin.SearchError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    searchAccounts$: Observable<Action> = this.actions$.ofType<bankLogin.SearchAccounts>(bankLogin.SEARCH_ACCOUNTS).pipe(
            map(action => action.payload),
            switchMap(payload => {
            const nextSearch$ = this.actions$.ofType(bankLogin.SEARCH_ACCOUNTS).pipe(skip(1));

            return this.bankLoginService.searchAccounts(payload).pipe(
                takeUntil(nextSearch$),
                map((items: BankAccount[]) => new bankLogin.SearchAccountsComplete(items)),
                catchError(err => of(new bankLogin.SearchAccountsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    searchBanks$: Observable<Action> = this.actions$.ofType<bankLogin.SearchBanks>(bankLogin.SEARCH_BANKS).pipe(
            switchMap(() => {
                const nextSearch$ = this.actions$.ofType(bankLogin.SEARCH_BANKS).pipe(skip(1));

                return this.bankLoginService.searchBanks().pipe(
                    takeUntil(nextSearch$),
                    map((items: Bank[]) => new bankLogin.SearchBanksComplete(items)),
                    catchError(err => of(new bankLogin.SearchBanksError(this.sanitiseError(err))))
                );
        })
    );

    @Effect()
    create$: Observable<Action> = this.actions$.ofType<bankLogin.Create>(bankLogin.CREATE).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.bankLoginService.create(payload).pipe(
                mergeMap((response: BankLogin) => [
                    new bankLogin.CreateComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new bankLogin.CreateError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<bankLogin.GetDetails>(bankLogin.GET_DETAILS).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.bankLoginService.get(payload).pipe(
                mergeMap((response: BankLogin) => [
                    new bankLogin.GetDetailsComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new bankLogin.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    update$: Observable<Action> = this.actions$.ofType<bankLogin.Edit>(bankLogin.EDIT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0) || action.payload === undefined) {
                return empty();
            }

                return this.bankLoginService.update(action.id, action.payload).pipe(
                    mergeMap((response: BankLogin) => [
                    new bankLogin.EditComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new bankLogin.EditError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    searchAccountsByLoginGuid$: Observable<Action> = this.actions$.ofType<bankLogin.SearchAccountsByLoginGuid>(bankLogin.SEARCH_ACCOUNTS_BY_LOGIN_GUID).pipe(
        switchMap(action => {
            const nextSearch$ = this.actions$.ofType(bankLogin.SEARCH_ACCOUNTS_BY_LOGIN_GUID).pipe(skip(1));

                return this.bankLoginService.searchUnusedAccountsByLoginGuid(action.payload).pipe(
                takeUntil(nextSearch$),
                    map((items: BankAccount[]) => new bankLogin.SearchAccountsBYLoginGuidCompleted(items)),
                    catchError(err => of(new bankLogin.SearchAccountsByLoginGuidError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    updateLoginInformation$: Observable<Action> = this.actions$.ofType<bankLogin.UpdateLoginInformation>(bankLogin.UPDATE_LOGIN_INFORMATION).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0) || action.payload === undefined) {
                return empty();
            }

                return this.bankLoginService.updateLoginInformation(action.id, action.payload).pipe(
                    mergeMap(() => [
                        new bankLogin.UpdateLoginInformationComplete()
                    ]),
                    catchError((err: HttpErrorResponse) => of(new bankLogin.UpdateLoginInformationError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    createLoginFromLoginRequest$: Observable<Action> = this.actions$.ofType<bankLogin.CreateLoginFromLoginRequest>(bankLogin.CREATE_LOGIN_FROM_LOGIN_REQUEST).pipe(
        switchMap(action => {
            if (action === undefined || action.payload === undefined) {
                return empty();
            }

                return this.bankLoginService.createLoginFromLoginRequest(action.payload).pipe(
                    mergeMap(() => [
                        new bankLogin.CreateLoginFromLoginRequestComplete()
                    ]),
                    catchError((err: HttpErrorResponse) => of(new bankLogin.CreateLoginFromLoginRequestError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getOwnerLoginList$: Observable<Action> = this.actions$.ofType<bankLogin.GetOwnerLoginList>(bankLogin.GET_OWNER_LOGIN_LIST).pipe(
        debounceTime(this.debounce, this.scheduler || async),
            switchMap(() => {
                const nextSearch$ = this.actions$.ofType(bankLogin.GET_OWNER_LOGIN_LIST).pipe(skip(1));

                return this.bankLoginService.getOwnersLogins().pipe(
                takeUntil(nextSearch$),
                map((items: BankLogin[]) => new bankLogin.GetOwnerLoginListComplete(items)),
                catchError(err => of(new bankLogin.GetOwnerLoginListError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    deactivate$: Observable<Action> = this.actions$.ofType<bankLogin.DeactivateLoginInformation>(bankLogin.DEACTIVATE_OWNER_LOGIN).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

                return this.bankLoginService.deactivateLogin(action.id).pipe(
                    mergeMap((response: BankLogin) => [
                        new bankLogin.DeactivateLoginInformationComplete()
                    ]),
                    catchError((err: HttpErrorResponse) => of(new bankLogin.DeactivateLoginInformationError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    delete$: Observable<Action> = this.actions$.ofType<bankLogin.DeleteLoginInformation>(bankLogin.DELETE_OWNER_LOGIN).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

                return this.bankLoginService.deleteLogin(action.id).pipe(
                    mergeMap((response: BankLogin) => [
                        new bankLogin.DeleteLoginInformationComplete()
                    ]),
                    catchError((err: HttpErrorResponse) => of(new bankLogin.DeleteLoginInformationError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    activate$: Observable<Action> = this.actions$.ofType<bankLogin.ActivateLoginInformation>(bankLogin.ACTIVATE_OWNER_LOGIN).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

            return this.bankLoginService.activateLogin(action.id).pipe(
                mergeMap((response: BankLogin) => [
                    new bankLogin.ActivateLoginInformationComplete()
                ]),
                catchError((err: HttpErrorResponse) => of(new bankLogin.ActivateLoginInformationError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getBlockedCardDetails$: Observable<Action> = this.actions$.ofType<bankLogin.GetBlockedCardDetails>(bankLogin.GET_BLOCKED_CARD_DETAILS).pipe(
        switchMap((action) => {
            const nextSearch$ = this.actions$.ofType(bankLogin.GET_BLOCKED_CARD_DETAILS).pipe(skip(1));

            return this.bankLoginService.getBlockedCardDetails(action.payload).pipe(
                takeUntil(nextSearch$),
                map((items: BlockedCardDetail[]) => new bankLogin.GetBlockedCardDetailsComplete(items)),
                catchError(err => of(new bankLogin.GetBlockedCardDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    showPassword$: Observable<Action> = this.actions$.ofType<bankLogin.ShowPassword>(bankLogin.SHOW_PASSWORD).pipe(
        switchMap(action => {
            if (action === undefined || !(action.payload > 0)) {
                return empty();
            }

            return this.bankLoginService.showPassword(action.payload).pipe(
                mergeMap((response: any) => [
                    new bankLogin.ShowPasswordCompleted(response.password)
                ]),
                catchError((err: HttpErrorResponse) => of(new bankLogin.ShowPasswordError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    qrCodeRegister$: Observable<Action> = this.actions$.ofType<bankLogin.QrCodeRegister>(bankLogin.QR_REGISTER_LOGIN).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

            return this.bankLoginService.qrCodeRegister(action.id).pipe(
                mergeMap((response: BankLogin) => [
                    new bankLogin.QrCodeRegisterComplete()
                ]),
                catchError((err: HttpErrorResponse) => of(new bankLogin.QrCodeRegisterError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getQRRegistrationDetails$: Observable<Action> = this.actions$.ofType<bankLogin.GetQRRegistrationDetails>(bankLogin.GET_QR_REGISTRATION_DETAILS).pipe(
        map(action => action.id),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.bankLoginService.getQRRegistrationDetails(payload).pipe(
                mergeMap((response: BankLogin) => [
                    new bankLogin.GetQRRegistrationDetailsComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new bankLogin.GetQRRegistrationDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    registerQrCode$: Observable<Action> = this.actions$.ofType<bankLogin.RegisterQrCode>(bankLogin.REGISTER_QR_CODE).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.bankLoginService.completeQrCodeRegistration(payload).pipe(
                mergeMap(() => [
                    new bankLogin.RegisterQrCodeComplete()
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new bankLogin.RegisterQrCodeError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getOTP$: Observable<Action> = this.actions$.ofType<bankLogin.GetOTP>(bankLogin.GET_OTP).pipe(
        switchMap(action => {
            if (action === undefined || !(action.payload > 0)) {
                return empty();
            }

            return this.bankLoginService.getOTP(action.payload).pipe(
                mergeMap((response: any) => [
                    new bankLogin.GetOTPCompleted(response.otp)
                ]),
                catchError((err: HttpErrorResponse) => of(new bankLogin.GetOTPError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    RegisterLoginRequest$: Observable<Action> = this.actions$.ofType<bankLogin.RegisterLoginRequest>(bankLogin.REGISTER_LOGIN_REQUEST).pipe(
        switchMap(action => {
           if (action === undefined || action.payload === undefined) {
                return empty();
            }

            return this.bankLoginService.registerLoginRequest(action.payload).pipe(
                mergeMap(() => [
                    new bankLogin.RegisterLoginRequestComplete()
                ]),
                catchError((err: HttpErrorResponse) => of(new bankLogin.RegisterLoginRequestError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    switchBankConnectionProgram$: Observable<Action> = this.actions$.ofType<bankLogin.SwitchBankConnectionProgram>(bankLogin.SWITCH_BANK_CONNECTION_PROGRAM).pipe(
        switchMap(action => {
            if (action === undefined || !(action.payload > 0)) {
                return empty();
            }
            return this.bankLoginService.switchBankConnectionProgram(action.payload).pipe(                
                mergeMap((response: any) => [
                    new bankLogin.SwitchBankConnectionProgramCompleted(response.bankConnectionProgram)
                ]),
                catchError((err: HttpErrorResponse) => of(new bankLogin.SwitchBankConnectionProgramError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private bankLoginService: BankLoginService,
        @Optional()
        @Inject(SEARCH_DEBOUNCE)
        private debounce: number = 300,
        @Optional()
        @Inject(SEARCH_SCHEDULER)
        private scheduler: Scheduler
    ) { super(); }
}