import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { AutoTransferService } from "../services/autoTransfer/auto-transfer.service";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as autoTransfer from '../actions/autoTransfer';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { AutoTransfer } from "../../models/autoTransfer";
import { HttpErrorResponse } from "@angular/common/http";
import { ListSearchResponse } from "../../models/types";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class AutoTransferEffects extends EffectBase {
    @Effect()
    search$: Observable<Action> = this.actions$.ofType<autoTransfer.Search>(autoTransfer.SEARCH).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        map(action => action.payload),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(autoTransfer.SEARCH).pipe(skip(1));

            return this.autoTransferService.search(query).pipe(
                takeUntil(nextSearch$),
                map((autoTransfers: ListSearchResponse<AutoTransfer[]>) => new autoTransfer.SearchComplete(autoTransfers)),
                catchError(err => of(new autoTransfer.SearchError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private autoTransferService: AutoTransferService,
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