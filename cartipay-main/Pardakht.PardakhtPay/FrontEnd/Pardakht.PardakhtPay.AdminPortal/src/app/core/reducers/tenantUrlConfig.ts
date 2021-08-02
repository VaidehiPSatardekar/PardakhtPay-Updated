import { TenantUrlConfig } from "../../models/tenant-url-config";
import * as tenantUrlConfig from '../actions/tenantUrlConfig';

export interface State {
    allTenantUrlConfigs: TenantUrlConfig[];
    tenantUrlConfigDetails: TenantUrlConfig;
    loading: boolean;
    getDetailLoading: boolean;
    updatePaymentGatewayDetails: TenantUrlConfig;
    error?: {
        searchError: string,
        createTenantUrlConfig: string,
        editTenantUrlConfig: string,
        getDetails: string,
        getAll: string
    };
    query: string;
    tenantUrlConfigCreated: TenantUrlConfig;
    tenantUrlConfigUpdateSuccess: boolean;
}

const initialState: State = {
    allTenantUrlConfigs: [],
    tenantUrlConfigDetails: undefined,
    loading: false,
    getDetailLoading: false,
    error: undefined,
    query: '',
    tenantUrlConfigCreated: undefined,
    updatePaymentGatewayDetails: undefined,
    tenantUrlConfigUpdateSuccess: false
};

export function reducer(state: State = initialState, action: tenantUrlConfig.Actions): State {
    switch (action.type) {
        case tenantUrlConfig.GET_ALL:

            return {
                ...state,
                allTenantUrlConfigs: [],
                loading: true,
                error: undefined
            }

        case tenantUrlConfig.GET_ALL_COMPLETE:
            return {
                ...state,
                allTenantUrlConfigs: action.payload.map(payload => payload),
                loading: false,
                error: undefined
            }

        case tenantUrlConfig.GET_ALL_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    searchError: '',
                    createTenantUrlConfig: '',
                    editTenantUrlConfig: '',
                    getDetails: '',
                    getAll: action.payload
                }
            }

        case tenantUrlConfig.CREATE:
            return {
                ...state,
                loading: true,
                error: undefined,
                tenantUrlConfigCreated: undefined,
            };

        case tenantUrlConfig.CREATE_COMPLETE:

             return {
                ...state,
                loading: false,
                tenantUrlConfigCreated: action.payload,
            };

        case tenantUrlConfig.CREATE_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    searchError: '',
                    createTenantUrlConfig: action.payload,
                    editTenantUrlConfig: '',
                    getDetails: '',
                    getAll: ''
                },
                tenantUrlConfigCreated: undefined,
            };

        case tenantUrlConfig.GET_DETAILS:
            return {
                ...state,
                getDetailLoading: true,
                error: undefined,
                tenantUrlConfigDetails: undefined,
            };

        case tenantUrlConfig.GET_DETAILS_COMPLETE:
            return {
                ...state,
                getDetailLoading: false,
                tenantUrlConfigDetails: action.payload,
            };

        case tenantUrlConfig.GET_DETAILS_ERROR:
            return {
                ...state,
                getDetailLoading: false,
                error: {
                    searchError: '',
                    createTenantUrlConfig: '',
                    editTenantUrlConfig: '',
                    getDetails: action.payload,
                    getAll: ''
                },
                tenantUrlConfigCreated: undefined,
            };



        case tenantUrlConfig.EDIT:
            return {
                ...state,
                loading: true,
                error: undefined,
                tenantUrlConfigCreated: undefined,
                tenantUrlConfigUpdateSuccess: false
            };

        case tenantUrlConfig.EDIT_COMPLETE:
            {
                return {
                    ...state,
                    loading: false,
                    tenantUrlConfigCreated: undefined,
                    tenantUrlConfigUpdateSuccess: true
                };
            }

        case tenantUrlConfig.EDIT_ERROR:
            return {
                ...state,
                loading: false,
                error: {
                    searchError: '',
                    createTenantUrlConfig: '',
                    editTenantUrlConfig: action.payload,
                    getDetails: '',
                    getAll: ''
                },
                tenantUrlConfigCreated: undefined,
            };
        case tenantUrlConfig.CLEAR_ERRORS:
            return {
                ...state,
                loading: false,
                getDetailLoading: false,
                error: undefined
            };
        case tenantUrlConfig.CLEAR:
            return initialState;
        default:
            return state;
    }
}