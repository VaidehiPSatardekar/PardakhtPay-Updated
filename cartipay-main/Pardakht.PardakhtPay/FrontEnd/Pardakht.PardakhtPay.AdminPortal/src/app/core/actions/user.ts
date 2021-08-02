// tslint:disable:typedef
// tslint:disable:max-classes-per-file
// tslint:disable:no-reserved-keywords

import { Action } from '@ngrx/store';
import * as signalR from '@aspnet/signalr';

import {
  ISuspension, StaffUser, ChangePasswordForm, BlockStaffUserRequest, DeleteStaffUserRequest, PasswordResetResponse, CreateStaffUserResponse,
  LoginAsStaffUserRequest, AddIdleMinutesRequest, StaffUserPerformanceTime
} from './../models/user-management.model';
import { LoginForm, LoginResponse } from './../models/user-management.model';
import { PlatformConfig } from '../models/platform.model';

export const INITIALIZE = '[UserAdminManagement] Initialize';
export const INITIALIZE_COMPLETE = '[UserAdminManagement] Initialize Complete';
export const INITIALIZE_ERROR = '[UserAdminManagement] Initialize Error';

export const LOGIN = '[UserAdminManagement] Login';
export const LOGIN_COMPLETE = '[UserAdminManagement] Login Complete';
export const LOGIN_ERROR = '[UserAdminManagement] Login Error';

export const LOGOUT = '[UserAdminManagement] Logout';
export const LOGOUT_COMPLETE = '[UserAdminManagement] Logout Complete';
export const LOGOUT_ERROR = '[UserAdminManagement] Logout Error';

export const LOGIN_EXPIRED = '[UserAdminManagement] Login Expired';

export const ADD_IDLE_TIME = '[UserAdminManagement] Add Idle Minutes';
export const ADD_IDLE_TIME_COMPLETE = '[UserAdminManagement] Add Idle Minutes Complete';
export const ADD_IDLE_TIME_ERROR = '[UserAdminManagement] Add Idle Minutes Error';

export const UPDATE_TRACKING_TIME = '[UserAdminManagement] Update Tracking Time';
export const UPDATE_TRACKING_TIME_COMPLETE = '[UserAdminManagement]  Update Tracking Time Complete';
export const UPDATE_TRACKING_TIME_ERROR = '[UserAdminManagement]  Update Tracking Time Error';

export const GET_STAFF_USERS = '[UserAdminManagement] Get Staff Users';
export const GET_STAFF_USERS_COMPLETE = '[UserAdminManagement] Get Staff Users Complete';
export const GET_STAFF_USERS_ERROR = '[UserAdminManagement] Get Staff Users Error';

export const CREATE_STAFF_USER = '[UserAdminManagement] Create Staff User';
export const CREATE_STAFF_USER_COMPLETE = '[UserAdminManagement] Create Staff User Complete';
export const CREATE_STAFF_USER_ERROR = '[UserAdminManagement] Create Staff User Error';

export const EDIT_STAFF_USER = '[UserAdminManagement] Edit Staff User';
export const EDIT_STAFF_USER_COMPLETE = '[UserAdminManagement] Edit Staff User Complete';
export const EDIT_STAFF_USER_ERROR = '[UserAdminManagement] Edit Staff User Error';

export const CHANGE_PASSWORD = '[UserAdminManagement] Change Password';
export const CHANGE_PASSWORD_COMPLETE = '[UserAdminManagement] Change Password Complete';
export const CHANGE_PASSWORD_ERROR = '[UserAdminManagement] Change Password Error';

export const CHANGE_PASSWORD_INIT = '[UserAdminManagement] Change Password Init';

export const FORGOT_PASSWORD = '[UserAdminManagement] Forgot Password';
export const FORGOT_PASSWORD_COMPLETE = '[UserAdminManagement] Forgot Password Complete';
export const FORGOT_PASSWORD_ERROR = '[UserAdminManagement] Forgot Password Error';

export const FORGOT_PASSWORD_BY_USERNAME = '[UserAdminManagement] Forgot Password By Username';
export const FORGOT_PASSWORD_BY_USERNAME_COMPLETE = '[UserAdminManagement] Forgot Password By Username Complete';
export const FORGOT_PASSWORD_BY_USERNAME_ERROR = '[UserAdminManagement] Forgot Password By Username Error';

export const RESET_PASSWORD = '[UserAdminManagement] Reset Password';
export const RESET_PASSWORD_COMPLETE = '[UserAdminManagement] Reset Password Complete';
export const RESET_PASSWORD_ERROR = '[UserAdminManagement] Reset Password Error';

export const BLOCK_STAFF_USER = '[UserAdminManagement] Block Staff User';
export const BLOCK_STAFF_USER_COMPLETE = '[UserAdminManagement] Block Staff User Complete';
export const BLOCK_STAFF_USER_ERROR = '[UserAdminManagement] Block Staff User Error';

export const DELETE_STAFF_USER = '[UserAdminManagement] Delete Staff User';
export const DELETE_STAFF_USER_COMPLETE = '[UserAdminManagement] Delete Staff User Complete';
export const DELETE_STAFF_USER_ERROR = '[UserAdminManagement] Delete Staff User Error';

export const LOGIN_AS_STAFF_USER = '[UserAdminManagement] Login As Staff User';
export const LOGIN_AS_STAFF_USER_COMPLETE = '[UserAdminManagement] Login As Staff User Complete';
export const LOGIN_AS_STAFF_USER_ERROR = '[UserAdminManagement] Login As Staff User Error';

export const GET_AFFILIATE_USERS = '[UserAdminManagement] Get Affiliate Users';
export const GET_AFFILIATE_USERS_COMPLETE = '[UserAdminManagement] Get Affiliate Users Complete';
export const GET_AFFILIATE_USERS_ERROR = '[UserAdminManagement] Get Affiliate Users Error';

export const GET_SYSTEM_USERS = '[UserAdminManagement] Get System Users';
export const GET_SYSTEM_USERS_COMPLETE = '[UserAdminManagement] Get System Users Complete';
export const GET_SYSTEM_USERS_ERROR = '[UserAdminManagement] Get System Users Error';

export const SET_PLATFORM = '[UserAdminManagement] Set Platform';

export const CLEAR_ERRORS = '[UserAdminManagement] Clear Errors';

/**
 * Every action is comprised of at least a type and an optional
 * payload. Expressing actions as classes enables powerful
 * type checking in reducer functions.
 *
 * See Discriminated Unions: https://www.typescriptlang.org/docs/handbook/advanced-types.html#discriminated-unions
 */

export class Initialize implements Action {
  readonly type = INITIALIZE;

  constructor() { }
}

export class InitializeComplete implements Action {
  readonly type = INITIALIZE_COMPLETE;

  constructor(public payload: StaffUser) { }
}

export class InitializeError implements Action {
  readonly type = INITIALIZE_ERROR;

  constructor(public payload: string) { }
}

export class Login implements Action {
  readonly type = LOGIN;

  constructor(public payload: LoginForm) { }
}

export class LoginComplete implements Action {
  readonly type = LOGIN_COMPLETE;

  constructor(public payload: LoginResponse) { }
}

export class LoginError implements Action {
  readonly type = LOGIN_ERROR;

  constructor(public payload: string) { }
}

export class Logout implements Action {
  readonly type = LOGOUT;
}

export class LogoutComplete implements Action {
  readonly type = LOGOUT_COMPLETE;
}

export class LogoutError implements Action {
  readonly type = LOGOUT_ERROR;

  constructor(public payload: string) { }
}

export class LoginExpired implements Action {
  readonly type = LOGIN_EXPIRED;
}


export class AddIdleTime implements Action {
  readonly type = ADD_IDLE_TIME;

  constructor(public addIdleMinutesRequest: AddIdleMinutesRequest) { }
}

export class AddIdleTimeComplete implements Action {
  readonly type = ADD_IDLE_TIME_COMPLETE;

  constructor(public payload: StaffUserPerformanceTime) { }
}

export class AddIdleTimeError implements Action {
  readonly type = ADD_IDLE_TIME_ERROR;

  constructor(public payload: string | any | null) { }
}

export class UpdateTrackingTime implements Action {
  readonly type = UPDATE_TRACKING_TIME;

  constructor() { }
}

export class UpdateTrackingTimeComplete implements Action {
  readonly type = UPDATE_TRACKING_TIME_COMPLETE;

  constructor(public payload: StaffUserPerformanceTime) { }
}

export class UpdateTrackingTimeError implements Action {
  readonly type = UPDATE_TRACKING_TIME_ERROR;

  constructor(public payload: string | any | null) { }
}


export class GetStaffUsers implements Action {
  readonly type = GET_STAFF_USERS;

  constructor(public tenantGuid: string | undefined, public brandId: number) { }
}

export class GetStaffUsersComplete implements Action {
  readonly type = GET_STAFF_USERS_COMPLETE;

  constructor(public payload: StaffUser[]) { }
}

export class GetStaffUsersError implements Action {
  readonly type = GET_STAFF_USERS_ERROR;

  constructor(public payload: string) { }
}

export class CreateStaffUser implements Action {
  readonly type = CREATE_STAFF_USER;

  constructor(public payload: StaffUser) { }
}

export class CreateStaffUserComplete implements Action {
  readonly type = CREATE_STAFF_USER_COMPLETE;

  constructor(public payload: CreateStaffUserResponse) { }
}

export class CreateStaffUserError implements Action {
  readonly type = CREATE_STAFF_USER_ERROR;

  constructor(public payload: string) { }
}

export class EditStaffUser implements Action {
  readonly type = EDIT_STAFF_USER;

  constructor(public payload: StaffUser) { }
}

export class EditStaffUserComplete implements Action {
  readonly type = EDIT_STAFF_USER_COMPLETE;

  constructor(public payload: StaffUser) { }
}

export class EditStaffUserError implements Action {
  readonly type = EDIT_STAFF_USER_ERROR;

  constructor(public payload: string) { }
}

export class ChangePassword implements Action {
  readonly type = CHANGE_PASSWORD;

  constructor(public payload: ChangePasswordForm) { }
}

export class ChangePasswordComplete implements Action {
  readonly type = CHANGE_PASSWORD_COMPLETE;

  constructor(public payload: boolean) { }
}

export class ChangePasswordError implements Action {
  readonly type = CHANGE_PASSWORD_ERROR;

  constructor(public payload: string) { }
}

export class ChangePasswordInit implements Action {
  readonly type = CHANGE_PASSWORD_INIT;
}

export class ForgotPassword implements Action {
  readonly type = FORGOT_PASSWORD;

  constructor(public email: string) { }
}

export class ForgotPasswordComplete implements Action {
  readonly type = FORGOT_PASSWORD_COMPLETE;
}

export class ForgotPasswordError implements Action {
  readonly type = FORGOT_PASSWORD_ERROR;

  constructor(public payload: string) { }
}

export class ForgotPasswordByUsername implements Action {
  readonly type = FORGOT_PASSWORD_BY_USERNAME;

  constructor(public username: string) { }
}

export class ForgotPasswordByUsernameComplete implements Action {
  readonly type = FORGOT_PASSWORD_BY_USERNAME_COMPLETE;
}

export class ForgotPasswordByUsernameError implements Action {
  readonly type = FORGOT_PASSWORD_BY_USERNAME_ERROR;

  constructor(public payload: string) { }
}

export class ResetPassword implements Action {
  readonly type = RESET_PASSWORD;

  constructor(public payload: string) { }
}

export class ResetPasswordComplete implements Action {
  readonly type = RESET_PASSWORD_COMPLETE;

  constructor(public payload: PasswordResetResponse) { }
}

export class ResetPasswordError implements Action {
  readonly type = RESET_PASSWORD_ERROR;

  constructor(public payload: string) { }
}

export class BlockStaffUser implements Action {
  readonly type = BLOCK_STAFF_USER;

  constructor(public payload: BlockStaffUserRequest) { }
}

export class BlockStaffUserComplete implements Action {
  readonly type = BLOCK_STAFF_USER_COMPLETE;

  constructor(public payload: StaffUser) { }
}

export class BlockStaffUserError implements Action {
  readonly type = BLOCK_STAFF_USER_ERROR;

  constructor(public payload: string) { }
}

export class DeleteStaffUser implements Action {
  readonly type = DELETE_STAFF_USER;

  constructor(public payload: DeleteStaffUserRequest) { }
}

export class DeleteStaffUserComplete implements Action {
  readonly type = DELETE_STAFF_USER_COMPLETE;

  constructor(public payload: StaffUser) { }
}

export class DeleteStaffUserError implements Action {
  readonly type = DELETE_STAFF_USER_ERROR;

  constructor(public payload: string) { }
}

export class LoginAsStaffUser implements Action {
  readonly type = LOGIN_AS_STAFF_USER;

  constructor(public payload: LoginAsStaffUserRequest) { }
}

export class LoginAsStaffUserComplete implements Action {
  readonly type = LOGIN_AS_STAFF_USER_COMPLETE;

  constructor(public payload: LoginResponse) { }
}

export class LoginAsStaffUserError implements Action {
  readonly type = LOGIN_AS_STAFF_USER_ERROR;

  constructor(public payload: string) { }
}

export class SetPlatform implements Action {
  readonly type = SET_PLATFORM;

  constructor(public payload: PlatformConfig) { }
}

export class GetAffiliateUsers implements Action {
  readonly type = GET_AFFILIATE_USERS;

  constructor(public tenantGuid: string | undefined) { }
}

export class GetAffiliateUsersComplete implements Action {
  readonly type = GET_AFFILIATE_USERS_COMPLETE;

  constructor(public payload: StaffUser[]) { }
}

export class GetAffiliateUsersError implements Action {
  readonly type = GET_AFFILIATE_USERS_ERROR;

  constructor(public payload: string) { }
}


export class GetSystemUsers implements Action {
  readonly type = GET_SYSTEM_USERS;

  constructor(public tenantGuid: string | undefined) { }
}

export class GetSystemUsersComplete implements Action {
  readonly type = GET_SYSTEM_USERS_COMPLETE;

  constructor(public payload: StaffUser[]) { }
}

export class GetSystemUsersError implements Action {
  readonly type = GET_SYSTEM_USERS_ERROR;

  constructor(public payload: string) { }
}
export class ClearErrors implements Action {
  readonly type = CLEAR_ERRORS;
}

/**
 * Export a type alias of all actions in this action group
 * so that reducers can easily compose action types
 */
export type Actions = ClearErrors | Initialize | InitializeComplete | InitializeError
  | Login | LoginComplete | LoginError | Logout | LogoutComplete | LogoutError | LoginExpired
  | AddIdleTime | AddIdleTimeComplete | AddIdleTimeError
  | UpdateTrackingTime | UpdateTrackingTimeComplete | UpdateTrackingTimeError
  | LoginAsStaffUser | LoginAsStaffUserComplete | LoginAsStaffUserError
  | CreateStaffUser | CreateStaffUserComplete | CreateStaffUserError
  | EditStaffUser | EditStaffUserComplete | EditStaffUserError | GetStaffUsers | GetStaffUsersComplete | GetStaffUsersError
  | ChangePassword | ChangePasswordComplete | ChangePasswordError | ChangePasswordInit
  | ForgotPassword | ForgotPasswordComplete | ForgotPasswordError
  | ForgotPasswordByUsername | ForgotPasswordByUsernameComplete | ForgotPasswordByUsernameError
  | ResetPassword | ResetPasswordComplete | ResetPasswordError
  | BlockStaffUser | BlockStaffUserComplete | BlockStaffUserError
  | DeleteStaffUser | DeleteStaffUserComplete | DeleteStaffUserError
  | SetPlatform | GetAffiliateUsers | GetAffiliateUsersComplete | GetAffiliateUsersError
  | GetSystemUsers | GetSystemUsersComplete | GetSystemUsersError;

