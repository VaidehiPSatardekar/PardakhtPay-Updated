import { GenericHelper } from '../../helpers/generic';

export class CreateStaffUserResponse {
  staffUser: StaffUser;
  password: string;
}

export class StaffUser {
  id: number;
  brandId: number;
  username: string;
  accountId: string;
  email: string;
  firstName: string;
  lastName: string;
  tenantGuid: string;
  parentAccountId: string;
  isBlocked: boolean;
  platformRoleMappings: StaffUserPlatformRoleContainer[];
  userType: UserType;
  tenantId: number;
  constructor(data?: any) {
    GenericHelper.populateData(this, data);
  }
}

export enum UserType {
  standard = 0,
  api = 1,
  affiliate = 2
}

export class StaffUserPlatformRoleContainer {
  platformGuid: string;
  roles: number[];
}

export interface ISuspension {
  id: number;
  reason: string;
  createdByAccountId: string;
  user: string;
  start: Date;
  end?: Date;
  created: Date;
  active: boolean;
}

export class UserSuspensionForm {
  reason: string;
  time: number;
  unitOfTime: 'hours' | 'days' | 'weeks' | 'months';
  block: boolean;

  constructor(data?: any) {
    GenericHelper.populateData(this, data);
  }
}

export class Role {
  id: number;
  name: string;
  permissions: Permission[];
  users: StaffUser[];
  roleHolderTypeId: string;
  isFixed: boolean;
  tenantGuid: string;
  isSelected: boolean;
  platformGuid: string;

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

export class CloneRoleRequest {
  role: Role;
  moveUsersToNewRole: boolean;
  tenantId: number;

  constructor(data?: any) {
    GenericHelper.populateData(this, data);
  }
}

export class CloneRoleResponse {
  originalRole: Role;
  newRole: Role;
}

export class StaffEditForm {
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  tenantId: number;

  constructor(data?: any) {
    GenericHelper.populateData(this, data);
  }
}

export interface ChangePasswordForm {
  newPassword: string;
  oldPassword: string;
  name: string;
  newPasswordConfirm: string;
}

export class BlockStaffUserRequest {
  constructor(public userId: number, public block: boolean) { }
}

export class DeleteStaffUserRequest {
  constructor(public userId: number) { }
}

export class LoginResponse {
  user: StaffUser;
  token: JsonWebToken;
}

export class JsonWebToken {
  accessToken: string;
  refreshToken: string;
  expires: Date | string;
}

// export class LoginResponse {
//   user: StaffUser;
//   accessToken: string;
//   refreshToken: string;
//   expires: Date | string;
// }

export class LoginForm {
  username: string;
  password: string;
  baseUrl: string;

  constructor(private captchaToken: string, data?: any) {
    GenericHelper.populateData(this, data);
  }
}

export class LoginAsStaffUserRequest {
  userName: string;
}

export class ResetPasswordForm {
  accountId: string;
  token: string;
  password: string;
}

export class PasswordResetResponse {
  newPassword: string;
  message: string;
}

export class AddIdleMinutesRequest {
  addIdleMinutes: number;
}

export class StaffUserPerformanceTime {
  idleMinutes: number;
  activeMinutes: number;
  logonMinutes: number;
}
