// tslint:disable:typedef
// tslint:disable:max-classes-per-file
// tslint:disable:no-reserved-keywords
import { Action } from '@ngrx/store';

import { CloneRoleRequest, CloneRoleResponse, Permission, PermissionGroup, Role } from './../models/user-management.model';

export const GET_ALL_ROLES = '[RoleAdminManagement] Get All Roles';
export const GET_ALL_ROLES_COMPLETE = '[RoleAdminManagement] Get All Roles Complete';
export const GET_ALL_ROLES_ERROR = '[RoleAdminManagement] Get All Roles Error';

export const GET_PERMISSIONS = '[RoleAdminManagement] Get Permissions';
export const GET_PERMISSIONS_COMPLETE = '[RoleAdminManagement] Get Permissions Complete';
export const GET_PERMISSIONS_ERROR = '[RoleAdminManagement] Get Permissions Error';

export const GET_PERMISSION_GROUPS = '[RoleAdminManagement] Get Permission Groups';
export const GET_PERMISSION_GROUPS_COMPLETE = '[RoleAdminManagement] Get Permission Groups Complete';
export const GET_PERMISSION_GROUPS_ERROR = '[RoleAdminManagement] Get Permission Groups Error';

export const CREATE_ROLE = '[RoleAdminManagement] Create Role';
export const CREATE_ROLE_COMPLETE = '[RoleAdminManagement] Create Role Complete';
export const CREATE_ROLE_ERROR = '[RoleAdminManagement] Create Role Error';

export const EDIT_ROLE = '[RoleAdminManagement] Edit Role';
export const EDIT_ROLE_COMPLETE = '[RoleAdminManagement] Edit Role Complete';
export const EDIT_ROLE_ERROR = '[RoleAdminManagement] Edit Role Error';

export const CLONE_ROLE = '[RoleAdminManagement] Clone Role';
export const CLONE_ROLE_COMPLETE = '[RoleAdminManagement] Clone Role Complete';
export const CLONE_ROLE_ERROR = '[RoleAdminManagement] Clone Role Error';

export const CLEAR_ERRORS = '[RoleAdminManagement] Clear Errors';

export class GetAllRoles implements Action {
  readonly type = GET_ALL_ROLES;

  constructor(public tenantGuid: string | undefined) { }
}

export class GetAllRolesComplete implements Action {
  readonly type = GET_ALL_ROLES_COMPLETE;

  constructor(public payload: Role[]) { }
}

export class GetAllRolesError implements Action {
  readonly type = GET_ALL_ROLES_ERROR;

  constructor(public payload: string) { }
}

export class GetPermissions implements Action {
  readonly type = GET_PERMISSIONS;
}

export class GetPermissionsComplete implements Action {
  readonly type = GET_PERMISSIONS_COMPLETE;

  constructor(public payload: Permission[]) { }
}

export class GetPermissionsError implements Action {
  readonly type = GET_PERMISSIONS_ERROR;

  constructor(public payload: string) { }
}

export class GetPermissionGroups implements Action {
  readonly type = GET_PERMISSION_GROUPS;
}

export class GetPermissionGroupsComplete implements Action {
  readonly type = GET_PERMISSION_GROUPS_COMPLETE;

  constructor(public payload: PermissionGroup[]) { }
}

export class GetPermissionGroupsError implements Action {
  readonly type = GET_PERMISSION_GROUPS_ERROR;

  constructor(public payload: string) { }
}

export class CreateRole implements Action {
  readonly type = CREATE_ROLE;

  constructor(public payload: Role) {}
}

export class CreateRoleComplete implements Action {
  readonly type = CREATE_ROLE_COMPLETE;

  constructor(public payload: Role) {}
}

export class CreateRoleError implements Action {
  readonly type = CREATE_ROLE_ERROR;

  constructor(public payload: string) {}
}

export class EditRole implements Action {
  readonly type = EDIT_ROLE;

  constructor(public payload: Role) {}
}

export class EditRoleComplete implements Action {
  readonly type = EDIT_ROLE_COMPLETE;

  constructor(public payload: Role) {}
}

export class EditRoleError implements Action {
  readonly type = EDIT_ROLE_ERROR;

  constructor(public payload: string) {}
}

export class CloneRole implements Action {
  readonly type = CLONE_ROLE;

  constructor(public payload: CloneRoleRequest) {}
}

export class CloneRoleComplete implements Action {
  readonly type = CLONE_ROLE_COMPLETE;

  constructor(public payload: CloneRoleResponse) {}
}

export class CloneRoleError implements Action {
  readonly type = CLONE_ROLE_ERROR;

  constructor(public payload: string) {}
}

export class ClearErrors implements Action {
  readonly type = CLEAR_ERRORS;
}

// /**
//  * Export a type alias of all actions in this action group
//  * so that reducers can easily compose action types
//  */
export type Actions = GetAllRoles | GetAllRolesComplete | GetAllRolesError | GetPermissions | GetPermissionsComplete | GetPermissionsError |
      GetPermissionGroups | GetPermissionGroupsComplete | GetPermissionGroupsError | CloneRole | CloneRoleComplete | CloneRoleError |
      CreateRole | CreateRoleComplete | CreateRoleError | EditRole | EditRoleComplete | EditRoleError | ClearErrors;
