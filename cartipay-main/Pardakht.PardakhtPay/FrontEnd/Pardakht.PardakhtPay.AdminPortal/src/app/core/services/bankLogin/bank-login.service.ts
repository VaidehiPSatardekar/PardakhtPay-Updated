import { Injectable } from '@angular/core';
import { IHttpOptions, ListSearchResponse } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { BankLogin, BankAccount, CreateLoginFromLoginRequestDTO, BankLoginUpdateDTO, QrCodeRegistrationRequest, RegisterLogin } from '../../../models/bank-login';
import { Bank } from '../../../models/bank';
import { BlockedCardDetail } from '../../../models/blocked-card-detail';

@Injectable({
  providedIn: 'root'
})
export class BankLoginService {
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
            const thisCaller: BankLoginService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/banklogin/';
        });
    }

    search(includeDeleteds: boolean): Observable<BankLogin[]> {
        return this.http.get<BankLogin[]>(this.baseUrl + 'getloginlist?includeDeleteds=' + includeDeleteds).pipe(
            tap(() => this.log('get bank logins')),
            catchError(this.handleError('getBankLogins', [])));
    }

    getOwnersLogins(): Observable<BankLogin[]> {
        return this.http.get<BankLogin[]>(this.baseUrl + 'getownerloginlist').pipe(
            tap(() => this.log('get owner bank logins')),
            catchError(this.handleError('getOwnerBankLogins', [])));
    }

    searchAccounts(includeDeleteds: boolean): Observable<BankAccount[]> {
        return this.http.get<BankAccount[]>(this.baseUrl + 'getaccountlist?includeDeleteds=' + includeDeleteds).pipe(
            tap(() => this.log('get bank accounts')),
            catchError(this.handleError('getBankAccounts', [])));
    }

    searchBanks(): Observable<Bank[]> {
        return this.http.get<Bank[]>(this.baseUrl + 'getbanks').pipe(
            tap(() => this.log('get banks')),
            catchError(this.handleError('getBanks', [])));
    }

    create(login: BankLogin): Observable<BankLogin> {
        return this.http.post<BankLogin>(this.baseUrl, login, this.httpOptions).pipe(
            tap((response) => this.log(`added bank login w/ Id=${response.id}`)),
            catchError(this.handleError<BankLogin>('addLogin'))
        );
    }

    get(id: number): Observable<BankLogin> {
        return this.http.get<BankLogin>(this.baseUrl + id).pipe(
            tap((response) => this.log(`get bank login w/ Id=${id}`)),
            catchError(this.handleError<BankLogin>('getBankLogin'))
        );
    }

    update(id: number, login: BankLogin): Observable<BankLogin> {
        return this.http.put<BankLogin>(this.baseUrl + id, login, this.httpOptions).pipe(
            tap((response) => this.log(`update bank login w/ Id=${response.id}`)),
            catchError(this.handleError<BankLogin>('updateBankLogin'))
        );
    }

    searchAccountsByLoginGuid(loginGuid: string): Observable<BankAccount[]> {
        return this.http.get<BankAccount[]>(this.baseUrl + 'getaccountlistbyloginguid/' + loginGuid).pipe(
            tap(() => this.log('get bank accounts by login guid')),
            catchError(this.handleError('getBankAccountsByLoginGuid', [])));
    }

    searchUnusedAccountsByLoginGuid(loginGuid: string): Observable<BankAccount[]> {
        return this.http.get<BankAccount[]>(this.baseUrl + 'getunusedaccountlistbyloginguid/' + loginGuid).pipe(
            tap(() => this.log('get bank accounts by login guid')),
            catchError(this.handleError('getBankAccountsByLoginGuid', [])));
    }

    updateLoginInformation(id: number, login: BankLoginUpdateDTO): Observable<boolean> {
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

    deactivateLogin(id: number): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'deactivatelogininformation/' + id, null, this.httpOptions).pipe(
            tap((response) => this.log(`deactivate login`)),
            catchError(this.handleError<any>('deactivate login'))
        );
    }

    activateLogin(id: number): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'activatelogininformation/' + id, null, this.httpOptions).pipe(
            tap((response) => this.log(`activate login`)),
            catchError(this.handleError<any>('activate login'))
        );
    }

    deleteLogin(id: number): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'deletelogininformation/' + id, null, this.httpOptions).pipe(
            tap((response) => this.log(`delete login information`)),
            catchError(this.handleError<any>('delete login information'))
        );
    }

    getBlockedCardDetails(accountGuid: string): Observable<BlockedCardDetail[]> {
        return this.http.get<BlockedCardDetail[]>(this.baseUrl + 'getblockedcarddetails/' + accountGuid).pipe(
            tap(() => this.log('get blocked card details')),
            catchError(this.handleError('getBlockedCardDetails', [])));
    }

    showPassword(id: number): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'showpassword/' + id, null, this.httpOptions).pipe(
            tap((response) => this.log(`show password`)),
            catchError(this.handleError<any>('show password'))
        );
    }

    qrCodeRegister(id: number): Observable<any> {        
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

    getQRRegistrationDetails(id: number): Observable<BankLogin> {
        return this.http.get<BankLogin>(this.baseUrl + 'getqrregistrationdetails/' + id).pipe(
            tap((response) => this.log(`get bank login w/ Id=${id}`)),
            catchError(this.handleError<BankLogin>('getBankLogin'))
        );
    }

    completeQrCodeRegistration(request: QrCodeRegistrationRequest) {
        return this.http.post<any>(this.baseUrl + 'registerqrcode', request, this.httpOptions).pipe(
            tap((response) => this.log(`qr code register login`)),
            catchError(this.handleError<any>('qr code register login'))
        );
    }

    getOTP(bankLoginId: number): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'getotp/' + bankLoginId, null, this.httpOptions).pipe(
            tap((response) => this.log(`get otp`)),
            catchError(this.handleError<any>('get otp'))
        );
    }

    registerLoginRequest(login: RegisterLogin): Observable<BankLogin> {        
        return this.http.post<RegisterLogin>(this.baseUrl + 'registerloginrequest', login, this.httpOptions).pipe(
            tap((response) => this.log(`register login request`)),
            catchError(this.handleError<any>('registerloginrequest'))
        );
    }

    switchBankConnectionProgram(id: number): Observable<any> {
        return this.http.post<any>(this.baseUrl + 'switchbankconnectionprogram/' + id, null, this.httpOptions).pipe(
            tap((response) => this.log(`switch bank connection program`)),
            catchError(this.handleError<any>('switch bank connection program'))
        );
    }
 
    private log(message: string): void {
        console.log('BankLoginService: ' + message);
    }
}