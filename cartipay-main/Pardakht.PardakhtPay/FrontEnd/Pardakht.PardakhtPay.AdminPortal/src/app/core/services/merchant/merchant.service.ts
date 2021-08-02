import { Injectable } from '@angular/core';
import { IHttpOptions } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { Merchant, MerchantSummary, MerchantCreate, MerchantEdit } from '../../../models/merchant-model';
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MerchantService {
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
            const thisCaller: MerchantService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/merchant/';
        });
    }

    search(query: string): Observable<MerchantSummary[]> {
        return this.http.get<MerchantSummary[]>(this.baseUrl + 'search/' + query).pipe(
            map(response => response || [])
        );
    }

    getMerchants(): Observable<MerchantSummary[]> {
        return this.http.get<MerchantSummary[]>(this.baseUrl).pipe(
            tap(() => this.log('get merchants')),
            catchError(this.handleError('getMerchants', [])));
    }

    create(merchant: MerchantCreate): Observable<Merchant> {
        return this.http.post<Merchant>(this.baseUrl, merchant, this.httpOptions).pipe(
            tap((response) => this.log(`added merchant w/ Id=${response.id}`)),
            catchError(this.handleError<Merchant>('addMerchant'))
        );
    }

    get(id: number): Observable<MerchantEdit> {
        return this.http.get<MerchantEdit>(this.baseUrl + id).pipe(
            tap((response) => this.log(`get merchant w/ Id=${id}`)),
            catchError(this.handleError<MerchantEdit>('getMerchant'))
        );
    }

    update(id: number, merchant: MerchantEdit): Observable<Merchant> {
        return this.http.put<Merchant>(this.baseUrl + id, merchant, this.httpOptions).pipe(
            tap((response) => this.log(`update merchant w/ Id=${response.id}`)),
            catchError(this.handleError<Merchant>('addMerchant'))
        );
    }

    delete(id: number): Observable<any> {
        return this.http.delete(this.baseUrl + id, this.httpOptions).pipe(
            tap((response) => this.log(`delete merchant`)),
            catchError(this.handleError<Merchant>('deleteMerchant'))
        );
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {


            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }

    private log(message: string): void {
        console.log('MercantService: ' + message);
    }
}
