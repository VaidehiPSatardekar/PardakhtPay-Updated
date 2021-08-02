import { Injectable } from '@angular/core';
import { IHttpOptions } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { MobileTransferCardAccount } from '../../../models/mobile-transfer';
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MobileTransferCardAccountService {
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
            const thisCaller: MobileTransferCardAccountService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/mobiletransferaccount/';
        });
    }

    search(query: string): Observable<MobileTransferCardAccount[]> {
        return this.http.get<MobileTransferCardAccount[]>(this.baseUrl).pipe(
            map(response => response || [])
        );
    }

    getMobileTransferCardAccounts(): Observable<MobileTransferCardAccount[]> {
        return this.http.get<MobileTransferCardAccount[]>(this.baseUrl).pipe(
            tap(() => this.log('get mobileTransferCardAccounts')),
            catchError(this.handleError('getMobileTransferCardAccounts', [])));
    }

    create(mobileTransferCardAccount: MobileTransferCardAccount): Observable<MobileTransferCardAccount> {
        return this.http.post<MobileTransferCardAccount>(this.baseUrl, mobileTransferCardAccount, this.httpOptions).pipe(
            tap((response) => this.log(`added mobileTransferCardAccount w/ Id=${response.id}`)),
            catchError(this.handleError<MobileTransferCardAccount>('addMobileTransferCardAccount'))
        );
    }

    get(id: number): Observable<MobileTransferCardAccount> {
        return this.http.get<MobileTransferCardAccount>(this.baseUrl + id).pipe(
            tap((response) => this.log(`get mobileTransferCardAccount w/ Id=${id}`)),
            catchError(this.handleError<MobileTransferCardAccount>('getMobileTransferCardAccount'))
        );
    }

    update(id: number, mobileTransferCardAccount: MobileTransferCardAccount): Observable<MobileTransferCardAccount> {
        return this.http.put<MobileTransferCardAccount>(this.baseUrl + id, mobileTransferCardAccount, this.httpOptions).pipe(
            tap((response) => this.log(`update mobileTransferCardAccount w/ Id=${response.id}`)),
            catchError(this.handleError<MobileTransferCardAccount>('addMobileTransferCardAccount'))
        );
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {


            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }

    private log(message: string): void {
        console.log('MobileTransferCardAccountService: ' + message);
    }
}

