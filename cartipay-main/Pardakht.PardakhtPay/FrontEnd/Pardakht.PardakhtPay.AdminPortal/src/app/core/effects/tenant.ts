import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as tenant from '../actions/tenant';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { HttpErrorResponse } from "@angular/common/http";
import { TenantService } from "../services/tenant/tenant.service";
import { Tenant, TenantCreate } from "../../models/tenant";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class TenantEffects extends EffectBase {

    @Effect()
    search$: Observable<Action> = this.actions$.ofType<tenant.Search>(tenant.SEARCH).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        //map(action => action.payload),
        switchMap(() => {
            const nextSearch$ = this.actions$.ofType(tenant.SEARCH).pipe(skip(1));
            return this.tenantService.search().pipe(takeUntil(nextSearch$), map((items: Tenant[]) => new tenant.SearchComplete(items)), catchError(err => of(new tenant.SearchError(this.sanitiseError(err)))));
        })
    );

    @Effect()
    create$: Observable<Action> = this.actions$.ofType<tenant.Create>(tenant.CREATE).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.tenantService.create(payload).pipe(
                mergeMap((response: Tenant) => [
                    new tenant.CreateComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new tenant.CreateError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<tenant.GetDetails>(tenant.GET_DETAILS).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.tenantService.get(payload).pipe(
                mergeMap((response: Tenant) => [
                    new tenant.GetDetailsComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new tenant.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private tenantService: TenantService,
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