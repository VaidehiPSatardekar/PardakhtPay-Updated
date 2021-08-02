import { Injectable } from '@angular/core';
import { IHttpOptions, ListSearchResponse } from '../../../models/types';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { IEnvironment } from 'app/core/environment/environment.model'
import { Observable } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { ResponseContentType } from '@angular/http/src/enums';
import { Response } from 'selenium-webdriver/http';
import { OwnerSetting } from 'app/models/owner-setting';

@Injectable({
  providedIn: 'root'
})
export class OwnerSettingService {
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
          const thisCaller: OwnerSettingService = service;
          thisCaller.baseUrl = es.serviceUrl + 'api/ownersetting/';
      });
  }

  get(): Observable<OwnerSetting> {
      return this.http.get<OwnerSetting>(this.baseUrl).pipe(
          tap((response) => this.log(`get OwnerSetting`)),
          catchError(this.handleError<OwnerSetting>('ownersetting'))
      );
  }
  
  save(item: OwnerSetting): Observable<OwnerSetting> {
      return this.http.post<OwnerSetting>(this.baseUrl, item, this.httpOptions).pipe(
          tap((response) => this.log(`save OwnerSetting}`)),
          catchError(this.handleError<OwnerSetting>('saveOwnerSettings'))
      );
  }

  private handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
      return (error: any): Observable<T> => {


          this.log(`${operation} failed: ${error.message}`);

          throw error;
      };
  }

  private log(message: string): void {
      console.log('OwnerSettingsService: ' + message);
  }
}
