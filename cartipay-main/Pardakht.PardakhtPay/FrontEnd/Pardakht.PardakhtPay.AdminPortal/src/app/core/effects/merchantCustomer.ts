import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { Scheduler, Observable, of, empty } from "rxjs";
import * as merchantCustomer from '../actions/merchantCustomer';
import { map, switchMap, catchError, mergeMap } from "rxjs/operators";
import { ListSearchResponse } from "../../models/types";
import { MerchantCustomer, MerchantRelation, MerchantCustomerCardNumbers, CustomerPhoneNumbers } from "../../models/merchant-customer";
import { MerchantCustomerService } from "../services/merchantcustomer/merchant-customer.service";
import { HttpErrorResponse } from "@angular/common/http";
import { DownloadPhoneNumbers } from "../actions/merchantCustomer";
import { RegisteredPhoneNumbers } from "../../models/user-segment-group";

export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');

@Injectable()
export class MerchantCustomerEffects extends EffectBase {
    @Effect()
    search$: Observable<Action> = this.actions$.ofType<merchantCustomer.Search>(merchantCustomer.SEARCH).pipe(
        map(action => action.payload),
        switchMap(query => {

            return this.merchantCustomerService.search(query).pipe(
                map((data: ListSearchResponse<MerchantCustomer[]>) => new merchantCustomer.SearchComplete(data)),
                catchError(err => of(new merchantCustomer.SearchError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getCustomers: Observable<Action> = this.actions$.ofType<merchantCustomer.GetPhoneNumberRelatedCustomers>(merchantCustomer.GET_PHONE_NUMBER_RELATED_CUSTOMERS).pipe(
        map(action => action.payload),
        switchMap(query => {

            return this.merchantCustomerService.getRelatedCustomers(query).pipe(
                map((data: MerchantRelation[]) => new merchantCustomer.GetPhoneNumberRelatedCustomersComplete(data)),
                catchError(err => of(new merchantCustomer.GetPhoneNumberRelatedCustomersError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getCardNumbersCount: Observable<Action> = this.actions$.ofType<merchantCustomer.GetCustomerCardNumbers>(merchantCustomer.GET_CARD_NUMBERS).pipe(
        map(action => action.payload),
        switchMap(query => {

            return this.merchantCustomerService.getCardNumbersCount(query).pipe(
                map((data: MerchantCustomerCardNumbers[]) => new merchantCustomer.GetCustomerCardNumbersComplete(data)),
                catchError(err => of(new merchantCustomer.GetCustomerCardNumbersError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getDetail$: Observable<Action> = this.actions$.ofType<merchantCustomer.GetDetails>(merchantCustomer.GET_DETAILS).pipe(
        map(action => action.payload),
        switchMap(payload => {
            if (payload === undefined) {
                return empty();
            }

            return this.merchantCustomerService.get(payload).pipe(
                mergeMap((response: MerchantCustomer) => [
                    new merchantCustomer.GetDetailsComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new merchantCustomer.GetDetailsError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    update$: Observable<Action> = this.actions$.ofType<merchantCustomer.Edit>(merchantCustomer.EDIT).pipe(
        switchMap(action => {
            if (action === undefined || !(action.id > 0) || action.payload === undefined) {
                return empty();
            }

            return this.merchantCustomerService.updateUserSegmentGroup(action.id, action.payload).pipe(
                mergeMap((response: MerchantCustomer) => [
                    new merchantCustomer.EditComplete(response)
                ]),
                catchError((err: HttpErrorResponse) => of(new merchantCustomer.EditError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    downloadPhoneNumbers$: Observable<Action> = this.actions$.ofType<merchantCustomer.DownloadPhoneNumbers>(merchantCustomer.DOWNLOAD_PHONENUMBERS).pipe(
        map(action => action.payload),
        switchMap(query => {
            return this.merchantCustomerService.downloadphonenumbers(query).pipe(
                map((data: CustomerPhoneNumbers) => new merchantCustomer.DownloadPhoneNumbersComplete(data)),
                catchError(err => of(new merchantCustomer.DownloadPhoneNumbersError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getRegisteredPhones$: Observable<Action> = this.actions$.ofType<merchantCustomer.GetRegisteredPhones>(merchantCustomer.GET_REGISTERED_PHONES).pipe(
        map(action => action.payload),
        switchMap(query => {
            return this.merchantCustomerService.getRegisteredPhones(query).pipe(
                map((data: RegisteredPhoneNumbers[]) => new merchantCustomer.GetRegisteredPhonesComplete(data)),
                catchError(err => of(new merchantCustomer.GetRegisteredPhonesError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    removeRegisteredPhones$: Observable<Action> = this.actions$.ofType<merchantCustomer.RemoveRegisteredPhones>(merchantCustomer.REMOVE_REGISTERED_PHONES).pipe(
        switchMap(action => {
            if (action === undefined || action.payload === undefined) {
                return empty();
            }
            return this.merchantCustomerService.removeRegisteredPhones(action.id, action.payload).pipe(
                mergeMap(() => [
                    new merchantCustomer.RemoveRegisteredPhonesComplete()
                ]),
                catchError((err: HttpErrorResponse) => of(new merchantCustomer.RemoveRegisteredPhonesError(this.sanitiseError(err))))
            );
        })
    );


    constructor(
        private actions$: Actions,
        private merchantCustomerService: MerchantCustomerService
    ) { super(); }
}