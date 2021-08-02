import { Inject, Injectable, InjectionToken, Optional } from '@angular/core';
import { Actions, Effect } from '@ngrx/effects';
import { Action } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { empty } from 'rxjs/observable/empty';
import { of } from 'rxjs/observable/of';
import { catchError } from 'rxjs/operators';
import { map } from 'rxjs/operators/map';
import { mergeMap } from 'rxjs/operators/mergeMap';
import { skip } from 'rxjs/operators/skip';
import { switchMap } from 'rxjs/operators/switchMap';
import { takeUntil } from 'rxjs/operators/takeUntil';
import { async } from 'rxjs/scheduler/async';

import { NotificationType } from 'angular2-notifications';
import * as role from '../actions/role';
// import { RoleService } from '../services/role.service';
// import { CloneRoleResponse, Permission, PermissionGroup, Role } from './../../models/role-management.model';
import { UserService } from './../services/user.service';
import { Role, Permission, PermissionGroup } from '../models/user-management.model';
import { EffectBase } from './effect-base';
import { AddOne, NotificationMessage } from '../actions/notification';
// import * as Notification from '../actions/notification';


// export const SEARCH_DEBOUNCE = new InjectionToken<number>('Search Debounce');
// export const SEARCH_SCHEDULER = new InjectionToken<Scheduler>('Search Scheduler');
/**
 * Effects offer a way to isolate and easily test side-effects within your
 * application.
 *
 * If you are unfamiliar with the operators being used in these examples, please
 * check out the sources below:
 *
 * Official Docs: http://reactivex.io/rxjs/manual/overview.html#categories-of-operators
 * RxJS 5 Operators By Example: https://gist.github.com/btroncone/d6cf141d6f2c00dc6b35
 */

@Injectable()
export class RoleEffects extends EffectBase {

  @Effect()
  getRole$: Observable<Action> = this.actions$.ofType<role.GetAllRoles>(role.GET_ALL_ROLES).pipe(
    switchMap((action: role.GetAllRoles) => {
      if (action === undefined) {
        return empty();
      }

      return this.userService.getRoles(action.tenantGuid).pipe(
        map((response: Role[]) => new role.GetAllRolesComplete(response)),
        catchError((err: any) => of(new role.GetAllRolesError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  getPermissions$: Observable<Action> = this.actions$.ofType<role.GetPermissions>(role.GET_PERMISSIONS).pipe(
    switchMap((action: role.GetPermissions) => {
      if (action === undefined) {
        return empty();
      }

      return this.userService.getPermissions().pipe(
        map((response: Permission[]) => new role.GetPermissionsComplete(response)),
        catchError((err: any) => of(new role.GetPermissionsError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  getPermissionGroups$: Observable<Action> = this.actions$.ofType<role.GetPermissionGroups>(role.GET_PERMISSION_GROUPS).pipe(
    switchMap((action: role.GetPermissionGroups) => {
      if (action === undefined) {
        return empty();
      }

      return this.userService.getPermissionGroups().pipe(
        map((response: PermissionGroup[]) => new role.GetPermissionGroupsComplete(response)),
        catchError((err: any) => of(new role.GetPermissionGroupsError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  edit$: Observable<Action> = this.actions$.ofType<role.EditRole>(role.EDIT_ROLE).pipe(
    switchMap((action: role.EditRole) => {
      if (action === undefined || action.payload === undefined) {
        return empty();
      }

      return this.userService.editRole(action.payload).pipe(
        mergeMap((response: Role) => [
          new role.EditRoleComplete(response),
          new AddOne(new NotificationMessage('Role ' + response.name + ' has been updated'))
        ]),
        catchError((err: any) => of(new role.EditRoleError(this.sanitiseError(err))))
      );
    })
  );

  @Effect()
  create$: Observable<Action> = this.actions$.ofType<role.CreateRole>(role.CREATE_ROLE).pipe(
    switchMap((action: role.CreateRole) => {
      if (action === undefined || action.payload === undefined) {
        return empty();
      }

      return this.userService.createRole(action.payload).pipe(
        mergeMap((response: Role) => [
          new role.CreateRoleComplete(response),
          new AddOne(new NotificationMessage("Role '" + response.name + "' has been created."))
        ]),
        catchError((err: any) => of(new role.CreateRoleError(this.sanitiseError(err))))
      );
    })
  );

  // @Effect()
  // cloneRole$: Observable<Action> = this.actions$.ofType<role.CloneRole>(role.CLONE_ROLE).pipe(
  //   switchMap((action: role.CloneRole) => {
  //     if (action === undefined || action.payload === undefined) {
  //       return empty();
  //     }

  //     return this.roleService.cloneRole(action.payload).pipe(
  //       mergeMap((response: CloneRoleResponse) => [
  //         new role.CloneRoleComplete(response),
  //         new notification.AddOne(new notification.NotificationMessage("Role '" + response.newRole.name + "' has been created."))
  //       ]),
  //       catchError((err: any) => of(new role.CloneRoleError(this.sanitiseError(err))))
  //     );
  //   })
  // );

  constructor(private actions$: Actions, private userService: UserService) { super(); }
}
