import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { TransactionService } from "../services/transaction/transaction.service";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as merchantBankAccount from '../actions/merchantBankAccount';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { MerchantBankAccount } from "../../models/merchant-model";
import { MerchantBankAccountService } from "../services/merchantBankAccount/merchant-bank-account.service";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class MerchantBankAccountEffects extends EffectBase {
    @Effect()
    search$: Observable<Action> = this.actions$.ofType<merchantBankAccount.GetAccounts>(merchantBankAccount.GET_ACCOUNTS).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        map(action => action.payload),
        switchMap(merchantId => {
            const nextSearch$ = this.actions$.ofType(merchantBankAccount.GET_ACCOUNTS).pipe(skip(1));

            return this.merchantBankAccountService.search(merchantId).pipe(
                takeUntil(nextSearch$),
                map((items: MerchantBankAccount[]) => new merchantBankAccount.GetAccountsComplete(items)),
                catchError(err => of(new merchantBankAccount.GetAccountsError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private merchantBankAccountService: MerchantBankAccountService,
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