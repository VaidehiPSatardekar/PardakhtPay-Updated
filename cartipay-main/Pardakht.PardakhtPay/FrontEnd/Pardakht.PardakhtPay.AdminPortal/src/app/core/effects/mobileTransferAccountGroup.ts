import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as mobileTransferCardAccountGroup from '../actions/mobileTransferAccountGroup';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { MobileTransferCardAccountGroup } from "../../models/mobile-transfer";
import { HttpErrorResponse } from "@angular/common/http";
import { MobileTransferAccountGroupService } from "../services/mobile-transfer-account-group/mobile-transfer-account-group.service";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');


@Injectable()
export class MobileTransferAccountGroupEffects extends EffectBase {
    
    @Effect()
    getAll$: Observable<Action> = this.actions$.ofType<mobileTransferCardAccountGroup.GetAll>(mobileTransferCardAccountGroup.GET_ALL).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(mobileTransferCardAccountGroup.GET_ALL).pipe(skip(1));

            return this.mobileTransferCardAccountGroupService.getCardToCardAccounts().pipe(
                takeUntil(nextSearch$),
                map((cardToCardAccounts: MobileTransferCardAccountGroup[]) => new mobileTransferCardAccountGroup.GetAllComplete(cardToCardAccounts)),
                catchError(err => of(new mobileTransferCardAccountGroup.GetAllError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    create$: Observable<Action> = this.actions$.ofType<mobileTransferCardAccountGroup.Create>(mobileTransferCardAccountGroup.CREATE).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.mobileTransferCardAccountGroupService.create(payload).pipe(
                mergeMap((response: MobileTransferCardAccountGroup) => [
                    new mobileTransferCardAccountGroup.CreateComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new mobileTransferCardAccountGroup.CreateError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<mobileTransferCardAccountGroup.GetDetails>(mobileTransferCardAccountGroup.GET_DETAILS).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.mobileTransferCardAccountGroupService.get(payload).pipe(
                mergeMap((response: MobileTransferCardAccountGroup) => [
                    new mobileTransferCardAccountGroup.GetDetailsComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new mobileTransferCardAccountGroup.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    update$: Observable<Action> = this.actions$.ofType<mobileTransferCardAccountGroup.Edit>(mobileTransferCardAccountGroup.EDIT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0) || action.payload === undefined) {
                return empty();
            }

                return this.mobileTransferCardAccountGroupService.update(action.id, action.payload).pipe(
                    mergeMap((response: MobileTransferCardAccountGroup) => [
                        new mobileTransferCardAccountGroup.EditComplete(response)
                ]),
                    catchError((err: HttpErrorResponse) => of(new mobileTransferCardAccountGroup.EditError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    delete: Observable<Action> = this.actions$.ofType<mobileTransferCardAccountGroup.Delete>(mobileTransferCardAccountGroup.DELETE).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

            return this.mobileTransferCardAccountGroupService.delete(action.id).pipe(
                mergeMap(() => [
                    new mobileTransferCardAccountGroup.DeleteComplete()
                ]),
                catchError((err: HttpErrorResponse) => of(new mobileTransferCardAccountGroup.DeleteError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private mobileTransferCardAccountGroupService: MobileTransferAccountGroupService,
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