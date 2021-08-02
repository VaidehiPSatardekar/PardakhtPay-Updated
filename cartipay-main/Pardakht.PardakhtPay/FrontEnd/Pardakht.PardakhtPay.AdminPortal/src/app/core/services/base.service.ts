import { HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { IHttpOptions } from '../../models/types';

export class BaseService {
  private localHttpOptions: IHttpOptions = {};

  getHttpOptions(): IHttpOptions { return this.localHttpOptions; }

  getHttpHeader(): any { return this.localHttpOptions.headers; }

  protected getStandardContentHeaders(): any {
    let headers: HttpHeaders = new HttpHeaders();
    // tslint:disable-next-line:no-backbone-get-set-outside-model
    headers = headers.set('Accept', 'application/json').set('Content-Type', 'application/json');

    return headers;
  }

  protected resetHeaders(): void {
    // reset headers
    const newHeaders: HttpHeaders = this.getStandardContentHeaders();
    this.localHttpOptions = { headers: newHeaders };
  }


  protected addTenantContextToHeaders(tenantContext: string): void {


    let headers: HttpHeaders = this.getStandardContentHeaders();
    if (tenantContext) {
      // tslint:disable-next-line:no-backbone-get-set-outside-model
      headers = headers.set('Account-Context', tenantContext);
      this.localHttpOptions = { headers: headers };
    }
  }

  protected addTenantContextToHeadersWithCustomHeaders(tenantContext: string, headers: HttpHeaders): void {
    if (tenantContext) {
      // tslint:disable-next-line:no-backbone-get-set-outside-model
      headers = headers.set('Account-Context', tenantContext);
      this.localHttpOptions = { headers: headers };
    }
  }



  protected addCustomHeaders(key: string, value: string): void {
    let _header = this.getHttpHeader();
    if (_header == undefined) {
      let headers: HttpHeaders = this.getStandardContentHeaders();
      headers = headers.append(key, value);
      this.localHttpOptions = { headers: headers };
    } else {
      _header = _header.append(key, value);
      this.localHttpOptions = { headers: _header };

    }
  }

  protected handleError<T>(operation: string = 'operation', result?: T): (error: any) => Observable<T> {
    return (error: any): Observable<T> => {
      throw error;
    };
  }
}
