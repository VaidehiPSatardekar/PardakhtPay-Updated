import * as account from '../../core/actions/account';
import { LoginResponse, Owner } from '../../models/account.model';
import { User } from '../../models/user-management.model';


export interface State {
    user: User;
    passwordChanged: boolean;
    loading: boolean;
    isLoggedIn: boolean;
    tenantGuid: string;
    isProviderAdmin: boolean;
    isProviderUser: boolean;
    isTenantAdmin: boolean;
    isStandardUser: boolean;
    accountGuid: string;
    parentAccountId: string;
    username: string;
    createdSuccess: boolean;
    detailUser: Owner;
    updateSuccess: boolean;
    owners: Owner[];
    error?: { [key: string]: string };
}

const initialState: State = {
    user: undefined,
    passwordChanged: false,
    tenantGuid: undefined,
    isProviderAdmin: false,
    isProviderUser: false,
    isTenantAdmin: false,
    isStandardUser: false,
    accountGuid: undefined,
    parentAccountId: undefined,
    username: undefined,
    createdSuccess: false,
    detailUser: undefined,
    updateSuccess: false,
    owners: undefined,
    loading: false,
    isLoggedIn: false,
    error: undefined
};

// tslint:disable-next-line:max-func-body-length
export function reducer(state: State = initialState, action: account.Actions): State {
    switch (action.type) {
        case account.LOGIN:
        case account.FORGOT_PASSWORD:
        case account.INITIALIZE:
        case account.RESET_PASSWORD:
            return {
                user: undefined,
                loading: true,
                isLoggedIn: false,
                tenantGuid: undefined,
                isProviderAdmin: false,
                isProviderUser: false,
                isTenantAdmin: false,
                isStandardUser: false,
                accountGuid: undefined,
                parentAccountId: undefined,
                username: undefined,
                createdSuccess: false,
                detailUser: undefined,
                updateSuccess: false,
                owners: undefined,
                passwordChanged: undefined,
                error: undefined
            };

        case account.CHANGE_PASSWORD:
            return {
                ...state,
                loading: true,
                passwordChanged: false
            };

        case account.CHANGE_PASSWORD_COMPLETE:
            return {
                ...state,
                loading: false,
                passwordChanged: action.payload
            };

        case account.CHANGE_PASSWORD_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    message: action.payload
                }
            };

        case account.LOGOUT:
        case account.LOGIN_EXPIRED:
        case account.RESET_PASSWORD_COMPLETE:
            return {
                user: undefined,
                isLoggedIn: false,
                tenantGuid: undefined,
                isProviderAdmin: false,
                isProviderUser: false,
                isTenantAdmin: false,
                isStandardUser: false,
                accountGuid: undefined,
                parentAccountId: undefined,
                username: undefined,
                createdSuccess: false,
                detailUser: undefined,
                updateSuccess: false,
                owners: undefined,
                passwordChanged: undefined,
                loading: false,
                error: undefined
            };

        case account.LOGIN_COMPLETE:
            return {
                ...state,
                user: action.payload.user,
                tenantGuid: action.tenantGuid,
                isProviderAdmin: action.isProviderAdmin,
                isProviderUser: action.isProviderUser,
                isTenantAdmin: action.isTenantAdmin,
                isStandardUser: action.isStandardUser,
                accountGuid: action.accountGuid,
                parentAccountId: action.parentAccountId,
                username: action.username,
                loading: false,
                isLoggedIn: (action.payload && action.payload.token && action.payload.token.accessToken && action.payload.token.accessToken !== '')
            };

        case account.LOGIN_ERROR:
            return {
                user: undefined,
                passwordChanged: undefined,
                loading: false,
                tenantGuid: undefined,
                isProviderAdmin: false,
                isProviderUser: false,
                isTenantAdmin: false,
                isStandardUser: false,
                accountGuid: undefined,
                parentAccountId: undefined,
                username: undefined,
                createdSuccess: false,
                detailUser: undefined,
                updateSuccess: false,
                owners: undefined,
                isLoggedIn: false,
                error: {
                    message: action.payload
                }
            };

        case account.SEND_PASSWORD_RESET_TOKEN:
            return {
                ...state,
                loading: true,
                error: undefined
            };

        case account.SEND_PASSWORD_RESET_TOKEN_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    sendpasswordreset: action.payload
                }
            };

        case account.RESET_PASSWORD_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    message: action.payload
                }
            };

        case account.INITIALIZE_COMPLETE:
            return {
                ...state,
                user: action.payload,
                passwordChanged: undefined,
                loading: false,
                tenantGuid: action.tenantGuid,
                isProviderAdmin: action.isProviderAdmin,
                isProviderUser: action.isProviderUser,
                isTenantAdmin: action.isTenantAdmin,
                isStandardUser: action.isStandardUser,
                accountGuid: action.accountGuid,
                parentAccountId: action.parentAccountId,
                username: action.username,
                createdSuccess: false,
                detailUser: undefined,
                updateSuccess: false,
                owners: undefined,
                error: undefined
            };

        case account.INITIALIZE_ERROR:
            return {
                user: undefined,
                loading: false,
                passwordChanged: undefined,
                isLoggedIn: false,
                tenantGuid: undefined,
                isProviderAdmin: false,
                isProviderUser: false,
                isTenantAdmin: false,
                isStandardUser: false,
                accountGuid: undefined,
                parentAccountId: undefined,
                username: undefined,
                createdSuccess: false,
                detailUser: undefined,
                updateSuccess: false,
                owners: undefined,
                error: {
                    message: action.payload
                }
            };

        case account.SEND_PASSWORD_RESET_TOKEN_COMPLETE:
        case account.FORGOT_PASSWORD_COMPLETE:
            return {
                ...state,
                loading: false,
                error: undefined
            };

        case account.FORGOT_PASSWORD_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    forgot: action.payload
                }
            };

        case account.CLEAR_ERRORS:
            return {
                ...state,
                owners: undefined,
                passwordChanged: undefined,
                updateSuccess: false,
                createdSuccess: false,
                detailUser: undefined,
                error: undefined
            };

        case account.GET_OWNERS:
            return {
                ...state,
                loading: true,
                owners: undefined
            };

        case account.GET_OWNERS_COMPLETE: {
            return {
                ...state,
                loading: false,
                owners: action.payload,
                error: undefined
            }
        }

        case account.GET_OWNERS_ERROR:
            return {
                ...state,
                loading: false,
                owners: undefined,
                error: {
                    message: action.payload
                }
            };

        case account.CREATE_OWNER:
            return {
                ...state,
                loading: true,
                createdSuccess: false,
                error: undefined
            };

        case account.CREATE_OWNER_COMPLETE:
            return {
                ...state,
                loading: false,
                createdSuccess: true,
                error: undefined
            };

        case account.CREATE_OWNER_ERROR:
            return {
                ...state,
                loading: false,
                createdSuccess: false,
                error: {
                    message: action.payload
                }
            };

        case account.GET_OWNER_DETAIL:
            return {
                ...state,
                loading: true,
                detailUser: undefined,
                error: undefined
            };

        case account.GET_OWNER_DETAIL_COMPLETE:
            return {
                ...state,
                loading: false,
                detailUser: action.payload,
                error: undefined
            };

        case account.GET_OWNER_DETAIL_ERROR:
            return {
                ...state,
                loading: false,
                detailUser: undefined,
                error: {
                    message: action.payload
                }
            };

        case account.EDIT_OWNER:
            return {
                ...state,
                loading: true,
                updateSuccess: false,
                error: undefined
            };

        case account.EDIT_OWNER_COMPLETE:
            return {
                ...state,
                loading: false,
                updateSuccess: true,
                error: undefined
            };

        case account.EDIT_OWNER_ERROR:
            return {
                ...state,
                loading: false,
                updateSuccess: false,
                error: {
                    message: action.payload
                }
            };

        default: {
            return state;
        }
    }
}
