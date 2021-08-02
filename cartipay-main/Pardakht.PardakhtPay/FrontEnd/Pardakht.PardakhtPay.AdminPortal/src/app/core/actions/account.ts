// tslint:disable:typedef
// tslint:disable:max-classes-per-file
// tslint:disable:no-reserved-keywords

import { HttpErrorResponse } from '@angular/common/http/src/response';
import { Action } from '@ngrx/store';
import { LoginForm, LoginResponse, ResetPasswordForm, ChangePasswordUser, Owner } from '../../models/account.model';
import { ISuspension, UserSuspensionForm } from '../../models/suspension.model';
import { User } from '../../models/user-management.model';

export const LOGIN = '[Account] Login';
export const LOGIN_COMPLETE = '[Account] Login Complete';
export const LOGIN_ERROR = '[Account] Login Error';


export const CHANGE_PASSWORD = '[Account] Change Password';
export const CHANGE_PASSWORD_COMPLETE = '[Account] Change Password Complete';
export const CHANGE_PASSWORD_ERROR = '[Account] Change Password Error';


export const LOGOUT = '[Account] Logout';
export const LOGOUT_COMPLETE = '[Account] Logout Complete';
export const LOGIN_EXPIRED = '[Account] Login Expired';

export const SEND_PASSWORD_RESET_TOKEN = '[Account] Send Password Reset Token';
export const SEND_PASSWORD_RESET_TOKEN_COMPLETE = '[Account] Send Password Reset Token Complete';
export const SEND_PASSWORD_RESET_TOKEN_ERROR = '[Account] Send Password Reset Token Error';

export const RESET_PASSWORD = '[Account] Reset Password';
export const RESET_PASSWORD_COMPLETE = '[Account] Reset Password Complete';
export const RESET_PASSWORD_ERROR = '[Account] Reset Password Error';

export const INITIALIZE = '[Account] Initialize';
export const INITIALIZE_COMPLETE = '[Account] Initialize Complete';
export const INITIALIZE_ERROR = '[Account] Initialize Error';

export const FORGOT_PASSWORD = '[Account] Forgot Password';
export const FORGOT_PASSWORD_COMPLETE = '[Account] Forgot Password Complete';
export const FORGOT_PASSWORD_ERROR = '[Account] Forgot Password Error';

export const CLEAR_ERRORS = '[Account] Clear Errors';

export const GET_OWNERS = '[Account] Get Owners';
export const GET_OWNERS_COMPLETE = '[Account] Get Owners Complete';
export const GET_OWNERS_ERROR = '[Account] Get Owners Error';

export const CREATE_OWNER = '[Account] Create Owner';
export const CREATE_OWNER_COMPLETE = '[Account] Create Owner Complete';
export const CREATE_OWNER_ERROR = '[Account] Create Owner Error';

export const GET_OWNER_DETAIL = '[Account] Get Owner Detail';
export const GET_OWNER_DETAIL_COMPLETE = '[Account] Get Owner Detail Complete';
export const GET_OWNER_DETAIL_ERROR = '[Account] Get Owner Detail Error';

export const EDIT_OWNER = '[Account] Edit Owner';
export const EDIT_OWNER_COMPLETE = '[Account] Edit Owner Complete';
export const EDIT_OWNER_ERROR = '[Account] Edit Owner Error';

/**
 * Every action is comprised of at least a type and an optional
 * payload. Expressing actions as classes enables powerful
 * type checking in reducer functions.
 *
 * See Discriminated Unions: https://www.typescriptlang.org/docs/handbook/advanced-types.html#discriminated-unions
 */


export class ChangePassword implements Action {
    readonly type = CHANGE_PASSWORD;

    constructor(public payload: ChangePasswordUser) { }
}

export class ChangePasswordComplete implements Action {
    readonly type = CHANGE_PASSWORD_COMPLETE;

    constructor(public payload: boolean) { }
}

export class ChangePasswordError implements Action {
    readonly type = CHANGE_PASSWORD_ERROR;

    constructor(public payload: string) { }
}



export class Login implements Action {
    readonly type = LOGIN;

    constructor(public payload: LoginForm) { }
}

export class LoginComplete implements Action {
    readonly type = LOGIN_COMPLETE;

    constructor(public payload: LoginResponse, public tenantGuid: string, public isProviderAdmin: boolean, public isProviderUser: boolean, public isTenantAdmin: boolean, public isStandardUser: boolean, public accountGuid: string, public username: string, public parentAccountId: string) { }
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

export class LoginExpired implements Action {
    readonly type = LOGIN_EXPIRED;
}

export class SendPasswordResetToken implements Action {
    readonly type = SEND_PASSWORD_RESET_TOKEN;

    constructor(public accountId: string, public tenantId: string) { }
}

export class SendPasswordResetTokenComplete implements Action {
    readonly type = SEND_PASSWORD_RESET_TOKEN_COMPLETE;
}

export class SendPasswordResetTokenError implements Action {
    readonly type = SEND_PASSWORD_RESET_TOKEN_ERROR;

    constructor(public payload: string) { }
}

export class ResetPassword implements Action {
    readonly type = RESET_PASSWORD;

    constructor(public payload: ResetPasswordForm) { }
}

export class ResetPasswordComplete implements Action {
    readonly type = RESET_PASSWORD_COMPLETE;
}

export class ResetPasswordError implements Action {
    readonly type = RESET_PASSWORD_ERROR;

    constructor(public payload: string) { }
}

export class ClearErrors implements Action {
    readonly type = CLEAR_ERRORS;
}

export class Initialize implements Action {
    readonly type = INITIALIZE;
}

export class InitializeComplete implements Action {
    readonly type = INITIALIZE_COMPLETE;

    constructor(public payload: User, public tenantGuid: string, public isProviderAdmin: boolean, public isProviderUser: boolean, public isTenantAdmin: boolean, public isStandardUser: boolean, public accountGuid: string, public username: string, public parentAccountId: string) { }
}

export class InitializeError implements Action {
    readonly type = INITIALIZE_ERROR;

    constructor(public payload: string) { }
}

export class ForgotPassword implements Action {
    readonly type = FORGOT_PASSWORD;

    constructor(public payload: LoginForm) { }
}

export class ForgotPasswordComplete implements Action {
    readonly type = FORGOT_PASSWORD_COMPLETE;
}

export class ForgotPasswordError implements Action {
    readonly type = FORGOT_PASSWORD_ERROR;

    constructor(public payload: string) { }
}

export class GetOwners implements Action {
    readonly type = GET_OWNERS;

    constructor() {

    }
}

export class GetOwnersComplete implements Action {
    readonly type = GET_OWNERS_COMPLETE;

    constructor(public payload: Owner[]) {

    }
}

export class GetOwnersError implements Action {
    readonly type = GET_OWNERS_ERROR;

    constructor(public payload: string) {

    }
}

export class CreateOwner implements Action {
    readonly type = CREATE_OWNER;

    constructor(public payload: Owner) {

    }
}

export class CreateOwnerComplete implements Action {
    readonly type = CREATE_OWNER_COMPLETE;

    constructor() {

    }
}

export class CreateOwnerError implements Action {
    readonly type = CREATE_OWNER_ERROR;

    constructor(public payload: string) {

    }
}

export class GetOwnerDetail implements Action {
    readonly type = GET_OWNER_DETAIL;

    constructor(public payload: number) {

    }
}

export class GetOwnerDetailComplete implements Action {
    readonly type = GET_OWNER_DETAIL_COMPLETE;

    constructor(public payload: Owner) {

    }
}

export class GetOwnerDetailError implements Action {
    readonly type = GET_OWNER_DETAIL_ERROR;

    constructor(public payload: string) {

    }
}

export class EditOwner implements Action {
    readonly type = EDIT_OWNER;

    constructor(public id: number, public payload: Owner) {

    }
}

export class EditOwnerComplete implements Action {
    readonly type = EDIT_OWNER_COMPLETE;

    constructor() {

    }
}

export class EditOwnerError implements Action {

    readonly type = EDIT_OWNER_ERROR;

    constructor(public payload: string) {

    }
}


/**
 * Export a type alias of all actions in this action group
 * so that reducers can easily compose action types
 */
export type Actions = ClearErrors | Login | LoginComplete | LoginError | Logout | LoginExpired | LogoutComplete | ResetPassword | ResetPasswordComplete | ResetPasswordError
    | Initialize | InitializeComplete | InitializeError | ForgotPassword | ForgotPasswordComplete | ForgotPasswordError
    | SendPasswordResetToken | SendPasswordResetTokenComplete | SendPasswordResetTokenError
    | ChangePassword | ChangePasswordComplete | ChangePasswordError
    | GetOwners | GetOwnersComplete | GetOwnersError
    | CreateOwner | CreateOwnerComplete | CreateOwnerError
    | GetOwnerDetail | GetOwnerDetailComplete | GetOwnerDetailError
    | EditOwner | EditOwnerComplete | EditOwnerError;
