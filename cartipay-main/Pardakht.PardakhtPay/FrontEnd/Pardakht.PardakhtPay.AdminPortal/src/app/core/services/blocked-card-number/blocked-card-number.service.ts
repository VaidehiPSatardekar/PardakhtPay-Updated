import { Injectable } from '@angular/core';
import { IHttpOptions } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { BlockedCardNumber } from '../../../models/blocked-card-number';

@Injectable({
  providedIn: 'root'
})
export class BlockedCardNumbersService {
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
            const thisCaller: BlockedCardNumbersService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/blockedcardnumber/';
        });
    }

    search(query: string): Observable<BlockedCardNumber[]> {
        return this.http.get<BlockedCardNumber[]>(this.baseUrl).pipe(
            map(response => response || [])
        );
    }

    getCardToCardAccounts(): Observable<BlockedCardNumber[]> {
        return this.http.get<BlockedCardNumber[]>(this.baseUrl).pipe(
            tap(() => this.log('get blocked Card numbers')),
            catchError(this.handleError('getBlockedCardNumbers', [])));
    }

    create(cardToCardAccount: BlockedCardNumber): Observable<BlockedCardNumber> {
        return this.http.post<BlockedCardNumber>(this.baseUrl, cardToCardAccount, this.httpOptions).pipe(
            tap((response) => this.log(`added blocked Card number w/ Id=${response.id}`)),
            catchError(this.handleError<BlockedCardNumber>('addBlockedCardNumber'))
        );
    }

    get(id: number): Observable<BlockedCardNumber> {
        return this.http.get<BlockedCardNumber>(this.baseUrl + id).pipe(
            tap((response) => this.log(`get blocked Card number w/ Id=${id}`)),
            catchError(this.handleError<BlockedCardNumber>('getCardToCardAccount'))
        );
    }

    update(id: number, cardToCardAccount: BlockedCardNumber): Observable<BlockedCardNumber> {
        return this.http.put<BlockedCardNumber>(this.baseUrl + id, cardToCardAccount, this.httpOptions).pipe(
            tap((response) => this.log(`update blocked Card number w/ Id=${response.id}`)),
            catchError(this.handleError<BlockedCardNumber>('updateBlockedCardNumber'))
        );
    }

    delete(id: number): Observable<any> {
        return this.http.delete(this.baseUrl + id, this.httpOptions).pipe(
            tap((response) => this.log(`delete cblocked Card number`)),
            catchError(this.handleError<BlockedCardNumber>('deleteBlockedCardNumber'))
        );
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {


            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }

    private log(message: string): void {
        console.log('BlockedCardNumberService: ' + message);
    }
}
