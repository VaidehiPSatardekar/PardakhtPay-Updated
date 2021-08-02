import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { TransactionService } from "../services/transaction/transaction.service";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as transaction from '../actions/transaction';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { TransactionSearch } from "../../models/transaction";
import { HttpErrorResponse } from "@angular/common/http";
import { ListSearchResponse } from "../../models/types";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class TransactionEffects extends EffectBase {
    @Effect()
    search$: Observable<Action> = this.actions$.ofType<transaction.Search>(transaction.SEARCH).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        map(action => action.payload),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(transaction.SEARCH).pipe(skip(1));
            return this.transactionService.search(query).pipe(
                takeUntil(nextSearch$),
                map((transactions: ListSearchResponse<TransactionSearch[]>) => new transaction.SearchComplete(transactions)),
                catchError(err => of(new transaction.SearchError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    setAsCompleted$: Observable<Action> = this.actions$.ofType<transaction.SetAsCompleted>(transaction.SET_AS_COMPLETED).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

                return this.transactionService.setAsCompleted(action.id).pipe(
                mergeMap(() => [
                        new transaction.SetAsCompletedComplete()
                ]),
                    catchError((err: HttpErrorResponse) => of(new transaction.SetAsCompletedError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    setAsExpired$: Observable<Action> = this.actions$.ofType<transaction.SetAsExpired>(transaction.SET_AS_EXPIRED).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

                return this.transactionService.setAsExpired(action.id).pipe(
                mergeMap(() => [
                    new transaction.SetAsExpiredComplete()
                ]),
                catchError((err: HttpErrorResponse) => of(new transaction.SetAsExpiredError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    transactionCallbackToMerchant$: Observable<Action> = this.actions$.ofType<transaction.TransactionCallbackToMerchant>(transaction.TRANSACTIONCALLBACKTOMERCHANT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

            return this.transactionService.transactionCallbackToMerchant(action.id).pipe(
                mergeMap((response: any) => [
                    new transaction.TransactionCallbackToMerchantComplete(response.callbackToMerchant)
                ]),
                catchError((err: HttpErrorResponse) => of(new transaction.TransactionCallbackToMerchantError(this.sanitiseError(err))))
            );
        })
    );


    constructor(
        private actions$: Actions,
        private transactionService: TransactionService,
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