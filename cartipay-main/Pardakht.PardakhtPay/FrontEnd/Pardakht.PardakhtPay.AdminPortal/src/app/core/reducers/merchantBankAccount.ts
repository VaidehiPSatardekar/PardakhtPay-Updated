import * as merchantBankAccount from '../actions/merchantBankAccount';
import { MerchantBankAccount } from '../../models/merchant-model';

export interface State {
    items: MerchantBankAccount[];
    loading: boolean;
    error?: {
        getAccountsError: string
    };
}

const initialState: State = {
    items: undefined,
    loading: false,
    error: undefined
}

export function reducer(state: State = initialState, action: merchantBankAccount.Actions): State {
    switch (action.type) {
        case merchantBankAccount.GET_ACCOUNTS:

            return {
                ...state,
                items: undefined,
                loading: true,
                error: undefined
            }

        case merchantBankAccount.GET_ACCOUNTS_COMPLETE:
            return {
                ...state,
                items: action.payload,
                loading: false,
                error: undefined
            }

        case merchantBankAccount.GET_ACCOUNTS_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    getAccountsError: action.payload
                }
            }
        case merchantBankAccount.CLEAR_ALL: {
            return initialState;
        }
        default:
            return state;
    }
}