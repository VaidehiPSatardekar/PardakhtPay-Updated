// tslint:disable:max-classes-per-file

import { GenericHelper } from '../helpers/generic';
import { User } from './user-management.model';

export class LoginResponse {
  user: User;
    //accessToken: string;
    //refreshToken: string;
    expires: Date | string;
    token: Token;
}

// export class AuthToken {
//   access_token: string;
//   token_type: string;
//   expires_in: number;
// }

export class ApplicationUser {
  // url: string;
  id: string;
  username: string;
  // fullName: string;
  email: string;
  // roles: string[];
}


export class ChangePasswordUser {
  oldPassword: string;
  newPassword: string;
  newpasswordConfirm : string;
  
  constructor(data?: any) {
    GenericHelper.populateData(this, data);
  }
}


export class LoginDetails {
  username: string;
  password: string;
}

export class LoginForm {
  username: string;
  password: string;
  loginAsUsername: string;
  baseUrl: string;

  constructor(private captchaToken: string, data?: any) {
    GenericHelper.populateData(this, data);
  }
}

export class ResetPasswordForm {
  accountId: string;
  token: string;
  password: string;
}

export class Owner {
    id: number;
    accountId: string;
    username: string;
    firstName: string;
    lastName: string;
    tenantGuid: string;
    email: string;
    isApiKeyUser: boolean;
    roleGuid: string[];
    tenantDomainPlatformMapGuid: string;

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class OwnerCreateDTO {
    id: number;
    accountId: string;
    username: string;
    firstName: string;
    lastName: string;
    tenantGuid: string;
    email: string;
    password: string;
    isApiKeyUser: boolean;
    roleGuid: string[];

    constructor(data?: any) {
        GenericHelper.populateData(this, data);
    }
}

export class Token {
    accessToken: string;
    refreshToken: string;
    expires: Date;
}