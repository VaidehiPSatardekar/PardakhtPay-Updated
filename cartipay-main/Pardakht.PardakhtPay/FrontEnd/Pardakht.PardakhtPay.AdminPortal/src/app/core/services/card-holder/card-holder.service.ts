import { Injectable } from '@angular/core';
import { IHttpOptions, ListSearchResponse } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { CardHolder } from '../../../models/card-holder';

@Injectable({
  providedIn: 'root'
})
export class CardHolderService {
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
            const thisCaller: CardHolderService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/cardholdername/';
        });
    }

    search(cardNumber: string): Observable<CardHolder> {
        return this.http.get<CardHolder>(this.baseUrl + 'getbycardnumber/' + cardNumber).pipe(
            map(response => response)
        );
    }
}
