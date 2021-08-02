import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as accounting from '../actions/accounting';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { ListSearchResponse } from "../../models/types";
import { DailyAccountingDTO } from "../../models/accounting";
import { AccountingService } from "../services/accounting/accounting.service";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class AccountingEffects extends EffectBase {
    @Effect()
    search$: Observable<Action> = this.actions$.ofType<accounting.Search>(accounting.SEARCH).pipe(
        //debounceTime(this.debounce, this.scheduler || async),
        map(action => action.payload),
        switchMap(query => {
            //const nextSearch$ = this.actions$.ofType(accounting.SEARCH).pipe(skip(1));

            return this.accountingService.search(query).pipe(
                //takeUntil(nextSearch$),
                map((data: DailyAccountingDTO[]) => new accounting.SearchComplete(data)),
                catchError(err => of(new accounting.SearchError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private accountingService: AccountingService,
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