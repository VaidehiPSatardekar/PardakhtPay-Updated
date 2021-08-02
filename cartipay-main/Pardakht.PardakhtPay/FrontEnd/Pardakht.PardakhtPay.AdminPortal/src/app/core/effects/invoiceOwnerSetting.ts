import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { InvoiceOwnerSettingService } from "../services/invoice-owner-setting/invoice-owner-setting.service";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as invoiceOwnerSetting from '../actions/invoiceOwnerSetting';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { InvoiceOwnerSetting, } from "../../models/invoice";
import { HttpErrorResponse } from "@angular/common/http";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class InvoiceOwnerSettingEffects extends EffectBase {
    

    @Effect()
    getAll$: Observable<Action> = this.actions$.ofType<invoiceOwnerSetting.GetAll>(invoiceOwnerSetting.GET_ALL).pipe(
        switchMap(query => {
            return this.invoiceOwnerSettingService.getAll().pipe(
                map((invoiceOwnerSettings: InvoiceOwnerSetting[]) => new invoiceOwnerSetting.GetAllComplete(invoiceOwnerSettings)),
                catchError(err => of(new invoiceOwnerSetting.GetAllError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    create$: Observable<Action> = this.actions$.ofType<invoiceOwnerSetting.Create>(invoiceOwnerSetting.CREATE).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.invoiceOwnerSettingService.create(payload).pipe(
                mergeMap((response: InvoiceOwnerSetting) => [
                    new invoiceOwnerSetting.CreateComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new invoiceOwnerSetting.CreateError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<invoiceOwnerSetting.GetDetails>(invoiceOwnerSetting.GET_DETAILS).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.invoiceOwnerSettingService.get(payload).pipe(
                mergeMap((response: InvoiceOwnerSetting) => [
                    new invoiceOwnerSetting.GetDetailsComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new invoiceOwnerSetting.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    update$: Observable<Action> = this.actions$.ofType<invoiceOwnerSetting.Edit>(invoiceOwnerSetting.EDIT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0) || action.payload === undefined) {
                return empty();
            }

            return this.invoiceOwnerSettingService.update(action.id, action.payload).pipe(
                mergeMap((response: InvoiceOwnerSetting) => [
                    new invoiceOwnerSetting.EditComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new invoiceOwnerSetting.EditError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private invoiceOwnerSettingService: InvoiceOwnerSettingService,
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