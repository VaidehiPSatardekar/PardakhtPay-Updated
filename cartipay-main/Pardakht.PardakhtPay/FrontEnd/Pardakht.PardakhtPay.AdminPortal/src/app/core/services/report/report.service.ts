import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { catchError } from 'rxjs/operators/catchError';
import { tap } from 'rxjs/operators/tap';
import { IHttpOptions, ListSearchResponse } from '../../../models/types';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { DashBoardWidget, DashBoardChartWidget, DashboardQuery, DashboardAccountStatusDTO, DashboardMerchantTransactionReportDTO, DepositBreakDownReport} from '../../../models/dashboard';
import { BankStatementReportSearchArgs, UserSegmentReportSearchArgs, UserSegmentReport, TenantBalance, TransactionReportSearchArgs, WithdrawalPaymentReportArgs, TenantBalanceSearchArgs } from 'app/models/report';
import { UserSegment } from 'app/models/user-segment-group';
import { IEnvironment } from 'app/core/environment/environment.model';

@Injectable({
  providedIn: 'root'
})
export class ReportService {
    private baseUrl: string;
    private httpOptions: IHttpOptions = {};

    constructor(private http: HttpClient,
        private environmentService: EnvironmentService) {
        const headers: HttpHeaders = new HttpHeaders();
        headers.append('Accept', 'application/json');
        headers.append('Content-Type', 'application/json');

        this.httpOptions = {
            headers: headers
        };

        environmentService.subscribe(this, (service: any, es: IEnvironment) => {
            const thisCaller: ReportService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/report/';
        });
    }
    getUserSegmentReport(query: UserSegmentReportSearchArgs): Observable<UserSegmentReport[]> {
        return this.http.post<UserSegmentReport[]>(this.baseUrl + 'getusersegmentreport/', query, this.httpOptions).pipe(
            tap((response: any) => this.log(`get user segment report`)),
            catchError(this.handleError<number>(`getUserSegmentReport`)));
    }

    getTenantBalance(args: TenantBalanceSearchArgs): Observable<TenantBalance[]> {
        return this.http.post<UserSegmentReport[]>(this.baseUrl + 'gettenantbalances/', args, this.httpOptions).pipe(
            tap((response: any) => this.log(`get tenant balance report`)),
            catchError(this.handleError<number>(`getTenantBalanceReport`)));
    }

    getDepositWithdrawalWidget(query: TransactionReportSearchArgs): Observable<DashBoardChartWidget> {

        return this.http.post<DashBoardChartWidget>(this.baseUrl + 'getdepositwithdrawalwidget/', query, this.httpOptions).pipe(
            tap((response: any) => this.log('get deposit withdrawal report widget')),
            catchError(this.handleError<number>('getSDepositWithdrawalReportWidget'))
        );
    }

    getWithdrawalPaymentWidget(query: WithdrawalPaymentReportArgs): Observable<DashBoardChartWidget> {

        return this.http.post<DashBoardChartWidget>(this.baseUrl + 'getwithdrawalpaymentwidget/', query, this.httpOptions).pipe(
            tap((response: any) => this.log('get withdrawal payment report widget')),
            catchError(this.handleError<number>('getWithdrawalPaymentReportWidget'))
        );
    }

    getDepositByAccountNumberWidget(query: TransactionReportSearchArgs): Observable<DashBoardChartWidget> {

        return this.http.post<DashBoardChartWidget>(this.baseUrl + 'getdepositbyaccountidwidget/', query, this.httpOptions).pipe(
            tap((response: any) => this.log('get deposit by account number report widget')),
            catchError(this.handleError<number>('getDepositByAccountNumberWidget'))
        );
    }

    getDepositBreakdownList(query: TransactionReportSearchArgs): Observable<ListSearchResponse<DepositBreakDownReport[]>> {
        debugger;
        return this.http.post < ListSearchResponse<DepositBreakDownReport[]>>(this.baseUrl + 'getdepositbybreakdownlist/', query, this.httpOptions).pipe(
            tap((response: any) => this.log('get deposit break down list')),
            catchError(this.handleError<number>('getdepositbybreakdownlist'))
        );
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {
            throw error;
        };
    }

    private log(message: string): void {
        console.log('DashboardService: ' + message);
    }
}
