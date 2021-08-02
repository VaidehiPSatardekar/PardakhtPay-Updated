import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { Observable } from 'rxjs/Observable';
import { catchError } from 'rxjs/operators/catchError';
import { tap } from 'rxjs/operators/tap';

import { IHttpOptions } from '../../../models/types';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { DashBoardWidget, DashBoardChartWidget, DashboardQuery, DashboardAccountStatusDTO, DashboardMerchantTransactionReportDTO, DashboardTransactionPaymentTypeReportDTO } from '../../../models/dashboard';
import { IEnvironment } from 'app/core/environment/environment.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
    private baseUrl: string;
    private httpOptions: IHttpOptions = {};

    constructor(private http: HttpClient, private environmentService: EnvironmentService) {
        const headers: HttpHeaders = new HttpHeaders();
        headers.append('Accept', 'application/json');
        headers.append('Content-Type', 'application/json');

        this.httpOptions = {
            headers: headers
        };

        environmentService.subscribe(this, (service: any, es: IEnvironment) => {
            const thisCaller: DashboardService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/dashboard/';
        });
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {
            throw error;
        };
    }

    private log(message: string): void {
        console.log('DashboardService: ' + message);
    }

    getTransactionWidget(query: DashboardQuery): Observable<DashBoardWidget> {

        return this.http.post<DashBoardWidget>(this.baseUrl + 'gettransactionreport/', query, this.httpOptions).pipe(
            tap((response: any) => this.log('get transaction widget')),
            catchError(this.handleError<number>('getTransactionWidget'))
        );
    }

    getMerchantTransactionWidget(query: DashboardQuery): Observable<DashboardMerchantTransactionReportDTO[]> {

        return this.http.post<DashBoardWidget>(this.baseUrl + 'getmerchanttransactionreport/', query, this.httpOptions).pipe(
            tap((response: any) => this.log('get merchant transaction widget')),
            catchError(this.handleError<number>('getMerchantTransactionWidget'))
        );
    }

    getTransactionGraphWidget(query: DashboardQuery): Observable<DashBoardChartWidget> {

        return this.http.post<DashBoardChartWidget>(this.baseUrl + 'gettransactiongraphreport/', query, this.httpOptions).pipe(
            tap((response: any) => this.log('get transaction graph widget')),
            catchError(this.handleError<number>('getTransactionGraphWidget'))
        );
    }

    getAccountingGraphWidget(query: DashboardQuery): Observable<DashBoardChartWidget> {

        return this.http.post<DashBoardChartWidget>(this.baseUrl + 'getaccountinggraphreport/', query, this.httpOptions).pipe(
            tap((response: any) => this.log('get accounting graph widget')),
            catchError(this.handleError<number>('getAccountingGraphWidget'))
        );
    }

    getAccountStatusWidget(query: DashboardQuery): Observable<DashboardAccountStatusDTO[]> {

        return this.http.post<DashboardAccountStatusDTO[]>(this.baseUrl + 'getaccountstatusreport/', query, this.httpOptions).pipe(
            tap((response: any) => this.log('get account status widget')),
            catchError(this.handleError<number>('getAccountStatusWidget'))
        );
    }

    getTransactionDepositBreakDownGraphWidget(query: DashboardQuery): Observable<DashBoardChartWidget> {
        return this.http.post<DashBoardChartWidget>(this.baseUrl + 'gettransactiondepositbreakdowngraphreport/', query, this.httpOptions).pipe(
            tap((response: any) => this.log('get transaction deposit break down graph widget')),
            catchError(this.handleError<number>('gettransactiondepositbreakdowngraphreport'))
        );
    }

    getTransactionWithdrawalWidget(query: DashboardQuery): Observable<DashBoardWidget> {

        return this.http.post<DashBoardWidget>(this.baseUrl + 'gettransactionreportforwithdrawal/', query, this.httpOptions).pipe(
            tap((response: any) => this.log('get transaction withdrawal widget')),
            catchError(this.handleError<number>('getTransactionWidget'))
        );
    }

    getTransactionByPaymentTypeWidget(query: DashboardQuery): Observable<DashboardTransactionPaymentTypeReportDTO[]> {

        return this.http.post<DashBoardWidget>(this.baseUrl + 'gettransactionbypaymenttypereport/', query, this.httpOptions).pipe(
            tap((response: any) => this.log('get merchant transaction widget')),
            catchError(this.handleError<number>('getMerchantTransactionWidget'))
        );
    }
}
