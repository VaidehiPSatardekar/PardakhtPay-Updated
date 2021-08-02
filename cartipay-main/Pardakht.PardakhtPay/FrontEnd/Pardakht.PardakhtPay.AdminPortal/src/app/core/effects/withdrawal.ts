import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as withdrawal from '../actions/withdrawal';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { HttpErrorResponse } from "@angular/common/http";
import { WithdrawalService } from "../services/withdrawal/withdrawal.service";
import { Withdrawal, WithdrawalSearch, WithdrawalCreate, WithdrawalReceipt, WithdrawalTransferHistory } from "../../models/withdrawal";
import { ListSearchResponse } from "../../models/types";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class WithdrawalEffects extends EffectBase {

    @Effect()
    search$: Observable<Action> = this.actions$.ofType<withdrawal.Search>(withdrawal.SEARCH).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        map(action => action.payload),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(withdrawal.SEARCH).pipe(skip(1));

            return this.withdrawalService.search(query).pipe(
                takeUntil(nextSearch$),
                map((items: ListSearchResponse<WithdrawalSearch[]>) => new withdrawal.SearchComplete(items)),
                catchError(err => of(new withdrawal.SearchError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    create$: Observable<Action> = this.actions$.ofType<withdrawal.Create>(withdrawal.CREATE).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.withdrawalService.create(payload).pipe(
                mergeMap((response: Withdrawal) => [
                    new withdrawal.CreateComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new withdrawal.CreateError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<withdrawal.GetDetails>(withdrawal.GET_DETAILS).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.withdrawalService.get(payload).pipe(
                mergeMap((response: Withdrawal) => [
                    new withdrawal.GetDetailsComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new withdrawal.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    update$: Observable<Action> = this.actions$.ofType<withdrawal.Edit>(withdrawal.EDIT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0) || action.payload === undefined) {
                return empty();
            }

            return this.withdrawalService.update(action.id, action.payload).pipe(
                mergeMap((response: Withdrawal) => [
                    new withdrawal.EditComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new withdrawal.EditError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    cancel$: Observable<Action> = this.actions$.ofType<withdrawal.Cancel>(withdrawal.CANCEL).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

            return this.withdrawalService.cancel(action.id).pipe(
                mergeMap((response: Withdrawal) => [
                    new withdrawal.CancelComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new withdrawal.CancelError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    retry$: Observable<Action> = this.actions$.ofType<withdrawal.Retry>(withdrawal.RETRY).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

            return this.withdrawalService.retry(action.id).pipe(
                mergeMap((response: Withdrawal) => [
                    new withdrawal.RetryComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new withdrawal.RetryError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    setAsCompleted$: Observable<Action> = this.actions$.ofType<withdrawal.SetAsCompleted>(withdrawal.SET_AS_COMPLETED).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

            return this.withdrawalService.setAsCompleted(action.id).pipe(
                mergeMap((response: Withdrawal) => [
                    new withdrawal.SetAsCompletedComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new withdrawal.SetAsCompletedError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    changeProcessType$: Observable<Action> = this.actions$.ofType<withdrawal.ChangeProcessType>(withdrawal.CHANGE_PROCESS_TYPE).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

            return this.withdrawalService.changeProcessType(action.id, action.processType).pipe(
                mergeMap((response: Withdrawal) => [
                    new withdrawal.ChangeProcessTypeComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new withdrawal.ChangeProcessTypeError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    changeAllProcessType$: Observable<Action> = this.actions$.ofType<withdrawal.ChangeAllProcessType>(withdrawal.CHANGE_ALL_PROCESS_TYPE).pipe(
        switchMap(action => {
            if (action === undefined) {
                return empty();
            }

            return this.withdrawalService.changeAllProcessType(action.args, action.processType).pipe(
                mergeMap(() => [
                    new withdrawal.ChangeAllProcessTypeComplete()
                ]),
                catchError((err: HttpErrorResponse) => of(new withdrawal.ChangeAllProcessTypeError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getHistories$: Observable<Action> = this.actions$.ofType<withdrawal.GetHistory>(withdrawal.GET_HISTORY).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.withdrawalService.getHistory(payload).pipe(
                mergeMap((response: WithdrawalTransferHistory[]) => [
                    new withdrawal.GetHistoryComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new withdrawal.GetHistoryError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getReceipt: Observable<Action> = this.actions$.ofType<withdrawal.GetReceipt>(withdrawal.GET_RECEIPT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

            return this.withdrawalService.getReceipt(action.id).pipe(
                mergeMap((response: WithdrawalReceipt) => [
                    new withdrawal.GetReceiptComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new withdrawal.GetReceiptError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    callbackToMerchant$: Observable<Action> = this.actions$.ofType<withdrawal.CallbackToMerchant>(withdrawal.CALLBACKTOMERCHANT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

            return this.withdrawalService.callbackToMerchant(action.id).pipe(
                mergeMap((response: any) => [
                    new withdrawal.CallbackToMerchantComplete(response.callbackToMerchant)                    
                ]),
                catchError((err: HttpErrorResponse) => of(new withdrawal.CallbackToMerchantError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private withdrawalService: WithdrawalService,
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