import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { Scheduler, Observable, of, empty } from "rxjs";
//import * as bankLogin from '../actions/bankLogin';
import * as loginDevice from '../actions/loginDeviceStatus';

import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { BankLogin, BankAccount } from "../../models/bank-login";
import { HttpErrorResponse } from "@angular/common/http";
import { BankLoginService } from "../services/bankLogin/bank-login.service";
import { LoginDeviceStatusService } from "../services/loginDeviceStatus/login-device-status.service";

import { Bank } from "../../models/bank";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class LoginDeviceStatusEffects extends EffectBase {
    @Effect()
  
  
    @Effect()
    getOwnerLoginList$: Observable<Action> = this.actions$.ofType<loginDevice.GetOwnerLoginList>(loginDevice.GET_OWNER_LOGIN_LIST).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        switchMap(() => {
            const nextSearch$ = this.actions$.ofType(loginDevice.GET_OWNER_LOGIN_LIST).pipe(skip(1));

            return this.loginDeviceStatus.getOwnersLogins().pipe(
                takeUntil(nextSearch$),
                map((items: BankLogin[]) => new loginDevice.GetOwnerLoginListComplete(items)),
                catchError(err => of(new loginDevice.GetOwnerLoginListError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    showLoginDeviceStatus$: Observable<Action> = this.actions$.ofType<loginDevice.ShowLoginDeviceStatus>(loginDevice.SHOW_LOGINDEVICESTATUS).pipe(
        switchMap(action => {
            if (action.payload == undefined || (action.payload == "") || (action.payload == "Input string was not in a correct format.")) {
                return empty();
            }
            return this.loginDeviceStatus.showLoginDeviceStatus(action.payload).pipe(                
                mergeMap((response: any) => [
                    new loginDevice.ShowLoginDeviceStatusCompleted(response.loginDeviceStatusDesc)
                ]),
                catchError((err: HttpErrorResponse) => of(new loginDevice.ShowLoginDeviceStatusError(this.sanitiseError(err))))
            );
            
        })
    );


    @Effect()
    showLoginListDeviceStatus$: Observable<Action> = this.actions$.ofType<loginDevice.ShowLoginListDeviceStatus>(loginDevice.SHOW_LOGINLIST_DEVICESTATUS).pipe(
        switchMap(action => {
            if (action == undefined) {
                return empty();
            }
            return this.loginDeviceStatus.showLoginListDeviceStatus().pipe(
                mergeMap((response: any) => [
                    new loginDevice.ShowLoginListDeviceStatusCompleted(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new loginDevice.ShowLoginListDeviceStatusError(this.sanitiseError(err))))
            );

        })
    );

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<loginDevice.GetDetails>(loginDevice.GET_DETAILS).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.bankLoginService.get(payload).pipe(
                mergeMap((response: BankLogin) => [
                    new loginDevice.GetDetailsComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new loginDevice.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

   

    constructor(
        private actions$: Actions,
        private bankLoginService: BankLoginService,
        private loginDeviceStatus: LoginDeviceStatusService,
        @Optional()
        @Inject(SEARCH_DEBOUNCE)
        private debounce: number = 300,
        @Optional()
        @Inject(SEARCH_SCHEDULER)
        private scheduler: Scheduler
    ) { super(); }
}