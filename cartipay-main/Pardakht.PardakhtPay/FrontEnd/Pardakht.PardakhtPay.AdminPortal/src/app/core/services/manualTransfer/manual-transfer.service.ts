import { Injectable } from '@angular/core';
import { IHttpOptions, ListSearchResponse } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { ManualTransfer, ManualTransferSearchArgs, ManualTransferDetail } from '../../../models/manual-transfer';
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { WithdrawalReceipt } from '../../../models/withdrawal';

@Injectable({
  providedIn: 'root'
})
export class ManualTransferService {
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
            const thisCaller: ManualTransferService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/manualTransfer/';
        });
    }

    search(args: ManualTransferSearchArgs): Observable<ListSearchResponse<ManualTransfer[]>> {
        return this.http.post<ListSearchResponse<ManualTransfer[]>>(this.baseUrl + 'search', args).pipe(
            map(response => response)
        );
    }

    create(manualTransfer: ManualTransfer): Observable<ManualTransfer> {        
        return this.http.post<ManualTransfer>(this.baseUrl, manualTransfer, this.httpOptions).pipe(
            tap((response) => this.log(`added manualTransfer w/ Id=${response.id}`)),
            catchError(this.handleError<ManualTransfer>('addManualTransfer'))
        );
    }

    get(id: number): Observable<ManualTransfer> {
        return this.http.get<ManualTransfer>(this.baseUrl + id).pipe(
            tap((response) => this.log(`get manualTransfer w/ Id=${id}`)),
            catchError(this.handleError<ManualTransfer>('getManualTransfer'))
        );
    }

    cancel(id: number): Observable<ManualTransfer> {
        return this.http.put<ManualTransfer>(this.baseUrl + 'cancel/' + id, null).pipe(
            tap((response) => this.log(`cancel manualTransfer w/ Id=${id}`)),
            catchError(this.handleError<ManualTransfer>('cancelManualTransfer'))
        );
    }

    canceldetail(id: number): Observable<ManualTransferDetail> {
        return this.http.post<ManualTransferDetail>(this.baseUrl + 'canceldetail/' + id, null).pipe(
            tap((response) => this.log(`cancel manualTransfer detail w/ Id=${id}`)),
            catchError(this.handleError<ManualTransferDetail>('cancelManualTransferDetail'))
        );
    }

    retrydetail(id: number): Observable<ManualTransferDetail> {
        return this.http.post<ManualTransferDetail>(this.baseUrl + 'retrydetail/' + id, null).pipe(
            tap((response) => this.log(`retry manualTransfer detail w/ Id=${id}`)),
            catchError(this.handleError<ManualTransferDetail>('retryManualTransferDetail'))
        );
    }

    setascompleteddetail(id: number): Observable<ManualTransferDetail> {
        return this.http.post<ManualTransferDetail>(this.baseUrl + 'setascompleteddetail/' + id, null).pipe(
            tap((response) => this.log(`setascompleted manualTransfer detail w/ Id=${id}`)),
            catchError(this.handleError<ManualTransferDetail>('setascompletedManualTransferDetail'))
        );
    }

    update(id: number, manualTransfer: ManualTransfer): Observable<ManualTransfer> {
        return this.http.put<ManualTransfer>(this.baseUrl + id, manualTransfer, this.httpOptions).pipe(
            tap((response) => this.log(`update manualTransfer w/ Id=${response.id}`)),
            catchError(this.handleError<ManualTransfer>('addManualTransfer'))
        );
    }

    getReceipt(id: number): Observable<WithdrawalReceipt> {
        let headers = new HttpHeaders({ 'Content-Type': 'application/json', 'Accept': 'application/pdf' });

        let options: IHttpOptions = { headers: headers, responseType: 'blob' as 'json', observe: 'response' as 'body' };

        return this.http.post<Response>(this.baseUrl + 'gettransferreceipt/' + id, null, options).pipe(
            tap((response) => this.log(`get manual transfer receipt w/`))
            ,
            map(response => {
                console.log(response);
                let d: WithdrawalReceipt = {
                    contentType: response.headers.get('Content-Type'),
                    data: response.body,
                    fileName: response.headers.get('File-Name')
                };

                return d;
            }),
            catchError(this.handleError<WithdrawalReceipt>('getManualTransferReceipt'))
        );
    }

    delete(id: number): Observable<any> {
        return this.http.delete(this.baseUrl + id, this.httpOptions).pipe(
            tap((response) => this.log(`delete manualTransfer`)),
            catchError(this.handleError<ManualTransfer>('deleteManualTransfer'))
        );
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {


            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }

    private log(message: string): void {
        console.log('ManualTransferService: ' + message);
    }
}