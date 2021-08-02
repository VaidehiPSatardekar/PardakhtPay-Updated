import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Response } from '@angular/http/src/static_response';
import { Observable } from 'rxjs/Observable';
import { of } from 'rxjs/observable/of';
import { catchError } from 'rxjs/operators/catchError';
import { finalize } from 'rxjs/operators/finalize';
import { map } from 'rxjs/operators/map';
import { tap } from 'rxjs/operators/tap';

import { CloneRoleRequest, CloneRoleResponse, Permission, PermissionGroup, Role } from '../../models/role-management.model';
import { IHttpOptions } from '../../models/types';
import { EnvironmentService } from 'app/core/environment/environment.service';
import { BaseService } from './base.service';
import { IEnvironment } from '../environment/environment.model';

@Injectable()
export class RoleService extends BaseService {
  private baseUrl: string;

  constructor(private http: HttpClient, private environmentService: EnvironmentService) {
    super();

    environmentService.subscribe(this, (service: any, es: IEnvironment) => {
      const thisCaller: RoleService = service;
      thisCaller.baseUrl = es.serviceUrl + 'api/role/';
    });
  }

  getRoles(tenantId: number = 0): Observable<Role[]> {
    return this.http.get<Role[]>(this.baseUrl + tenantId, super.getHttpOptions()).pipe(
      map((response: Role[]) => response)
    );
  }

  getPermissions(): Observable<Permission[]> {
    return this.http.get<Permission[]>(this.baseUrl + 'get-permissions', super.getHttpOptions()).pipe(
      map((response: Permission[]) => response)
    );
  }

  getPermissionGroups(): Observable<PermissionGroup[]> {
    return this.http.get<PermissionGroup[]>(this.baseUrl + 'get-permission-groups', super.getHttpOptions()).pipe(
      map((response: PermissionGroup[]) => response)
    );
  }

  editRole(role: Role): Observable<Role> {
    return this.http.put<Role>(this.baseUrl, role).pipe(
      map((response: Role) => response)
    );
  }

  createRole(role: Role): Observable<Role> {
    return this.http.post<Role>(this.baseUrl, role).pipe(
      map((response: Role) => response)
    );
  }

  cloneRole(request: CloneRoleRequest): Observable<CloneRoleResponse> {
    return this.http.post<CloneRoleResponse>(this.baseUrl + 'clone-role', request).pipe(
      map((response: CloneRoleResponse) => response)
    );
  }
}
