import { Injectable } from '@angular/core';
import { IHttpOptions } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { MerchantBankAccount } from '../../../models/merchant-model';
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MerchantBankAccountService {
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
            const thisCaller: MerchantBankAccountService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/merchantbankaccount/';
        });
    }

    search(merchantId: number): Observable<MerchantBankAccount[]> {
        return this.http.get<MerchantBankAccount[]>(this.baseUrl + 'getbymerchantid/' + merchantId).pipe(
            tap(() => this.log('get merchant accounts')),
            catchError(this.handleError('getMerchantAccounts', [])));
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {


            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }

    private log(message: string): void {
        console.log('MercantBankAccountService: ' + message);
    }
}
