import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action, Store } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as applicationSettings from '../actions/applicationSettings';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { HttpErrorResponse } from "@angular/common/http";
import { ApplicationSettingsService } from "../services/applicationSettings/application-settings.service";
import { ApplicationSettings, TransferStatusDescription } from "../../models/application-settings";
import * as coreState from '../../core';
import 'rxjs/add/operator/withLatestFrom';
import { OwnerSetting } from "app/models/owner-setting";
import { OwnerSettingService } from "../services/ownerSettings/owner-setting.service";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class ApplicationSettingsEffects extends EffectBase {

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<applicationSettings.GetDetails>(applicationSettings.GET_DETAILS).pipe(
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.applicationSettingsService.get().pipe(
                mergeMap((response: ApplicationSettings) => [
                    new applicationSettings.GetDetailsComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new applicationSettings.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getTransferStatusList$: Observable<Action> = this.actions$.ofType<applicationSettings.GetTransferStatus>(applicationSettings.GET_TRANSFER_STATUS)
        .withLatestFrom(this.store)
        .pipe(
            switchMap(([action, state]) => {

                let items = state.applicationSettings.transferStatuses;

                if (items) {
                    return of(new applicationSettings.GetTransferStatusComplete(items));
                }
                else {
                    return this.applicationSettingsService.getTranfserStatuses().pipe(
                        mergeMap((response: TransferStatusDescription[]) => [
                            new applicationSettings.GetTransferStatusComplete(response)
                        ]),
                        catchError((err: HttpErrorResponse) => of(new applicationSettings.GetTransferStatusError(this.sanitiseError(err))))
                    );
                }
            })
        );

    @Effect()
    update$: Observable<Action> = this.actions$.ofType<applicationSettings.Edit>(applicationSettings.EDIT).pipe(
        switchMap(action => {
            if (action === undefined || action.payload === undefined) {
                return empty();
            }

            return this.applicationSettingsService.update(action.payload).pipe(
                mergeMap((response: ApplicationSettings) => [
                    new applicationSettings.EditComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new applicationSettings.EditError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getOwnerSetting$: Observable<Action> = this.actions$.ofType<applicationSettings.GetOwnerSetting>(applicationSettings.GET_OWNER_SETTING).pipe(
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.ownerSettingService.get().pipe(
                mergeMap((response: OwnerSetting) => [
                    new applicationSettings.GetOwnerSettingCompleted(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new applicationSettings.GetOwnerSettingError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    saveOwnerSetting$: Observable<Action> = this.actions$.ofType<applicationSettings.SaveOwnerSetting>(applicationSettings.SAVE_OWNER_SETTING).pipe(
        switchMap(action => {
            if (action === undefined || action.payload === undefined) {
                return empty();
            }

            return this.ownerSettingService.save(action.payload).pipe(
                mergeMap((response: OwnerSetting) => [
                    new applicationSettings.SaveOwnerSettingCompleted(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new applicationSettings.SaveOwnerSettingError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private store: Store<coreState.State>,
        private applicationSettingsService: ApplicationSettingsService,
        private ownerSettingService: OwnerSettingService,
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