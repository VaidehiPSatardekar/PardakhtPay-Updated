import * as autoTransfer from '../actions/autoTransfer';
import { ListSearchResponse } from '../../models/types';
import { AutoTransfer } from '../../models/autoTransfer';

export interface State {
    autoTransfers: ListSearchResponse<AutoTransfer[]>;
    loading: boolean;
    error?: {
        searchError: string
    };
}

const initialState: State = {
    autoTransfers: undefined,
    loading: false,
    error: undefined
}

export function reducer(state: State = initialState, action: autoTransfer.Actions): State {
    switch (action.type) {
        case autoTransfer.SEARCH:

            return {
                ...state,
                autoTransfers: undefined,
                loading: true,
                error: undefined
            }

        case autoTransfer.SEARCH_COMPLETE:
            return {
                ...state,
                autoTransfers: action.payload,
                loading: false,
                error: undefined
            }

        case autoTransfer.SEARCH_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    searchError: action.payload
                }
            }

        case autoTransfer.CLEAR_ALL:
            return initialState;
        default:
            return state;
    }
}