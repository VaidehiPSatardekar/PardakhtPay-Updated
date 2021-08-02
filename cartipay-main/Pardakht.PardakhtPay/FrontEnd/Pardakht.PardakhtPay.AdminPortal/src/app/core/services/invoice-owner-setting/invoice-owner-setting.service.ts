import { Injectable } from '@angular/core';
import { IHttpOptions } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { InvoiceOwnerSetting } from 'app/models/invoice';

@Injectable({
  providedIn: 'root'
})
export class InvoiceOwnerSettingService {
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
            const thisCaller: InvoiceOwnerSettingService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/financialinvoiceownersetting/';
        });
    }

    getAll(): Observable<InvoiceOwnerSetting[]> {
        return this.http.get<InvoiceOwnerSetting[]>(this.baseUrl).pipe(
            tap(() => this.log('get invoice owner settings')),
            catchError(this.handleError('getInvoiceOwnerSettings', [])));
    }

    create(setting: InvoiceOwnerSetting): Observable<InvoiceOwnerSetting> {
        return this.http.post<InvoiceOwnerSetting>(this.baseUrl, setting, this.httpOptions).pipe(
            tap((response) => this.log(`added invoice owner setting w/ Id=${response.id}`)),
            catchError(this.handleError<InvoiceOwnerSetting>('addInvoiceOwnerSetting'))
        );
    }

    get(id: number): Observable<InvoiceOwnerSetting> {
        return this.http.get<InvoiceOwnerSetting>(this.baseUrl + id).pipe(
            tap((response) => this.log(`get invoice owner setting w/ Id=${id}`)),
            catchError(this.handleError<InvoiceOwnerSetting>('getInvoiceOwnerSetting'))
        );
    }

    update(id: number, setting: InvoiceOwnerSetting): Observable<InvoiceOwnerSetting> {
        return this.http.put<InvoiceOwnerSetting>(this.baseUrl + id, setting, this.httpOptions).pipe(
            tap((response) => this.log(`update invoice owner setting w/ Id=${response.id}`)),
            catchError(this.handleError<InvoiceOwnerSetting>('updateInvoiceOwnerSetting'))
        );
    }

    delete(id: number): Observable<any> {
        return this.http.delete(this.baseUrl + id, this.httpOptions).pipe(
            tap((response) => this.log(`delete invoice owner setting`)),
            catchError(this.handleError<InvoiceOwnerSetting>('deleteInvoiceOwnerSetting'))
        );
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {


            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }

    private log(message: string): void {
        console.log('InvoiceOwnerSettingService: ' + message);
    }
}