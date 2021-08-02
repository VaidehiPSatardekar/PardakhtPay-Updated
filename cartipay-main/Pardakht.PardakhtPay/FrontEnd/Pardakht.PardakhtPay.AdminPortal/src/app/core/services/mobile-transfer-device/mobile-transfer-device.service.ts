import { Injectable } from '@angular/core';
import { IHttpOptions } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { MobileTransferDevice } from '../../../models/mobile-transfer';
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class MobileTransferDeviceService {
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
            const thisCaller: MobileTransferDeviceService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/mobiletransferdevice/';
        });
    }

    search(query: string): Observable<MobileTransferDevice[]> {
        return this.http.get<MobileTransferDevice[]>(this.baseUrl).pipe(
            map(response => response || [])
        );
    }

    getMobileTransferDevices(): Observable<MobileTransferDevice[]> {
        return this.http.get<MobileTransferDevice[]>(this.baseUrl).pipe(
            tap(() => this.log('get mobileTransferDevices')),
            catchError(this.handleError('getMobileTransferDevices', [])));
    }

    create(mobileTransferDevice: MobileTransferDevice): Observable<MobileTransferDevice> {
        return this.http.post<MobileTransferDevice>(this.baseUrl, mobileTransferDevice, this.httpOptions).pipe(
            tap((response) => this.log(`added mobileTransferDevice w/ Id=${response.id}`)),
            catchError(this.handleError<MobileTransferDevice>('addMobileTransferDevice'))
        );
    }

    get(id: number): Observable<MobileTransferDevice> {
        return this.http.get<MobileTransferDevice>(this.baseUrl + id).pipe(
            tap((response) => this.log(`get mobileTransferDevice w/ Id=${id}`)),
            catchError(this.handleError<MobileTransferDevice>('getMobileTransferDevice'))
        );
    }

    update(id: number, mobileTransferDevice: MobileTransferDevice): Observable<MobileTransferDevice> {
        return this.http.put<MobileTransferDevice>(this.baseUrl + id, mobileTransferDevice, this.httpOptions).pipe(
            tap((response) => this.log(`update mobileTransferDevice w/ Id=${response.id}`)),
            catchError(this.handleError<MobileTransferDevice>('updateMobileTransferDevice'))
        );
    }

    sendSms(id: number): Observable<MobileTransferDevice> {
        return this.http.post<MobileTransferDevice>(this.baseUrl + 'sendsms/' + id, null, this.httpOptions).pipe(
            tap((response) => this.log(`send sms mobileTransferDevice w/ Id=${response.id}`)),
            catchError(this.handleError<MobileTransferDevice>('sendSmsMobileTransferDevice'))
        );
    }

    activate(id: number, code: string): Observable<MobileTransferDevice> {
        return this.http.post<MobileTransferDevice>(this.baseUrl + 'activate/' + id, { verificationCode: code }, this.httpOptions).pipe(
            tap((response) => this.log(`activate mobileTransferDevice w/ Id=${response.id}`)),
            catchError(this.handleError<MobileTransferDevice>('activateMobileTransferDevice'))
        );
    }

    checkStatus(id: number): Observable<MobileTransferDevice> {
        return this.http.post<MobileTransferDevice>(this.baseUrl + 'checkstatus/' + id, null, this.httpOptions).pipe(
            tap((response) => this.log(`check status mobileTransferDevice w/ Id=${response.id}`)),
            catchError(this.handleError<MobileTransferDevice>('checkStatusMobileTransferDevice'))
        );
    }

    remove(id: number): Observable<MobileTransferDevice> {
        return this.http.post<MobileTransferDevice>(this.baseUrl + 'remove/' + id, null, this.httpOptions).pipe(
            tap((response) => this.log(`remove mobileTransferDevice w/ Id=${response.id}`)),
            catchError(this.handleError<MobileTransferDevice>('removeMobileTransferDevice'))
        );
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {


            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }

    private log(message: string): void {
        console.log('CardToCardService: ' + message);
    }
}

