import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action, Store } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { MerchantService } from "../services/merchant/merchant.service";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as merchant from '../actions/merchant';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { Merchant, MerchantCreate, MerchantEdit } from "../../models/merchant-model";
import { HttpErrorResponse } from "@angular/common/http";
import * as coreState from '../../core';
import 'rxjs/add/operator/withLatestFrom';

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class MerchantEffects extends EffectBase {

    @Effect()
    search$: Observable<Action> = this.actions$.ofType<merchant.Search>(merchant.SEARCH)
        .withLatestFrom(this.store)
        .pipe(
            switchMap(([action, state]) => {

                let items = state.merchant.merchants;

                if (items) {
                    return of(new merchant.SearchComplete(items));
                }
                else {
                    return this.merchantService.search(action.payload).pipe(
                        map((merchants: Merchant[]) => new merchant.SearchComplete(merchants)),
                        catchError(err => of(new merchant.SearchError(this.sanitiseError(err))))
                    );
                }
            })
        );

    @Effect()
    getAll$: Observable<Action> = this.actions$.ofType<merchant.GetAll>(merchant.GET_ALL)
        .withLatestFrom(this.store)
        .pipe(
            switchMap(([action, state]) => {
                let items = state.merchant.allMerchants;

                if (items) {
                    return of(new merchant.GetAllComplete(items));
                }
                else {

                    return this.merchantService.getMerchants().pipe(
                        map((merchants: Merchant[]) => new merchant.GetAllComplete(merchants)),
                        catchError(err => of(new merchant.GetAllError(this.sanitiseError(err))))
                    );
                }
            })
        );

    @Effect()
    create$: Observable<Action> = this.actions$.ofType<merchant.Create>(merchant.CREATE).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.merchantService.create(payload).pipe(
                mergeMap((response: MerchantCreate) => [
                    new merchant.CreateComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new merchant.CreateError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<merchant.GetDetails>(merchant.GET_DETAILS).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.merchantService.get(payload).pipe(
                mergeMap((response: MerchantEdit) => [
                    new merchant.GetDetailsComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new merchant.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    update$: Observable<Action> = this.actions$.ofType<merchant.Edit>(merchant.EDIT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0) || action.payload === undefined) {
                return empty();
            }

            return this.merchantService.update(action.id, action.payload).pipe(
                mergeMap((response: MerchantEdit) => [
                    new merchant.EditComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new merchant.EditError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    delete: Observable<Action> = this.actions$.ofType<merchant.Delete>(merchant.DELETE).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

            return this.merchantService.delete(action.id).pipe(
                mergeMap(() => [
                    new merchant.DeleteComplete()
                ]),
                catchError((err: HttpErrorResponse) => of(new merchant.DeleteError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private store: Store<coreState.State>,
        private merchantService: MerchantService,
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