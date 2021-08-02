import { InvoicePayment } from "../../models/invoice";
import * as invoicePayment from '../actions/invoicePayment';
import { ListSearchResponse } from "app/models/types";

export interface State {
    allInvoicePayments: ListSearchResponse<InvoicePayment[]>;
    invoicePaymentDetails: InvoicePayment;
    loading: boolean;
    getDetailLoading: boolean;
    error?: {
        searchError: string,
        createInvoicePayment: string,
        editInvoicePayment: string,
        getDetails: string,
        getAll: string
    };
    query: string;
    invoicePaymentCreated: InvoicePayment;
    invoicePaymentUpdateSuccess: boolean;
}

const initialState: State = {
    allInvoicePayments: undefined,
    invoicePaymentDetails: undefined,
    loading: false,
    getDetailLoading: false,
    error: undefined,
    query: '',
    invoicePaymentCreated: undefined,
    invoicePaymentUpdateSuccess: false
};

export function reducer(state: State = initialState, action: invoicePayment.Actions): State {
    switch (action.type) {
        case invoicePayment.SEARCH:

            return {
                ...state,
                allInvoicePayments: undefined,
                loading: true,
                error: undefined
            }

        case invoicePayment.SEARCH_COMPLETE:
            return {
                ...state,
                allInvoicePayments: action.payload,
                loading: false,
                error: undefined
            }

        case invoicePayment.SEARCH_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    searchError: '',
                    createInvoicePayment: '',
                    editInvoicePayment: '',
                    getDetails: '',
                    getAll: action.payload
                }
            }

        case invoicePayment.CREATE:
            return {
                ...state,
                loading: true,
                error: undefined,
                invoicePaymentCreated: undefined,
            };

        case invoicePayment.CREATE_COMPLETE:

             return {
                ...state,
                loading: false,
                invoicePaymentCreated: action.payload,
            };

        case invoicePayment.CREATE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    searchError: '',
                    createInvoicePayment: action.payload,
                    editInvoicePayment: '',
                    getDetails: '',
                    getAll: ''
                },
                invoicePaymentCreated: undefined,
            };

        case invoicePayment.GET_DETAILS:
            return {
                ...state,
                getDetailLoading: true,
                error: undefined,
                invoicePaymentDetails: undefined,
            };

        case invoicePayment.GET_DETAILS_COMPLETE:
            return {
                ...state,
                getDetailLoading: false,
                invoicePaymentDetails: action.payload,
            };

        case invoicePayment.GET_DETAILS_ERROR:
            return {
                ...state,
                getDetailLoading: false,
                error: {
                    searchError: '',
                    createInvoicePayment: '',
                    editInvoicePayment: '',
                    getDetails: action.payload,
                    getAll: ''
                },
                invoicePaymentCreated: undefined,
            };



        case invoicePayment.EDIT:
            return {
                ...state,
                loading: true,
                error: undefined,
                invoicePaymentCreated: undefined,
                invoicePaymentUpdateSuccess: false
            };

        case invoicePayment.EDIT_COMPLETE:
            {
                return {
                    ...state,
                    loading: false,
                    invoicePaymentCreated: undefined,
                    invoicePaymentUpdateSuccess: true
                };
            }

        case invoicePayment.EDIT_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    searchError: '',
                    createInvoicePayment: '',
                    editInvoicePayment: action.payload,
                    getDetails: '',
                    getAll: ''
                },
                invoicePaymentCreated: undefined,
            };
        case invoicePayment.CLEAR_ERRORS:
            return {
                ...state,
                loading: false,
                getDetailLoading: false,
                error: undefined
            };
        case invoicePayment.CLEAR:
            return initialState;
        default:
            return state;
    }
}