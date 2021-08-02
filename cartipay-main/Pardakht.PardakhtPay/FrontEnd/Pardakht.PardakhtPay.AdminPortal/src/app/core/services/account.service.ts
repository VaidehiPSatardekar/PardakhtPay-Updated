import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable, Injector } from '@angular/core';

import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { of } from 'rxjs/observable/of';
import { catchError } from 'rxjs/operators/catchError';
import { finalize } from 'rxjs/operators/finalize';
import { map } from 'rxjs/operators/map';
import { tap } from 'rxjs/operators/tap';

import * as coreState from '../../core';
import * as roleActions from '../../core/actions/role';
import { JwtHelper } from '../../helpers/jwtHelper';
import { LoginForm, LoginResponse, ResetPasswordForm, ChangePasswordUser, Owner } from '../../models/account.model';
import { Permission } from '../../models/role-management.model';
import { IHttpOptions } from '../../models/types';
import { User } from '../../models/user-management.model';
import { IEnvironment } from 'app/core/environment/environment.model'
import { EnvironmentService } from 'app/core/environment/environment.service';
import { BaseService } from './base.service';
import { RoleService } from './role.service';
import { TenantService } from './tenant/tenant.service';

export const ProviderAdmin = 'CP-PROVIDER-ADMIN';
export const TenantAdmin = 'CP-TENANT-ADMIN';
export const SeeAllUsers = 'SEE-ALL-OWNERS';
export const StandardUser = 'StandardUser';

@Injectable()
export class AccountService extends BaseService {
    private baseUrl: string;
    private userServiceUrl: string;
    private allowAllTenants: boolean;
    private platformGuid: string;
    private jwtHelper: JwtHelper = new JwtHelper();
    
    // private permissions$: Observable<Permission[]>;
    //private permissions: Permission[];

    constructor(private http: HttpClient, private environmentService: EnvironmentService, private store: Store<coreState.State>, private tenantService : TenantService) {
        super();

        environmentService.subscribe(this, (service: any, es: IEnvironment) => {
            const thisCaller: AccountService = service;
            thisCaller.baseUrl = es.serviceUrl + 'api/';
            //thisCaller.userServiceUrl = es.userServiceUrl + 'api/';
            //thisCaller.allowAllTenants = es.allowAllTenants;
            thisCaller.platformGuid = es.platformGuid;
            // console.log('as - constructor');
            //this.permissions = environmentService.permissions;
        });
    }

    getCurrentUser(): Observable<User> {
        return this.http.get<User>(this.baseUrl + 'get-staff-user-account-details');
    }

    // this is requesting a password reset on behalf of a customer or staff user
    sendPasswordResetToken(accountId: string, tenantContext: string): Observable<boolean> {
        super.addTenantContextToHeaders(tenantContext);

        return this.http.post(this.baseUrl + 'reset-password/' + accountId, null, super.getHttpOptions()).pipe(
            map((response: any) => response === null),
            catchError(super.handleError<boolean>('sendPasswordResetToken')),
            finalize(() => super.resetHeaders())
        );
    }

    changePassword(model: ChangePasswordUser): Observable<boolean> {
        return this.http.post(this.baseUrl + 'account/change-password', model, super.getHttpOptions()).pipe(
            map((response: any) => response === null),
            catchError(super.handleError<boolean>('changePassword'))
        );
    }

    resetPassword(model: ResetPasswordForm): Observable<boolean> {
        return this.http.post(this.baseUrl + 'reset-password', model, super.getHttpOptions()).pipe(
            map((response: any) => response === null),
            catchError(super.handleError<boolean>('resetPassword'))
        );
    }

    logIn(model: LoginForm): Observable<LoginResponse> {
        return this.http.post<LoginResponse>(this.baseUrl + 'staffuser/staff-user-login', model, super.getHttpOptions()).pipe(
            tap((response: LoginResponse) => {
                localStorage.setItem('token', response.token.accessToken);
                localStorage.setItem('refreshToken', response.token.refreshToken);
            }),
            catchError(super.handleError<LoginResponse>('login'))
        );
    }

    getOwners(): Observable<Owner[]> {
        //if (!this.isUserProviderUser()) {
            return this.http.get<Owner[]>(this.baseUrl + 'StaffUser/' + this.platformGuid + '/' + this.tenantService.getCurrentTenant(), super.getHttpOptions()).pipe(
                catchError(super.handleError<Owner[]>('get staff user as provider'))
            );
        //} else {
        //    return this.http.get<Owner[]>(this.baseUrl + 'StaffUser/' + this.platformGuid, super.getHttpOptions()).pipe(
        //        catchError(super.handleError<Owner[]>('get staff user as user'))
        //    );
        //}
    }

    getOwnerDetail(id: number): Observable<Owner> {
        return this.http.get<Owner>(this.baseUrl + 'account/' + id, super.getHttpOptions()).pipe(
            catchError(super.handleError<Owner>('getOwnerDetail'))
        );
    }

    logOut(): void {
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
    }

    forgotPassword(model: LoginForm): Observable<boolean> {
        model.baseUrl = location.origin + '/';

        return this.http.post(this.baseUrl + 'forgot-password/', model, super.getHttpOptions()).pipe(
            map((response: any) => response === null),
            catchError(super.handleError<boolean>('forgotPassword'))
        );
    }

    createUser(model: Owner): Observable<Owner> {
        return this.http.post<Owner>(this.baseUrl + 'account', model).pipe(
            catchError(super.handleError<Owner>('create user')));
    }

    updateOwner(id: number, model: Owner): Observable<Owner> {
        return this.http.put<Owner>(this.baseUrl + 'account/' + id, model).pipe(
            catchError(super.handleError<Owner>('update user')));
    }

    getToken(): string {
        return localStorage.getItem('token');
    }

    isAuthenticated(): boolean {
        // return a boolean reflecting whether or not the token is expired
        const token: string = this.getToken();
        const jwtHelper: JwtHelper = new JwtHelper();
        console.log(token);
        return token != null && !jwtHelper.isTokenExpired(token);
    }

    tenantGuid(): string {

        //if (this.allowAllTenants == true || this.isUserProviderAdmin()) {
        //    return undefined;
        //}

        const token: string = this.getToken();

        if (token == null || token == '') {
            return undefined;
        }

        const jwtHelper: JwtHelper = new JwtHelper();

        return jwtHelper.getTenantGuid(token);
    }

    getParentAccountId(): string {

        //if (this.allowAllTenants == true || this.isUserProviderAdmin()) {
        //    return undefined;
        //}

        const token: string = this.getToken();

        if (token == null || token == '') {
            return undefined;
        }

        const jwtHelper: JwtHelper = new JwtHelper();

        return jwtHelper.getParentAccountId(token);
    }

    getAccountGuid(): string {

        const token: string = this.getToken();

        if (token == null || token == '') {
            return undefined;
        }

        const jwtHelper: JwtHelper = new JwtHelper();

        return jwtHelper.getAccountGuid(token);
    }

    getUsername(): string {
        const token: string = this.getToken();

        if (token == null || token == '') {
            return undefined;
        }

        const jwtHelper: JwtHelper = new JwtHelper();

        return jwtHelper.getUsername(token);
    }

    isUserAuthorizedForTask(permissionCode: string): boolean {
        const token: string = this.getToken();
        if (token) {
            const tokenPayload: any = this.jwtHelper.decodeToken(token);
            if (tokenPayload.role && tokenPayload.role.some) {
                return tokenPayload.role.some((x: string) => x === permissionCode);
            } else if (tokenPayload.role) {
                return tokenPayload.role === permissionCode;
            }
        }

        return false;
    }

    isUserProviderAdmin(): boolean {
        //return true;
        var pa = this.isUserAuthorizedForTask(ProviderAdmin);

        return pa;
    }

    isUserProviderUser(): boolean {
        var result = this.tenantGuid() == undefined;

        return result;
    }

    isUserTenantAdmin(): boolean {
        return this.isUserAuthorizedForTask(TenantAdmin);
    }

    isUserStandardUser(): boolean {
        return this.isUserAuthorizedForTask(StandardUser);
    }

    filterOwners(items: Owner[]): Owner[] {
        //items = items.filter(t => t.isApiKeyUser == false);
        if (this.isUserProviderUser() || this.isUserAuthorizedForTask(SeeAllUsers)) {
            return items;
        }

        if (this.isUserTenantAdmin()) {
            var tenantGuid = this.tenantGuid();

            return items;//.filter(t => t.tenantGuid == tenantGuid);
        }

        return items.filter(t => t.accountId == this.getAccountGuid() || t.accountId == this.getParentAccountId());
    }
}
