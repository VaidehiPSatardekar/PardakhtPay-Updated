import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { AccountService } from '../core/services/account.service';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {

  // we need to inject Injector as we can't inject AuthService as it will create a circular dependency
  constructor(private injector: Injector) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
      // console.log(request.headers);
    const accountService: AccountService = this.injector.get(AccountService);
    request = request.clone({
      // tslint:disable-next-line:no-backbone-get-set-outside-model
      headers: request.headers.set('Authorization', `Bearer ${accountService.getToken()}`)
    });
    // console.log(request.headers);

    return next.handle(request);
  }
}
