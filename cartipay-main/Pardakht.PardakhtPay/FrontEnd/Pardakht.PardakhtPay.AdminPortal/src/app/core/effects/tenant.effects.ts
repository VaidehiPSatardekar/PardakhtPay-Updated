import { HttpErrorResponse } from '@angular/common/http';

import { Injectable, InjectionToken, Optional, Inject } from '@angular/core';
import { Effect, Actions } from '@ngrx/effects';
import { Action, Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { Scheduler } from 'rxjs/Scheduler';
import { async } from 'rxjs/scheduler/async';
import { empty } from 'rxjs/observable/empty';
import { of } from 'rxjs/observable/of';
import { catchError, map, switchMap, debounceTime, skip, takeUntil, tap, mergeMap } from 'rxjs/operators';

import * as tenant from '../actions/tenant.actions';
import { TenantPlatformMap, Tenant, Platform } from '../models/tenant.model';
import { TenantService } from '../services/tenant.service';

import { NotificationType } from 'angular2-notifications';
import { PaymentSetting } from '../models/payment-setting.model';
import * as coreState from '../../core/index';
import { AddOne, NotificationMessage } from '../actions/notification';
import { LookupItem, TenantPlatformMapFinancialDocumentSettings } from '../models/shared.model';
import { EffectBase } from './effect-base';

/**
 * Effects offer a way to isolate and easily test side-effects within your
 * application.
 *
 * If you are unfamiliar with the operators being used in these examples, please
 * check out the sources below:
 *
 * Official Docs: http://reactivex.io/rxjs/manual/overview.html#categories-of-operators
 * RxJS 5 Operators By Example: https://gist.github.com/btroncone/d6cf141d6f2c00dc6b35
 */

@Injectable()
export class TenantEffects extends EffectBase {

  @Effect()
  updateTenant$: Observable<Action> = this.actions$.ofType<tenant.UpdateTenant>(tenant.UPDATE_TENANT).pipe(
    switchMap(action => {
      if (action === undefined || action.payload === undefined) {
        return empty();
      }

      return this.tenantManagementService.updateTenant(action.payload).pipe(
        mergeMap((response: TenantPlatformMap) => [
          new tenant.UpdateTenantComplete(response),
          new AddOne(new NotificationMessage('Tenant ' + response.brandName + ' updated'))
        ]),
        catchError((err: HttpErrorResponse) => of(new tenant.UpdateTenantError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  listTenantWithCurrency$: Observable<Action> = this.actions$.ofType<tenant.GetTenantSelectList>(tenant.GET_TENANT_LIST_WITH_CURRENCY).pipe(
    switchMap(action => {
      return this.tenantManagementService.getTenantMappingListCurrency().pipe(
        mergeMap((response: TenantPlatformMap[]) => [
          new tenant.GetTenantListWithCurrencyComplete(response),
        ]),
        catchError((err: HttpErrorResponse) => of(new tenant.GetTenantListWithCurrencyError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  listTenantWithLanguages$: Observable<Action> = this.actions$.ofType<tenant.GetTenantsWithLanguages>(tenant.GET_TENANT_LANGUAGES).pipe(
    switchMap(action => {
      return this.tenantManagementService.getTenantsWithLanguages(action.payload).pipe(
        mergeMap((response: TenantPlatformMap) => [
          new tenant.GetTenantsWithLanguagesComplete(response),
        ]),
        catchError((err: HttpErrorResponse) => of(new tenant.GetTenantsWithLanguagesError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  getTenantCurrency$: Observable<Action> = this.actions$.ofType<tenant.GetTenantCurrency>(tenant.GET_AD_TENANT_CURRENCY).pipe(
    switchMap(action => {
      return this.tenantManagementService.getTenantCurrency(action.payload).pipe(
        mergeMap((response: TenantPlatformMap) => [
          new tenant.GetTenantCurrencyComplete(response),
        ]),
        catchError((err: HttpErrorResponse) => of(new tenant.GetTenantCurrencyError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  listTenantWithOptions$: Observable<Action> = this.actions$.ofType<tenant.GetTenantSelectList>(tenant.GET_TENANT_LIST_WITH_OPTIONS).pipe(
    switchMap(action => {
      return this.tenantManagementService.getTenantMappingListOptions().pipe(
        mergeMap((response: TenantPlatformMap[]) => [
          new tenant.GetTenantListWithOptionsComplete(response),
        ]),
        catchError((err: HttpErrorResponse) => of(new tenant.GetTenantListWithOptionsError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  listTenantSelect$: Observable<Action> = this.actions$.ofType<tenant.GetTenantSelectList>(tenant.GET_TENANT_SELECT_LIST).pipe(
    switchMap(action => {
      return this.tenantManagementService.getTenantSelectList().pipe(
        mergeMap((response: TenantPlatformMap[]) => [
          new tenant.GetTenantSelectListComplete(response),
        ]),
        catchError((err: HttpErrorResponse) => of(new tenant.GetTenantSelectListError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  getTenants$: Observable<Action> = this.actions$.ofType<tenant.GetTenants>(tenant.GET_TENANTS).pipe(
    switchMap(action => {
      return this.tenantManagementService.getTenants().pipe(
        mergeMap((response: TenantPlatformMap[]) => [
          new tenant.GetTenantsComplete(response),
        ]),
        catchError((err: HttpErrorResponse) => of(new tenant.GetTenantsError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  listTenantMapping$: Observable<Action> = this.actions$.ofType<tenant.GetTenantMappingList>(tenant.GET_TENANT_MAPPING_LIST).pipe(
    switchMap(action => {
      return this.tenantManagementService.getTenantMappingList().pipe(
        mergeMap((response: TenantPlatformMap[]) => [
          new tenant.GetTenantMappingListComplete(response),
        ]),
        catchError((err: HttpErrorResponse) => of(new tenant.GetTenantMappingListError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  getTenantList$: Observable<Action> = this.actions$.ofType<tenant.GetTenantList>(tenant.GET_TENANT_LIST)
    .withLatestFrom(this.store)
    .pipe(
      switchMap(([action, state]) => {

        if (state.adminManagementTenant) {
          const items = state.adminManagementTenant.tenantLookupList;
          if (items && items.length > 0) {
            return of(new tenant.GetTenantListComplete(items));
          }
        }

        return this.tenantManagementService.getTenantLookupList().pipe(
          map((response: Tenant[]) => new tenant.GetTenantListComplete(response)),
          catchError(err => of(new tenant.GetTenantListError(this.sanitiseError(err))))
        );

      })
    );


  @Effect()
  getCurrencyList$: Observable<Action> = this.actions$.ofType<tenant.GetCurrencyList>(tenant.GET_CURRENCY_LIST)
    .withLatestFrom(this.store)
    .pipe(
      switchMap(([action, state]) => {

        let cache = false;
        if (state.adminManagementTenant && state.adminManagementTenant.currencyList) {
          cache = true;
        }

        if (cache) {
          return of(new tenant.GetCurrencyListComplete(state.adminManagementTenant.currencyList));
        }
        else {

          return this.tenantManagementService.getCurrencyList().pipe(
            map((tenants: LookupItem[]) => new tenant.GetCurrencyListComplete(tenants)),
            catchError(err => of(new tenant.GetLookupListError(this.sanitiseError(err))))
          );
        }
      })
    );

  @Effect()
  getDocumentList$: Observable<Action> = this.actions$.ofType<tenant.GetDocumentList>(tenant.GET_DOCUMENT_LIST)
    .withLatestFrom(this.store)
    .pipe(
      switchMap(([action, state]) => {

        if (state.adminManagementTenant) {
          const items = state.adminManagementTenant.documentList;
          if (items && items.length > 0) {
            return of(new tenant.GetDocumentListComplete(items));
          }
        }

        return this.tenantManagementService.getDocumentList().pipe(
          map((response: LookupItem[]) => new tenant.GetDocumentListComplete(response)),
          catchError(err => of(new tenant.GetDocumentListError(this.sanitiseError(err))))
        );
      })
    );

  @Effect()
  getFinancialActionDocumentSettingsList$: Observable<Action> = this.actions$.ofType<tenant.GetFinDocumentSettingList>(tenant.GET_FIN_DOC_SETTINGS)
    .withLatestFrom(this.store)
    .pipe(
      switchMap(([action, state]) => {

        if (state.adminManagementTenant) {
          const items = state.adminManagementTenant.financialActionDocumentList;
          if (items && items.length > 0) {
            return of(new tenant.GetFinDocumentSettingListComplete(items));
          }
        }

        return this.tenantManagementService.getFinancialActionDocumentSettings().pipe(
          map((response: TenantPlatformMapFinancialDocumentSettings[]) => new tenant.GetFinDocumentSettingListComplete(response)),
          catchError(err => of(new tenant.GetFinDocumentSettingListError(this.sanitiseError(err))))
        );
      })
    );

  @Effect()
  getLanguageList$: Observable<Action> = this.actions$.ofType<tenant.GetLanguageList>(tenant.GET_LANGUAGE_LIST)
    .withLatestFrom(this.store)
    .pipe(
      switchMap(([action, state]) => {

        if (state.adminManagementTenant) {
          const items = state.adminManagementTenant.languageList;
          if (items && items.length > 0) {
            return of(new tenant.GetLanguageListComplete(items));
          }
        }

        return this.tenantManagementService.getLanguageList().pipe(
          map((response: LookupItem[]) => new tenant.GetLanguageListComplete(response)),
          catchError(err => of(new tenant.GetLookupListError(this.sanitiseError(err))))
        );
      })
    );

  @Effect()
  getCountryList$: Observable<Action> = this.actions$.ofType<tenant.GetCountryList>(tenant.GET_COUNTRY_LIST)
    .withLatestFrom(this.store)
    .pipe(
      switchMap(([action, state]) => {


        if (state.adminManagementTenant) {
          const items = state.adminManagementTenant.countryList;
          if (items && items.length > 0) {
            return of(new tenant.GetCountryListComplete(items));
          }
        }


        return this.tenantManagementService.getCountryList().pipe(
          map((response: LookupItem[]) => new tenant.GetCountryListComplete(response)),
          catchError(err => of(new tenant.GetLookupListError(this.sanitiseError(err))))
        );

      })
    );

  @Effect()
  getTimeZoneList$: Observable<Action> = this.actions$.ofType<tenant.GetTimeZoneList>(tenant.GET_TIMEZONE_LIST)
    .withLatestFrom(this.store)
    .pipe(
      switchMap(([action, state]) => {

        if (state.adminManagementTenant) {
          const items = state.adminManagementTenant.timeZoneList;
          if (items && items.length > 0) {
            return of(new tenant.GetTimeZoneListComplete(items));
          }
        }

        return this.tenantManagementService.getTimeZoneList().pipe(
          map((response: LookupItem[]) => new tenant.GetTimeZoneListComplete(response)),
          catchError(err => of(new tenant.GetLookupListError(this.sanitiseError(err))))
        );

      })
    );

  @Effect()
  getPlatform$: Observable<Action> = this.actions$.ofType<tenant.GetPlatformDetails>(tenant.GET_PLATFORM_DETAILS)
    .withLatestFrom(this.store)
    .pipe(
      switchMap(([action, state]) => {

        if (state.adminManagementTenant) {
          const items = state.adminManagementTenant.platformDetails;
          if (items) {
            return of(new tenant.GetPlatformDetailsComplete(items));
          }
        }

        return this.tenantManagementService.getPlatform().pipe(
          map((response: Platform) => new tenant.GetPlatformDetailsComplete(response)),
          catchError(err => of(new tenant.GetPlatformDetailsError(this.sanitiseError(err))))
        );

      })
    );


  @Effect()
  getTenantPaymentSettingList$: Observable<Action> = this.actions$.ofType<tenant.GetTenantPaymentSettingList>(tenant.GET_TENANT_PAYMENT_SETTING_LIST).pipe(
    switchMap(action => {
      return this.tenantManagementService.getTenantPaymentSettingList(action.payload).pipe(
        mergeMap((response: PaymentSetting[]) => [
          new tenant.GetTenantPaymentSettingListComplete(response),
        ]),
        catchError((err: HttpErrorResponse) => of(new tenant.GetTenantPaymentSettingListError(this.sanitiseError(err))))
      );
    })
  );


  @Effect()
  getTenantAvailablePaymentSettings$: Observable<Action> = this.actions$.ofType<tenant.GetTenantAvailablePaymentSettings>(tenant.GET_TENANT_AVAILABLE_PAYMENT_SETTING_LIST).pipe(
    switchMap(action => {
      return this.tenantManagementService.getTenantAvailablePaymentSettings(action.payload).pipe(
        mergeMap((response: PaymentSetting[]) => [
          new tenant.GetTenantAvailablePaymentSettingsComplete(response),
        ]),
        catchError((err: HttpErrorResponse) => of(new tenant.GetTenantAvailablePaymentSettingsError(this.sanitiseError(err))))
      );
    })
  );


  // @Effect()
  // detailsTenant$: Observable<Action> = this.actions$.ofType<tenant.GetTenantDetails>(tenant.GET_TENANT_DETAILS).pipe(
  //   switchMap(action => {
  //     return this.tenantManagementService.getTenantListByTenantGuid(action.tenantGuid).pipe(
  //       mergeMap((response: TenantPlatformMap) => [
  //         new tenant.GetTenantDetailsComplete(response),
  //       ]),
  //       catchError((err: HttpErrorResponse) => of(new tenant.GetTenantDetailsError(this.sanitiseError(err))))
  //     );
  //   })
  // );

  @Effect()
  createTenant$: Observable<Action> = this.actions$.ofType<tenant.CreateTenant>(tenant.CREATE_TENANT).pipe(
    switchMap(action => {
      return this.tenantManagementService.createTenant(action.payload).pipe(
        mergeMap((response: TenantPlatformMap) => [
          new tenant.CreateTenantComplete(response),
          new AddOne(new NotificationMessage('Tenant ' + response.brandName + ' created'))
        ]),
        catchError((err: HttpErrorResponse) => of(new tenant.CreateTenantError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  getDomainAvailability$: Observable<Action> = this.actions$.ofType<tenant.PostDomainAvailabilityCheck>(tenant.POST_DOMAIN_AVAILABILITY_CHECK).pipe(
    switchMap(action => {
      return this.tenantManagementService.checkDomainAvailability(action.domainAvailabilityCheckRequest).pipe(
        map((response: boolean) => new tenant.PostDomainAvailabilityCheckComplete(response)),
        catchError((err: HttpErrorResponse) => of(new tenant.PostDomainAvailabilityCheckError(this.sanitiseError(err))))
      );
    })
  );

  // tslint:disable-next-line:member-ordering
  constructor(
    private actions$: Actions,
    private store: Store<coreState.State>,
    private tenantManagementService: TenantService,
    @Optional()
    /**
       * You inject an optional Scheduler that will be undefined
       * in normal application usage, but its injected here so that you can mock out
       * during testing using the RxJS TestScheduler for simulating passages of time.
       */
    @Optional()
    private scheduler: Scheduler
  ) { super(); }
}
