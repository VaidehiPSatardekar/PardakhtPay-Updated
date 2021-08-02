import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { TransferAccountService } from "../services/transferAccount/transfer-account.service";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as transferAccount from '../actions/transferAccount';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { TransferAccount } from "../../models/transfer-account";
import { HttpErrorResponse } from "@angular/common/http";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class TransferAccountEffects extends EffectBase {

    @Effect()
    search$: Observable<Action> = this.actions$.ofType<transferAccount.Search>(transferAccount.SEARCH).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        map(action => action.payload),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(transferAccount.SEARCH).pipe(skip(1));

            return this.transferAccountService.search(query).pipe(
                takeUntil(nextSearch$),
                map((tenants: TransferAccount[]) => new transferAccount.SearchComplete(tenants)),
                catchError(err => of(new transferAccount.SearchError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getAll$: Observable<Action> = this.actions$.ofType<transferAccount.GetAll>(transferAccount.GET_ALL).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(transferAccount.GET_ALL).pipe(skip(1));

            return this.transferAccountService.getTransferAccounts().pipe(
                takeUntil(nextSearch$),
                map((transferAccounts: TransferAccount[]) => new transferAccount.GetAllComplete(transferAccounts)),
                catchError(err => of(new transferAccount.GetAllError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    create$: Observable<Action> = this.actions$.ofType<transferAccount.Create>(transferAccount.CREATE).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.transferAccountService.create(payload).pipe(
                mergeMap((response: TransferAccount) => [
                    new transferAccount.CreateComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new transferAccount.CreateError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<transferAccount.GetDetails>(transferAccount.GET_DETAILS).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.transferAccountService.get(payload).pipe(
                mergeMap((response: TransferAccount) => [
                    new transferAccount.GetDetailsComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new transferAccount.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    update$: Observable<Action> = this.actions$.ofType<transferAccount.Edit>(transferAccount.EDIT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0) || action.payload === undefined) {
                return empty();
            }

            return this.transferAccountService.update(action.id, action.payload).pipe(
                mergeMap((response: TransferAccount) => [
                    new transferAccount.EditComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new transferAccount.EditError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    delete: Observable<Action> = this.actions$.ofType<transferAccount.Delete>(transferAccount.DELETE).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

                return this.transferAccountService.delete(action.id).pipe(
                mergeMap(() => [
                    new transferAccount.DeleteComplete()
                ]),
                catchError((err: HttpErrorResponse) => of(new transferAccount.DeleteError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private transferAccountService: TransferAccountService,
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