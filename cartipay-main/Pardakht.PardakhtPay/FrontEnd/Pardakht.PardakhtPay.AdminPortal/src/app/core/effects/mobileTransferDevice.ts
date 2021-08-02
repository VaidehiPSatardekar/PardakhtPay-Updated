import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { MobileTransferDeviceService } from "../services/mobile-transfer-device/mobile-transfer-device.service";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as mobileTransferDevice from '../actions/mobile-transfer-device';
import { debounceTime, map, switchMap, skip, takeUntil, catchError, mergeMap } from "rxjs/operators";
import { async } from "rxjs/internal/scheduler/async";
import { MobileTransferDevice } from "../../models/mobile-transfer";
import { HttpErrorResponse } from "@angular/common/http";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class MobileTransferDeviceEffects extends EffectBase {

    @Effect()
    getAll$: Observable<Action> = this.actions$.ofType<mobileTransferDevice.GetAll>(mobileTransferDevice.GET_ALL).pipe(
        debounceTime(this.debounce, this.scheduler || async),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(mobileTransferDevice.GET_ALL).pipe(skip(1));

            return this.mobileTransferDeviceService.getMobileTransferDevices().pipe(
                takeUntil(nextSearch$),
                map((mobileTransferDevices: MobileTransferDevice[]) => new mobileTransferDevice.GetAllComplete(mobileTransferDevices)),
                catchError(err => of(new mobileTransferDevice.GetAllError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    create$: Observable<Action> = this.actions$.ofType<mobileTransferDevice.Create>(mobileTransferDevice.CREATE).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.mobileTransferDeviceService.create(payload).pipe(
                mergeMap((response: MobileTransferDevice) => [
                    new mobileTransferDevice.CreateComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new mobileTransferDevice.CreateError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<mobileTransferDevice.GetDetails>(mobileTransferDevice.GET_DETAILS).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.mobileTransferDeviceService.get(payload).pipe(
                mergeMap((response: MobileTransferDevice) => [
                    new mobileTransferDevice.GetDetailsComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new mobileTransferDevice.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    update$: Observable<Action> = this.actions$.ofType<mobileTransferDevice.Edit>(mobileTransferDevice.EDIT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0) || action.payload === undefined) {
                return empty();
            }

            return this.mobileTransferDeviceService.update(action.id, action.payload).pipe(
                mergeMap((response: MobileTransferDevice) => [
                    new mobileTransferDevice.EditComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new mobileTransferDevice.EditError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    sendSms$: Observable<Action> = this.actions$.ofType<mobileTransferDevice.SendSms>(mobileTransferDevice.SEND_SMS).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

                return this.mobileTransferDeviceService.sendSms(action.id).pipe(
                mergeMap((response: MobileTransferDevice) => [
                    new mobileTransferDevice.SendSmsComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new mobileTransferDevice.SendSmsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    activate$: Observable<Action> = this.actions$.ofType<mobileTransferDevice.Activate>(mobileTransferDevice.ACTIVATE).pipe(
            switchMap(action => {
                if (action === undefined || !(action.id > 0) || action.code == undefined || action.code == '') {
                return empty();
            }

                return this.mobileTransferDeviceService.activate(action.id, action.code).pipe(
                mergeMap((response: MobileTransferDevice) => [
                    new mobileTransferDevice.ActivateComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new mobileTransferDevice.ActivateError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    checkStatus$: Observable<Action> = this.actions$.ofType<mobileTransferDevice.CheckStatus>(mobileTransferDevice.CHECK_STATUS).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

                return this.mobileTransferDeviceService.checkStatus(action.id).pipe(
                mergeMap((response: MobileTransferDevice) => [
                    new mobileTransferDevice.CheckStatusComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new mobileTransferDevice.CheckStatusError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    remove$: Observable<Action> = this.actions$.ofType<mobileTransferDevice.Remove>(mobileTransferDevice.REMOVE).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0)) {
                return empty();
            }

                return this.mobileTransferDeviceService.remove(action.id).pipe(
                mergeMap((response: MobileTransferDevice) => [
                    new mobileTransferDevice.RemoveComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new mobileTransferDevice.RemoveError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private mobileTransferDeviceService: MobileTransferDeviceService,
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