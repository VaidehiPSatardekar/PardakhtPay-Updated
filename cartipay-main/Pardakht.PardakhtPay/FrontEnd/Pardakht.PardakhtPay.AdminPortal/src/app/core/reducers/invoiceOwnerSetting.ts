import { InvoiceOwnerSetting } from "../../models/invoice";
import * as invoiceOwnerSetting from '../actions/invoiceOwnerSetting';

export interface State {
    allInvoiceOwnerSettings: InvoiceOwnerSetting[];
    invoiceOwnerSettingDetails: InvoiceOwnerSetting;
    loading: boolean;
    getDetailLoading: boolean;
    error?: {
        searchError: string,
        createInvoiceOwnerSetting: string,
        editInvoiceOwnerSetting: string,
        getDetails: string,
        getAll: string
    };
    query: string;
    invoiceOwnerSettingCreated: InvoiceOwnerSetting;
    invoiceOwnerSettingUpdateSuccess: boolean;
}

const initialState: State = {
    allInvoiceOwnerSettings: [],
    invoiceOwnerSettingDetails: undefined,
    loading: false,
    getDetailLoading: false,
    error: undefined,
    query: '',
    invoiceOwnerSettingCreated: undefined,
    invoiceOwnerSettingUpdateSuccess: false
};

export function reducer(state: State = initialState, action: invoiceOwnerSetting.Actions): State {
    switch (action.type) {
        case invoiceOwnerSetting.GET_ALL:

            return {
                ...state,
                allInvoiceOwnerSettings: [],
                loading: true,
                error: undefined
            }

        case invoiceOwnerSetting.GET_ALL_COMPLETE:
            return {
                ...state,
                allInvoiceOwnerSettings: action.payload.map(payload => payload),
                loading: false,
                error: undefined
            }

        case invoiceOwnerSetting.GET_ALL_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    searchError: '',
                    createInvoiceOwnerSetting: '',
                    editInvoiceOwnerSetting: '',
                    getDetails: '',
                    getAll: action.payload
                }
            }

        case invoiceOwnerSetting.CREATE:
            return {
                ...state,
                loading: true,
                error: undefined,
                invoiceOwnerSettingCreated: undefined,
            };

        case invoiceOwnerSetting.CREATE_COMPLETE:

             return {
                ...state,
                loading: false,
                invoiceOwnerSettingCreated: action.payload,
            };

        case invoiceOwnerSetting.CREATE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    searchError: '',
                    createInvoiceOwnerSetting: action.payload,
                    editInvoiceOwnerSetting: '',
                    getDetails: '',
                    getAll: ''
                },
                invoiceOwnerSettingCreated: undefined,
            };

        case invoiceOwnerSetting.GET_DETAILS:
            return {
                ...state,
                getDetailLoading: true,
                error: undefined,
                invoiceOwnerSettingDetails: undefined,
            };

        case invoiceOwnerSetting.GET_DETAILS_COMPLETE:
            return {
                ...state,
                getDetailLoading: false,
                invoiceOwnerSettingDetails: action.payload,
            };

        case invoiceOwnerSetting.GET_DETAILS_ERROR:
            return {
                ...state,
                getDetailLoading: false,
                error: {
                    searchError: '',
                    createInvoiceOwnerSetting: '',
                    editInvoiceOwnerSetting: '',
                    getDetails: action.payload,
                    getAll: ''
                },
                invoiceOwnerSettingCreated: undefined,
            };



        case invoiceOwnerSetting.EDIT:
            return {
                ...state,
                loading: true,
                error: undefined,
                invoiceOwnerSettingCreated: undefined,
                invoiceOwnerSettingUpdateSuccess: false
            };

        case invoiceOwnerSetting.EDIT_COMPLETE:
            {
                return {
                    ...state,
                    loading: false,
                    invoiceOwnerSettingCreated: undefined,
                    invoiceOwnerSettingUpdateSuccess: true
                };
            }

        case invoiceOwnerSetting.EDIT_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    searchError: '',
                    createInvoiceOwnerSetting: '',
                    editInvoiceOwnerSetting: action.payload,
                    getDetails: '',
                    getAll: ''
                },
                invoiceOwnerSettingCreated: undefined,
            };
        case invoiceOwnerSetting.CLEAR_ERRORS:
            return {
                ...state,
                loading: false,
                getDetailLoading: false,
                error: undefined
            };
        case invoiceOwnerSetting.CLEAR:
            return initialState;
        default:
            return state;
    }
}