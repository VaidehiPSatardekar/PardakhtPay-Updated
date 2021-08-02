import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { TenantService } from '../core/services/tenant/tenant.service';

@Injectable()
export class TenantInterceptor implements HttpInterceptor {

    // we need to inject Injector as we can't inject AuthService as it will create a circular dependency
    constructor(private injector: Injector) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        //console.log(request.headers);
        const tenantService: TenantService = this.injector.get(TenantService);

        var tenant = tenantService.getCurrentTenant();

        if (request.headers != undefined && request.headers != null && tenant != null && tenant != undefined) {
            request = request.clone({
                // tslint:disable-next-line:no-backbone-get-set-outside-model
                headers: request.headers.set('account-context', tenantService.getCurrentTenant())
            });
        }
        // console.log(request.headers);

        return next.handle(request);
    }
}