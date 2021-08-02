// tslint:disable:max-func-body-length
// tslint:disable:cyclomatic-complexity
import { StaffUser, PasswordResetResponse, CreateStaffUserResponse, StaffUserPerformanceTime } from '../models/user-management.model';
import * as user from '../actions/user';
import { PlatformConfig } from '../models/platform.model';

export interface State {
    isLoggedIn: boolean;
    user: StaffUser;
    users: StaffUser[];
    affiliateUsers: StaffUser[];
    systemUsers: StaffUser[];
    loading: boolean;
    userPerformanceTime: StaffUserPerformanceTime;
    error?: { [key: string]: string };
    query: string;
    // updateCount: number;
    userUpdateSuccess: boolean;
    userCreated: CreateStaffUserResponse;
    platformConfig: PlatformConfig;
    passwordChanged: boolean;
    passwordResetResult: PasswordResetResponse;
}

const initialState: State = {
    isLoggedIn: false,
    user: undefined,
    users: [],
    affiliateUsers: [],
    systemUsers: [],
    loading: false,
    userPerformanceTime: undefined,
    error: undefined,
    query: '',
    userUpdateSuccess: undefined,
    userCreated: undefined,
    // updateCount: 0,
    platformConfig: undefined,
    passwordChanged: false,
    passwordResetResult: undefined,
};

function retrieveError(data: string | { [key: string]: string[] }): string {
    let error: any;

    if (data !== undefined && data !== null && typeof data === 'object') {
        const keys: string[] = Object.keys(data);
        const len: number = keys.length;
        let i = 0;

        while (i < len) {
            const val: any = data[keys[i]];
            if (error === undefined) {
                error = typeof val === 'object' ? val[0] : data;
            }
            i = i + 1;
        }
    }
    else {
        error = data;
    }
    return error;
}


export function reducer(state: State = initialState, action: user.Actions): State {
    switch (action.type) {

        case user.RESET_PASSWORD:
            return {
                ...state,
                passwordResetResult: undefined,
                loading: true,
                error: undefined
            };

        case user.LOGIN:
        case user.INITIALIZE:
        case user.FORGOT_PASSWORD:
        case user.FORGOT_PASSWORD_BY_USERNAME:
            return {
                ...state,
                user: undefined,
                loading: true,
                isLoggedIn: false,
                error: undefined
            };

        case user.INITIALIZE_COMPLETE:
            return {
                ...state,
                user: action.payload,
                passwordChanged: false,
                isLoggedIn: action.payload !== undefined,
                loading: false,
                error: undefined
            };

        case user.INITIALIZE_ERROR:
            return {
                ...state,
                user: undefined,
                loading: false,
                passwordChanged: false,
                isLoggedIn: false,
                error: {
                    message: action.payload
                }
            };

        case user.LOGIN_COMPLETE:
            return {
                ...state,
                loading: false,
                user: action.payload ? action.payload.user : undefined,
                isLoggedIn: (action.payload && action.payload.token && action.payload.token.accessToken && action.payload.token.accessToken !== '')
            };

        case user.FORGOT_PASSWORD_ERROR:
        case user.FORGOT_PASSWORD_BY_USERNAME_ERROR:
        case user.LOGIN_ERROR:
            return {
                ...state,
                user: undefined,
                loading: false,
                isLoggedIn: false,
                error: {
                    login: action.payload
                }
            };

        case user.LOGIN_AS_STAFF_USER:
            return {
                ...state,
                loading: true,
                error: undefined
            };

        case user.LOGIN_AS_STAFF_USER_COMPLETE:
            return {
                ...state,
                loading: false,
                user: action.payload ? action.payload.user : undefined
            };

        case user.LOGIN_AS_STAFF_USER_ERROR: {
            return {
                ...state,
                loading: false,
                error: {
                    loginAs: action.payload
                }
            };
        }

        case user.LOGOUT:
        case user.LOGIN_EXPIRED:
            return {
                ...state,
                user: undefined,
                isLoggedIn: false,
                passwordChanged: false,
                loading: false,
                error: undefined
            };

        case user.ADD_IDLE_TIME:
            {
                return {
                    ...state,
                    loading: true,
                    error: undefined
                };
            }

        case user.ADD_IDLE_TIME_COMPLETE: {
            return {
                ...state,
                userPerformanceTime: action.payload,
                loading: false
            };
        }

        case user.ADD_IDLE_TIME_ERROR: {
            const error: string = retrieveError(action.payload);

            return {
                ...state,
                userPerformanceTime: undefined,
                loading: false,
                error: {
                    addedIdleMinutes: error
                }
            };
        }

        case user.UPDATE_TRACKING_TIME:
            {
                return {
                    ...state,
                    loading: true,
                    error: undefined
                };
            }

        case user.UPDATE_TRACKING_TIME_COMPLETE: {
            return {
                ...state,
                userPerformanceTime: action.payload,
                loading: false
            };
        }

        case user.UPDATE_TRACKING_TIME_ERROR: {
            const error: string = retrieveError(action.payload);

            return {
                ...state,
                userPerformanceTime: undefined,
                loading: false,
                error: {
                    updateTrackingTime: error
                }
            };
        }

        case user.RESET_PASSWORD_COMPLETE:
            return {
                ...state,
                passwordResetResult: action.payload,
                loading: false,
                error: undefined
            };

        case user.FORGOT_PASSWORD_BY_USERNAME_COMPLETE:
        case user.FORGOT_PASSWORD_COMPLETE:
            return {
                ...state,
                loading: false,
                error: undefined
            };

        case user.RESET_PASSWORD_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    message: action.payload
                }
            };

        case user.EDIT_STAFF_USER:
        case user.CREATE_STAFF_USER:
            return {
                ...state,
                userUpdateSuccess: false,
                userCreated: undefined,
                passwordResetResult: undefined,
                loading: true,
                error: undefined
            };

        case user.BLOCK_STAFF_USER:
            return {
                ...state,
                loading: true,
                error: undefined
            };

        case user.DELETE_STAFF_USER:
            return {
                ...state,
                loading: true,
                error: undefined
            };

        case user.GET_STAFF_USERS:
            return {
                ...state,
                users: undefined,
                loading: true,
                error: undefined,
                passwordResetResult: undefined,
            };
        case user.GET_AFFILIATE_USERS:
            return {
                ...state,
                affiliateUsers: undefined,
                loading: true,
                error: undefined,
                passwordResetResult: undefined,
            };
        case user.GET_SYSTEM_USERS:
            return {
                ...state,
                systemUsers: undefined,
                loading: true,
                error: undefined,
                passwordResetResult: undefined,
            };

        case user.CREATE_STAFF_USER_ERROR: {
            return {
                ...state,
                loading: false,
                passwordResetResult: undefined,
                error: {
                    create: action.payload
                }
            };
        }

        case user.CREATE_STAFF_USER_COMPLETE: {
            const existingUsers: StaffUser[] = [...state.users];

            existingUsers.push(action.payload.staffUser);

            return {
                ...state,
                loading: false,
                users: existingUsers,
                userCreated: action.payload
            };
        }

        case user.DELETE_STAFF_USER_COMPLETE: {
            const existingUsers: StaffUser[] = [...state.users];
            const existingUserIndex = existingUsers.findIndex((u: StaffUser) => u.id === action.payload.id);

            if (existingUserIndex !== - 1) {
                // replace existing item on update
                existingUsers.splice(existingUserIndex, 1);
            }

            return {
                ...state,
                loading: false,
                users: existingUsers,
                userUpdateSuccess: true
                // updateCount: state.updateCount = state.updateCount + 1
            };
        }

        case user.BLOCK_STAFF_USER_COMPLETE:
        case user.EDIT_STAFF_USER_COMPLETE: {
            const existingUsers: StaffUser[] = [...state.users];
            const existingUserIndex = existingUsers.findIndex((u: StaffUser) => u.id === action.payload.id);

            if (existingUserIndex !== - 1) {
                // replace existing item on update
                existingUsers.splice(existingUserIndex, 1, action.payload);
            }
            return {
                ...state,
                loading: false,
                users: existingUsers,
                userUpdateSuccess: true
                // updateCount: state.updateCount = state.updateCount + 1
            };
        }

        case user.CHANGE_PASSWORD:
            return {
                ...state,
                loading: true,
                passwordChanged: false,
                error: undefined
            };

        case user.CHANGE_PASSWORD_COMPLETE:
            return {
                ...state,
                loading: false,
                passwordChanged: action.payload
            };

        case user.CHANGE_PASSWORD_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    message: action.payload
                }
            };

        case user.CHANGE_PASSWORD_INIT: {
            return {
                ...state,
                loading: false,
                passwordChanged: false
            };
        }

        case user.GET_STAFF_USERS_COMPLETE: {
            return {
                ...state,
                loading: false,
                users: action.payload
            };
        }
        case user.GET_AFFILIATE_USERS_COMPLETE: {
            return {
                ...state,
                loading: false,
                affiliateUsers: action.payload
            };
        }
        case user.GET_SYSTEM_USERS_COMPLETE: {
            return {
                ...state,
                loading: false,
                systemUsers: action.payload
            };
        }
        case user.BLOCK_STAFF_USER_ERROR:
        case user.DELETE_STAFF_USER_ERROR:
        case user.EDIT_STAFF_USER_ERROR: {
            return {
                ...state,
                loading: false,
                error: {
                    edit: action.payload
                }
            };
        }

        case user.GET_STAFF_USERS_ERROR: {
            return {
                ...state,
                loading: false,
                error: {
                    get: action.payload
                }
            };
        }
        case user.GET_SYSTEM_USERS_ERROR:
        case user.GET_AFFILIATE_USERS_ERROR: {
            return {
                ...state,
                loading: false,
                error: {
                    get: action.payload
                }
            };
        }

        case user.SET_PLATFORM: {
            return {
                ...state,
                platformConfig: action.payload
            };
        }

        case user.CLEAR_ERRORS:
            return {
                ...state,
                passwordResetResult: undefined,
                passwordChanged: false,
                userCreated: undefined,
                error: undefined
            };

        default: {
            return state;
        }
    }
}
