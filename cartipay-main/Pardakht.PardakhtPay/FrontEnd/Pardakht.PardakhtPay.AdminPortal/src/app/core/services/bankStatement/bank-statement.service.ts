import { Injectable } from '@angular/core';
import { IHttpOptions, ListSearchResponse } from '../../../models/types';
import { HttpHeaders, HttpClient } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { Observable } from 'rxjs';
import { BankStatementItem, BankStatementItemSearchArgs } from '../../../models/bank-statement-item';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class BankStatementService {
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
            const thisCaller: BankStatementService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/statement/';
        });
    }

    search(args: BankStatementItemSearchArgs): Observable<ListSearchResponse<BankStatementItem[]>> {
        return this.http.post<ListSearchResponse<BankStatementItem[]>>(this.baseUrl + 'search', args).pipe(
            map(response => response)
        );
    }
}