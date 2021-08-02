import { BlockedCardNumber } from "../../models/blocked-Card-number";
import * as blockedCardNumber from '../actions/blockedCardNumber';


export interface State {
    allBlockedCardNumbers: BlockedCardNumber[];
    blockedCardNumberDetails: BlockedCardNumber;
    loading: boolean;
    getDetailLoading: boolean;
    updated: BlockedCardNumber;
    error?: {
        searchError: string,
        createBlockedCardNumber: string,
        editBlockedCardNumber: string,
        getDetails: string,
        getAll: string,
        deleteError: string
    };
    query: string;
    blockedCardNumberCreated: BlockedCardNumber;
    blockedCardNumberUpdateSuccess: boolean;
    cardToCardAccountDeletedSuccess: boolean;
}

const initialState: State = {
    allBlockedCardNumbers: [],
    blockedCardNumberDetails: undefined,
    loading: false,
    getDetailLoading: false,
    error: undefined,
    query: '',
    blockedCardNumberCreated: undefined,
    updated: undefined,
    blockedCardNumberUpdateSuccess: false,
    cardToCardAccountDeletedSuccess: false
};

export function reducer(state: State = initialState, action: blockedCardNumber.Actions): State {
    switch (action.type) {
        case blockedCardNumber.GET_ALL:

            return {
                ...state,
                allBlockedCardNumbers: [],
                loading: true,
                error: undefined
            }

        case blockedCardNumber.GET_ALL_COMPLETE:
            return {
                ...state,
                allBlockedCardNumbers: action.payload.map(payload => payload),
                loading: false,
                error: undefined
            }

        case blockedCardNumber.GET_ALL_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getAll: action.payload
                }
            }

        case blockedCardNumber.CREATE:
            return {
                ...state,
                loading: true,
                error: undefined,
                blockedCardNumberCreated: undefined,
            };

        case blockedCardNumber.CREATE_COMPLETE:
            
            return {
                ...state,
                loading: false,
                blockedCardNumberCreated: action.payload,
            };

        case blockedCardNumber.CREATE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    createBlockedCardNumber: action.payload
                },
                blockedCardNumberCreated: undefined,
            };

        case blockedCardNumber.GET_DETAILS:
            return {
                ...state,
                getDetailLoading: true,
                error: undefined,
                blockedCardNumberDetails: undefined,
            };

        case blockedCardNumber.GET_DETAILS_COMPLETE:
            return {
                ...state,
                getDetailLoading: false,
                blockedCardNumberDetails: action.payload,
            };

        case blockedCardNumber.GET_DETAILS_ERROR:
            return {
                ...state,
                getDetailLoading: false,
                error: {
                    ...state.error,
                    getDetails: action.payload
                },
                blockedCardNumberCreated: undefined,
            };

        case blockedCardNumber.EDIT:
            return {
                ...state,
                loading: true,
                error: undefined,
                blockedCardNumberCreated: undefined,
                blockedCardNumberUpdateSuccess: false
            };

        case blockedCardNumber.EDIT_COMPLETE:
            {
                return {
                    ...state,
                    loading: false,
                    blockedCardNumberCreated: undefined,
                    blockedCardNumberUpdateSuccess: true
                };
            }

        case blockedCardNumber.EDIT_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    editBlockedCardNumber: action.payload
                },
                blockedCardNumberCreated: undefined,
            };

        case blockedCardNumber.DELETE:
            return {
                ...state,
                loading: true,
                error: undefined,
                cardToCardAccountDeletedSuccess: false
            };

        case blockedCardNumber.DELETE_COMPLETE:
            {
                return {
                    ...state,
                    loading: false,
                    cardToCardAccountDeletedSuccess: true
                };
            }

        case blockedCardNumber.DELETE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    deleteError: action.payload
                },
                cardToCardAccountDeletedSuccess: false
            };
        case blockedCardNumber.CLEAR_ERRORS:
            return {
                ...state,
                loading: false,
                error: undefined
            };
        case blockedCardNumber.CLEAR:
            return initialState;
        default:
            return state;
    }
}