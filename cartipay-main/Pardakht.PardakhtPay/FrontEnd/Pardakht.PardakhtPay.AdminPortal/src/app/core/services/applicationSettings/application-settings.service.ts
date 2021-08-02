import { Injectable } from '@angular/core';
import { IHttpOptions, ListSearchResponse } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { ResponseContentType } from '@angular/http/src/enums';
import { Response } from 'selenium-webdriver/http';
import { ApplicationSettings, TransferStatusDescription } from '../../../models/application-settings';
@Injectable({
    providedIn: 'root'
})
export class ApplicationSettingsService {
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
            const thisCaller: ApplicationSettingsService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/applicationsettings/';
        });
    }

    get(): Observable<ApplicationSettings> {
        return this.http.get<ApplicationSettings>(this.baseUrl).pipe(
            tap((response) => this.log(`get ApplicationSettings`)),
            catchError(this.handleError<ApplicationSettings>('getApplicationSettings'))
        );
    }

    getTranfserStatuses(): Observable<TransferStatusDescription[]> {
        return this.http.get<TransferStatusDescription[]>(this.baseUrl + 'gettransferstatuslist').pipe(
            tap((response) => this.log(`get TransferStatusList`)),
            catchError(this.handleError<TransferStatusDescription[]>('getTransferStatusList'))
        );
    }

    update(item: ApplicationSettings): Observable<ApplicationSettings> {
        return this.http.put<ApplicationSettings>(this.baseUrl, item, this.httpOptions).pipe(
            tap((response) => this.log(`update ApplicationSettings}`)),
            catchError(this.handleError<ApplicationSettings>('udpateApplicationSettings'))
        );
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {


            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }

    private log(message: string): void {
        console.log('ApplicationSettingsService: ' + message);
    }
}
