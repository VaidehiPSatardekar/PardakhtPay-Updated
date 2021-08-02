import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as userSegmentGroup from '../actions/userSegmentGroup';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { UserSegmentGroup } from "../../models/user-segment-group";
import { HttpErrorResponse } from "@angular/common/http";
import { UserSegmentGroupService } from "../services/userSegmentGroup/user-segment-group.service";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');


@Injectable()
export class UserSegmentGroupEffects extends EffectBase {

    @Effect()
    getAll$: Observable<Action> = this.actions$.ofType<userSegmentGroup.GetAll>(userSegmentGroup.GET_ALL).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(userSegmentGroup.GET_ALL).pipe(skip(1));

            return this.userSegmentGroupService.getItems().pipe(
                takeUntil(nextSearch$),
                map((userSegmentGroups: UserSegmentGroup[]) => new userSegmentGroup.GetAllComplete(userSegmentGroups)),
                catchError(err => of(new userSegmentGroup.GetAllError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    create$: Observable<Action> = this.actions$.ofType<userSegmentGroup.Create>(userSegmentGroup.CREATE).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.userSegmentGroupService.create(payload).pipe(
                mergeMap((response: UserSegmentGroup) => [
                    new userSegmentGroup.CreateComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new userSegmentGroup.CreateError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<userSegmentGroup.GetDetails>(userSegmentGroup.GET_DETAILS).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.userSegmentGroupService.get(payload).pipe(
                mergeMap((response: UserSegmentGroup) => [
                    new userSegmentGroup.GetDetailsComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new userSegmentGroup.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    update$: Observable<Action> = this.actions$.ofType<userSegmentGroup.Edit>(userSegmentGroup.EDIT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0) || action.payload === undefined) {
                return empty();
            }

            return this.userSegmentGroupService.update(action.id, action.payload).pipe(
                mergeMap((response: UserSegmentGroup) => [
                    new userSegmentGroup.EditComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new userSegmentGroup.EditError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    delete: Observable<Action> = this.actions$.ofType<userSegmentGroup.Delete>(userSegmentGroup.DELETE).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

            return this.userSegmentGroupService.delete(action.id).pipe(
                mergeMap(() => [
                    new userSegmentGroup.DeleteComplete()
                ]),
                catchError((err: HttpErrorResponse) => of(new userSegmentGroup.DeleteError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private userSegmentGroupService: UserSegmentGroupService,
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