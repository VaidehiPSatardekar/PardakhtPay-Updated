import { HttpClient } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { catchError } from 'rxjs/operators/catchError';
import { finalize } from 'rxjs/operators/finalize';
import { tap } from 'rxjs/operators/tap';
import { from } from 'rxjs/observable/from';
import { Store } from '@ngrx/store';
import { map } from 'rxjs/operators';
import { BaseService } from '../services/base.service';
import { EnvironmentService } from '../environment/environment.service';
import { IEnvironment } from '../environment/environment.model';
import { JwtHelper } from '../../helpers/jwtHelper';
import {
  ISuspension, Role, CloneRoleRequest, CloneRoleResponse, PermissionGroup, Permission, StaffUser, ChangePasswordForm, BlockStaffUserRequest,
  DeleteStaffUserRequest, LoginForm, LoginResponse, JsonWebToken, PasswordResetResponse, CreateStaffUserResponse, LoginAsStaffUserRequest,
  AddIdleMinutesRequest, StaffUserPerformanceTime
} from './../models/user-management.model';
import * as userActions from './../actions/user';
import * as coreState from './../index';

@Injectable({
  providedIn: 'root'
})
export class UserService extends BaseService {
  private baseUrl: string;
  private platformGuid: string;
  private parentAccountEnabled: boolean;

  private jwtHelper: JwtHelper = new JwtHelper();

  loadAndSetEnvironment(): void {
    this.environmentService.subscribe(this, (service: any, es: IEnvironment) => {
      this.baseUrl = es.serviceUrl;
      this.platformGuid = es.platformConfig.platformGuid;
      this.store.dispatch(new userActions.SetPlatform(es.platformConfig));
    });
  }

  constructor(private http: HttpClient, private store: Store<coreState.State>,
    private environmentService: EnvironmentService) {
    super();
    this.loadAndSetEnvironment();
  }

  logIn(model: LoginForm): Observable<LoginResponse> {
    const url = this.baseUrl + 'api/staffuser/staff-user-login';

    return this.http.post<LoginResponse>(url, model, super.getHttpOptions()).pipe(
      tap((response: LoginResponse) => {
        // console.log(response);
        localStorage.setItem('token', response.token.accessToken);
        // localStorage.setItem('refreshToken', response.refreshToken);
        // const tokenPayload: any = this.jwtHelper.decodeToken(response.token.accessToken);
      }),
      catchError(super.handleError<LoginResponse>('login'))
    );
  }

  logInAsStaffUser(model: LoginAsStaffUserRequest): Observable<LoginResponse> {

    const url = this.baseUrl + 'api/staffuser/staff-user-login-as';

    return this.http.post<LoginResponse>(url, model, super.getHttpOptions()).pipe(
      tap((response: LoginResponse) => {
        // console.log(response);
        localStorage.setItem('token', response.token.accessToken);
        // window.location.reload();
      }),
      catchError(super.handleError<LoginResponse>('login'))
    );
  }

  refreshToken(): Observable<JsonWebToken> {
    const url = this.baseUrl + 'api/staffuser/refresh-token';
    console.log('service - refreshing token');
    return this.http.get<JsonWebToken>(url, super.getHttpOptions()).pipe(
      tap((response: JsonWebToken) => {
        localStorage.setItem('token', response.accessToken);
        // localStorage.setItem('refreshToken', response.refreshToken);
        // const tokenPayload: any = this.jwtHelper.decodeToken(response.accessToken);
      }),
      catchError(super.handleError<JsonWebToken>('login'))
    );
  }

  getMinutesUntilTokenExpiry(): number {
    const token = this.getToken();
    if (token !== null) {
      const expiryDate = this.jwtHelper.getTokenExpirationDate(token);
      const now = new Date();
      const diffMs = (expiryDate.getTime() - now.getTime());
      const diffMins = Math.round(((diffMs % 86400000) % 3600000) / 60000); // minutes
      // console.log(expiryDate);
      // console.log(diffMs);
      return diffMins;
    }

    return 0;
  }

  logOut(): Observable<void> {
    const url = this.baseUrl + 'api/staffuser/staff-user-logout';
    return this.http.get<void>(url, super.getHttpOptions()).pipe(
      catchError(super.handleError<void>('logOut'))
    );
  }

  addIdleTime(addIdleMinutesRequest: AddIdleMinutesRequest): Observable<StaffUserPerformanceTime> {

    const putUrl = `${this.baseUrl}api/staffuser/addIdleMinutes`;

    return this.http.put<any>(putUrl, addIdleMinutesRequest, super.getHttpOptions()).pipe(
      catchError(this.handleError<string>('addIdleMinutes'))
    );
  }

  updateTrackingTime(): Observable<StaffUserPerformanceTime> {

    const putUrl = `${this.baseUrl}api/staffuser/updateTrackingTime`;

    return this.http.put<any>(putUrl, {}, super.getHttpOptions()).pipe(
      catchError(this.handleError<string>('updateTrackingTime'))
    );
  }

  getCurrentUser(): Observable<StaffUser> {
    return this.http.get<StaffUser>(this.baseUrl + 'api/staffuser');
  }

  editStaffUser(user: StaffUser): Observable<StaffUser> {
    return this.http.put<StaffUser>(this.baseUrl + 'api/staffuser', user, super.getHttpOptions()).pipe(
      tap((response: any) => console.log(`edited staff user w/ Id=${response.id}`)),
      catchError(super.handleError<StaffUser>('editStaffUser'))
    );
  }

  createStaffUser(user: StaffUser): Observable<CreateStaffUserResponse> {
    if (user.platformRoleMappings && user.platformRoleMappings.length === 1) {
      // temp whilst single-platform editing in use
      user.platformRoleMappings[0].platformGuid = this.platformGuid;
    }

    return this.http.post<CreateStaffUserResponse>(this.baseUrl + 'api/staffuser', user, super.getHttpOptions());
  }

  changePassword(model: ChangePasswordForm): Observable<boolean> {
    return this.http.post(this.baseUrl + 'api/staffuser/change-password', model, super.getHttpOptions()).pipe(
      map((response: any) => response === null),
      catchError(super.handleError<boolean>('changePassword'))
    );
  }

  // this is requesting a password reset on behalf of a staff user
  resetPassword(accountId: string): Observable<PasswordResetResponse> {
    const request = { accountId: accountId, platformGuid: this.platformGuid };

    return this.http.post<PasswordResetResponse>(this.baseUrl + 'api/staffuser/reset-password', request, super.getHttpOptions());
  }

  forgotPassword(email: string): Observable<boolean> {
    const request = { email: email, platformGuid: this.platformGuid };

    return this.http.post(this.baseUrl + 'api/staffuser/forgot-password', request, super.getHttpOptions()).pipe(
      map((response: any) => true)
    );
  }

  forgotPasswordByUsername(username: string): Observable<boolean> {
    const request = { username: username, platformGuid: this.platformGuid };

    return this.http.post(this.baseUrl + 'api/staffuser/forgot-password-by-username', request, super.getHttpOptions()).pipe(
      map((response: any) => true)
    );
  }

  blockStaffUser(request: BlockStaffUserRequest): Observable<StaffUser> {
    return this.http.post<StaffUser>(this.baseUrl + 'api/staffuser/block-staff-user', request, super.getHttpOptions());
  }

  deleteStaffUser(request: DeleteStaffUserRequest): Observable<StaffUser> {
    return this.http.post<StaffUser>(this.baseUrl + 'api/staffuser/delete-staff-user', request, super.getHttpOptions());
  }

  getStaffUsers(tenantGuid: string | undefined, brandId: number): Observable<StaffUser[]> {
    let url = this.baseUrl + 'api/staffuser/' + this.platformGuid;
    if (tenantGuid !== undefined) {
      url = url + '/' + tenantGuid;
    }
    if (brandId !== null) {
      url = url + '/' + brandId;
    }
    return this.http.get<StaffUser[]>(url);
  }

  getAffiliateUsers(tenantGuid: string | undefined): Observable<StaffUser[]> {
    let url = this.baseUrl + 'api/affiliateuser/' + this.platformGuid;
    if (tenantGuid !== undefined) {
      url = url + '/' + tenantGuid;
    }
    return this.http.get<StaffUser[]>(url);
  }

  getSystemUsers(tenantGuid: string | undefined): Observable<StaffUser[]> {
    let url = this.baseUrl + 'api/systemuser/' + this.platformGuid;
    if (tenantGuid !== undefined) {
      url = url + '/' + tenantGuid;
    }
    return this.http.get<StaffUser[]>(url);
  }

  getRoles(tenantGuid: string | undefined): Observable<Role[]> {
    let url = this.baseUrl + 'api/role/' + this.platformGuid;
    if (tenantGuid !== undefined) {
      url = url + '/' + tenantGuid;
    }
    return this.http.get<Role[]>(url);
  }

  getPermissions(): Observable<Permission[]> {
    return this.http.get<Permission[]>(this.baseUrl + 'api/role/get-permissions', super.getHttpOptions()).pipe(
      map((response: Permission[]) => response)
    );
  }

  getPermissionGroups(): Observable<PermissionGroup[]> {
    const url = this.baseUrl + 'api/role/get-permission-groups/' + this.platformGuid;
    return this.http.get<PermissionGroup[]>(url);
  }

  editRole(role: Role): Observable<Role> {
    role.platformGuid = this.platformGuid;
    return this.http.put<Role>(this.baseUrl + 'api/role', role);
  }

  createRole(role: Role): Observable<Role> {
    role.platformGuid = this.platformGuid;
    return this.http.post<Role>(this.baseUrl + 'api/role', role);
  }

  cloneRole(request: CloneRoleRequest): Observable<CloneRoleResponse> {
    return this.http.post<CloneRoleResponse>(this.baseUrl + 'api/role/clone-role', request).pipe(
      map((response: CloneRoleResponse) => response)
    );
  }

  getToken(): string {
    return localStorage.getItem('token');
  }

  getTenantGuid(): string {
    let token: string = this.getToken();
    const jwtHelper: JwtHelper = new JwtHelper();
    return jwtHelper.getTenantGuid(token);
  }

  isAuthenticated(): boolean {
    // return a boolean reflecting whether or not the token is expired
    const token: string = this.getToken();
    const jwtHelper: JwtHelper = new JwtHelper();

    return token != null && !jwtHelper.isTokenExpired(token);
  }
  isUserAuthorizedForTask(permissionCode: string): boolean {
    const token: string = this.getToken();
    if (token) {
      const tokenPayload: any = this.jwtHelper.decodeToken(token);
      if (tokenPayload.role && tokenPayload.role.some) {
        return tokenPayload.role.some((x: string) => x === permissionCode);
      }
    }

    return false;
  }
}
