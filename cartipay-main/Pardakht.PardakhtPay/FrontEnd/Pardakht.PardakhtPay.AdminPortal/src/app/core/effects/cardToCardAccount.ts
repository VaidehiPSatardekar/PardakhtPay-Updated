import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { CardToCardAccountService } from "../services/cardToCard/card-to-card.service";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as cardToCardAccount from '../actions/cardToCardAccount';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { CardToCardAccount } from "../../models/card-to-card-account";
import { HttpErrorResponse } from "@angular/common/http";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class CardToCardAccountEffects extends EffectBase {

    @Effect()
    search$: Observable<Action> = this.actions$.ofType<cardToCardAccount.Search>(cardToCardAccount.SEARCH).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        map(action => action.payload),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(cardToCardAccount.SEARCH).pipe(skip(1));

            return this.cardToCardAccountService.search(query).pipe(
                takeUntil(nextSearch$),
                map((tenants: CardToCardAccount[]) => new cardToCardAccount.SearchComplete(tenants)),
                catchError(err => of(new cardToCardAccount.SearchError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getAll$: Observable<Action> = this.actions$.ofType<cardToCardAccount.GetAll>(cardToCardAccount.GET_ALL).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(cardToCardAccount.GET_ALL).pipe(skip(1));

            return this.cardToCardAccountService.getCardToCardAccounts().pipe(
                takeUntil(nextSearch$),
                map((cardToCardAccounts: CardToCardAccount[]) => new cardToCardAccount.GetAllComplete(cardToCardAccounts)),
                catchError(err => of(new cardToCardAccount.GetAllError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    create$: Observable<Action> = this.actions$.ofType<cardToCardAccount.Create>(cardToCardAccount.CREATE).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.cardToCardAccountService.create(payload).pipe(
                mergeMap((response: CardToCardAccount) => [
                    new cardToCardAccount.CreateComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new cardToCardAccount.CreateError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<cardToCardAccount.GetDetails>(cardToCardAccount.GET_DETAILS).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.cardToCardAccountService.get(payload).pipe(
                mergeMap((response: CardToCardAccount) => [
                    new cardToCardAccount.GetDetailsComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new cardToCardAccount.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    update$: Observable<Action> = this.actions$.ofType<cardToCardAccount.Edit>(cardToCardAccount.EDIT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0) || action.payload === undefined) {
                return empty();
            }

            return this.cardToCardAccountService.update(action.id, action.payload).pipe(
                mergeMap((response: CardToCardAccount) => [
                    new cardToCardAccount.EditComplete(response)
                    //,
                    //new notification.AddOne(new notification.NotificationMessage("Tenant '" + response.title + "' has been created."))
                ]),
                catchError((err: HttpErrorResponse) => of(new cardToCardAccount.EditError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private cardToCardAccountService: CardToCardAccountService,
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