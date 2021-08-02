import { Injectable } from '@angular/core';
import { CanActivate, Router, CanActivateChild, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AccountService } from '../core/services/account.service';
import { Observable } from 'rxjs/Observable';
import { CanLoad } from '@angular/router/src/interfaces';
import { Route } from '@angular/compiler/src/core';
import { FuseConfigService } from '../../@fuse/services/config.service';

@Injectable()
export class AuthGuard implements CanActivate  {

  constructor(private router: Router, private accountService: AccountService, private fuseConfig: FuseConfigService) { }

  canActivate(): boolean {
    const isLoggedOn: boolean = this.accountService.isAuthenticated();
    if (!isLoggedOn) {
      // workaround to issue where if user tries to navigate via the address bar the guard re-directs to login page but the nav still appears
      // this.fuseConfig.config({
      //   layout: {
      //     navigation: 'none',
      //     toolbar: 'none',
      //     footer: 'none'
      //   }
      // });
      this.router.navigate(['/login']);
    }

    return isLoggedOn;
  }

}
