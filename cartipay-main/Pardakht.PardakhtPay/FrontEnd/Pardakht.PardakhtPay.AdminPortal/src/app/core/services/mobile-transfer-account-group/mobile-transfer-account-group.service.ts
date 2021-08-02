import { Injectable } from '@angular/core';
import { IHttpOptions } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { MobileTransferCardAccountGroup } from '../../../models/mobile-transfer';
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class MobileTransferAccountGroupService {
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
            const thisCaller: MobileTransferAccountGroupService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/mobiletransfercardaccountgroup/';
        });
    }

    search(query: string): Observable<MobileTransferCardAccountGroup[]> {
        return this.http.get<MobileTransferCardAccountGroup[]>(this.baseUrl).pipe(
            map(response => response || [])
        );
    }

    getCardToCardAccounts(): Observable<MobileTransferCardAccountGroup[]> {
        return this.http.get<MobileTransferCardAccountGroup[]>(this.baseUrl).pipe(
            tap(() => this.log('get cardToCardAccount Groups')),
            catchError(this.handleError('getMobileTransferCardAccountGroups', [])));
    }

    create(cardToCardAccount: MobileTransferCardAccountGroup): Observable<MobileTransferCardAccountGroup> {
        return this.http.post<MobileTransferCardAccountGroup>(this.baseUrl, cardToCardAccount, this.httpOptions).pipe(
            tap((response) => this.log(`added cardToCardAccount group w/ Id=${response.id}`)),
            catchError(this.handleError<MobileTransferCardAccountGroup>('addMobileTransferCardAccountGroup'))
        );
    }

    get(id: number): Observable<MobileTransferCardAccountGroup> {
        return this.http.get<MobileTransferCardAccountGroup>(this.baseUrl + id).pipe(
            tap((response) => this.log(`get cardToCardAccount group w/ Id=${id}`)),
            catchError(this.handleError<MobileTransferCardAccountGroup>('getCardToCardAccount'))
        );
    }

    update(id: number, cardToCardAccount: MobileTransferCardAccountGroup): Observable<MobileTransferCardAccountGroup> {
        return this.http.put<MobileTransferCardAccountGroup>(this.baseUrl + id, cardToCardAccount, this.httpOptions).pipe(
            tap((response) => this.log(`update mobileTransferCardAccountGroup w/ Id=${response.id}`)),
            catchError(this.handleError<MobileTransferCardAccountGroup>('updateMobileTransferCardAccountGroup'))
        );
    }

    delete(id: number): Observable<any> {
        return this.http.delete(this.baseUrl + id, this.httpOptions).pipe(
            tap((response) => this.log(`delete mobileTransferCardAccountGroup`)),
            catchError(this.handleError<MobileTransferCardAccountGroup>('deleteMobileTransferCardAccountGroup'))
        );
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {


            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }

    private log(message: string): void {
        console.log('MobileTransferCardAccountGroupService: ' + message);
    }
}
