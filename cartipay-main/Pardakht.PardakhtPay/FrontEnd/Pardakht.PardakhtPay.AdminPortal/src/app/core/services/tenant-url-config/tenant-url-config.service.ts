import { Injectable } from '@angular/core';
import { IHttpOptions } from '../../../models/types';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { IEnvironment } from 'app/core/environment/environment.model'
import { TenantUrlConfig } from '../../../models/tenant-url-config';
import { Observable } from 'rxjs';
import { map, tap, catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class TenantUrlConfigService {
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
            const thisCaller: TenantUrlConfigService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/tenantconfiguration/';
        });
    }

    getAll(): Observable<TenantUrlConfig[]> {
        return this.http.get<TenantUrlConfig[]>(this.baseUrl).pipe(
            map(response => response || [])
        );
    }

    create(config: TenantUrlConfig): Observable<TenantUrlConfig> {
        return this.http.post<TenantUrlConfig>(this.baseUrl, config, this.httpOptions).pipe(
            tap((response) => this.log(`added tenant url config w/ Id=${response.id}`)),
            catchError(this.handleError<TenantUrlConfig>('addTenantUrlConfig'))
        );
    }

    get(id: number): Observable<TenantUrlConfig> {
        return this.http.get<TenantUrlConfig>(this.baseUrl + id).pipe(
            tap((response) => this.log(`get tenant url config w/ Id=${id}`)),
            catchError(this.handleError<TenantUrlConfig>('getTenantUrlConfig'))
        );
    }

    update(id: number, config: TenantUrlConfig): Observable<TenantUrlConfig> {
        return this.http.put<TenantUrlConfig>(this.baseUrl + id, config, this.httpOptions).pipe(
            tap((response) => this.log(`update tenant url config w/ Id=${response.id}`)),
            catchError(this.handleError<TenantUrlConfig>('addTenantUrlConfig'))
        );
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {


            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }

    private log(message: string): void {
        console.log('TenantUrlConfigService: ' + message);
    }
}
