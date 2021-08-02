import * as invoice from '../actions/invoice';
import { Invoice } from '../../models/invoice';
import { ListSearchResponse } from '../../models/types';

export interface State {
    invoices: ListSearchResponse<Invoice[]>;
    invoiceDetail: Invoice;
    loading: boolean;
    detailLoading: boolean;
    error?: {
        searchError: string,
        getDetails: string
    };
}

const initialState: State = {
    invoices: undefined,
    invoiceDetail: undefined,
    loading: false,
    detailLoading: false,
    error: undefined
};

export function reducer(state: State = initialState, action: invoice.Actions): State {
    switch (action.type) {
        case invoice.SEARCH:

            return {
                ...state,
                invoices: undefined,
                invoiceDetail: undefined,
                loading: true,
                error: undefined
            }

        case invoice.SEARCH_COMPLETE:
            return {
                ...state,
                invoices: action.payload,
                invoiceDetail: undefined,
                loading: false,
                error: undefined
            }

        case invoice.SEARCH_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    ...state.error,
                    searchError: action.payload
                }
            }

        case invoice.GET_DETAILS:
            return {
                ...state,
                detailLoading: true,
                error: undefined,
                invoiceDetail: undefined,
            };

        case invoice.GET_DETAILS_COMPLETE:
            return {
                ...state,
                detailLoading: false,
                invoiceDetail: action.payload,
            };

        case invoice.GET_DETAILS_ERROR:
            return {
                ...state,
                detailLoading: false,
                error: {
                    ...state.error,
                    getDetails: action.payload
                }
            };
            
        case invoice.CLEAR_ERRORS:
            return {
                ...state,
                error: undefined
            };
        case invoice.CLEAR:
            return initialState;
        default:
            return state;
    }
}