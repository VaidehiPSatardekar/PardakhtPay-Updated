import { Injectable } from '@angular/core';
import { IHttpOptions } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { BlockedPhoneNumber } from '../../../models/blocked-phone-number';

@Injectable({
  providedIn: 'root'
})
export class BlockedPhoneNumbersService {
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
            const thisCaller: BlockedPhoneNumbersService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/blockedphonenumber/';
        });
    }

    search(query: string): Observable<BlockedPhoneNumber[]> {
        return this.http.get<BlockedPhoneNumber[]>(this.baseUrl).pipe(
            map(response => response || [])
        );
    }

    getCardToCardAccounts(): Observable<BlockedPhoneNumber[]> {
        return this.http.get<BlockedPhoneNumber[]>(this.baseUrl).pipe(
            tap(() => this.log('get blocked phone numbers')),
            catchError(this.handleError('getBlockedPhoneNumbers', [])));
    }

    create(cardToCardAccount: BlockedPhoneNumber): Observable<BlockedPhoneNumber> {
        return this.http.post<BlockedPhoneNumber>(this.baseUrl, cardToCardAccount, this.httpOptions).pipe(
            tap((response) => this.log(`added blocked phone number w/ Id=${response.id}`)),
            catchError(this.handleError<BlockedPhoneNumber>('addBlockedPhoneNumber'))
        );
    }

    get(id: number): Observable<BlockedPhoneNumber> {
        return this.http.get<BlockedPhoneNumber>(this.baseUrl + id).pipe(
            tap((response) => this.log(`get blocked phone number w/ Id=${id}`)),
            catchError(this.handleError<BlockedPhoneNumber>('getCardToCardAccount'))
        );
    }

    update(id: number, cardToCardAccount: BlockedPhoneNumber): Observable<BlockedPhoneNumber> {
        return this.http.put<BlockedPhoneNumber>(this.baseUrl + id, cardToCardAccount, this.httpOptions).pipe(
            tap((response) => this.log(`update blocked phone number w/ Id=${response.id}`)),
            catchError(this.handleError<BlockedPhoneNumber>('updateBlockedPhoneNumber'))
        );
    }

    delete(id: number): Observable<any> {
        return this.http.delete(this.baseUrl + id, this.httpOptions).pipe(
            tap((response) => this.log(`delete cblocked phone number`)),
            catchError(this.handleError<BlockedPhoneNumber>('deleteBlockedPhoneNumber'))
        );
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {


            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }

    private log(message: string): void {
        console.log('BlockedPhoneNumberService: ' + message);
    }
}
