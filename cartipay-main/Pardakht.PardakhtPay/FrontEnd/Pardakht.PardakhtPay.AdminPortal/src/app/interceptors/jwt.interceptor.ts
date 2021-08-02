import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpResponse,
  HttpErrorResponse,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/do';
import { Store } from '@ngrx/store';
import * as coreState from '../core';
import * as accountActions from '../core/actions/account';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor(private router: Router, private store: Store<coreState.State>) { }

  // intercepts every request to see if there is a 401 - unauthorized
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request)
      .do((event: HttpEvent<any>) => {
        if (event instanceof HttpResponse) {
          // do stuff with response if you want
        }
      }, (err: any) => {
        if (err instanceof HttpErrorResponse) {
          if (err.status === 401) {
            this.store.dispatch(new accountActions.LoginExpired());
            // redirect to the login route
            this.router.navigate(['/login']);
          }
        }
      });
  }
}
