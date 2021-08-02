import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { MobileTransferCardAccountService } from "../services/mobile-transfer-card-account/mobile-transfer-card-account.service";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as mobileTransferCardAccount from '../actions/mobileTransferCardAccount';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { MobileTransferCardAccount } from "../../models/mobile-transfer";
import { HttpErrorResponse } from "@angular/common/http";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class MobileTransferCardAccountEffects extends EffectBase {

    @Effect()
    getAll$: Observable<Action> = this.actions$.ofType<mobileTransferCardAccount.GetAll>(mobileTransferCardAccount.GET_ALL).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(mobileTransferCardAccount.GET_ALL).pipe(skip(1));

            return this.mobileTransferCardAccountService.getMobileTransferCardAccounts().pipe(
                takeUntil(nextSearch$),
                map((mobileTransferCardAccounts: MobileTransferCardAccount[]) => new mobileTransferCardAccount.GetAllComplete(mobileTransferCardAccounts)),
                catchError(err => of(new mobileTransferCardAccount.GetAllError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    create$: Observable<Action> = this.actions$.ofType<mobileTransferCardAccount.Create>(mobileTransferCardAccount.CREATE).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.mobileTransferCardAccountService.create(payload).pipe(
                mergeMap((response: MobileTransferCardAccount) => [
                    new mobileTransferCardAccount.CreateComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new mobileTransferCardAccount.CreateError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<mobileTransferCardAccount.GetDetails>(mobileTransferCardAccount.GET_DETAILS).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.mobileTransferCardAccountService.get(payload).pipe(
                mergeMap((response: MobileTransferCardAccount) => [
                    new mobileTransferCardAccount.GetDetailsComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new mobileTransferCardAccount.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    update$: Observable<Action> = this.actions$.ofType<mobileTransferCardAccount.Edit>(mobileTransferCardAccount.EDIT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0) || action.payload === undefined) {
                return empty();
            }

            return this.mobileTransferCardAccountService.update(action.id, action.payload).pipe(
                mergeMap((response: MobileTransferCardAccount) => [
                    new mobileTransferCardAccount.EditComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new mobileTransferCardAccount.EditError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private mobileTransferCardAccountService: MobileTransferCardAccountService,
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
}