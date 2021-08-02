import { Injectable } from '@angular/core';
import { IHttpOptions } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { CardToCardAccount } from '../../../models/card-to-card-account';
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class CardToCardAccountService {
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
            const thisCaller: CardToCardAccountService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/cardToCardAccount/';
        });
    }

    search(query: string): Observable<CardToCardAccount[]> {
        return this.http.get<CardToCardAccount[]>(this.baseUrl).pipe(
            map(response => response || [])
        );
    }

    getCardToCardAccounts(): Observable<CardToCardAccount[]> {
        return this.http.get<CardToCardAccount[]>(this.baseUrl).pipe(
            tap(() => this.log('get cardToCardAccounts')),
            catchError(this.handleError('getCardToCardAccounts', [])));
    }

    create(cardToCardAccount: CardToCardAccount): Observable<CardToCardAccount> {
        return this.http.post<CardToCardAccount>(this.baseUrl, cardToCardAccount, this.httpOptions).pipe(
            tap((response) => this.log(`added cardToCardAccount w/ Id=${response.id}`)),
            catchError(this.handleError<CardToCardAccount>('addCardToCardAccount'))
        );
    }

    get(id: number): Observable<CardToCardAccount> {
        return this.http.get<CardToCardAccount>(this.baseUrl + id).pipe(
            tap((response) => this.log(`get cardToCardAccount w/ Id=${id}`)),
            catchError(this.handleError<CardToCardAccount>('getCardToCardAccount'))
        );
    }

    update(id: number, cardToCardAccount: CardToCardAccount): Observable<CardToCardAccount> {
        return this.http.put<CardToCardAccount>(this.baseUrl + id, cardToCardAccount, this.httpOptions).pipe(
            tap((response) => this.log(`update cardToCardAccount w/ Id=${response.id}`)),
            catchError(this.handleError<CardToCardAccount>('addCardToCardAccount'))
        );
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {


            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }

    private log(message: string): void {
        console.log('CardToCardService: ' + message);
    }
}
