import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as cardToCardAccountGroup from '../actions/cardToCardAccountGroup';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { CardToCardAccountGroup } from "../../models/card-to-card-account-group";
import { HttpErrorResponse } from "@angular/common/http";
import { CardToCardAccountGroupService } from "../services/cardToCardAccountGroup/card-to-card-account-group.service";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');


@Injectable()
export class CardToCardAccountGroupEffects extends EffectBase {
    
    @Effect()
    getAll$: Observable<Action> = this.actions$.ofType<cardToCardAccountGroup.GetAll>(cardToCardAccountGroup.GET_ALL).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(cardToCardAccountGroup.GET_ALL).pipe(skip(1));

            return this.cardToCardAccountGroupService.getCardToCardAccounts().pipe(
                takeUntil(nextSearch$),
                map((cardToCardAccounts: CardToCardAccountGroup[]) => new cardToCardAccountGroup.GetAllComplete(cardToCardAccounts)),
                catchError(err => of(new cardToCardAccountGroup.GetAllError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    create$: Observable<Action> = this.actions$.ofType<cardToCardAccountGroup.Create>(cardToCardAccountGroup.CREATE).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.cardToCardAccountGroupService.create(payload).pipe(
                mergeMap((response: CardToCardAccountGroup) => [
                    new cardToCardAccountGroup.CreateComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new cardToCardAccountGroup.CreateError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<cardToCardAccountGroup.GetDetails>(cardToCardAccountGroup.GET_DETAILS).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.cardToCardAccountGroupService.get(payload).pipe(
                mergeMap((response: CardToCardAccountGroup) => [
                    new cardToCardAccountGroup.GetDetailsComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new cardToCardAccountGroup.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    update$: Observable<Action> = this.actions$.ofType<cardToCardAccountGroup.Edit>(cardToCardAccountGroup.EDIT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0) || action.payload === undefined) {
                return empty();
            }

                return this.cardToCardAccountGroupService.update(action.id, action.payload).pipe(
                    mergeMap((response: CardToCardAccountGroup) => [
                        new cardToCardAccountGroup.EditComplete(response)
                ]),
                    catchError((err: HttpErrorResponse) => of(new cardToCardAccountGroup.EditError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    delete: Observable<Action> = this.actions$.ofType<cardToCardAccountGroup.Delete>(cardToCardAccountGroup.DELETE).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

            return this.cardToCardAccountGroupService.delete(action.id).pipe(
                mergeMap(() => [
                    new cardToCardAccountGroup.DeleteComplete()
                ]),
                catchError((err: HttpErrorResponse) => of(new cardToCardAccountGroup.DeleteError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private cardToCardAccountGroupService: CardToCardAccountGroupService,
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