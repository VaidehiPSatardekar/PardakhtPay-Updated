import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { BankStatementService } from "../services/bankstatement/bank-statement.service";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as bankStatement from '../actions/bankStatement';
import { debounceTime, map, switchMap, skip, takeUntil, catchError } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { BankStatementItem } from "../../models/bank-statement-item";
import { ListSearchResponse } from "../../models/types";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class BankStatementItemEffects extends EffectBase {
    @Effect()
    search$: Observable<Action> = this.actions$.ofType<bankStatement.Search>(bankStatement.SEARCH).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        map(action => action.payload),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(bankStatement.SEARCH).pipe(skip(1));

            return this.bankStatementService.search(query).pipe(
                takeUntil(nextSearch$),
                map((bankStatements: ListSearchResponse<BankStatementItem[]>) => new bankStatement.SearchComplete(bankStatements)),
                catchError(err => of(new bankStatement.SearchError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private bankStatementService: BankStatementService,
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