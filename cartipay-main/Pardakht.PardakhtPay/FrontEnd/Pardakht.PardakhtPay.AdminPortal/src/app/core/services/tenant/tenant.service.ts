import { Injectable } from '@angular/core';
import { TenantCreate, Tenant } from '../../../models/tenant';
import { Observable, Subject, ReplaySubject, of } from 'rxjs';
import { IHttpOptions, ListSearchResponse } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { tap, catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class TenantService {
    private tenantServiceUrl: string;
    private userServiceUrl: string;
    private baseUrl: string;
    private httpOptions: IHttpOptions = {};
    private platformId: string;

    constructor(private http: HttpClient, private environmentService: EnvironmentService) {
        const headers: HttpHeaders = new HttpHeaders();
        headers.append('Accept', 'application/json');
        headers.append('Content-Type', 'application/json');

        this.httpOptions = {
            headers: headers
        };

        environmentService.subscribe(this, (service: any, es: IEnvironment) => {
            const thisCaller: TenantService = service;
            //thisCaller.tenantServiceUrl = es.tenantServiceUrl + 'api/TenantDomainPlatformMap/';
            this.userServiceUrl = es.serviceUrl;
            thisCaller.baseUrl = es.serviceUrl + 'api/tenant/';
            thisCaller.platformId = es.platformGuid;
        });
    }

    search(): Observable<Tenant[]> {
        // return this.http.get<Tenant[]>(this.tenantServiceUrl + 'get-by-platformId/' + this.platformId).pipe(
        //    map(response => response)
        // );
        // return this.http.get<Tenant[]>(this.baseUrl + 'gettenants').pipe(
        //     map(response => response)
        // );

        var returnValue = new Tenant();
        returnValue.id = 1;
        returnValue.domainGuid = '1';
        returnValue.domainName = 'abc.com';
        returnValue.tenantDomainPlatformMapGuid = '1';
        returnValue.tenantGuid = '1';
        returnValue.tenantName = '1';
        returnValue.tenantStatus = 1;
        return of([returnValue]);
    }

    get(payload: number): any {
        throw new Error("Method not implemented.");
    }

    create(payload: TenantCreate): Observable<Tenant> {
        //var subject = new ReplaySubject<Tenant>();
        //this.http.post<Tenant>(this.tenantServiceUrl, payload).subscribe((data) => {
        //    var userData = {
        //        tenantGuid: data.tenantGuid,
        //        username: payload.tenantAdminUsername,
        //        password: payload.tenantAdminPassword,
        //        email: payload.tenantAdminEmail,
        //        firstName: payload.tenantAdminFirstName,
        //        lastName: payload.tenantAdminLastName,
        //        roleGuid: []
        //    };
        //    this.http.post(this.userServiceUrl + 'api/user', userData).subscribe((response) => {
        //        subject.next(data);
        //        subject.complete();
        //    });
        //});

        //return subject;

        return this.http.post<Tenant>(this.baseUrl, payload, this.httpOptions).pipe(
            tap((response) => this.log(`added tenant w/ Id=${response.id}`)),
            catchError(this.handleError<Tenant>('addTenant'))
        );
    }

    getCurrentTenant(): string {
        //console.log('selected tenant');
        return localStorage.getItem('selectedTenantGuid');
    }

    setCurrentTenant(guid: string): boolean {
        localStorage.setItem('selectedTenantGuid', guid);
        return true;
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {


            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }

    private log(message: string): void {
        console.log('Tenant Service: ' + message);
    }
}
