import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { TranslateService } from '@ngx-translate/core';

@Injectable()
export class LocaleInterceptor implements HttpInterceptor {

  constructor(private translateService: TranslateService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (this.translateService && this.translateService.currentLang) {
      const localeId = this.translateService.currentLang;
      request = request.clone({
        headers: request.headers.set('locale', localeId)
      });
    }

    // console.log(request.headers);

    return next.handle(request);
  }
}
