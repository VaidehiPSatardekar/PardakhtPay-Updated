import { CardToCardAccountGroup } from "../../models/card-to-card-account-group";
import * as cardToCardAccountGroup from '../actions/cardToCardAccountGroup';


export interface State {
    allCardToCardAccountGroups: CardToCardAccountGroup[];
    cardToCardAccountGroupDetails: CardToCardAccountGroup;
    loading: boolean;
    getDetailLoading: boolean;
    updated: CardToCardAccountGroup;
    error?: {
        searchError: string,
        createCardToCardAccountGroup: string,
        editCardToCardAccountGroup: string,
        getDetails: string,
        getAll: string,
        deleteError: string
    };
    query: string;
    cardToCardAccountGroupCreated: CardToCardAccountGroup;
    cardToCardAccountGroupUpdateSuccess: boolean;
    cardToCardAccountDeletedSuccess: boolean;
}

const initialState: State = {
    allCardToCardAccountGroups: [],
    cardToCardAccountGroupDetails: undefined,
    loading: false,
    getDetailLoading: false,
    error: undefined,
    query: '',
    cardToCardAccountGroupCreated: undefined,
    updated: undefined,
    cardToCardAccountGroupUpdateSuccess: false,
    cardToCardAccountDeletedSuccess: false
};

export function reducer(state: State = initialState, action: cardToCardAccountGroup.Actions): State {
    switch (action.type) {
        case cardToCardAccountGroup.GET_ALL:

            return {
                ...state,
                allCardToCardAccountGroups: [],
                loading: true,
                error: undefined
            }

        case cardToCardAccountGroup.GET_ALL_COMPLETE:
            return {
                ...state,
                allCardToCardAccountGroups: action.payload.map(payload => payload),
                loading: false,
                error: undefined
            }

        case cardToCardAccountGroup.GET_ALL_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getAll: action.payload
                }
            }

        case cardToCardAccountGroup.CREATE:
            return {
                ...state,
                loading: true,
                error: undefined,
                cardToCardAccountGroupCreated: undefined,
            };

        case cardToCardAccountGroup.CREATE_COMPLETE:
            
            return {
                ...state,
                loading: false,
                cardToCardAccountGroupCreated: action.payload,
            };

        case cardToCardAccountGroup.CREATE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    createCardToCardAccountGroup: action.payload
                },
                cardToCardAccountGroupCreated: undefined,
            };

        case cardToCardAccountGroup.GET_DETAILS:
            return {
                ...state,
                getDetailLoading: true,
                error: undefined,
                cardToCardAccountGroupDetails: undefined,
            };

        case cardToCardAccountGroup.GET_DETAILS_COMPLETE:
            return {
                ...state,
                getDetailLoading: false,
                cardToCardAccountGroupDetails: action.payload,
            };

        case cardToCardAccountGroup.GET_DETAILS_ERROR:
            return {
                ...state,
                getDetailLoading: false,
                error: {
                    ...state.error,
                    getDetails: action.payload
                },
                cardToCardAccountGroupCreated: undefined,
            };

        case cardToCardAccountGroup.EDIT:
            return {
                ...state,
                loading: true,
                error: undefined,
                cardToCardAccountGroupCreated: undefined,
                cardToCardAccountGroupUpdateSuccess: false
            };

        case cardToCardAccountGroup.EDIT_COMPLETE:
            {
                return {
                    ...state,
                    loading: false,
                    cardToCardAccountGroupCreated: undefined,
                    cardToCardAccountGroupUpdateSuccess: true
                };
            }

        case cardToCardAccountGroup.EDIT_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    editCardToCardAccountGroup: action.payload
                },
                cardToCardAccountGroupCreated: undefined,
            };

        case cardToCardAccountGroup.DELETE:
            return {
                ...state,
                loading: true,
                error: undefined,
                cardToCardAccountDeletedSuccess: false
            };

        case cardToCardAccountGroup.DELETE_COMPLETE:
            {
                return {
                    ...state,
                    loading: false,
                    cardToCardAccountDeletedSuccess: true
                };
            }

        case cardToCardAccountGroup.DELETE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    deleteError: action.payload
                },
                cardToCardAccountDeletedSuccess: false
            };
        case cardToCardAccountGroup.CLEAR_ERRORS:
            return {
                ...state,
                loading: false,
                error: undefined
            };
        case cardToCardAccountGroup.CLEAR:
            return initialState;
        default:
            return state;
    }
}