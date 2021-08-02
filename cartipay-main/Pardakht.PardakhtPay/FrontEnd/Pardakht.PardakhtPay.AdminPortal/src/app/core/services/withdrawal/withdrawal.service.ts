import { Injectable } from '@angular/core';
import { IHttpOptions, ListSearchResponse } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { Withdrawal, WithdrawalSearchArgs, WithdrawalSearch, WithdrawalCreate, WithdrawalReceipt, WithdrawalTransferHistory } from '../../../models/withdrawal';
import { ResponseContentType } from '@angular/http/src/enums';
import { Response } from 'selenium-webdriver/http';

@Injectable({
  providedIn: 'root'
})
export class WithdrawalService {
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
            const thisCaller: WithdrawalService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/withdrawal/';
        });
    }

    search(args: WithdrawalSearchArgs): Observable<ListSearchResponse<WithdrawalSearch[]>> {
        return this.http.post<ListSearchResponse<WithdrawalSearch[]>>(this.baseUrl + 'search', args).pipe(
            map(response => response)
        );
    }

    create(item: WithdrawalCreate): Observable<Withdrawal> {
        return this.http.post<Withdrawal>(this.baseUrl, item, this.httpOptions).pipe(
            tap((response) => this.log(`added withdrawal w/ Id=${response.id}`)),
            catchError(this.handleError<Withdrawal>('addWithdrawal'))
        );
    }

    get(id: number): Observable<Withdrawal> {
        return this.http.get<Withdrawal>(this.baseUrl + id).pipe(
            tap((response) => this.log(`get withdrawal w/ Id=${id}`)),
            catchError(this.handleError<Withdrawal>('getWithdrawal'))
        );
    }

    update(id: number, item: Withdrawal): Observable<Withdrawal> {
        return this.http.put<Withdrawal>(this.baseUrl + id, item, this.httpOptions).pipe(
            tap((response) => this.log(`update withdrawal w/ Id=${response.id}`)),
            catchError(this.handleError<Withdrawal>('updateWithdrawal'))
        );
    }

    cancel(id: number): Observable<Withdrawal> {
        return this.http.post<Withdrawal>(this.baseUrl + 'cancel/' + id, null, this.httpOptions).pipe(
            tap((response) => this.log(`cancel withdrawal w/ Id=${response.id}`)),
            catchError(this.handleError<Withdrawal>('cancelWithdrawal'))
        );
    }

    retry(id: number): Observable<Withdrawal> {
        return this.http.post<Withdrawal>(this.baseUrl + 'retry/' + id, null, this.httpOptions).pipe(
            tap((response) => this.log(`retry withdrawal w/ Id=${response.id}`)),
            catchError(this.handleError<Withdrawal>('retryWithdrawal'))
        );
    }

    setAsCompleted(id: number): Observable<Withdrawal> {
        return this.http.post<Withdrawal>(this.baseUrl + 'setascompleted/' + id, null, this.httpOptions).pipe(
            tap((response) => this.log(`set as completed withdrawal w/ Id=${response.id}`)),
            catchError(this.handleError<Withdrawal>('setAsCompletedWithdrawal'))
        );
    }

    changeProcessType(id: number, processType: number): Observable<Withdrawal> {
        return this.http.post<Withdrawal>(this.baseUrl + 'changeprocesstype/' + id + '/' + processType, null, this.httpOptions).pipe(
            tap((response) => this.log(`change process type withdrawal w/ Id=${response.id}`)),
            catchError(this.handleError<Withdrawal>('changeProcessTypeWithdrawal'))
        );
    }

    changeAllProcessType(args: WithdrawalSearchArgs, processType: number): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'changeallprocesstype/' + processType, args, this.httpOptions).pipe(
            tap((response) => this.log(`change all process type withdrawal`)),
            catchError(this.handleError<any>('changeAllProcessTypeWithdrawal'))
        );
    }

    getHistory(id: number): Observable<WithdrawalTransferHistory[]> {
        return this.http.get<WithdrawalTransferHistory[]>(this.baseUrl + 'gethistory/' + id).pipe(
            tap((response) => this.log(`get withdrawal histories w/ Id=${id}`)),
            catchError(this.handleError<WithdrawalTransferHistory[]>('getWithdrawalHistories'))
        );
    }

    getReceipt(id: number): Observable<WithdrawalReceipt> {
        let headers = new HttpHeaders({ 'Content-Type': 'application/json', 'Accept': 'application/pdf' });

        let options: IHttpOptions = { headers: headers, responseType: 'blob' as 'json', observe: 'response' as 'body' };

        return this.http.post<Response>(this.baseUrl + 'gettransferreceipt/' + id, null, options).pipe(
            tap((response) => this.log(`get withdrawal receipt w/`))
            ,
            map(response => {
                console.log(response);
                let d: WithdrawalReceipt = {
                    contentType: response.headers.get('Content-Type'),
                    data: response.body,
                    fileName: response.headers.get('File-Name')
                };

                return d;
            }),
            catchError(this.handleError<WithdrawalReceipt>('getWithdrawalReceipt'))
        );
    }

    callbackToMerchant(id: number): Observable<string> {
        return this.http.post<string>(this.baseUrl + 'withdrawalcallbacktomerchant/' + id, null, this.httpOptions).pipe(
            tap((response) => this.log(`callback withdrawal w/ ${response}`)),
            catchError(this.handleError<string>('withdrawalCallback'))
        );
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {


            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }

    private log(message: string): void {
        console.log('WithdrawalService: ' + message);
    }
}

