import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IHttpOptions, ListSearchResponse } from '../../../models/types';
import { IEnvironment } from 'app/core/environment/environment.model'
import { MerchantCustomer, MerchantCustomerSearchArgs, MerchantRelation, MerchantCustomerCardNumbers, CustomerPhoneNumbers } from '../../../models/merchant-customer';
import { Observable } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';
import { DownloadPhoneNumbers } from '../../actions/merchantCustomer';
import { RegisteredPhoneNumbers } from '../../../models/user-segment-group';

@Injectable({
  providedIn: 'root'
})
export class MerchantCustomerService {
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
            const thisCaller: MerchantCustomerService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/merchantcustomer/';
        });
    }

    get(id: number): Observable<MerchantCustomer> {
        return this.http.get<MerchantCustomer>(this.baseUrl + id).pipe(
            tap((response) => this.log(`get merchant customer w/ Id=${id}`)),
            catchError(this.handleError<MerchantCustomer>('getMerchantCustomer'))
        );
    }

    search(args: MerchantCustomerSearchArgs): Observable<ListSearchResponse<MerchantCustomer[]>> {
        return this.http.post<ListSearchResponse<MerchantCustomer[]>>(this.baseUrl + 'search', args).pipe(
            map(response => response)
        );
    }

    getRelatedCustomers(id: number): Observable<MerchantRelation[]> {
        return this.http.post<MerchantRelation[]>(this.baseUrl + 'getRelatedCustomers/' + id, null).pipe(
            map(response => response)
        );
    }

    getCardNumbersCount(id: number): Observable<MerchantCustomerCardNumbers[]> {
        return this.http.post<MerchantCustomerCardNumbers[]>(this.baseUrl + 'getCardNumbersCount/' + id, null).pipe(
            map(response => response)
        );
    }

    updateUserSegmentGroup(id: number, merchantCustomer: MerchantCustomer): Observable<MerchantCustomer> {
        return this.http.post<MerchantCustomer>(this.baseUrl + 'updateusersegmentgroup/' + id, merchantCustomer, this.httpOptions).pipe(
            tap((response) => this.log(`update merchant customer user segment group w/ Id=${response.id}`)),
            catchError(this.handleError<MerchantCustomer>('updateMerchantCustomerUserSegmentGroup'))
        );
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {
            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }

    downloadphonenumbers(args: MerchantCustomerSearchArgs): Observable<CustomerPhoneNumbers> {
        let headers = new HttpHeaders({ 'Content-Type': 'application/json', 'Accept': 'application/csv' });
        let options: IHttpOptions = { headers: headers, responseType: 'blob' as 'json', observe: 'response' as 'body' };

        return this.http.post<Response>(this.baseUrl + 'exportphonenumbers', args, options).pipe(
            tap((response) => this.log(`get customer phone numbers w/`))
            ,
            map(response => {
                console.log(response);
                let d: CustomerPhoneNumbers = {
                    contentType: response.headers.get('Content-Type'),
                    data: response.body,
                    fileName: response.headers.get('File-Name')
                };

                return d;
            }),
            catchError(this.handleError<CustomerPhoneNumbers>('customerPhoneNumbers'))

            ////return this.http.post<CustomerPhoneNumbers>(this.baseUrl + 'search', args).pipe(
            ////    map(response => response)
            ////);
        );
    }

    getRegisteredPhones(id: number): Observable<RegisteredPhoneNumbers[]> {
        return this.http.post<RegisteredPhoneNumbers[]>(this.baseUrl + 'getregisteredphones/' + id, null).pipe(
            map(response => response)
        );
    }

    removeRegisteredPhones(id: number,registeredPhones: RegisteredPhoneNumbers[]): Observable<RegisteredPhoneNumbers> {
        debugger;
        return this.http.post<RegisteredPhoneNumbers[]>(this.baseUrl + 'removeregisteredphones/'+id, registeredPhones, this.httpOptions).pipe(
            tap((response) => this.log(`remove registered phones`)),
            catchError(this.handleError<any>('removeregisteredphone'))
        );
    }

    private log(message: string): void {
        console.log('MerchantCustomerService: ' + message);
    }
}
