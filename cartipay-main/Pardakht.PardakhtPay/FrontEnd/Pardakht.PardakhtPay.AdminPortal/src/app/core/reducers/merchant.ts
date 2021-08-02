import { Merchant, MerchantCreate } from "../../models/merchant-model";
import * as merchant from '../actions/merchant';

export interface State {
    merchants: Merchant[];
    allMerchants: Merchant[];
    merchantDetails: Merchant;
    loading: boolean;
    getDetailLoading: boolean;
    updatePaymentGatewayDetails: Merchant;
    error?: {
        searchError: string,
        createMerchant: string,
        editMerchant: string,
        getDetails: string,
        getAll: string,
        deleteError: string
    };
    query: string;
    merchantCreated: MerchantCreate;
    merchantUpdateSuccess: boolean;
    deleteSuccess: boolean;
}

const initialState: State = {
    merchants: undefined,
    allMerchants: undefined,
    merchantDetails: undefined,
    loading: false,
    getDetailLoading: false,
    error: undefined,
    query: '',
    merchantCreated: undefined,
    updatePaymentGatewayDetails: undefined,
    merchantUpdateSuccess: false,
    deleteSuccess: false
};

export function reducer(state: State = initialState, action: merchant.Actions): State {
    switch (action.type) {
        case merchant.SEARCH:

            return {
                ...state,
                merchantDetails: undefined,
                loading: true,
                merchantCreated: undefined,
                error: undefined
            }

        case merchant.SEARCH_COMPLETE:
            return {
                ...state,
                merchants: action.payload.map(payload => payload),
                merchantDetails: undefined,
                loading: false,
                error: undefined,
                query: state.query,
                merchantCreated: undefined
            }

        case merchant.SEARCH_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    searchError: action.payload
                }
            }
        case merchant.GET_ALL:

            return {
                ...state,
                loading: true,
                error: undefined
            }

        case merchant.GET_ALL_COMPLETE:
            return {
                ...state,
                allMerchants: action.payload.map(payload => payload),
                loading: false,
                error: undefined
            }

        case merchant.GET_ALL_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    getAll: action.payload
                }
            }

        case merchant.CREATE:
            return {
                ...state,
                loading: true,
                error: undefined,
                merchantCreated: undefined,
            };

        case merchant.CREATE_COMPLETE:


            const newMerchants: Merchant[] = [
                ...state.merchants
            ];
            newMerchants.push(action.payload);
            return {
                ...state,
                merchants: newMerchants,
                allMerchants: newMerchants,
                loading: false,
                merchantCreated: action.payload,
            };

        case merchant.CREATE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    createMerchant: action.payload
                },
                merchantCreated: undefined,
            };

        case merchant.GET_DETAILS:
            return {
                ...state,
                getDetailLoading: true,
                error: undefined,
                merchantDetails: undefined,
            };

        case merchant.GET_DETAILS_COMPLETE:
            return {
                ...state,
                getDetailLoading: false,
                merchantDetails: action.payload,
            };

        case merchant.GET_DETAILS_ERROR:
            return {
                ...state,
                getDetailLoading: false,
                error: {
                    ...state.error,
                    getDetails: action.payload
                },
                merchantCreated: undefined,
            };



        case merchant.EDIT:
            return {
                ...state,
                loading: true,
                error: undefined,
                merchantCreated: undefined,
                merchantUpdateSuccess: false
            };

        case merchant.EDIT_COMPLETE:
            {

                const newMerchants: Merchant[] = [
                    ...state.merchants
                ];
                newMerchants.splice(newMerchants.findIndex(q => q.id === action.payload.id), 1, action.payload);
                return {
                    ...state,
                    merchants: newMerchants,
                    allMerchants: newMerchants,
                    loading: false,
                    merchantCreated: undefined,
                    merchantUpdateSuccess: true
                };
            }

        case merchant.EDIT_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    editMerchant: action.payload
                },
                merchantCreated: undefined,
            };

        case merchant.DELETE:
            return {
                ...state,
                loading: true,
                error: undefined,
                deleteSuccess: false
            };

        case merchant.DELETE_COMPLETE:
            {
                return {
                    ...state,
                    merchants: undefined,
                    allMerchants: undefined,
                    loading: false,
                    deleteSuccess: true
                };
            }

        case merchant.DELETE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    deleteError: action.payload
                },
                deleteSuccess: false,
            };

        case merchant.CLEAR_ERRORS:
            return {
                ...state,
                loading: false,
                getDetailLoading: false,
                error: undefined
            };
        case merchant.CLEAR:
            return initialState;
        default:
            return state;
    }
}