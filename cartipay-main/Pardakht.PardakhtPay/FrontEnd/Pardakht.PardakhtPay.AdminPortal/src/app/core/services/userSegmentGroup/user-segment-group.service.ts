import { Injectable } from '@angular/core';
import { IHttpOptions } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { UserSegmentGroup } from '../../../models/user-segment-group';
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class UserSegmentGroupService {
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
            const thisCaller: UserSegmentGroupService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/userSegmentGroup/';
        });
    }

    search(query: string): Observable<UserSegmentGroup[]> {
        return this.http.get<UserSegmentGroup[]>(this.baseUrl).pipe(
            map(response => response || [])
        );
    }

    getItems(): Observable<UserSegmentGroup[]> {
        return this.http.get<UserSegmentGroup[]>(this.baseUrl).pipe(
            tap(() => this.log('get user segment Groups')),
            catchError(this.handleError('getUserSegmentGroups', [])));
    }

    create(cardToCardAccount: UserSegmentGroup): Observable<UserSegmentGroup> {
        return this.http.post<UserSegmentGroup>(this.baseUrl, cardToCardAccount, this.httpOptions).pipe(
            tap((response) => this.log(`added user segment group w/ Id=${response.id}`)),
            catchError(this.handleError<UserSegmentGroup>('addUserSegmentGroup'))
        );
    }

    get(id: number): Observable<UserSegmentGroup> {
        return this.http.get<UserSegmentGroup>(this.baseUrl + id).pipe(
            tap((response) => this.log(`get user segment group w/ Id=${id}`)),
            catchError(this.handleError<UserSegmentGroup>('getUserSegmentGroup'))
        );
    }

    update(id: number, cardToCardAccount: UserSegmentGroup): Observable<UserSegmentGroup> {
        return this.http.put<UserSegmentGroup>(this.baseUrl + id, cardToCardAccount, this.httpOptions).pipe(
            tap((response) => this.log(`update userSegmentGroup w/ Id=${response.id}`)),
            catchError(this.handleError<UserSegmentGroup>('updateUserSegmentGroup'))
        );
    }

    delete(id: number): Observable<any> {
        return this.http.delete(this.baseUrl + id, this.httpOptions).pipe(
            tap((response) => this.log(`delete userSegmentGroup`)),
            catchError(this.handleError<UserSegmentGroup>('deleteUserSegmentGroup'))
        );
    }

    private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
        return (error: any): Observable<T> => {


            this.log(`${operation} failed: ${error.message}`);

            throw error;
        };
    }

    private log(message: string): void {
        console.log('UserSegmentGroupService: ' + message);
    }
}
