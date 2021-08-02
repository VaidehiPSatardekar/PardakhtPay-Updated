import * as cardHolder from '../actions/cardHolder';
import { ListSearchResponse } from '../../models/types';
import { CardHolder } from '../../models/card-holder';

export interface State {
    cardHolder: CardHolder;
    loading: boolean;
    error?: {
        searchError: string
    };
}

const initialState: State = {
    cardHolder: undefined,
    loading: false,
    error: undefined
}

export function reducer(state: State = initialState, action: cardHolder.Actions): State {
    switch (action.type) {
        case cardHolder.SEARCH:

            return {
                ...state,
                cardHolder: undefined,
                loading: true,
                error: undefined
            }

        case cardHolder.SEARCH_COMPLETE:
            return {
                ...state,
                cardHolder: action.payload,
                loading: false,
                error: undefined
            }

        case cardHolder.SEARCH_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    searchError: action.payload
                }
            }

        case cardHolder.CLEAR_ALL:
            return initialState;
        default:
            return state;
    }
}