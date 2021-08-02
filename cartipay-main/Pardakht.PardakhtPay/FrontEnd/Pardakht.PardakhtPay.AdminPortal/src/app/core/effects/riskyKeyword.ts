import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as riskyKeyword from '../actions/riskyKeyword';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { HttpErrorResponse } from "@angular/common/http";
import { RiskyKeywordService } from "../services/riskyKeywords/risky-keyword.service";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class RiskyKeywordEffects extends EffectBase {

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<riskyKeyword.GetDetails>(riskyKeyword.GET_DETAILS).pipe(
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

                return this.riskyKeywordService.get().pipe(
                mergeMap((response: string[]) => [
                    new riskyKeyword.GetDetailsComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new riskyKeyword.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    update$: Observable<Action> = this.actions$.ofType<riskyKeyword.Edit>(riskyKeyword.EDIT).pipe(
        switchMap(action => {
            if (action === undefined ||  action.payload === undefined) {
                return empty();
            }

                return this.riskyKeywordService.update(action.payload).pipe(
                    mergeMap((response: string[]) => [
                    new riskyKeyword.EditComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new riskyKeyword.EditError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private riskyKeywordService: RiskyKeywordService,
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