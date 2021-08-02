import { Injectable } from '@angular/core';
import { IHttpOptions, ListSearchResponse } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { BankLogin, BankAccount, CreateLoginFromLoginRequestDTO, BankLoginUpdateDTO } from '../../../models/bank-login';
import { Bank } from '../../../models/bank';

@Injectable({
  providedIn: 'root'
})
export class QRCodeRegisterService {
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
            const thisCaller: QRCodeRegisterService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/qrcoderegister/';
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

    
    updateLoginInformation(id : number, login: BankLoginUpdateDTO): Observable<boolean> {
        return this.http.post<boolean>(this.baseUrl + 'updatelogininformation/' + id, login, this.httpOptions).pipe(
            tap((response) => this.log(`update bank login information`)),
            catchError(this.handleError<boolean>('updateBankLoginInformation'))
        );
    }

    createLoginFromLoginRequest(login: CreateLoginFromLoginRequestDTO): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'createloginfromloginrequest', login, this.httpOptions).pipe(
            tap((response) => this.log(`create login from login request`)),
            catchError(this.handleError<any>('createloginfromloginrequest'))
        );
    }

   
    qrCodeRegister(id: number): Observable<any> {
        debugger;
        return this.http.post<any>(this.baseUrl + 'createqrregister/' + id, null, this.httpOptions).pipe(
            tap((response) => this.log(`qr code register login`)),
            catchError(this.handleError<any>('qr code register login'))
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