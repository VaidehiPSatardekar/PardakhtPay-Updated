import { BlockedPhoneNumber } from "../../models/blocked-phone-number";
import * as blockedPhoneNumber from '../actions/blockedPhoneNumber';


export interface State {
    allBlockedPhoneNumbers: BlockedPhoneNumber[];
    blockedPhoneNumberDetails: BlockedPhoneNumber;
    loading: boolean;
    getDetailLoading: boolean;
    updated: BlockedPhoneNumber;
    error?: {
        searchError: string,
        createBlockedPhoneNumber: string,
        editBlockedPhoneNumber: string,
        getDetails: string,
        getAll: string,
        deleteError: string
    };
    query: string;
    blockedPhoneNumberCreated: BlockedPhoneNumber;
    blockedPhoneNumberUpdateSuccess: boolean;
    cardToCardAccountDeletedSuccess: boolean;
}

const initialState: State = {
    allBlockedPhoneNumbers: [],
    blockedPhoneNumberDetails: undefined,
    loading: false,
    getDetailLoading: false,
    error: undefined,
    query: '',
    blockedPhoneNumberCreated: undefined,
    updated: undefined,
    blockedPhoneNumberUpdateSuccess: false,
    cardToCardAccountDeletedSuccess: false
};

export function reducer(state: State = initialState, action: blockedPhoneNumber.Actions): State {
    switch (action.type) {
        case blockedPhoneNumber.GET_ALL:

            return {
                ...state,
                allBlockedPhoneNumbers: [],
                loading: true,
                error: undefined
            }

        case blockedPhoneNumber.GET_ALL_COMPLETE:
            return {
                ...state,
                allBlockedPhoneNumbers: action.payload.map(payload => payload),
                loading: false,
                error: undefined
            }

        case blockedPhoneNumber.GET_ALL_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getAll: action.payload
                }
            }

        case blockedPhoneNumber.CREATE:
            return {
                ...state,
                loading: true,
                error: undefined,
                blockedPhoneNumberCreated: undefined,
            };

        case blockedPhoneNumber.CREATE_COMPLETE:
            
            return {
                ...state,
                loading: false,
                blockedPhoneNumberCreated: action.payload,
            };

        case blockedPhoneNumber.CREATE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    createBlockedPhoneNumber: action.payload
                },
                blockedPhoneNumberCreated: undefined,
            };

        case blockedPhoneNumber.GET_DETAILS:
            return {
                ...state,
                getDetailLoading: true,
                error: undefined,
                blockedPhoneNumberDetails: undefined,
            };

        case blockedPhoneNumber.GET_DETAILS_COMPLETE:
            return {
                ...state,
                getDetailLoading: false,
                blockedPhoneNumberDetails: action.payload,
            };

        case blockedPhoneNumber.GET_DETAILS_ERROR:
            return {
                ...state,
                getDetailLoading: false,
                error: {
                    ...state.error,
                    getDetails: action.payload
                },
                blockedPhoneNumberCreated: undefined,
            };

        case blockedPhoneNumber.EDIT:
            return {
                ...state,
                loading: true,
                error: undefined,
                blockedPhoneNumberCreated: undefined,
                blockedPhoneNumberUpdateSuccess: false
            };

        case blockedPhoneNumber.EDIT_COMPLETE:
            {
                return {
                    ...state,
                    loading: false,
                    blockedPhoneNumberCreated: undefined,
                    blockedPhoneNumberUpdateSuccess: true
                };
            }

        case blockedPhoneNumber.EDIT_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    editBlockedPhoneNumber: action.payload
                },
                blockedPhoneNumberCreated: undefined,
            };

        case blockedPhoneNumber.DELETE:
            return {
                ...state,
                loading: true,
                error: undefined,
                cardToCardAccountDeletedSuccess: false
            };

        case blockedPhoneNumber.DELETE_COMPLETE:
            {
                return {
                    ...state,
                    loading: false,
                    cardToCardAccountDeletedSuccess: true
                };
            }

        case blockedPhoneNumber.DELETE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    deleteError: action.payload
                },
                cardToCardAccountDeletedSuccess: false
            };
        case blockedPhoneNumber.CLEAR_ERRORS:
            return {
                ...state,
                loading: false,
                error: undefined
            };
        case blockedPhoneNumber.CLEAR:
            return initialState;
        default:
            return state;
    }
}