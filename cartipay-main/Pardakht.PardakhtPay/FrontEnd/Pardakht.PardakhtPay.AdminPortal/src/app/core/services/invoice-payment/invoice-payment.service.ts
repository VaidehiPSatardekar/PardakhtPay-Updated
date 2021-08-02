import { Injectable } from '@angular/core';
import { IHttpOptions, ListSearchResponse } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { InvoicePayment, InvoicePaymentSearchArgs } from 'app/models/invoice';

@Injectable({
  providedIn: 'root'
})
export class InvoicePaymentService {
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
          const thisCaller: InvoicePaymentService = service;
          thisCaller.baseUrl = es.serviceUrl + 'api/invoicepayment/';
      });
  }

  search(args: InvoicePaymentSearchArgs): Observable<ListSearchResponse<InvoicePayment[]>> {
      return this.http.post<ListSearchResponse<InvoicePayment[]>>(this.baseUrl + 'search', args).pipe()
  }

  create(setting: InvoicePayment): Observable<InvoicePayment> {
      return this.http.post<InvoicePayment>(this.baseUrl, setting, this.httpOptions).pipe(
          tap((response) => this.log(`added invoice payment w/ Id=${response.id}`)),
          catchError(this.handleError<InvoicePayment>('addInvoicePayment'))
      );
  }

  get(id: number): Observable<InvoicePayment> {
      return this.http.get<InvoicePayment>(this.baseUrl + id).pipe(
          tap((response) => this.log(`get invoice payment w/ Id=${id}`)),
          catchError(this.handleError<InvoicePayment>('getInvoicePayment'))
      );
  }

  update(id: number, setting: InvoicePayment): Observable<InvoicePayment> {
      return this.http.put<InvoicePayment>(this.baseUrl + id, setting, this.httpOptions).pipe(
          tap((response) => this.log(`update invoice payment w/ Id=${response.id}`)),
          catchError(this.handleError<InvoicePayment>('updateInvoicePayment'))
      );
  }

  delete(id: number): Observable<any> {
      return this.http.delete(this.baseUrl + id, this.httpOptions).pipe(
          tap((response) => this.log(`delete invoice payment`)),
          catchError(this.handleError<InvoicePayment>('deleteInvoicePayment'))
      );
  }

  private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
      return (error: any): Observable<T> => {


          this.log(`${operation} failed: ${error.message}`);

          throw error;
      };
  }

  private log(message: string): void {
      console.log('InvoicePaymentService: ' + message);
  }
}