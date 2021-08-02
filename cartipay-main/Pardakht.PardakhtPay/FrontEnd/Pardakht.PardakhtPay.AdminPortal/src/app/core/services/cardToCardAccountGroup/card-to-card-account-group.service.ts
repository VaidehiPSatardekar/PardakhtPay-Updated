import { Injectable } from '@angular/core';
import { IHttpOptions } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { CardToCardAccountGroup } from '../../../models/card-to-card-account-group';
import { Observable , of} from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class CardToCardAccountGroupService {
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
            const thisCaller: CardToCardAccountGroupService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/cardToCardAccountGroup/';
        });
    }

    search(query: string): Observable<CardToCardAccountGroup[]> {
        return this.http.get<CardToCardAccountGroup[]>(this.baseUrl).pipe(
            map(response => response || [])
        );
    }

    getCardToCardAccounts(): Observable<CardToCardAccountGroup[]> {
          return of([]);
    }

    create(cardToCardAccount: CardToCardAccountGroup): Observable<CardToCardAccountGroup> {
        return this.http.post<CardToCardAccountGroup>(this.baseUrl, cardToCardAccount, this.httpOptions).pipe(
            tap((response) => this.log(`added cardToCardAccount group w/ Id=${response.id}`)),
            catchError(this.handleError<CardToCardAccountGroup>('addCardToCardAccountGroup'))
        );
    }

    get(id: number): Observable<CardToCardAccountGroup> {
        return this.http.get<CardToCardAccountGroup>(this.baseUrl + id).pipe(
            tap((response) => this.log(`get cardToCardAccount group w/ Id=${id}`)),
            catchError(this.handleError<CardToCardAccountGroup>('getCardToCardAccount'))
        );
    }

    update(id: number, cardToCardAccount: CardToCardAccountGroup): Observable<CardToCardAccountGroup> {
        return this.http.put<CardToCardAccountGroup>(this.baseUrl + id, cardToCardAccount, this.httpOptions).pipe(
            tap((response) => this.log(`update cardToCardAccountGroup w/ Id=${response.id}`)),
            catchError(this.handleError<CardToCardAccountGroup>('updateCardToCardAccountGroup'))
        );
    }

    delete(id: number): Observable<any> {
        return this.http.delete(this.baseUrl + id, this.httpOptions).pipe(
            tap((response) => this.log(`delete cardToCardAccountGroup`)),
            catchError(this.handleError<CardToCardAccountGroup>('deleteCardToCardAccountGroup'))
        );
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {


            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }

    private log(message: string): void {
        console.log('CardToCardAccountGroupService: ' + message);
    }
}
