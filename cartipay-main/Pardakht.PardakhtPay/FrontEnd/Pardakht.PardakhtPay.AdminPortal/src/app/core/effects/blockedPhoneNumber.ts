import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as blockedPhoneNumber from '../actions/blockedPhoneNumber';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { BlockedPhoneNumber } from "../../models/blocked-phone-number";
import { HttpErrorResponse } from "@angular/common/http";
import { BlockedPhoneNumbersService } from "../services/blocked-phone-number/blocked-phone-numbers.service";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');


@Injectable()
export class BlockedPhoneNumberEffects extends EffectBase {
    
    @Effect()
    getAll$: Observable<Action> = this.actions$.ofType<blockedPhoneNumber.GetAll>(blockedPhoneNumber.GET_ALL).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(blockedPhoneNumber.GET_ALL).pipe(skip(1));

            return this.blockedPhoneNumberService.getCardToCardAccounts().pipe(
                takeUntil(nextSearch$),
                map((cardToCardAccounts: BlockedPhoneNumber[]) => new blockedPhoneNumber.GetAllComplete(cardToCardAccounts)),
                catchError(err => of(new blockedPhoneNumber.GetAllError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    create$: Observable<Action> = this.actions$.ofType<blockedPhoneNumber.Create>(blockedPhoneNumber.CREATE).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.blockedPhoneNumberService.create(payload).pipe(
                mergeMap((response: BlockedPhoneNumber) => [
                    new blockedPhoneNumber.CreateComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new blockedPhoneNumber.CreateError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<blockedPhoneNumber.GetDetails>(blockedPhoneNumber.GET_DETAILS).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.blockedPhoneNumberService.get(payload).pipe(
                mergeMap((response: BlockedPhoneNumber) => [
                    new blockedPhoneNumber.GetDetailsComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new blockedPhoneNumber.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    update$: Observable<Action> = this.actions$.ofType<blockedPhoneNumber.Edit>(blockedPhoneNumber.EDIT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0) || action.payload === undefined) {
                return empty();
            }

                return this.blockedPhoneNumberService.update(action.id, action.payload).pipe(
                    mergeMap((response: BlockedPhoneNumber) => [
                        new blockedPhoneNumber.EditComplete(response)
                ]),
                    catchError((err: HttpErrorResponse) => of(new blockedPhoneNumber.EditError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    delete: Observable<Action> = this.actions$.ofType<blockedPhoneNumber.Delete>(blockedPhoneNumber.DELETE).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

            return this.blockedPhoneNumberService.delete(action.id).pipe(
                mergeMap(() => [
                    new blockedPhoneNumber.DeleteComplete()
                ]),
                catchError((err: HttpErrorResponse) => of(new blockedPhoneNumber.DeleteError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private blockedPhoneNumberService: BlockedPhoneNumbersService,
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