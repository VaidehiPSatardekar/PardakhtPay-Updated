
import { GenericHelper } from '../helpers/generic';
import { User } from './user-management.model';

export class Role {
  id: number;
  name: string;
  permissions: Permission[];
  users: User[];
  roleHolderTypeId: string;
  isFixed: boolean;
  tenantId: number;
  isSelected: boolean;

  constructor(data?: any) {
    GenericHelper.populateData(this, data);
  }
}

export class Permission {
  id: number;
  name: string;
  groupName: string;
  code: string;
  isRestricted: boolean;
  isSelected: boolean;
}

export class PermissionGroup {
  id: number;
  name: string;
  permissions: Permission[];
}

// tslint:disable-next-line:max-classes-per-file
export class CloneRoleRequest {
  role: Role;
  moveUsersToNewRole: boolean;
  tenantId: number;

  constructor(data?: any) {
    GenericHelper.populateData(this, data);
  }
}

// tslint:disable-next-line:max-classes-per-file
export class CloneRoleResponse {
  originalRole: Role;
  newRole: Role;
}
