import { Injectable } from '@angular/core';
import { IHttpOptions } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { ListSearchResponse } from 'app/models/types';
import { IEnvironment } from 'app/core/environment/environment.model'
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { Invoice, InvoiceSearchArgs } from 'app/models/invoice';

@Injectable({
  providedIn: 'root'
})
export class InvoiceService {
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
        const thisCaller: InvoiceService = service;
        thisCaller.baseUrl = es.serviceUrl + 'api/financialinvoice/';
    });
}

search(args: InvoiceSearchArgs): Observable<ListSearchResponse<Invoice[]>> {
    return this.http.post<ListSearchResponse<Invoice[]>>(this.baseUrl + 'search', args).pipe();
}

get(id: number): Observable<Invoice> {
    return this.http.get<Invoice>(this.baseUrl + id).pipe(
        tap((response) => this.log(`get invoice w/ Id=${id}`)),
        catchError(this.handleError<Invoice>('getInvoiceOwnerSetting'))
    );
}

private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
    return (error: any): Observable<T> => {


        this.log(`${operation} failed: ${error.message}`);

        throw error;
    };
}

private log(message: string): void {
    console.log('InvoiceService: ' + message);
}
}