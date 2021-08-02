import * as loginDeviceStatus from '../actions/loginDeviceStatus';
import { BankLogin, BankAccount } from '../../models/bank-login';
import { Bank } from '../../models/bank';

export interface State {
    bankLogins: BankLogin[];
    bankAccounts: BankAccount[];
    ownerLogins: BankLogin[];
    updateSuccess: boolean;
    detail: BankLogin;
    banks: Bank[];
    loading: boolean;
    accountsLoading: boolean;
    banksLoading: boolean;
    getDetailLoading: boolean;
    updateLoginSuccess: boolean;
    createLoginFromLoginRequestLoading: boolean;
    createLoginSuccess: boolean;
    ownerBankLoginLoading: boolean;
    loginDeviceStatusDesc: string;
    qrCodeRegisterSuccess: boolean;
    qrCodeRegister: boolean;
    qrCodeRegistrationCompleted: boolean,
    error?: {
        searchError: string,
        searchBankAccountError: string;
        searchBanksError: string;
        createError: string;
        getDetailError: string;
        updateLogin: string;
        createLoginError: string;
        showloginDeviceStatusDescError: string;
        qrCodeRegisterError: string;
    };
}

const initialState: State = {
    bankLogins: undefined,
    bankAccounts: undefined,
    ownerLogins: undefined,
    updateSuccess: false,
    detail: undefined,
    banks: undefined,
    loading: false,
    accountsLoading: false,
    banksLoading: false,
    getDetailLoading: false,
    updateLoginSuccess: false,
    createLoginFromLoginRequestLoading: false,
    createLoginSuccess: false,
    ownerBankLoginLoading: false,
    error: undefined,
    loginDeviceStatusDesc: undefined,
    qrCodeRegisterSuccess: false,
    qrCodeRegister: false,
    qrCodeRegistrationCompleted: false
}

export function reducer(state: State = initialState, action: loginDeviceStatus.Actions): State {
    switch (action.type) {
        
       case loginDeviceStatus.GET_DETAILS:
            return {
                ...state,
                getDetailLoading: true,
                qrCodeRegistrationCompleted: false,
                error: undefined,
                detail: undefined,
            };

        case loginDeviceStatus.GET_DETAILS_COMPLETE:
            return {
                ...state,
                getDetailLoading: false,
                qrCodeRegistrationCompleted: false,
                detail: action.payload,
            };

        case loginDeviceStatus.GET_DETAILS_ERROR:
            return {
                ...state,
                getDetailLoading: false,
                qrCodeRegistrationCompleted: false,
                error: {
                    ...state.error,
                    getDetailError: action.payload
                },
                detail: undefined,
            };


        case loginDeviceStatus.SHOW_LOGINDEVICESTATUS:

            return {
                ...state,
                loginDeviceStatusDesc: undefined,
                loading: true,
                error: {
                    ...state.error,
                    showloginDeviceStatusDescError: undefined
                }
            }

        case loginDeviceStatus.SHOW_LOGINDEVICESTATUS_COMPLETED:
            return {
                ...state,
                loginDeviceStatusDesc: action.payload,
                loading: false,
                error: {
                    ...state.error,
                    showloginDeviceStatusDescError: undefined
                }
            }

        case loginDeviceStatus.SHOW_LOGINDEVICESTATUS_ERROR:
            return {
                ...state,
                loginDeviceStatusDesc: undefined,
                loading: false,
                error: {
                    ...state.error,
                    showloginDeviceStatusDescError: action.payload
                }
            }

      
        case loginDeviceStatus.CLEAR_ALL:
            return initialState;
        default:
            return state;
    }
}