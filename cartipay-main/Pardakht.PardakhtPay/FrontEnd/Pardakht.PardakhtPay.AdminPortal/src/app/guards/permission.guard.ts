import { Injectable, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { StaffUser } from 'app/core/models/user-management.model';
import { UserService } from 'app/core/services/user.service';
import { Observable } from 'rxjs';

import { AccountService } from '../core/services/account.service';

@Injectable()
export class PermissionGuard {

    constructor(public router: Router, private accountService: AccountService, private userService: UserService) { }

    isAllowed(permissionCode: string): boolean {
      if (permissionCode) {
        // tslint:disable-next-line:no-unnecessary-local-variable
        const isAllowed: boolean = this.accountService.isUserAuthorizedForTask(permissionCode);
        // console.log('Checked permission ' + permissionCode + ' - allowed = ' + isAllowed);

        return isAllowed;
      }

      return false;
    }

    logout() {
      this.router.navigate(['/login']);
    }

    getCurrentUser(): Observable<StaffUser> {
      return this.userService.getCurrentUser();
    }
    
    isUserAuthorized(permission: string): boolean {
      return this.userService.isUserAuthorizedForTask(permission);
    }
  
}
