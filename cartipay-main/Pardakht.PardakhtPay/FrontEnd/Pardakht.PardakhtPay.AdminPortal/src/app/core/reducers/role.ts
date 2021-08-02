// tslint:disable-next-line:max-line-length
// tslint:disable:max-func-body-length
// tslint:disable:cyclomatic-complexity
import { Permission, PermissionGroup, Role } from '../models/user-management.model';
import * as roles from '../actions/role';

export interface State {
  roles: Role[];
  newRole: Role;
  permissions: Permission[];
  permissionGroups: PermissionGroup[];
  createSuccess: boolean;
  updateSuccess: boolean;
  loading: boolean;
  error?: { [key: string]: string };
}

const initialState: State = {
  roles: undefined,
  newRole: undefined,
  permissions: undefined,
  permissionGroups: undefined,
  createSuccess: undefined,
  updateSuccess: false,
  loading: false,
  error: undefined
};

export function reducer(state: State = initialState, action: roles.Actions): State {

  switch (action.type) {

    case roles.EDIT_ROLE: {
      return {
        ...state,
        updateSuccess: false,
        loading: true,
        error: undefined
      };
    }

    case roles.GET_PERMISSIONS:
    case roles.GET_PERMISSION_GROUPS:
    case roles.CREATE_ROLE:
    case roles.EDIT_ROLE:
    case roles.CLONE_ROLE:
    case roles.GET_ALL_ROLES: {
      return {
        ...state,
        loading: true,
        error: undefined
      };
    }

    case roles.GET_ALL_ROLES_COMPLETE:
      return {
        ...state,
        roles: action.payload,
        loading: false,
        error: undefined
      };

    case roles.GET_PERMISSION_GROUPS_ERROR:
    case roles.GET_PERMISSIONS_ERROR:
    case roles.GET_ALL_ROLES_ERROR:
      return {
        ...state,
        loading: false,
        error: {
          get: action.payload
        }
      };

    case roles.GET_PERMISSIONS_COMPLETE:
      return {
        ...state,
        permissions: action.payload,
        loading: false,
        error: undefined
      };

    case roles.GET_PERMISSION_GROUPS_COMPLETE:
      return {
        ...state,
        permissionGroups: action.payload,
        loading: false,
        error: undefined
      };

    case roles.CREATE_ROLE_COMPLETE:
      return {
        ...state,
        createSuccess: true,
        loading: false,
        error: undefined
      };

    case roles.EDIT_ROLE_COMPLETE: {
      const newRoles: Role[] = [
        ...state.roles
      ];
      newRoles.splice(newRoles.findIndex((q: Role) => q.id === action.payload.id), 1, action.payload);

      return {
        ...state,
        roles: newRoles,
        updateSuccess: true,
        loading: false,
        error: undefined
      };
    }

    case roles.CLONE_ROLE_COMPLETE:
      // replace the original role as it may have had users changed
      const newRoles2: Role[] = [
        ...state.roles
      ];
      newRoles2.splice(newRoles2.findIndex((q: Role) => q.id === action.payload.originalRole.id), 1, action.payload.originalRole);

      return {
        ...state,
        roles: [action.payload.newRole, ...newRoles2],
        loading: false,
        error: undefined
      };

    case roles.CREATE_ROLE_ERROR:
    case roles.EDIT_ROLE_ERROR:
    case roles.CLONE_ROLE_ERROR:
      return {
        ...state,
        loading: false,
        error: {
          edit: action.payload
        }
      };

    case roles.CLEAR_ERRORS:
      return {
        ...state,
        error: undefined
      };

    default: {
      return state;
    }
  }
}
