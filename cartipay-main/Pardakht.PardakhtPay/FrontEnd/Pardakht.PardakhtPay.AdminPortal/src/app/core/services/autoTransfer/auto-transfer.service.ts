import { Injectable } from '@angular/core';
import { IHttpOptions, ListSearchResponse } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { AutoTransfer, AutoTransferSearchArgs } from '../../../models/autotransfer';

@Injectable({
  providedIn: 'root'
})
export class AutoTransferService {
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
            const thisCaller: AutoTransferService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/autotransfer/';
        });
    }

    search(args: AutoTransferSearchArgs): Observable<ListSearchResponse<AutoTransfer[]>> {
        return this.http.post<ListSearchResponse<AutoTransfer[]>>(this.baseUrl + 'search', args).pipe(
            map(response => response)
        );
    }
}
