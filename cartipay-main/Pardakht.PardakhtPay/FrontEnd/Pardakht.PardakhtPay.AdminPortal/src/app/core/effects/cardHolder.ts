import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { CardHolderService } from "../services/card-holder/card-holder.service";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as cardHolder from '../actions/cardHolder';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { CardHolder } from "../../models/card-holder";
import { HttpErrorResponse } from "@angular/common/http";
import { ListSearchResponse } from "../../models/types";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class CardHolderEffects extends EffectBase {
    @Effect()
    search$: Observable<Action> = this.actions$.ofType<cardHolder.Search>(cardHolder.SEARCH).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        map(action => action.payload),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(cardHolder.SEARCH).pipe(skip(1));

            return this.cardHolderService.search(query).pipe(
                takeUntil(nextSearch$),
                map((cardHolders: CardHolder) => new cardHolder.SearchComplete(cardHolders)),
                catchError(err => of(new cardHolder.SearchError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private cardHolderService: CardHolderService,
        @Optional()
        @Inject(SEARCH_DEBOUNCE)
        private debounce: number = 300,
        @Optional()
        @Inject(SEARCH_SCHEDULER)
        private scheduler: Scheduler
    ) { super(); }
}