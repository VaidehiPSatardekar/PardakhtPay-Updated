import { Injectable } from '@angular/core';
import { IHttpOptions, ListSearchResponse } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { ResponseContentType } from '@angular/http/src/enums';
import { Response } from 'selenium-webdriver/http';

@Injectable({
  providedIn: 'root'
})
export class RiskyKeywordService {
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
            const thisCaller: RiskyKeywordService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/riskykeyword/';
        });
    }

    get(): Observable<string[]> {
        return this.http.get<string[]>(this.baseUrl).pipe(
            tap((response) => this.log(`get RiskyKeywords`)),
            catchError(this.handleError<string[]>('getRiskyKeywords'))
        );
    }

    update(item: string[]): Observable<string[]> {
        return this.http.put<string[]>(this.baseUrl, item, this.httpOptions).pipe(
            tap((response) => this.log(`update RiskyKeywords`)),
            catchError(this.handleError<string[]>('udpateRiskyKeywords'))
        );
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {


            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }

    private log(message: string): void {
        console.log('RiskyKeywordsService: ' + message);
    }
}
