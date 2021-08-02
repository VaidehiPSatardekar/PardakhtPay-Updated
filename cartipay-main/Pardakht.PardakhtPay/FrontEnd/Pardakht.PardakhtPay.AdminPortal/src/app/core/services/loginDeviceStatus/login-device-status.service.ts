import { Injectable } from '@angular/core';
import { IHttpOptions, ListSearchResponse } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { BankLogin, BankAccount, CreateLoginFromLoginRequestDTO, BankLoginUpdateDTO, QrCodeRegistrationRequest } from '../../../models/bank-login';
import { Bank } from '../../../models/bank';
import { BlockedCardDetail } from '../../../models/blocked-card-detail';

@Injectable({
  providedIn: 'root'
})
export class LoginDeviceStatusService {
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
            const thisCaller: LoginDeviceStatusService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/banklogin/';
        });
    }

    getOwnersLogins(): Observable<BankLogin[]> {
        return this.http.get<BankLogin[]>(this.baseUrl + 'getownerloginlist').pipe(
            tap(() => this.log('get owner bank logins')),
            catchError(this.handleError('getOwnerBankLogins', [])));
    }


    get(id: number): Observable<BankLogin> {
        return this.http.get<BankLogin>(this.baseUrl + id).pipe(
            tap((response) => this.log(`get bank login w/ Id=${id}`)),
            catchError(this.handleError<BankLogin>('getBankLogin'))
        );
    }

  

    showLoginDeviceStatus(id: string): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'showlogindevicestatus/' + id, null, this.httpOptions).pipe(
            tap((response) => this.log(`show login device status`)),
            catchError(this.handleError<any>('show login device status'))
        );
    }

    showLoginListDeviceStatus(): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'showloginListdevicestatus', null, this.httpOptions).pipe(
            tap((response) => this.log(`show login device status`)),
            catchError(this.handleError<any>('show login device status'))
        );
    }

   

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {


            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }
    
    private log(message: string): void {
        console.log('BankLoginService: ' + message);
    }
}