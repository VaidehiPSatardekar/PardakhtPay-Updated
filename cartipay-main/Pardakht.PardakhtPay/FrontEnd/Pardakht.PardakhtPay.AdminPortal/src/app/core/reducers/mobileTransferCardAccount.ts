import { MobileTransferCardAccount } from "../../models/mobile-transfer";
import * as mobileTransferCardAccount from '../actions/mobileTransferCardAccount';

export interface State {
    mobileTransferCardAccounts: MobileTransferCardAccount[];
    allMobileTransferCardAccounts: MobileTransferCardAccount[];
    mobileTransferCardAccountDetails: MobileTransferCardAccount;
    loading: boolean;
    getDetailLoading: boolean;
    updatePaymentGatewayDetails: MobileTransferCardAccount;
    error?: {
        createMobileTransferCardAccount: string,
        editMobileTransferCardAccount: string,
        getDetails: string,
        getAll: string
    };
    mobileTransferCardAccountCreated: MobileTransferCardAccount;
    mobileTransferCardAccountUpdateSuccess: boolean;
}

const initialState: State = {
    mobileTransferCardAccounts: [],
    allMobileTransferCardAccounts: [],
    mobileTransferCardAccountDetails: undefined,
    loading: false,
    getDetailLoading: false,
    error: undefined,
    mobileTransferCardAccountCreated: undefined,
    updatePaymentGatewayDetails: undefined,
    mobileTransferCardAccountUpdateSuccess: false
};

export function reducer(state: State = initialState, action: mobileTransferCardAccount.Actions): State {
    switch (action.type) {
        case mobileTransferCardAccount.GET_ALL:

            return {
                ...state,
                allMobileTransferCardAccounts: [],
                loading: true,
                error: undefined
            }

        case mobileTransferCardAccount.GET_ALL_COMPLETE:
            return {
                ...state,
                allMobileTransferCardAccounts: action.payload.map(payload => payload),
                loading: false,
                error: undefined
            }

        case mobileTransferCardAccount.GET_ALL_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getAll: action.payload
                }
            }

        case mobileTransferCardAccount.CREATE:
            return {
                ...state,
                loading: true,
                error: undefined,
                mobileTransferCardAccountCreated: undefined,
            };

        case mobileTransferCardAccount.CREATE_COMPLETE:


            const newMobileTransferCardAccounts: MobileTransferCardAccount[] = [
                ...state.mobileTransferCardAccounts
            ];
            newMobileTransferCardAccounts.push(action.payload);
            return {
                ...state,
                mobileTransferCardAccounts: newMobileTransferCardAccounts,
                loading: false,
                mobileTransferCardAccountCreated: action.payload,
            };

        case mobileTransferCardAccount.CREATE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    createMobileTransferCardAccount: action.payload
                },
                mobileTransferCardAccountCreated: undefined,
            };

        case mobileTransferCardAccount.GET_DETAILS:
            return {
                ...state,
                getDetailLoading: true,
                error: undefined,
                mobileTransferCardAccountDetails: undefined,
            };

        case mobileTransferCardAccount.GET_DETAILS_COMPLETE:
            return {
                ...state,
                getDetailLoading: false,
                mobileTransferCardAccountDetails: action.payload,
            };

        case mobileTransferCardAccount.GET_DETAILS_ERROR:
            return {
                ...state,
                getDetailLoading: false,
                error: {
                    ...state.error,
                    getDetails: action.payload
                },
                mobileTransferCardAccountCreated: undefined,
            };

        case mobileTransferCardAccount.EDIT:
            return {
                ...state,
                loading: true,
                error: undefined,
                mobileTransferCardAccountCreated: undefined,
                mobileTransferCardAccountUpdateSuccess: false
            };

        case mobileTransferCardAccount.EDIT_COMPLETE:
            {

                const newMobileTransferCardAccounts: MobileTransferCardAccount[] = [
                    ...state.mobileTransferCardAccounts
                ];
                newMobileTransferCardAccounts.splice(newMobileTransferCardAccounts.findIndex(q => q.id === action.payload.id), 1, action.payload);
                return {
                    ...state,
                    mobileTransferCardAccounts: newMobileTransferCardAccounts,
                    loading: false,
                    mobileTransferCardAccountCreated: undefined,
                    mobileTransferCardAccountUpdateSuccess: true
                };
            }

        case mobileTransferCardAccount.EDIT_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    editMobileTransferCardAccount: action.payload
                },
                mobileTransferCardAccountCreated: undefined,
            };
        case mobileTransferCardAccount.CLEAR_ERRORS:
            return {
                ...state,
                loading: false,
                error: undefined
            };
        case mobileTransferCardAccount.CLEAR:
            return initialState;
        default:
            return state;
    }
}