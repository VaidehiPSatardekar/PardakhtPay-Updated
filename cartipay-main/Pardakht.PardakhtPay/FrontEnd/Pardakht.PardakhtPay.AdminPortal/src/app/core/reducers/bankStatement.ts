import * as bankStatement from '../actions/bankStatement';
import { ListSearchResponse } from '../../models/types';
import { BankStatementItem } from '../../models/bank-statement-item';

export interface State {
    bankStatements: ListSearchResponse<BankStatementItem[]>;
    loading: boolean;
    error?: {
        searchError: string
    };
}

const initialState: State = {
    bankStatements: undefined,
    loading: false,
    error: undefined
}

export function reducer(state: State = initialState, action: bankStatement.Actions): State {
    switch (action.type) {
        case bankStatement.SEARCH:

            return {
                ...state,
                bankStatements: undefined,
                loading: true,
                error: undefined
            }

        case bankStatement.SEARCH_COMPLETE:
            return {
                ...state,
                bankStatements: action.payload,
                loading: false,
                error: undefined
            }

        case bankStatement.SEARCH_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    searchError: action.payload
                }
            }

        case bankStatement.CLEAR_ALL:
            return initialState;
        default:
            return state;
    }
}