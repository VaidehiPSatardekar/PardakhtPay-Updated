import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { InvoicePaymentService } from "../services/invoice-payment/invoice-payment.service";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as invoicePayment from '../actions/invoicePayment';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { InvoicePayment, } from "../../models/invoice";
import { HttpErrorResponse } from "@angular/common/http";
import { ListSearchResponse } from "app/models/types";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class InvoicePaymentEffects extends EffectBase {
    

    @Effect()
    search$: Observable<Action> = this.actions$.ofType<invoicePayment.Search>(invoicePayment.SEARCH).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        map(action => action.payload),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(invoicePayment.SEARCH).pipe(skip(1));

            return this.invoicePaymentService.search(query).pipe(
                takeUntil(nextSearch$),
                map((items: ListSearchResponse<InvoicePayment[]>) => new invoicePayment.SearchComplete(items)),
                catchError(err => of(new invoicePayment.SearchError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    create$: Observable<Action> = this.actions$.ofType<invoicePayment.Create>(invoicePayment.CREATE).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.invoicePaymentService.create(payload).pipe(
                mergeMap((response: InvoicePayment) => [
                    new invoicePayment.CreateComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new invoicePayment.CreateError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<invoicePayment.GetDetails>(invoicePayment.GET_DETAILS).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.invoicePaymentService.get(payload).pipe(
                mergeMap((response: InvoicePayment) => [
                    new invoicePayment.GetDetailsComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new invoicePayment.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    update$: Observable<Action> = this.actions$.ofType<invoicePayment.Edit>(invoicePayment.EDIT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0) || action.payload === undefined) {
                return empty();
            }

            return this.invoicePaymentService.update(action.id, action.payload).pipe(
                mergeMap((response: InvoicePayment) => [
                    new invoicePayment.EditComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new invoicePayment.EditError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private invoicePaymentService: InvoicePaymentService,
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