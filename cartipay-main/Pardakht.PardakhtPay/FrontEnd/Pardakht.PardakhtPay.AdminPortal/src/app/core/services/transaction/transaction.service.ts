import { Injectable } from '@angular/core';
import { IHttpOptions, ListSearchResponse } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { TransactionSearch, TransactionSearchArgs } from '../../../models/transaction';

@Injectable({
  providedIn: 'root'
})
export class TransactionService {
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
            const thisCaller: TransactionService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/transaction/';
        });
    }

    search(args: TransactionSearchArgs): Observable<ListSearchResponse<TransactionSearch[]>> {
        return this.http.post<ListSearchResponse<TransactionSearch[]>>(this.baseUrl + 'search', args).pipe(
        );
    }

    setAsCompleted(id: number): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'setascompleted/' + id, null, this.httpOptions).pipe(
        );
    }

    setAsExpired(id: number): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'setasexpired/' + id, null, this.httpOptions).pipe(
        );
    }

    transactionCallbackToMerchant(id: number): Observable<string> {
        return this.http.post<string>(this.baseUrl + 'transactioncallbacktomerchant/' + id, null, this.httpOptions).pipe(            
        );
    }
}
