import { Injectable, Optional, Inject, InjectionToken } from "@angular/core";
import { EffectBase } from "./effect-base";
import { Action } from '@ngrx/store';
import { Actions, Effect } from "@ngrx/effects";
import { Observable, of } from "rxjs";
import * as report from '../actions/report';
import { map, switchMap, skip, takeUntil, catchError } from "rxjs/operators";
import { ReportService } from "../services/report/report.service";
import { DashBoardChartWidget, DepositBreakDownReport } from "app/models/dashboard";
import { UserSegmentReport, TenantBalance } from "app/models/report";
import { ListSearchResponse } from '../../models/types';

@Injectable()
export class ReportEffects extends EffectBase {
    @Effect()
    getUserSegmentReport$: Observable<Action> = this.actions$.ofType<report.GetUserSegmentReport>(report.GET_USER_SEGMENT_REPORT).pipe(
        map(action => action.payload),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(report.GET_USER_SEGMENT_REPORT).pipe(skip(1));

            return this.reportService.getUserSegmentReport(query).pipe(
                takeUntil(nextSearch$),
                map((reports: UserSegmentReport[]) => new report.GetUserSegmentReportCompleted(reports)),
                catchError(err => of(new report.GetUserSegmentReportError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getTenantBalance$: Observable<Action> = this.actions$.ofType<report.GetTenantBalance>(report.GET_TENANT_BALANCE).pipe(
        switchMap(query => {
            return this.reportService.getTenantBalance(query.payload).pipe(
                map((reports: TenantBalance[]) => new report.GetTenantBalanceCompleted(reports)),
            );
        })
    );

    @Effect()
    getDepositWithdrawalWidget$: Observable<Action> = this.actions$.ofType<report.GetDepositWithdrawalWidget>(report.GET_DEPOSIT_WITHDRAWAL_WIDGET).pipe(
        map(action => action.payload),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(report.GET_DEPOSIT_WITHDRAWAL_WIDGET).pipe(skip(1));

            return this.reportService.getDepositWithdrawalWidget(query).pipe(
                takeUntil(nextSearch$),
                map((reports: DashBoardChartWidget) => new report.GetDepositWithdrawalWidgetComplete(reports)),
                catchError(err => of(new report.GetDepositWithdrawalWidgetError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getWithdrawalPaymentWidget$: Observable<Action> = this.actions$.ofType<report.GetWithdrawalPaymentWidget>(report.GET_WITHDRAWAL_PAYMENT_WIDGET).pipe(
        map(action => action.payload),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(report.GET_WITHDRAWAL_PAYMENT_WIDGET).pipe(skip(1));

            return this.reportService.getWithdrawalPaymentWidget(query).pipe(
                takeUntil(nextSearch$),
                map((reports: DashBoardChartWidget) => new report.GetWithdrawalPaymentWidgetComplete(reports)),
                catchError(err => of(new report.GetWithdrawalPaymentWidgetError(this.sanitiseError(err))))
            );
        })
    );

    @Effect()
    getDepositByAccountNumberWidget$: Observable<Action> = this.actions$.ofType<report.GetDepositByAccountNumberWidget>(report.GET_DEPOSIT_BY_ACCOUNTNUMBER_WIDGET).pipe(
        map(action => action.payload),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(report.GET_DEPOSIT_BY_ACCOUNTNUMBER_WIDGET).pipe(skip(1));

            return this.reportService.getDepositByAccountNumberWidget(query).pipe(
                takeUntil(nextSearch$),
                map((reports: DashBoardChartWidget) => new report.GetDepositByAccountNumberWidgetComplete(reports)),
                catchError(err => of(new report.GetDepositByAccountNumberWidgetError(this.sanitiseError(err))))
            );
        })
    );x

    @Effect()
    getDepositBreakdownList$: Observable<Action> = this.actions$.ofType<report.GetDepositBreakDownList>(report.GET_DEPOSIT_BREAKDOWN_LIST).pipe(
        map(action => action.payload),
        switchMap(query => {
            const nextSearch$ = this.actions$.ofType(report.GET_DEPOSIT_BREAKDOWN_LIST).pipe(skip(1));
            debugger;
            return this.reportService.getDepositBreakdownList(query).pipe(
                takeUntil(nextSearch$),
                map((reports: ListSearchResponse<DepositBreakDownReport[]>) => new report.GetDepositBreakDownListComplete(reports)),
                catchError(err => of(new report.GetDepositBreakDownListError(this.sanitiseError(err))))
            );
        })
    );

    constructor(
        private actions$: Actions,
        private reportService: ReportService
    ) { super(); }
}