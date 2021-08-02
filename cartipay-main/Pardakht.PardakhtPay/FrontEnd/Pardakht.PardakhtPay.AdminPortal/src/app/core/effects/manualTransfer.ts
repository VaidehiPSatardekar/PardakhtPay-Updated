import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { ManualTransferService } from "../services/manualTransfer/manual-transfer.service";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as manualTransfer from '../actions/manualTransfer';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { ManualTransfer, ManualTransferDetail } from "../../models/manual-transfer";
import { HttpErrorResponse } from "@angular/common/http";
import { ListSearchResponse } from "../../models/types";
import { WithdrawalReceipt } from "../../models/withdrawal";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class ManualTransferEffects extends EffectBase {

    @Effect()
    search$: Observable<Action> = this.actions$.ofType<manualTransfer.Search>(manualTransfer.SEARCH).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        map(action => action.payload),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(manualTransfer.SEARCH).pipe(skip(1));

            return this.manualTransferService.search(query).pipe(
                takeUntil(nextSearch$),
                map((tenants: ListSearchResponse<ManualTransfer[]>) => new manualTransfer.SearchComplete(tenants)),
                catchError(err => of(new manualTransfer.SearchError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    create$: Observable<Action> = this.actions$.ofType<manualTransfer.Create>(manualTransfer.CREATE).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.manualTransferService.create(payload).pipe(
                mergeMap((response: ManualTransfer) => [
                    new manualTransfer.CreateComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new manualTransfer.CreateError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<manualTransfer.GetDetails>(manualTransfer.GET_DETAILS).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.manualTransferService.get(payload).pipe(
                mergeMap((response: ManualTransfer) => [
                    new manualTransfer.GetDetailsComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new manualTransfer.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    cancel$: Observable<Action> = this.actions$.ofType<manualTransfer.Cancel>(manualTransfer.CANCEL).pipe(
        map(action => action.id),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.manualTransferService.cancel(payload).pipe(
                mergeMap((response: ManualTransfer) => [
                    new manualTransfer.CancelComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new manualTransfer.CancelError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    canceldetail$: Observable<Action> = this.actions$.ofType<manualTransfer.CancelDetail>(manualTransfer.CANCEL_DETAIL).pipe(
        map(action => action.id),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.manualTransferService.canceldetail(payload).pipe(
                mergeMap((response: ManualTransferDetail) => [
                    new manualTransfer.CancelDetailComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new manualTransfer.CancelDetailError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    retrydetail$: Observable<Action> = this.actions$.ofType<manualTransfer.RetryDetail>(manualTransfer.RETRY_DETAIL).pipe(
        map(action => action.id),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.manualTransferService.retrydetail(payload).pipe(
                mergeMap((response: ManualTransferDetail) => [
                    new manualTransfer.RetryDetailComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new manualTransfer.RetryDetailError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    setascompleteddetail$: Observable<Action> = this.actions$.ofType<manualTransfer.SetAsCompletedDetail>(manualTransfer.SETASCOMPLETED_DETAIL).pipe(
        map(action => action.id),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.manualTransferService.setascompleteddetail(payload).pipe(
                mergeMap((response: ManualTransferDetail) => [
                    new manualTransfer.SetAsCompletedDetailComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new manualTransfer.SetAsCompletedDetailError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    update$: Observable<Action> = this.actions$.ofType<manualTransfer.Edit>(manualTransfer.EDIT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0) || action.payload === undefined) {
                return empty();
            }

            return this.manualTransferService.update(action.id, action.payload).pipe(
                mergeMap((response: ManualTransfer) => [
                    new manualTransfer.EditComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new manualTransfer.EditError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    delete: Observable<Action> = this.actions$.ofType<manualTransfer.Delete>(manualTransfer.DELETE).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

                return this.manualTransferService.delete(action.id).pipe(
                mergeMap(() => [
                    new manualTransfer.DeleteComplete()
                ]),
                catchError((err: HttpErrorResponse) => of(new manualTransfer.DeleteError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getReceipt: Observable<Action> = this.actions$.ofType<manualTransfer.GetReceipt>(manualTransfer.GET_RECEIPT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

                return this.manualTransferService.getReceipt(action.id).pipe(
                mergeMap((response: WithdrawalReceipt) => [
                    new manualTransfer.GetReceiptComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new manualTransfer.GetReceiptError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private manualTransferService: ManualTransferService,
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