import { Injectable } from '@angular/core';
import { IHttpOptions } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { TransferAccount } from '../../../models/transfer-account';
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class TransferAccountService {
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
            const thisCaller: TransferAccountService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/transferAccount/';
        });
    }

    search(query: string): Observable<TransferAccount[]> {
        return this.http.get<TransferAccount[]>(this.baseUrl).pipe(
            map(response => response || [])
        );
    }

    getTransferAccounts(): Observable<TransferAccount[]> {
        return this.http.get<TransferAccount[]>(this.baseUrl).pipe(
            tap(() => this.log('get transferAccounts')),
            catchError(this.handleError('getTransferAccounts', [])));
    }

    create(transferAccount: TransferAccount): Observable<TransferAccount> {
        return this.http.post<TransferAccount>(this.baseUrl, transferAccount, this.httpOptions).pipe(
            tap((response) => this.log(`added transferAccount w/ Id=${response.id}`)),
            catchError(this.handleError<TransferAccount>('addTransferAccount'))
        );
    }

    get(id: number): Observable<TransferAccount> {
        return this.http.get<TransferAccount>(this.baseUrl + id).pipe(
            tap((response) => this.log(`get transferAccount w/ Id=${id}`)),
            catchError(this.handleError<TransferAccount>('getTransferAccount'))
        );
    }

    update(id: number, transferAccount: TransferAccount): Observable<TransferAccount> {
        return this.http.put<TransferAccount>(this.baseUrl + id, transferAccount, this.httpOptions).pipe(
            tap((response) => this.log(`update transferAccount w/ Id=${response.id}`)),
            catchError(this.handleError<TransferAccount>('addTransferAccount'))
        );
    }

    delete(id: number): Observable<any> {
        return this.http.delete(this.baseUrl + id, this.httpOptions).pipe(
            tap((response) => this.log(`delete transferAccount`)),
            catchError(this.handleError<TransferAccount>('deleteTransferAccount'))
        );
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {


            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }

    private log(message: string): void {
        console.log('TransferAccountService: ' + message);
    }
}
