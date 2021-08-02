import * as accounting from '../actions/accounting';
import { ListSearchResponse } from '../../models/types';
import { TransactionSearch } from '../../models/transaction';
import { DailyAccountingDTO } from '../../models/accounting';

export interface State {
    items: DailyAccountingDTO[];
    loading: boolean;
    error?: {
        searchError: string
    };
}

const initialState: State = {
    items: undefined,
    loading: false,
    error: undefined
}

export function reducer(state: State = initialState, action: accounting.Actions): State {
    switch (action.type) {
        case accounting.SEARCH:

            return {
                ...state,
                items: undefined,
                loading: true,
                error: undefined
            }

        case accounting.SEARCH_COMPLETE:
            return {
                ...state,
                items: action.payload,
                loading: false,
                error: undefined
            }

        case accounting.SEARCH_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    searchError: action.payload
                }
            }
        case accounting.CLEAR_ALL:
            return initialState;
        default:
            return state;
    }
}