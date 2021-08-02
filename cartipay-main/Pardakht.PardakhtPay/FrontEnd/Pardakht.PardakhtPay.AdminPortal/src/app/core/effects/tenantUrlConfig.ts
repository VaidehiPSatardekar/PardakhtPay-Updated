import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { TenantUrlConfigService } from "../services/tenant-url-config/tenant-url-config.service";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as tenantUrlConfig from '../actions/tenantUrlConfig';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { TenantUrlConfig, } from "../../models/tenant-url-config";
import { HttpErrorResponse } from "@angular/common/http";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class TenantUrlConfigEffects extends EffectBase {
    

    @Effect()
    getAll$: Observable<Action> = this.actions$.ofType<tenantUrlConfig.GetAll>(tenantUrlConfig.GET_ALL).pipe(
        switchMap(query => {
            return this.tenantUrlConfigService.getAll().pipe(
                map((tenantUrlConfigs: TenantUrlConfig[]) => new tenantUrlConfig.GetAllComplete(tenantUrlConfigs)),
                catchError(err => of(new tenantUrlConfig.GetAllError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    create$: Observable<Action> = this.actions$.ofType<tenantUrlConfig.Create>(tenantUrlConfig.CREATE).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.tenantUrlConfigService.create(payload).pipe(
                mergeMap((response: TenantUrlConfig) => [
                    new tenantUrlConfig.CreateComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new tenantUrlConfig.CreateError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<tenantUrlConfig.GetDetails>(tenantUrlConfig.GET_DETAILS).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.tenantUrlConfigService.get(payload).pipe(
                mergeMap((response: TenantUrlConfig) => [
                    new tenantUrlConfig.GetDetailsComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new tenantUrlConfig.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    update$: Observable<Action> = this.actions$.ofType<tenantUrlConfig.Edit>(tenantUrlConfig.EDIT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0) || action.payload === undefined) {
                return empty();
            }

            return this.tenantUrlConfigService.update(action.id, action.payload).pipe(
                mergeMap((response: TenantUrlConfig) => [
                    new tenantUrlConfig.EditComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new tenantUrlConfig.EditError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private tenantUrlConfigService: TenantUrlConfigService,
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