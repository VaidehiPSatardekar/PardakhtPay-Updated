import { CardToCardAccount } from "../../models/card-to-card-account";
import * as cardToCardAccount from '../actions/cardToCardAccount';

export interface State {
    cardToCardAccounts: CardToCardAccount[];
    allCardToCardAccounts: CardToCardAccount[];
    cardToCardAccountDetails: CardToCardAccount;
    loading: boolean;
    getDetailLoading: boolean;
    updatePaymentGatewayDetails: CardToCardAccount;
    error?: {
        searchError: string,
        createCardToCardAccount: string,
        editCardToCardAccount: string,
        getDetails: string,
        getAll: string
    };
    query: string;
    cardToCardAccountCreated: CardToCardAccount;
    cardToCardAccountUpdateSuccess: boolean;
}

const initialState: State = {
    cardToCardAccounts: [],
    allCardToCardAccounts: [],
    cardToCardAccountDetails: undefined,
    loading: false,
    getDetailLoading: false,
    error: undefined,
    query: '',
    cardToCardAccountCreated: undefined,
    updatePaymentGatewayDetails: undefined,
    cardToCardAccountUpdateSuccess: false
};

export function reducer(state: State = initialState, action: cardToCardAccount.Actions): State {
    switch (action.type) {
        case cardToCardAccount.SEARCH:

            return {
                ...state,
                cardToCardAccounts: [],
                cardToCardAccountDetails: undefined,
                loading: true,
                cardToCardAccountCreated: undefined,
                error: undefined
            }

        case cardToCardAccount.SEARCH_COMPLETE:
            return {
                ...state,
                cardToCardAccounts: action.payload.map(payload => payload),
                cardToCardAccountDetails: undefined,
                loading: false,
                error: undefined,
                query: state.query,
                cardToCardAccountCreated: undefined
            }

        case cardToCardAccount.SEARCH_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    searchError: action.payload
                }
            }
        case cardToCardAccount.GET_ALL:

            return {
                ...state,
                allCardToCardAccounts : [],
                loading: true,
                error: undefined
            }

        case cardToCardAccount.GET_ALL_COMPLETE:
            return {
                ...state,
                allCardToCardAccounts: action.payload.map(payload => payload),
                loading: false,
                error: undefined
            }

        case cardToCardAccount.GET_ALL_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getAll: action.payload
                }
            }

        case cardToCardAccount.CREATE:
            return {
                ...state,
                loading: true,
                error: undefined,
                cardToCardAccountCreated: undefined,
            };

        case cardToCardAccount.CREATE_COMPLETE:


            const newCardToCardAccounts: CardToCardAccount[] = [
                ...state.cardToCardAccounts
            ];
            newCardToCardAccounts.push(action.payload);
            return {
                ...state,
                cardToCardAccounts: newCardToCardAccounts,
                loading: false,
                cardToCardAccountCreated: action.payload,
            };

        case cardToCardAccount.CREATE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    createCardToCardAccount: action.payload
                },
                cardToCardAccountCreated: undefined,
            };

        case cardToCardAccount.GET_DETAILS:
            return {
                ...state,
                getDetailLoading: true,
                error: undefined,
                cardToCardAccountDetails: undefined,
            };

        case cardToCardAccount.GET_DETAILS_COMPLETE:
            return {
                ...state,
                getDetailLoading: false,
                cardToCardAccountDetails: action.payload,
            };

        case cardToCardAccount.GET_DETAILS_ERROR:
            return {
                ...state,
                getDetailLoading: false,
                error: {
                    ...state.error,
                    getDetails: action.payload
                },
                cardToCardAccountCreated: undefined,
            };



        case cardToCardAccount.EDIT:
            return {
                ...state,
                loading: true,
                error: undefined,
                cardToCardAccountCreated: undefined,
                cardToCardAccountUpdateSuccess: false
            };

        case cardToCardAccount.EDIT_COMPLETE:
            {

                const newCardToCardAccounts: CardToCardAccount[] = [
                    ...state.cardToCardAccounts
                ];
                newCardToCardAccounts.splice(newCardToCardAccounts.findIndex(q => q.id === action.payload.id), 1, action.payload);
                return {
                    ...state,
                    cardToCardAccounts: newCardToCardAccounts,
                    loading: false,
                    cardToCardAccountCreated: undefined,
                    cardToCardAccountUpdateSuccess: true
                };
            }

        case cardToCardAccount.EDIT_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    editCardToCardAccount: action.payload
                },
                cardToCardAccountCreated: undefined,
            };
        case cardToCardAccount.CLEAR_ERRORS:
            return {
                ...state,
                loading: false,
                error: undefined
            };
        case cardToCardAccount.CLEAR:
            return initialState;
        default:
            return state;
    }
}