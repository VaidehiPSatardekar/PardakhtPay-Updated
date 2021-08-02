import * as transaction from '../actions/transaction';
import { ListSearchResponse } from '../../models/types';
import { TransactionSearch } from '../../models/transaction';

export interface State {
    transactions: ListSearchResponse<TransactionSearch[]>;
    loading: boolean;
    setAsCompletedSuccess: boolean,
    setAsExpiredSuccess: boolean,
    callbackToMerchant: string;
    error?: {
        searchError: string,
        setAsCompleted: string,
        setAsExpired: string,
        transactionCallbackToMerchantError: string;
    };
}

const initialState: State = {
    transactions: undefined,
    loading: false,
    setAsCompletedSuccess: false,
    setAsExpiredSuccess: false,
    error: undefined,
    callbackToMerchant: undefined
}

export function reducer(state: State = initialState, action: transaction.Actions): State {
    switch (action.type) {
        case transaction.SEARCH:

            return {
                ...state,
                transactions: undefined,
                loading: true,
                error: undefined
            }

        case transaction.SEARCH_COMPLETE:
            return {
                ...state,
                transactions: action.payload,
                loading: false,
                error: undefined
            }

        case transaction.SEARCH_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    searchError: action.payload
                }
            }
        case transaction.SET_AS_COMPLETED:
            return {
                ...state,
                loading: false,
                setAsCompletedSuccess: false,
                error: undefined
            };
        case transaction.SET_AS_COMPLETED_COMPLETE:
            return {
                ...state,
                setAsCompletedSuccess: true,
                error: undefined
            };
        case transaction.SET_AS_COMPLETED_ERROR:
            return {
                ...state,
                setAsCompletedSuccess: false,
                error: {
                    ...state.error,
                    setAsCompleted: action.payload
                }
            }
        case transaction.SET_AS_EXPIRED:
            return {
                ...state,
                loading: false,
                setAsExpiredSuccess: false,
                error: undefined
            };
        case transaction.SET_AS_EXPIRED_COMPLETE:
            return {
                ...state,
                setAsExpiredSuccess: true,
                error: undefined
            };
        case transaction.SET_AS_EXPIRED_ERROR:
            return {
                ...state,
                setAsExpiredSuccess: false,
                error: {
                    ...state.error,
                    setAsExpired: action.payload
                }
            }
        case transaction.TRANSACTIONCALLBACKTOMERCHANT:
            return {
                ...state,
                loading: true,
                callbackToMerchant: undefined,
                error: {
                    ...state.error,
                    transactionCallbackToMerchantError: undefined
                }
            };
        case transaction.TRANSACTIONCALLBACKTOMERCHANT_COMPLETE:
            return {
                ...state,
                loading: false,
                callbackToMerchant: action.payload,
                error: {
                    ...state.error,
                    transactionCallbackToMerchantError: undefined
                }
            };
        case transaction.TRANSACTIONCALLBACKTOMERCHANT_ERROR:
            return {
                ...state,
                loading: false,
                callbackToMerchant: undefined,
                error: {
                    ...state.error,
                    transactionCallbackToMerchantError: action.payload
                }
            }
        case transaction.CLEAR_ALL:
            return initialState;
        default:
            return state;
    }
}