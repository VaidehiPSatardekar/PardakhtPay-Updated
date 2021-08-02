import { EffectBase } from './effect-base';
import { Inject, Injectable, InjectionToken, Optional } from '@angular/core';
import { Actions, Effect } from '@ngrx/effects';
import { Action } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { of } from 'rxjs/observable/of';
import { catchError } from 'rxjs/operators/catchError';
import { map } from 'rxjs/operators/map';
import { switchMap } from 'rxjs/operators/switchMap';
import { Scheduler } from 'rxjs/Scheduler';
import * as dashboard from '../actions/dashboard';
import { DashboardService } from '../services/dashboard/dashboard.service';
import { DashBoardWidget, DashBoardChartWidget, DashboardAccountStatusDTO, DashboardMerchantTransactionReportDTO, DashboardTransactionPaymentTypeReportDTO } from '../../models/dashboard';

export const SEARCH_DEBOUNCE: InjectionToken<number> = new InjectionToken<number>('Search Debounce');
export const SEARCH_SCHEDULER: InjectionToken<Scheduler> = new InjectionToken<Scheduler>('Search Scheduler');


@Injectable()
export class DashboardEffects extends EffectBase {

    @Effect()
    getTransactionWidget$: Observable<Action> = this.actions$.ofType<dashboard.GetTransactionWidget>(dashboard.GET_TRANSACTION_WIDGET).pipe(
        switchMap((action: dashboard.GetTransactionWidget) => {
            return this.dashboardService.getTransactionWidget(action.payload).pipe(
                map((widget: DashBoardWidget) => new dashboard.GetTransactionWidgetComplete(widget)),
                catchError((err: any) => of(new dashboard.GetTransactionWidgetError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getMerchantTransactionWidget$: Observable<Action> = this.actions$.ofType<dashboard.GetMerchantTransactionWidget>(dashboard.GET_MERCHANT_TRANSACTION_WIDGET).pipe(
        switchMap((action: dashboard.GetMerchantTransactionWidget) => {
                return this.dashboardService.getMerchantTransactionWidget(action.payload).pipe(
                    map((widget: DashboardMerchantTransactionReportDTO[]) => new dashboard.GetMerchantTransactionWidgetComplete(widget)),
                catchError((err: any) => of(new dashboard.GetMerchantTransactionWidgetError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getTransactionGraphWidget$: Observable<Action> = this.actions$.ofType<dashboard.GetTransactionGraphWidget>(dashboard.GET_TRANSACTION_GRAPH_WIDGET).pipe(
        switchMap((action: dashboard.GetTransactionGraphWidget) => {
            return this.dashboardService.getTransactionGraphWidget(action.payload).pipe(
                map((widget: DashBoardChartWidget) => new dashboard.GetTransactionGraphWidgetComplete(widget)),
                catchError((err: any) => of(new dashboard.GetTransactionGraphWidgetError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getAccountingGraphWidget$: Observable<Action> = this.actions$.ofType<dashboard.GetAccountingGraphWidget>(dashboard.GET_ACCOUNTING_GRAPH_WIDGET).pipe(
        switchMap((action: dashboard.GetAccountingGraphWidget) => {
            return this.dashboardService.getAccountingGraphWidget(action.payload).pipe(
                map((widget: DashBoardChartWidget) => new dashboard.GetAccountingGraphWidgetComplete(widget)),
                catchError((err: any) => of(new dashboard.GetAccountingGraphWidgetError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getAccountStatusWidget$: Observable<Action> = this.actions$.ofType<dashboard.GetAccountStatusWidget>(dashboard.GET_ACCOUNT_STATUS_WIDGET).pipe(
        switchMap((action: dashboard.GetAccountStatusWidget) => {
            return this.dashboardService.getAccountStatusWidget(action.payload).pipe(
                map((widget: DashboardAccountStatusDTO[]) => new dashboard.GetAccountStatusWidgetComplete(widget)),
                catchError((err: any) => of(new dashboard.GetAccountStatusWidgetError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getTransactionDepositBreakDownGraphWidget$: Observable<Action> = this.actions$.ofType<dashboard.GetTransactionDepositBreakDownGraphWidget>(dashboard.GET_TRANSACTION_DEPOSIT_BREAKDOWN_GRAPH_WIDGET).pipe(
        switchMap((action: dashboard.GetTransactionDepositBreakDownGraphWidget) => {
            return this.dashboardService.getTransactionDepositBreakDownGraphWidget(action.payload).pipe(
                map((widget: DashBoardChartWidget) => new dashboard.GetTransactionDepositBreakDownGraphWidgetComplete(widget)),
                catchError((err: any) => of(new dashboard.GetTransactionDepositBreakDownGraphWidgetError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getTransactionWithdrawalWidget$: Observable<Action> = this.actions$.ofType<dashboard.GetTransactionWithdrawalWidget>(dashboard.GET_TRANSACTION_WITHDRAWAL_WIDGET).pipe(
        switchMap((action: dashboard.GetTransactionWithdrawalWidget) => {
            return this.dashboardService.getTransactionWithdrawalWidget(action.payload).pipe(
                map((widget: DashBoardWidget) => new dashboard.GetTransactionWithdrawalWidgetComplete(widget)),
                catchError((err: any) => of(new dashboard.GetTransactionWithdrawalWidgetError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getTransactionPaymentTypeWidget$: Observable<Action> = this.actions$.ofType<dashboard.GetTransactionByPaymentTypeWidget>(dashboard.GET_TRANSACTION_BY_PAYMENTTYPE_WIDGET).pipe(
        switchMap((action: dashboard.GetTransactionByPaymentTypeWidget) => {
            return this.dashboardService.getTransactionByPaymentTypeWidget(action.payload).pipe(
                map((widget: DashboardTransactionPaymentTypeReportDTO[]) => new dashboard.GetTransactionByPaymentTypeWidgetComplete(widget)),
                catchError((err: any) => of(new dashboard.GeTransactionByPaymentTypeWidgetError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private dashboardService: DashboardService,
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
