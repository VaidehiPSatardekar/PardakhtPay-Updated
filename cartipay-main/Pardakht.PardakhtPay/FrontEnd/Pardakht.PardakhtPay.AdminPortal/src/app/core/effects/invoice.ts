import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as invoice from '../actions/invoice';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { HttpErrorResponse } from "@angular/common/http";
import { InvoiceService } from "../services/invoice/invoice.service";
import { Invoice } from "../../models/invoice";
import { ListSearchResponse } from "../../models/types";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class InvoiceEffects extends EffectBase {

    @Effect()
    search$: Observable<Action> = this.actions$.ofType<invoice.Search>(invoice.SEARCH).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        map(action => action.payload),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(invoice.SEARCH).pipe(skip(1));

            return this.invoiceService.search(query).pipe(
                takeUntil(nextSearch$),
                map((items: ListSearchResponse<Invoice[]>) => new invoice.SearchComplete(items)),
                catchError(err => of(new invoice.SearchError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<invoice.GetDetails>(invoice.GET_DETAILS).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.invoiceService.get(payload).pipe(
                mergeMap((response: Invoice) => [
                    new invoice.GetDetailsComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new invoice.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private invoiceService: InvoiceService,
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