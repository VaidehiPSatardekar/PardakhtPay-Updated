import { Injectable } from '@angular/core';
import { IHttpOptions, ListSearchResponse } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { TransactionSearchArgs } from '../../../models/transaction';
import { DailyAccountingDTO, AccountingSearchArgs } from '../../../models/accounting';

@Injectable({
  providedIn: 'root'
})
export class AccountingService {
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
            const thisCaller: AccountingService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/accounting/';
        });
    }

    search(args: AccountingSearchArgs): Observable<DailyAccountingDTO[]> {
        //console.log('search accounting');
        return this.http.post<DailyAccountingDTO[]>(this.baseUrl + 'search', args).pipe(
            map(response => response)
        );
    }
}
