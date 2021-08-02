import { Injectable, Injector } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router } from '@angular/router';
import { UserService } from 'app/core/services/user.service';

import { AccountService } from '../core/services/account.service';
import { PermissionGuard } from './permission.guard';

@Injectable()
export class PermissionRouteGuard extends PermissionGuard implements CanActivate {

    constructor(public router: Router, accountService: AccountService, userService: UserService) {
        super(router, accountService,userService);
    }

    canActivate(route: ActivatedRouteSnapshot): boolean {
        if (route.data) {
            const canNavigate: boolean = super.isAllowed(route.data.permissionCode);
            if (!canNavigate) {
                this.router.navigate(['/login']);
            }

            return canNavigate;
        }

        return false;
    }
}
